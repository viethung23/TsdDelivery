using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.ShippingRate.Request;
using TsdDelivery.Application.Models.ShippingRate.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services.V1;

public class ShippingRateService : IShippingRateService
{
    private readonly IUnitOfWork _unitOfWork;

    public ShippingRateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<OperationResult<List<ShippingRateResponse>>> GetAllShippingRate()
    {
        var result = new OperationResult<List<ShippingRateResponse>>();
        try
        {
            var shippingRates = await _unitOfWork.ShippingRateRepository.GetAllAsync();
            var list = new List<ShippingRateResponse>();
            foreach (var shippingRate in shippingRates)
            {
                var x = new ShippingRateResponse()
                {
                    Id = shippingRate.Id,
                    Price = shippingRate.Price,
                    KmFrom = shippingRate.KmFrom,
                    KmTo = shippingRate.KmTo
                };
                list.Add(x);
            }
            result.Payload = list;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<ShippingRateResponse>> CreateShippingRate(CreateShippingRateRequest request)
    {
        var result = new OperationResult<ShippingRateResponse>();
        try
        {
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
            var shippingRate = new ShippingRate()
            {
                KmTo = request.KmTo,
                Price = request.Price,
                KmFrom = request.KmFrom,
                Service = service
            };
            await _unitOfWork.ShippingRateRepository.AddAsync(shippingRate);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<ShippingRateResponse>> DeleteShippingRate(Guid shippingRateId, Guid serviceId)
    {
        var result = new OperationResult<ShippingRateResponse>();
        try
        {
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId);
            var shippingRate = await _unitOfWork.ShippingRateRepository.GetByIdAsync(shippingRateId);
            await _unitOfWork.ShippingRateRepository.Delete(shippingRate);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<List<ShippingRateResponse>>> GetShippingRatesByService(Guid serviceId)
    {
        var result = new OperationResult<List<ShippingRateResponse>>();
        try
        {
            var entity = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId);
            var shippingRates = await _unitOfWork.ShippingRateRepository.GetMulti(x => x.ServiceId == serviceId);
            var list = new List<ShippingRateResponse>();
            foreach (var shippingRate in shippingRates)
            {
                var x = new ShippingRateResponse()
                {
                    Id = shippingRate.Id,
                    Price = shippingRate.Price,
                    KmFrom = shippingRate.KmFrom,
                    KmTo = shippingRate.KmTo
                };
                list.Add(x);
            }
            result.Payload = list;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }
}