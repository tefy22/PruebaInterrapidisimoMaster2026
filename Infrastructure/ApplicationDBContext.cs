using Application.Exceptions;
using Domain.Abstractions;
using Domain.Registrations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public sealed class ApplicationDbContext :DbContext , IUnitOfWork
    {
        private readonly IPublisher _publisher;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher) : base(options)
        {
            _publisher = publisher;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<SharedSubjectDto>(eb =>
            {
                eb.HasNoKey();
                eb.Property<string>(nameof(SharedSubjectDto.SubjectId)).HasColumnName("SubjectId");
                eb.Property<string>(nameof(SharedSubjectDto.SubjectName)).HasColumnName("SubjectName");
                eb.Property<string>(nameof(SharedSubjectDto.NameStudent)).HasColumnName("NameStudent");
            });

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        {
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);

                //pubicacion de los domainevents
                await PublishDomainEventAsync();

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("La excepcion por concurrencia se disparó", ex);
            }
        }

        private async Task PublishDomainEventAsync()
        {
            var domainEvents = ChangeTracker.Entries<Entity>()
                .Select(e => e.Entity)
                .SelectMany(e =>
                {
                    var domainEvents = e.GetDomainEvents();
                    e.ClearDomainEvents();
                    return domainEvents;
                }).ToList();

            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent);
            }
        }
    }
}
