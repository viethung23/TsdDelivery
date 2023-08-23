using Microsoft.AspNetCore.Mvc;

namespace TsdDelivery.Api.Controllers;

public class ReservationController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Calculate_order_total()
    {
        return Ok();
    }
}