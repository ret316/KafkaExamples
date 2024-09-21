using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Shared;

public class MasstransitProducerContext : SagaDbContext
{
    public MasstransitProducerContext(DbContextOptions<MasstransitProducerContext> options) : base(options)
    {
        
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new JobTypeSagaMap(false);
            yield return new JobSagaMap(false);
            yield return new JobAttemptSagaMap(false);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
