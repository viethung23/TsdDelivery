using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Reservation.Request;

namespace TsdDelivery.Api.Controllers;

public class ReservationController : BaseController
{
    private readonly IReservationService _reservationService;
    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }
    
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Calculate_Amount_By_Services_And_Km(CalculatePriceRequest request)
    {
        var response = await _reservationService.CalculateTotalPrice(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Customer
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateModel]
    [Authorize]
    public async Task<IActionResult> CreateReservation(CreateReservationRequest request)
    {
        var response = await _reservationService.CreateReservation(request);
        return (response.IsError) ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllReservation()
    {
        var response = await _reservationService.GetAllReservation();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for driver
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAwaitingDriverReservation()
    {
        var response = await _reservationService.GetAwaitingDriverReservation();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for driver
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AcceptReservation(Guid driverId,Guid reservationId)
    {
        return Ok();
    }
}