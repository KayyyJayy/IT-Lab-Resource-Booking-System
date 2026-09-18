using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using LabBookingSystem.Data;
using LabBookingSystem.Models;
using LabBookingSystem.Models.ViewModels;

namespace LabBookingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity!.IsAuthenticated)
            return View("Landing");

        var userId = _userManager.GetUserId(User)!;
        var now = DateTime.UtcNow;

        // Build calendar events from approved bookings
        var approvedBookings = await _db.Bookings
            .Include(b => b.Resource)
            .Include(b => b.User)
            .Where(b => b.Status == BookingStatus.Approved && b.EndTime >= now)
            .ToListAsync();

        var calendarEvents = approvedBookings.Select(b => new
        {
            id = b.Id,
            title = $"{b.Resource.Name} – {b.User.FullName}",
            start = b.StartTime.ToString("o"),
            end = b.EndTime.ToString("o"),
            color = b.UserId == userId ? "#0d6efd" : "#6c757d",
            url = $"/Bookings/Details/{b.Id}"
        });

        var vm = new DashboardViewModel
        {
            TotalResources = await _db.Resources.CountAsync(r => r.IsAvailable),
            TotalBookings = await _db.Bookings.CountAsync(),
            PendingApprovals = await _db.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
            TodayBookings = await _db.Bookings.CountAsync(b =>
                b.StartTime.Date == DateTime.UtcNow.Date && b.Status == BookingStatus.Approved),

            MyUpcomingBookings = await _db.Bookings
                .Include(b => b.Resource)
                .Where(b => b.UserId == userId && b.StartTime >= now
                    && (b.Status == BookingStatus.Approved || b.Status == BookingStatus.Pending))
                .OrderBy(b => b.StartTime)
                .Take(5)
                .ToListAsync(),

            RecentBookings = await _db.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .Take(10)
                .ToListAsync(),

            CalendarEventsJson = JsonSerializer.Serialize(calendarEvents)
        };

        return View("Dashboard", vm);
    }

    public IActionResult Error() => View();
}
