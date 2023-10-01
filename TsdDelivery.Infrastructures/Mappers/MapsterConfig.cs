using Mapster;
using TsdDelivery.Application.Models.Reservation.Response;
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

        config.NewConfig<Reservation, ReservationResponse>()
            .Map(dest => dest.GoodsDto, src => src.Goods);

        config.NewConfig<Reservation, ReservationAwaitingDriverResponse>()
            .Map(dest => dest.GoodsDto, src => src.Goods);
        
        config.NewConfig<Reservation, ReservationAwaitingDriverDetailResponse>()
            .Map(dest => dest.GoodsDto, src => src.Goods);
        //config.NewConfig<Service, ServiceResponse>();
    }
}