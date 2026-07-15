using FitnessStudioBooking.Application;
using FitnessStudioBooking.Domain;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudioBooking.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerPackage> CustomerPackages => Set<CustomerPackage>();
    public DbSet<TimetableSchedule> TimetableSchedules => Set<TimetableSchedule>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<WaitlistEntry> WaitlistEntries => Set<WaitlistEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Business>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<CustomerPackage>(entity =>
        {
            entity.HasOne(x => x.Customer).WithMany(x => x.Packages).HasForeignKey(x => x.CustomerId);
            entity.HasOne(x => x.Business).WithMany(x => x.Packages).HasForeignKey(x => x.BusinessId);
        });

        modelBuilder.Entity<TimetableSchedule>(entity =>
        {
            entity.Property(x => x.ClassName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.InstructorName).HasMaxLength(160).IsRequired();
            entity.HasOne(x => x.Business).WithMany(x => x.TimetableSchedules).HasForeignKey(x => x.BusinessId);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasOne(x => x.Customer).WithMany(x => x.Bookings).HasForeignKey(x => x.CustomerId);
            entity.HasOne(x => x.CustomerPackage).WithMany().HasForeignKey(x => x.CustomerPackageId);
            entity.HasOne(x => x.TimetableSchedule).WithMany(x => x.Bookings).HasForeignKey(x => x.TimetableScheduleId);
            entity.HasIndex(x => new { x.CustomerId, x.TimetableScheduleId, x.Status });
        });

        modelBuilder.Entity<WaitlistEntry>(entity =>
        {
            entity.HasOne(x => x.Customer).WithMany(x => x.WaitlistEntries).HasForeignKey(x => x.CustomerId);
            entity.HasOne(x => x.CustomerPackage).WithMany().HasForeignKey(x => x.CustomerPackageId);
            entity.HasOne(x => x.TimetableSchedule).WithMany(x => x.WaitlistEntries).HasForeignKey(x => x.TimetableScheduleId);
            entity.HasIndex(x => new { x.TimetableScheduleId, x.Status, x.JoinedAt });
        });

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        var now = new DateTimeOffset(2026, 7, 9, 0, 0, 0, TimeSpan.Zero);

        modelBuilder.Entity<Business>().HasData(
            new Business { Id = 1, Name = "Rezerv Fitness" },
            new Business { Id = 2, Name = "Downtown Yoga" });

        modelBuilder.Entity<Customer>().HasData(
            Enumerable.Range(1, 10).Select(i => new Customer
            {
                Id = i,
                FullName = $"Customer {i}",
                Email = $"customer{i}@example.com"
            }));

        modelBuilder.Entity<CustomerPackage>().HasData(
            new CustomerPackage { Id = 1, CustomerId = 1, BusinessId = 1, TotalCredits = 10, RemainingCredits = 8, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 2, CustomerId = 2, BusinessId = 1, TotalCredits = 5, RemainingCredits = 0, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 3, CustomerId = 3, BusinessId = 2, TotalCredits = 10, RemainingCredits = 10, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 4, CustomerId = 4, BusinessId = 1, TotalCredits = 10, RemainingCredits = 10, ExpiryDate = now.AddDays(-1) },
            new CustomerPackage { Id = 5, CustomerId = 5, BusinessId = 1, TotalCredits = 10, RemainingCredits = 9, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 6, CustomerId = 6, BusinessId = 1, TotalCredits = 10, RemainingCredits = 10, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 7, CustomerId = 7, BusinessId = 1, TotalCredits = 10, RemainingCredits = 10, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 8, CustomerId = 8, BusinessId = 2, TotalCredits = 10, RemainingCredits = 10, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 9, CustomerId = 9, BusinessId = 2, TotalCredits = 10, RemainingCredits = 10, ExpiryDate = now.AddMonths(2) },
            new CustomerPackage { Id = 10, CustomerId = 10, BusinessId = 1, TotalCredits = 10, RemainingCredits = 10, ExpiryDate = now.AddMonths(2) });

        modelBuilder.Entity<TimetableSchedule>().HasData(
            new TimetableSchedule { Id = 1, BusinessId = 1, ClassName = "Yoga Class", InstructorName = "John Doe", StartTime = now.AddDays(2).AddHours(10), EndTime = now.AddDays(2).AddHours(11), AvailableSlots = 2 },
            new TimetableSchedule { Id = 2, BusinessId = 1, ClassName = "HIIT", InstructorName = "May Lin", StartTime = now.AddDays(2).AddHours(10.5), EndTime = now.AddDays(2).AddHours(11.5), AvailableSlots = 5 },
            new TimetableSchedule { Id = 3, BusinessId = 1, ClassName = "Pilates", InstructorName = "Aung Ko", StartTime = now.AddDays(3).AddHours(9), EndTime = now.AddDays(3).AddHours(10), AvailableSlots = 1 },
            new TimetableSchedule { Id = 4, BusinessId = 2, ClassName = "Hot Yoga", InstructorName = "Nora", StartTime = now.AddDays(3).AddHours(10), EndTime = now.AddDays(3).AddHours(11), AvailableSlots = 3 },
            new TimetableSchedule { Id = 5, BusinessId = 2, ClassName = "Stretch", InstructorName = "Chris", StartTime = now.AddDays(4).AddHours(8), EndTime = now.AddDays(4).AddHours(9), AvailableSlots = 4 },
            new TimetableSchedule { Id = 6, BusinessId = 1, ClassName = "Spin", InstructorName = "David", StartTime = now.AddDays(4).AddHours(18), EndTime = now.AddDays(4).AddHours(19), AvailableSlots = 2 },
            new TimetableSchedule { Id = 7, BusinessId = 1, ClassName = "Boxing", InstructorName = "Thiri", StartTime = now.AddDays(5).AddHours(17), EndTime = now.AddDays(5).AddHours(18), AvailableSlots = 8 },
            new TimetableSchedule { Id = 8, BusinessId = 2, ClassName = "Meditation", InstructorName = "Zin", StartTime = now.AddDays(5).AddHours(7), EndTime = now.AddDays(5).AddHours(8), AvailableSlots = 10 },
            new TimetableSchedule { Id = 9, BusinessId = 1, ClassName = "Strength", InstructorName = "Myo", StartTime = now.AddDays(6).AddHours(12), EndTime = now.AddDays(6).AddHours(13), AvailableSlots = 6 },
            new TimetableSchedule { Id = 10, BusinessId = 2, ClassName = "Flow Yoga", InstructorName = "Ei", StartTime = now.AddDays(6).AddHours(16), EndTime = now.AddDays(6).AddHours(17), AvailableSlots = 1 });

        modelBuilder.Entity<Booking>().HasData(
            new Booking { Id = 1, CustomerId = 1, CustomerPackageId = 1, TimetableScheduleId = 1, Status = BookingStatus.Booked, BookedAt = now },
            new Booking { Id = 2, CustomerId = 5, CustomerPackageId = 5, TimetableScheduleId = 1, Status = BookingStatus.Booked, BookedAt = now },
            new Booking { Id = 3, CustomerId = 3, CustomerPackageId = 3, TimetableScheduleId = 10, Status = BookingStatus.Booked, BookedAt = now });

        modelBuilder.Entity<WaitlistEntry>().HasData(
            new WaitlistEntry { Id = 1, CustomerId = 6, CustomerPackageId = 6, TimetableScheduleId = 1, Status = WaitlistStatus.Waiting, JoinedAt = now.AddMinutes(1) },
            new WaitlistEntry { Id = 2, CustomerId = 7, CustomerPackageId = 7, TimetableScheduleId = 1, Status = WaitlistStatus.Waiting, JoinedAt = now.AddMinutes(2) });
    }
}
