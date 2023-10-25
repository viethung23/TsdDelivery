using Microsoft.AspNetCore.Mvc;

namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/transactions")] 
[ApiController]
[ApiVersion("2.0")]
public class TransactionController : BaseController
{
    
}