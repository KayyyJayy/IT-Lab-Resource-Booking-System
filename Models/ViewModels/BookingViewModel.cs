using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LabBookingSystem.Models.ViewModels;

public class BookingViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please select a resource.")]
    [Display(Name = "Resource")]
    public int ResourceId { get; set; }

    [Required(ErrorMessage = "Please enter a start date and time.")]
    [Display(Name = "Start Date & Time")]
    public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);

    [Required(ErrorMessage = "Please enter an end date and time.")]
    [Display(Name = "End Date & Time")]
    public DateTime EndTime { get; set; } = DateTime.Now.AddHours(2);

    [Required(ErrorMessage = "Please describe the purpose of the booking.")]
    [StringLength(300, MinimumLength = 10, ErrorMessage = "Purpose must be between 10 and 300 characters.")]
    [Display(Name = "Purpose / Description")]
    public string Purpose { get; set; } = string.Empty;

    [Range(1, 500, ErrorMessage = "Attendees must be between 1 and 500.")]
    [Display(Name = "Number of Attendees")]
    public int NumberOfAttendees { get; set; } = 1;

    public SelectList? ResourceList { get; set; }
}

public class AdminBookingActionViewModel
{
    public int BookingId { get; set; }
    public string Action { get; set; } = string.Empty; // "Approve" or "Reject"

    [StringLength(500)]
    [Display(Name = "Notes (optional)")]
    public string? AdminNotes { get; set; }
}
