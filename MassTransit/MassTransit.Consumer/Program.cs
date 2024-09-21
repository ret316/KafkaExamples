using MassTransit;
using MassTransit.Consumer;
using MassTransit.Consumer.Components;
using MassTransit.Shared;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<MessageCollection>();

builder.Services.AddDbContext<MasstransitConsumerContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=MTConsumer;Username=postgres;Password=123qweDSA");
});

var npgBuilder = new NpgsqlConnectionStringBuilder("Host=localhost;Port=5432;Database=MTConsumer;Username=postgres;Password=123qweDSA");

builder.Services.AddOptions<SqlTransportOptions>().Configure(options =>
{
    options.Host = npgBuilder.Host;
    options.Database = npgBuilder.Database;
    options.Schema = "transport";
    options.Username = npgBuilder.Username;
    options.Password = npgBuilder.Password;
    options.AdminUsername = npgBuilder.Username;
    options.AdminPassword = npgBuilder.Password;
});

builder.Services.AddHostedService<MigrationHostedService<MasstransitConsumerContext>>();

builder.Services.AddPostgresMigrationHostedService();

builder.Services.AddMassTransit(options =>
{
    options.SetEntityFrameworkSagaRepositoryProvider(r =>
    {
        r.ExistingDbContext<MasstransitConsumerContext>();
        r.UsePostgres();
    });

    options.AddSagaRepository<JobSaga>()
    .EntityFrameworkRepository(r =>
    {
        r.ExistingDbContext<MasstransitConsumerContext>();
        r.UsePostgres();
    });
    options.AddSagaRepository<JobTypeSaga>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<MasstransitConsumerContext>();
            r.UsePostgres();
        });
    options.AddSagaRepository<JobAttemptSaga>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<MasstransitConsumerContext>();
            r.UsePostgres();
        });

    options.SetKebabCaseEndpointNameFormatter();

    options.AddEntityFrameworkOutbox<MasstransitConsumerContext>(o =>
    {
        o.UsePostgres();
    });

    options.AddConfigureEndpointsCallback((context, _, cfg) =>
    {
        cfg.UseDelayedRedelivery(r =>
        {
            r.Interval(10000, 15000);
        });

        cfg.UseMessageRetry(r =>
        {
            r.Interval(25, 50);
        });

        cfg.UseEntityFrameworkOutbox<MasstransitConsumerContext>(context);
    });

    options.UsingPostgres((context, cfg) =>
    {
        cfg.UseSqlMessageScheduler();

        cfg.AutoStart = true;

        cfg.ConfigureEndpoints(context);
    });

    options.AddRider(rider =>
    {
        rider.AddConsumer<MassTransitMessageConsumer>();

        rider.UsingKafka((context, k) =>
        {
            //k.Host("kafka:9091");
            k.Host("localhost:29091");

            k.TopicEndpoint<MassTransitMessage>(/*"^topic-[0-9]*"*/ "masstransit-topic", "consumer-group-name", e =>
            {
                //e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<MassTransitMessageConsumer>(context);
                e.CreateIfMissing();
                e.UseMessageRetry(r =>
                {
                    r.Immediate(5);
                });
                //e.UseInMemoryOutbox(context);

                e.UseEntityFrameworkOutbox<MasstransitConsumerContext>(context);
                e.UseCircuitBreaker();
            });
        });
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
