using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.FluentAPIs;

public class VehicleConfig : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(m => m.User)
                .WithMany(ux => ux.Vehicles)
                .HasForeignKey(pk => pk.UserId);

        builder.HasOne(x => x.VehicleType)
            .WithMany(v => v.Vehicles)
            .HasForeignKey(pk => pk.VehicleTypeId);
    }
}
