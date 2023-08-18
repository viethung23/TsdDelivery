using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.FluentAPIs;

public class ReservationConfig : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Goods);

        builder.HasOne(u => u.User)
            .WithMany(r => r.Reservations)
            .HasForeignKey(pk => pk.UserId);

        builder.HasOne(u => u.User)
            .WithMany(r => r.Reservations)
            .HasForeignKey(pk => pk.DriverId);
    }
}
