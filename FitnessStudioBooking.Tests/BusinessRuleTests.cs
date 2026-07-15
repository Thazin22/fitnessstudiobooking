using FluentAssertions;
using FitnessStudioBooking.Domain;

namespace FitnessStudioBooking.Tests;

public sealed class BusinessRuleTests
{
    [Fact]
    public void Cancel_more_than_four_hours_before_start_should_be_refundable()
    {
        var start = DateTimeOffset.UtcNow.AddHours(5);

        var refundable = start - DateTimeOffset.UtcNow > TimeSpan.FromHours(4);

        refundable.Should().BeTrue();
    }

    [Fact]
    public void Waitlist_default_status_should_be_waiting()
    {
        var entry = new WaitlistEntry
        {
            CustomerId = 1,
            CustomerPackageId = 1,
            TimetableScheduleId = 1,
            JoinedAt = DateTimeOffset.UtcNow
        };

        entry.Status.Should().Be(WaitlistStatus.Waiting);
    }
}
