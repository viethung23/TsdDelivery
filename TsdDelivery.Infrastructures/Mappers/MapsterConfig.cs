using Mapster;
using TsdDelivery.Application.Models.Service.Response;
using TsdDelivery.Application.Models.VehicleType.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Mappers;

public class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<VehicleType, VehicleTypeDetailResponse>()
            .Map(dest => dest.Services, src => src.services);
        
        //config.NewConfig<Service, ServiceResponse>();
    }
}