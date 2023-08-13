﻿using TsdDelivery.Application.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TsdDelivery.Application.Services;

public class ClaimsService : IClaimsService
{
    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        // todo implementation to get the current userId
        var Id = httpContextAccessor.HttpContext?.User?.FindFirstValue("userId");
        GetCurrentUserId = string.IsNullOrEmpty(Id) ? Guid.Empty : Guid.Parse(Id);
    }

    public Guid GetCurrentUserId { get; }
}
