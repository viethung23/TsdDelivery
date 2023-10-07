using TsdDelivery.Application.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TsdDelivery.Application.Services;

public class ClaimsService : IClaimsService
{
    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        // todo implementation to get the current userId
        var Id = httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId");
        GetCurrentUserId = string.IsNullOrEmpty(Id) ? Guid.Empty : Guid.Parse(Id);
        Host = httpContextAccessor.HttpContext?.Request!.Host!.ToString()!;
        IpAddress = httpContextAccessor.HttpContext!.Connection!.LocalIpAddress!.ToString();
    }

    public Guid GetCurrentUserId { get; }
    public string Host { get; }
    public string IpAddress { get; }
}
