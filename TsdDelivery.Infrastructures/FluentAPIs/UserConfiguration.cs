using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.FluentAPIs;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Email).HasMaxLength(50);

        builder.Property(x => x.FullName).HasMaxLength(50);

        builder.Property(x => x.PhoneNumber).HasMaxLength(12);

        builder.HasOne(r => r.Role)
            .WithMany(ur => ur.Users)
            .HasForeignKey(pk => pk.RoleId);

    }
}
