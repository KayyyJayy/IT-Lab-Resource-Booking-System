using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LabBookingSystem.Data;
using LabBookingSystem.Models;
using LabBookingSystem.Models.ViewModels;

namespace LabBookingSystem.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    
    public async Task<IActionResult> Dashboard()
    {
        var now = DateTime.UtcNow;

        var vm = new DashboardViewModel
        {
            TotalResources = await _db.Resources.CountAsync(),
            TotalBookings = await _db.Bookings.CountAsync(),
            PendingApprovals = await _db.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
            TodayBookings = await _db.Bookings.CountAsync(b =>
                b.StartTime.Date == DateTime.UtcNow.Date && b.Status == BookingStatus.Approved),

            RecentBookings = await _db.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .Take(10)
                .ToListAsync()
        };

        return View(vm);
    }


    public async Task<IActionResult> Approvals()
    {
        var pending = await _db.Bookings
            .Include(b => b.Resource)
            .Include(b => b.User)
            .Where(b => b.Status == BookingStatus.Pending)
            .OrderBy(b => b.StartTime)
            .ToListAsync();

        return View(pending);
    }

    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(AdminBookingActionViewModel model)
    {
        var booking = await _db.Bookings.FindAsync(model.BookingId);
        if (booking == null) return NotFound();

        booking.Status = BookingStatus.Approved;
        booking.AdminNotes = model.AdminNotes;
        booking.ApprovedByUserId = _userManager.GetUserId(User);
        booking.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = $"Booking #{booking.Id} approved.";
        return RedirectToAction(nameof(Approvals));
    }

   
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(AdminBookingActionViewModel model)
    {
        var booking = await _db.Bookings.FindAsync(model.BookingId);
        if (booking == null) return NotFound();

        booking.Status = BookingStatus.Rejected;
        booking.AdminNotes = model.AdminNotes;
        booking.ApprovedByUserId = _userManager.GetUserId(User);
        booking.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Error"] = $"Booking #{booking.Id} rejected.";
        return RedirectToAction(nameof(Approvals));
    }

    
    public async Task<IActionResult> Reports(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? DateTime.Today.AddDays(-30);
        var toDate = (to ?? DateTime.Today).AddDays(1).AddTicks(-1);

        var bookings = await _db.Bookings
            .Include(b => b.Resource)
            .Include(b => b.User)
            .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var vm = new ReportsViewModel
        {
            FromDate = fromDate,
            ToDate = toDate.Date,
            TotalBookings = bookings.Count,
            ApprovedBookings = bookings.Count(b => b.Status == BookingStatus.Approved),
            RejectedBookings = bookings.Count(b => b.Status == BookingStatus.Rejected),
            CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled),
            PendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending),
            BookingHistory = bookings,
            ResourceUsage = bookings
                .Where(b => b.Status == BookingStatus.Approved)
                .GroupBy(b => b.Resource)
                .Select(g => new ResourceUsageStat
                {
                    ResourceName = g.Key.Name,
                    ResourceType = g.Key.Type.ToString(),
                    BookingCount = g.Count(),
                    TotalHours = g.Sum(b => (b.EndTime - b.StartTime).TotalHours)
                })
                .OrderByDescending(r => r.BookingCount)
                .ToList()
        };

        return View(vm);
    }

   
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();
        var userRoles = new Dictionary<string, IList<string>>();

        foreach (var user in users)
            userRoles[user.Id] = await _userManager.GetRolesAsync(user);

        ViewBag.UserRoles = userRoles;
        return View(users);
    }
}
