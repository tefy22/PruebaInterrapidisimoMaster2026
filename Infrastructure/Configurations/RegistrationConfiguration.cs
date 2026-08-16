using Domain.Registrations;
using Domain.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal sealed class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.ToTable(nameof(Registration));
            builder.HasKey(r => r.Id);

            builder.Property(r => r.StudentId).IsRequired();
            builder.Property(r => r.RegistrationDate).IsRequired();
            builder.Property(r => r.Status).HasConversion<int>();

            // Mapear la navegación pública Details y usar acceso por campo
            builder.Metadata.FindNavigation(nameof(Registration.Details))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);


        // Reemplazar la conversión de lectura de Rating para extraer el valor del Result
            builder.OwnsMany<RegistrationDetail>(r => r.Details, navigationBuilder =>
            {
                navigationBuilder.ToTable("RegistrationDetails");
                navigationBuilder.HasKey(r => r.Id);
                navigationBuilder.Property<Guid>(r => r.SubjectId).IsRequired();
                navigationBuilder.Property(d => d.Rating)
                    .HasConversion(
                        rating => rating.Value,              // Cómo se guarda en la BD
                        value => Rating.Create(value).Value  // Cómo se reconstruye al leer de la BD
                    )
                    .HasColumnName("Rating");
            });
            
        }
    }
}
