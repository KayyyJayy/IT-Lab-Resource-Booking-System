using System.ComponentModel.DataAnnotations;

namespace LabBookingSystem.Models;

public enum BookingStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled,
    Completed
}

public class Booking
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    [Required]
    [Display(Name = "Resource")]
    public int ResourceId { get; set; }
    public virtual Resource Resource { get; set; } = null!;

    [Required]
    [Display(Name = "Start Date & Time")]
    public DateTime StartTime { get; set; }

    [Required]
    [Display(Name = "End Date & Time")]
    public DateTime EndTime { get; set; }

    [Required, StringLength(300)]
    [Display(Name = "Purpose / Description")]
    public string Purpose { get; set; } = string.Empty;

    [Range(1, 500)]
    [Display(Name = "Number of Attendees")]
    public int NumberOfAttendees { get; set; } = 1;

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [StringLength(500)]
    [Display(Name = "Admin Notes")]
    public string? AdminNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "Approved/Rejected By")]
    public string? ApprovedByUserId { get; set; }

    // Computed helper
    public TimeSpan Duration => EndTime - StartTime;
    public bool IsUpcoming => StartTime > DateTime.UtcNow && Status == BookingStatus.Approved;
    public bool CanBeCancelled => Status is BookingStatus.Pending or BookingStatus.Approved && StartTime > DateTime.UtcNow;
}
