using FitnessStudioBooking.Application;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudioBooking.Api.Controllers;

[ApiController]
[Route("api/timetable")]
public sealed class TimetableController(TimetableService timetableService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TimetableDto>>> List([FromQuery] int? business, [FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        return Ok(await timetableService.ListAsync(business, date, cancellationToken));
    }
}
