using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Contracts.Common;
using TsdDelivery.Application.Models;

namespace TsdDelivery.Api.Controllers.V1;

[Route("api/[controller]/[action]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult HandleErrorResponse(List<Error> errors)
    {
        var apiError = new ErrorResponse();

        if (errors.Any(e => e.Code == ErrorCode.NotFound))
        {
            var error = errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound);

            apiError.StatusCode = 404;
            apiError.StatusPhrase = "Not Found";
            apiError.Timestamp = DateTime.Now;
            apiError.Errors.Add(error.Message);

            return NotFound(apiError);
        }
        if(errors.Any(e => e.Code == ErrorCode.NoContent))
        {
            var error = errors.FirstOrDefault(e => e.Code == ErrorCode.NoContent);

            apiError.StatusCode = 204;
            apiError.StatusPhrase = "No Content";
            apiError.Timestamp = DateTime.Now;
            apiError.Errors.Add(error.Message);

            return NoContent();
        }

        apiError.StatusCode = 400;
        apiError.StatusPhrase = "Bad request";
        apiError.Timestamp = DateTime.Now;
        errors.ForEach(e => apiError.Errors.Add(e.Message));
        return StatusCode(400, apiError);
    }
}
