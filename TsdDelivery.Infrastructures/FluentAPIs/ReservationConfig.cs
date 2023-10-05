using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.FluentAPIs;

public class ReservationConfig : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(seed: 1000);

        builder.OwnsOne(x => x.Goods)
            .Property(x => x.Length).HasColumnType("decimal(18,2)");
        
        builder.OwnsOne(x => x.Goods)
            .Property(x => x.Height).HasColumnType("decimal(18,2)");
        
        builder.OwnsOne(x => x.Goods)
            .Property(x => x.Width).HasColumnType("decimal(18,2)");
        
        builder.OwnsOne(x => x.Goods)
            .Property(x => x.Weight).HasColumnType("decimal(18,2)");
        
        builder.Property(x => x.TotallPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Distance).HasColumnType("decimal(18,2)");

        builder.HasOne(u => u.Driver)
            .WithMany(r => r.ReservationDrivers)
            .HasForeignKey(pk => pk.DriverId);
        
        builder.HasOne(u => u.User)
            .WithMany(r => r.ReservationUsers)
            .HasForeignKey(pk => pk.UserId);

        
    }
}
