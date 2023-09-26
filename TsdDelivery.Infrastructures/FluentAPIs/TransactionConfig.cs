using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.FluentAPIs;

public class TransactionConfig : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        
        builder.HasOne(rd => rd.Reservation)
            .WithMany(rd => rd.Transactions)
            .HasForeignKey(pk => pk.ReservationId);
        
        builder.HasOne(rd => rd.Wallet)
            .WithMany(rd => rd.Transactions)
            .HasForeignKey(pk => pk.WalletId);
    }
}