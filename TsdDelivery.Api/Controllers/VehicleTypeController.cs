using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TsdDelivery.Api.Controllers;

public class VehicleTypeController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllVehicleType()
    {
        return Ok();
    }
}
