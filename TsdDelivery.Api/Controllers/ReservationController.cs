using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Coordinates;
using TsdDelivery.Application.Models.Reservation.Enum;
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
    /// Api for Driver
    /// </summary>
    /// <param name="Latitude"></param>
    /// <param name="Longitude"></param>
    /// <param name="LatitudeDes"></param>
    /// <param name="LongitudeDes"></param>
    /// <param name="isNow"></param>
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
    /// Api for Driver
    /// </summary>
    /// <param name="reservationId"></param>
    /// <param name="Latitude"></param>
    /// <param name="Longitude"></param>
    /// <param name="LatitudeDes"></param>
    /// <param name="LongitudeDes"></param>
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
    /// Api for Driver 
    /// </summary>
    /// <param name="driverId"></param>
    /// <param name="reservationId"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    /// <remarks>
    ///     0 - Nhận đơn, 
    ///     1 - Đã lấy hàng và đang giao, 
    ///     2 - Giao thành công
    /// </remarks>
    [HttpPost]
    [ValidateGuid]
    [Authorize(Policy = "RequireDriverRole")]
    public async Task<IActionResult> DriverReservationAction(Guid driverId,Guid reservationId, DriverReservationAction action)
    {
        var response = await _reservationService.DriverReservationAction(driverId, reservationId, action);
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

    /// <summary>
    /// Api for Driver
    /// </summary>
    /// <param name="driverId"></param>
    /// <returns></returns>
    [HttpGet("/api/current-reservation-of-driver")]
    [ValidateGuid]
    [Authorize(Policy = "RequireDriverRole")]
    public async Task<IActionResult> GetCurrentAcceptedReservationByDriver(Guid driverId)
    {
        var response = await _reservationService.GetCurrentAcceptedReservationbyDriver(driverId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("/api/current-reservation-of-user")]
    [ValidateGuid]
    [Authorize(Policy = "RequireDriverRole")]
    public async Task<IActionResult> GetCurrentAcceptedReservationByUser(Guid userId)
    {
        var response = await _reservationService.GetCurrentAcceptedReservationbyUser(userId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
}