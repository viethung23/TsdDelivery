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

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateReservation(CreateReservationRequest request)
    {
        var response = await _reservationService.CreateReservation(request);
        return (response.IsError) ? HandleErrorResponse(response.Errors) : Ok("Create Success");
    }
}