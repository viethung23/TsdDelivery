﻿using System.Security.Claims;
using TsdDelivery.Application.Interface;

namespace TsdDelivery.Api.Services;

public class ClaimsService : IClaimsService
{
    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        // todo implementation to get the current userId
        var Id = httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId");
        GetCurrentUserId = string.IsNullOrEmpty(Id) ? Guid.Empty : Guid.Parse(Id);
    }

    public Guid GetCurrentUserId { get; }
}
