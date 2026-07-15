using FitnessStudioBooking.Application;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudioBooking.Api.Controllers;

[ApiController]
public sealed class BookingsController(BookingService bookingService) : ControllerBase
{
    [HttpPost("api/bookings")]
    public async Task<IActionResult> Book(BookClassRequest request, CancellationToken cancellationToken)
    {
        var result = await bookingService.BookAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("api/bookings/cancel")]
    public async Task<IActionResult> Cancel(CancelBookingRequest request, CancellationToken cancellationToken)
    {
        var result = await bookingService.CancelAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("api/waitlist")]
    public async Task<IActionResult> JoinWaitlist(JoinWaitlistRequest request, CancellationToken cancellationToken)
    {
        var result = await bookingService.JoinWaitlistAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
