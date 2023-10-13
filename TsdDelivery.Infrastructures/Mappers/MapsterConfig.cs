using System.Reflection.PortableExecutable;
using Mapster;
using TsdDelivery.Application.Models.Reservation.Response;
using TsdDelivery.Application.Models.Service.Response;
using TsdDelivery.Application.Models.User.Response;
using TsdDelivery.Application.Models.Vehicle.Response;
using TsdDelivery.Application.Models.VehicleType.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Mappers;

public class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<VehicleType, VehicleTypeDetailResponse>()
            .Map(dest => dest.Services, src => src.services);
        
        config.NewConfig<Vehicle, VehicleResponse>()
            .Map(dest => dest.VehicleTypeDto, src => src.VehicleType);

        config.NewConfig<Reservation, ReservationResponse>()
            .Map(dest => dest.GoodsDto, src => src.Goods);

        config.NewConfig<Reservation, ReservationAwaitingDriverResponse>()
            .Map(dest => dest.GoodsDto, src => src.Goods);
        
        config.NewConfig<Reservation, ReservationAwaitingDriverDetailResponse>()
            .Map(dest => dest.GoodsDto, src => src.Goods)
            .Map(dest => dest.VehicleType,src => src.reservationDetails.Select(x => x.Service.VehicleType.VehicleTypeName).FirstOrDefault());
        
        config.NewConfig<Reservation, ReservationHistoryResponse>()
            .Map(dest => dest.GoodsName, src => src.Goods.Name)
            .Map(dest => dest.VehicleType,src => src.reservationDetails.Select(x => x.Service.VehicleType.VehicleTypeName).FirstOrDefault());

        config.NewConfig<Reservation, ReservationHistoryDetailResponse>()
            .Map(dest => dest.sender, src => src.User.FullName)
            .Map(dest => dest.GoodsDto, src => src.Goods)
            .Map(dest => dest.DriverDto, src => src.Driver);

        config.NewConfig<Reservation, ReservationsResponse>()
            .Map(dest => dest.GoodsDto, src => src.Goods)
            .Map(dest => dest.SenderDto, src => src.User);

        config.NewConfig<Reservation, ReservationResponsee>()
            .Map(dest => dest.GoodsDto, src => src.Goods);

        config.NewConfig<User, UserResponse>()
            .Map(dest => dest.RoleName, src => src.Role.RoleName)
            .Map(dest => dest.VehicleDto, src => src.Vehicles.FirstOrDefault())
            .Map(dest => dest.IsDelete, src => src.IsDeleted);
        //config.NewConfig<Service, ServiceResponse>();
    }
}