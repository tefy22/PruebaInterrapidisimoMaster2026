using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(nameof(Role));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                   .HasConversion(d => d.ToString(), s => ParseRolesDetailsOrThrow(s))
                   .HasMaxLength(50)
                   .IsRequired()
                   .HasColumnName("Description");

        }

        private static RolesDetails ParseRolesDetailsOrThrow(string s)
        {
            if (Enum.TryParse<RolesDetails>(s, out var r))
                return r;
            throw new InvalidOperationException($"Valor inválido para Roles.Description en la BD: '{s}'");
        }
    }
}
