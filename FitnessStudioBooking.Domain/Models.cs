namespace FitnessStudioBooking.Domain;

public enum BookingStatus
{
    Booked = 1,
    Cancelled = 2
}

public enum WaitlistStatus
{
    Waiting = 1,
    Promoted = 2,
    Cancelled = 3,
    Expired = 4
}

public sealed class Business
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<CustomerPackage> Packages { get; set; } = [];
    public List<TimetableSchedule> TimetableSchedules { get; set; } = [];
}

public sealed class Customer
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public List<CustomerPackage> Packages { get; set; } = [];
    public List<Booking> Bookings { get; set; } = [];
    public List<WaitlistEntry> WaitlistEntries { get; set; } = [];
}

public sealed class CustomerPackage
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int BusinessId { get; set; }
    public int TotalCredits { get; set; }
    public int RemainingCredits { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public Customer? Customer { get; set; }
    public Business? Business { get; set; }
}

public sealed class TimetableSchedule
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public required string ClassName { get; set; }
    public required string InstructorName { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public int AvailableSlots { get; set; }
    public Business? Business { get; set; }
    public List<Booking> Bookings { get; set; } = [];
    public List<WaitlistEntry> WaitlistEntries { get; set; } = [];
}

public sealed class Booking
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CustomerPackageId { get; set; }
    public int TimetableScheduleId { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Booked;
    public DateTimeOffset BookedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public bool CreditRefunded { get; set; }
    public Customer? Customer { get; set; }
    public CustomerPackage? CustomerPackage { get; set; }
    public TimetableSchedule? TimetableSchedule { get; set; }
}

public sealed class WaitlistEntry
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CustomerPackageId { get; set; }
    public int TimetableScheduleId { get; set; }
    public WaitlistStatus Status { get; set; } = WaitlistStatus.Waiting;
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? PromotedAt { get; set; }
    public Customer? Customer { get; set; }
    public CustomerPackage? CustomerPackage { get; set; }
    public TimetableSchedule? TimetableSchedule { get; set; }
}
