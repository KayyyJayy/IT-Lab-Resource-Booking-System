namespace LabBookingSystem.Models.ViewModels;

public class DashboardViewModel
{
    // Stats cards
    public int TotalResources { get; set; }
    public int TotalBookings { get; set; }
    public int PendingApprovals { get; set; }
    public int TodayBookings { get; set; }

    // Upcoming bookings for the logged-in user
    public List<Booking> MyUpcomingBookings { get; set; } = new();

    // Recent bookings (for admin)
    public List<Booking> RecentBookings { get; set; } = new();

    // Calendar events 
    public string CalendarEventsJson { get; set; } = "[]";
}

public class ReportsViewModel
{
    public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-30);
    public DateTime ToDate { get; set; } = DateTime.Today;

    public int TotalBookings { get; set; }
    public int ApprovedBookings { get; set; }
    public int RejectedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int PendingBookings { get; set; }

    public List<ResourceUsageStat> ResourceUsage { get; set; } = new();
    public List<Booking> BookingHistory { get; set; } = new();
}

public class ResourceUsageStat
{
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public double TotalHours { get; set; }
}
