using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LabBookingSystem.Data;
using LabBookingSystem.Models;

namespace LabBookingSystem.Controllers;

[Authorize]
public class ResourcesController : Controller
{
    private readonly ApplicationDbContext _db;

    public ResourcesController(ApplicationDbContext db) => _db = db;

    
    public async Task<IActionResult> Index(string? type, string? search)
    {
        var query = _db.Resources.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Name.Contains(search) || r.Location.Contains(search));

        if (Enum.TryParse<ResourceType>(type, out var resourceType))
            query = query.Where(r => r.Type == resourceType);

        ViewBag.Search = search;
        ViewBag.Type = type;

        return View(await query.OrderBy(r => r.Type).ThenBy(r => r.Name).ToListAsync());
    }

    
    public async Task<IActionResult> Details(int id)
    {
        var resource = await _db.Resources
            .Include(r => r.Bookings.Where(b =>
                b.EndTime >= DateTime.UtcNow &&
                (b.Status == BookingStatus.Approved || b.Status == BookingStatus.Pending)))
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (resource == null) return NotFound();
        return View(resource);
    }

    
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new Resource());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Resource resource)
    {
        if (!ModelState.IsValid) return View(resource);

        resource.CreatedAt = DateTime.UtcNow;
        _db.Resources.Add(resource);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Resource \"{resource.Name}\" created successfully.";
        return RedirectToAction(nameof(Index));
    }

    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource == null) return NotFound();
        return View(resource);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Resource resource)
    {
        if (id != resource.Id) return BadRequest();
        if (!ModelState.IsValid) return View(resource);

        _db.Update(resource);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Resource \"{resource.Name}\" updated.";
        return RedirectToAction(nameof(Index));
    }

    
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource == null) return NotFound();

        _db.Resources.Remove(resource);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Resource \"{resource.Name}\" deleted.";
        return RedirectToAction(nameof(Index));
    }
}
