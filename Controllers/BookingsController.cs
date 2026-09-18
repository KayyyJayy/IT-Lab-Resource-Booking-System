using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabBookingSystem.Data;
using LabBookingSystem.Models;
using LabBookingSystem.Models.ViewModels;

namespace LabBookingSystem.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

  
    public async Task<IActionResult> MyBookings()
    {
        var userId = _userManager.GetUserId(User)!;
        var bookings = await _db.Bookings
            .Include(b => b.Resource)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();

        return View(bookings);
    }

    
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Index(string? status, string? search)
    {
        var query = _db.Bookings
            .Include(b => b.Resource)
            .Include(b => b.User)
            .AsQueryable();

        if (Enum.TryParse<BookingStatus>(status, out var bookingStatus))
            query = query.Where(b => b.Status == bookingStatus);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(b =>
                b.User.FullName.Contains(search) ||
                b.Resource.Name.Contains(search) ||
                b.Purpose.Contains(search));

        ViewBag.Status = status;
        ViewBag.Search = search;

        return View(await query.OrderByDescending(b => b.CreatedAt).ToListAsync());
    }

    
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");

        var booking = await _db.Bookings
            .Include(b => b.Resource)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();
        if (!isAdmin && booking.UserId != userId) return Forbid();

        return View(booking);
    }

    
    public async Task<IActionResult> Create(int? resourceId)
    {
        var resources = await _db.Resources
            .Where(r => r.IsAvailable)
            .OrderBy(r => r.Name)
            .ToListAsync();

        var vm = new BookingViewModel
        {
            ResourceId = resourceId ?? 0,
            StartTime = DateTime.Now.Date.AddHours(9),
            EndTime = DateTime.Now.Date.AddHours(10),
            ResourceList = new SelectList(resources, "Id", "Name", resourceId)
        };

        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingViewModel vm)
    {
        // Re-populate dropdown list on any return
        var resources = await _db.Resources.Where(r => r.IsAvailable).OrderBy(r => r.Name).ToListAsync();
        vm.ResourceList = new SelectList(resources, "Id", "Name", vm.ResourceId);

        if (!ModelState.IsValid) return View(vm);

        
        if (vm.StartTime >= vm.EndTime)
        {
            ModelState.AddModelError("EndTime", "End time must be after start time.");
            return View(vm);
        }

        if (vm.StartTime < DateTime.Now)
        {
            ModelState.AddModelError("StartTime", "Start time cannot be in the past.");
            return View(vm);
        }

        var resource = await _db.Resources.FindAsync(vm.ResourceId);
        if (resource == null)
        {
            ModelState.AddModelError("ResourceId", "Resource not found.");
            return View(vm);
        }

        if (vm.NumberOfAttendees > resource.Capacity)
        {
            ModelState.AddModelError("NumberOfAttendees",
                $"This resource has a capacity of {resource.Capacity}. Please reduce attendee count.");
            return View(vm);
        }

        
        var hasConflict = await _db.Bookings.AnyAsync(b =>
            b.ResourceId == vm.ResourceId &&
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.Rejected &&
            b.StartTime < vm.EndTime &&
            b.EndTime > vm.StartTime);

        if (hasConflict)
        {
            ModelState.AddModelError(string.Empty,
                "This resource is already booked for part or all of the selected time slot. Please choose a different time.");
            return View(vm);
        }

        
        var booking = new Booking
        {
            UserId = _userManager.GetUserId(User)!,
            ResourceId = vm.ResourceId,
            StartTime = vm.StartTime,
            EndTime = vm.EndTime,
            Purpose = vm.Purpose,
            NumberOfAttendees = vm.NumberOfAttendees,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Booking submitted! You will be notified once it is reviewed.";
        return RedirectToAction(nameof(MyBookings));
    }

    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var booking = await _db.Bookings.FindAsync(id);

        if (booking == null) return NotFound();
        if (booking.UserId != userId && !User.IsInRole("Admin")) return Forbid();
        if (!booking.CanBeCancelled)
        {
            TempData["Error"] = "This booking cannot be cancelled.";
            return RedirectToAction(nameof(MyBookings));
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Booking cancelled.";
        return RedirectToAction(nameof(MyBookings));
    }
}
