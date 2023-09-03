using Microsoft.AspNetCore.Mvc;

namespace TsdDelivery.Api.Controllers;
[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult test() => Ok("Test oke roi");
}