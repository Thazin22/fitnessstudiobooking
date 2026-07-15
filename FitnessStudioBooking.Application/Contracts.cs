using FitnessStudioBooking.Domain;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudioBooking.Application;

public interface IAppDbContext
{
    DbSet<Business> Businesses { get; }
    DbSet<Customer> Customers { get; }
    DbSet<CustomerPackage> CustomerPackages { get; }
    DbSet<TimetableSchedule> TimetableSchedules { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<WaitlistEntry> WaitlistEntries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IRedisLock
{
    Task<IAsyncDisposable?> AcquireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken);
}

public sealed record ApiResult(bool Success, string Message)
{
    public static ApiResult Ok(string message) => new(true, message);
    public static ApiResult Fail(string message) => new(false, message);
}

public sealed record AvailablePackageDto(int BusinessId, string BusinessName, int TotalCredits, int ValidityDays);
public sealed record CustomerPackageDto(int Id, int CustomerId, int BusinessId, string BusinessName, int TotalCredits, int RemainingCredits, DateTimeOffset ExpiryDate);
public sealed record TimetableDto(int ScheduleId, string ClassName, string InstructorName, DateTimeOffset StartTime, DateTimeOffset EndTime, int AttendanceCount, int AvailableSlots, string BusinessName);
public sealed record PurchasePackageRequest(int CustomerId, int BusinessId, int TotalCredits, DateTimeOffset? ExpiryDate);
public sealed record BookClassRequest(int CustomerId, int CustomerPackageId, int TimetableScheduleId, bool JoinWaitlistIfFull = true);
public sealed record CancelBookingRequest(int BookingId);
public sealed record JoinWaitlistRequest(int CustomerId, int CustomerPackageId, int TimetableScheduleId);
