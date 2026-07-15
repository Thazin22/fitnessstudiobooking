using FitnessStudioBooking.Application;
using Microsoft.AspNetCore.Mvc;

namespace FitnessStudioBooking.Api.Controllers;

[ApiController]
[Route("api/packages")]
public sealed class PackagesController(PackageService packageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AvailablePackageDto>>> List(CancellationToken cancellationToken)
    {
        return Ok(await packageService.ListAsync(cancellationToken));
    }

    [HttpPost("purchase")]
    public async Task<ActionResult<CustomerPackageDto>> Purchase(PurchasePackageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await packageService.PurchaseAsync(request, cancellationToken));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}
