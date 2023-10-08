using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Coordinates;
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
    [Authorize(Policy = "RequireUserRole")]
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
    [Authorize(Policy = "RequireAdminRole")]
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
    [ValidateModel]
    [Authorize(Policy = "RequireDriverRole")]
    public async Task<IActionResult> GetAwaitingDriverReservation(double Latitude = 0.0D, double  Longitude = 0.0D,double LatitudeDes = 0.0D, double LongitudeDes = 0.0D ,bool isNow = true)
    {
        var currentCoordinates = new CurrentCoordinates() { Latitude = Latitude, Longitude = Longitude };
        var destinationCoordinates = new DestinationCoordinates()
            { LatitudeDes = LatitudeDes, LongitudeDes = LongitudeDes };
        var response = await _reservationService.GetAwaitingDriverReservation(currentCoordinates,destinationCoordinates,isNow);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for driver
    /// </summary>
    /// <param name="id"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    [HttpGet]
    [ValidateModel]
    [ValidateGuid]
    [Authorize(Policy = "RequireDriverRole")]
    public async Task<IActionResult> GetAwaitingDriverReservationDetail(Guid reservationId,double Latitude = 0.0D, double  Longitude = 0.0D,double LatitudeDes = 0.0D, double LongitudeDes = 0.0D)
    {
        var currentCoordinates = new CurrentCoordinates() { Latitude = Latitude, Longitude = Longitude };
        var destinationCoordinates = new DestinationCoordinates()
            { LatitudeDes = LatitudeDes, LongitudeDes = LongitudeDes };
        var response = await _reservationService.GetAwaitingDriverReservationDetail(reservationId,currentCoordinates,destinationCoordinates);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for driver
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ValidateGuid]
    [Authorize(Policy = "RequireDriverRole")]
    public async Task<IActionResult> AcceptReservation(Guid driverId,Guid reservationId)
    {
        var response = await _reservationService.AcceptReservation(driverId, reservationId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Customer
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "RequireUserRole")]
    public async Task<IActionResult> GetReservationHistoryForUser()
    {
        var response = await _reservationService.GetReservationHistoryForUser();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Customer, Admin 
    /// </summary>
    /// <param name="reservationId"></param>
    /// <returns></returns>
    [HttpGet]
    [ValidateGuid]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetReservationHistoryDetailForUser(Guid reservationId)
    {
        var response = await _reservationService.GetReservationHistoryDetailForUser(reservationId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}