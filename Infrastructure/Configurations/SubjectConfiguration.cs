using Domain.Subjects;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable(nameof(Subject));

            builder.HasKey(x => x.Id);
            var nameConverter = new ValueConverter<Name, string>(n => n.Value, v => Name.Create(v).Value);
            var creditsConverter = new ValueConverter<Credits, int>(ln => ln.Value, v => Credits.Create(v).Value);

            builder.Property(x => x.Name)
                    .HasConversion(nameConverter)
                    .IsRequired()
                    .HasMaxLength(200);

            builder.Property(x => x.Credits)
                    .HasConversion(creditsConverter)
                    .IsRequired();

            builder.Property(x => x.Status)
               .HasConversion<int>()
               .IsRequired();

            builder.HasOne(s => s.User)
                   .WithMany()
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
