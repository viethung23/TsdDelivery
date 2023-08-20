﻿using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Models.VehicleType.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface;

public interface IVehicleService
{
    public Task<OperationResult<List<VehicleTypeResponse>>> GetAllVehicleType();
    public Task<OperationResult<VehicleTypeResponse>> CreateVehicleType(CreateVehicleType request, IFormFile? blob = null);
    public Task<OperationResult<VehicleTypeResponse>> DeleteVehicleType(Guid id);
   
}