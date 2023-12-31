﻿using Microsoft.EntityFrameworkCore.Storage;
using TsdDelivery.Application.Repositories;

namespace TsdDelivery.Application;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }
    public IVehicleTypeReposiory VehicleTypeReposiory { get; }
    public IVehicleRepository VehicleRepository { get; }
    public IServiceRepository ServiceRepository { get; }
    public IShippingRateRepository ShippingRateRepository { get; }
    public IReservationRepository ReservationRepository { get; }
    public IReservationDetailRepository ReservationDetailRepository { get; }
    public IWalletRepository WalletRepository { get; }
    public ITransactionRepository TransactionRepository { get; }
    public IUserLoginRepository UserLoginRepository { get; }

    public Task<int> SaveChangeAsync();
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
