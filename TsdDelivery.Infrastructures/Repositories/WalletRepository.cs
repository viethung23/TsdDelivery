using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class WalletRepository : GenericRepository<Wallet>,IWalletRepository
{
    public WalletRepository(AppDbContext appDbContext
        ,IClaimsService claimsService
        ,ICurrentTime currentTime) : base(appDbContext , claimsService , currentTime)
    {
        
    }
    
}