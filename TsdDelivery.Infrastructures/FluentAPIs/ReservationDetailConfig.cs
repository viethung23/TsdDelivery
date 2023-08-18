using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Cryptography.X509Certificates;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.FluentAPIs;

public class ReservationDetailConfig : IEntityTypeConfiguration<ReservationDetail>
{
    public void Configure(EntityTypeBuilder<ReservationDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(rd => rd.Service)
            .WithMany(rd => rd.reservationDetails)
            .HasForeignKey(pk => pk.ServiceId);

        builder.HasOne(rd => rd.Reservation)
            .WithMany(rd => rd.reservationDetails)
            .HasForeignKey(pk => pk.ReservationId);
    }
}
