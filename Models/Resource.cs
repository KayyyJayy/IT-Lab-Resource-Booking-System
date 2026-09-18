using System.ComponentModel.DataAnnotations;

namespace LabBookingSystem.Models;

public enum ResourceType
{
    [Display(Name = "Computer Lab")]
    ComputerLab,
    [Display(Name = "Workstation")]
    Workstation,
    [Display(Name = "Projector")]
    Projector,
    [Display(Name = "AV Equipment")]
    AVEquipment,
    [Display(Name = "Physics Lab")]
    PhysicsLab,
    [Display(Name = "Chemistry Lab")]
    ChemistryLab,
    [Display(Name = "Biology Lab")]
    BiologyLab
}

public class Resource
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Resource Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    public ResourceType Type { get; set; }

    [Required, StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 500)]
    [Display(Name = "Capacity (seats)")]
    public int Capacity { get; set; } = 1;

    [Display(Name = "Available for Booking")]
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
