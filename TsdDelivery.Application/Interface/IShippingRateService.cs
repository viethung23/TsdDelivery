using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.ShippingRate.Request;
using TsdDelivery.Application.Models.ShippingRate.Response;

namespace TsdDelivery.Application.Interface;

public interface IShippingRateService
{
    public Task<OperationResult<List<ShippingRateResponse>>> GetAllShippingRate();
    public Task<OperationResult<ShippingRateResponse>> CreateShippingRate(CreateShippingRateRequest request);
    public Task<OperationResult<ShippingRateResponse>> DeleteShippingRate(Guid shippingRateId, Guid serviceId);
    public Task<OperationResult<List<ShippingRateResponse>>> GetShippingRatesByService(Guid serviceId);
}