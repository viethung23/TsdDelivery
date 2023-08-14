using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.FluentAPIs;

public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // set PK
        builder.HasKey(k => new {k.UserId, k.RoleId});

        // set relation
        builder.HasOne(m => m.User)
                .WithMany(ux => ux.UserRoles)
                .HasForeignKey(pk => pk.UserId);

        builder.HasOne(r => r.Role)
            .WithMany(ur => ur.UserRoles)
            .HasForeignKey(pk => pk.RoleId);
    }
}
