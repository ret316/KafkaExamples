using MassTransit;
using MassTransit.Producer.Components;
using MassTransit.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<MasstransitProducerContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=MTProducer;Username=postgres;Password=123qweDSA");
});

var npgBuilder = new NpgsqlConnectionStringBuilder("Host=localhost;Port=5432;Database=MTProducer;Username=postgres;Password=123qweDSA");

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

builder.Services.AddHostedService<MigrationHostedService<MasstransitProducerContext>>();

builder.Services.AddPostgresMigrationHostedService();

builder.Services.AddMassTransit(options =>
{
    options.SetEntityFrameworkSagaRepositoryProvider(r =>
    {
        r.ExistingDbContext<MasstransitProducerContext>();
        r.UsePostgres();
    });

    options.AddSagaRepository<JobSaga>()
    .EntityFrameworkRepository(r =>
    {
        r.ExistingDbContext<MasstransitProducerContext>();
        r.UsePostgres();
    });
    options.AddSagaRepository<JobTypeSaga>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<MasstransitProducerContext>();
            r.UsePostgres();
        });
    options.AddSagaRepository<JobAttemptSaga>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<MasstransitProducerContext>();
            r.UsePostgres();
        });

    options.SetKebabCaseEndpointNameFormatter();

    options.AddEntityFrameworkOutbox<MasstransitProducerContext>(o =>
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

        cfg.UseEntityFrameworkOutbox<MasstransitProducerContext>(context);
    });

    options.UsingPostgres((context, cfg) =>
    {
        cfg.UseSqlMessageScheduler();

        cfg.AutoStart = true;

        cfg.ConfigureEndpoints(context);
    });

    options.AddRider(rider =>
    {
        rider.AddProducer<string, MassTransitMessage>("masstransit-topic");
        rider.UsingKafka((context, k) =>
        {
            //k.Host("kafka:9091");
            k.Host("localhost:29091");
            k.ConfigureSend(c =>
            {
                c.UseRetry(o => o.Immediate(5));
                c.UseCircuitBreaker();
            });
        });
    });
});

builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
    {
        options.WaitUntilStarted = true;
        options.StartTimeout = TimeSpan.FromSeconds(10);
        options.StopTimeout = TimeSpan.FromSeconds(30);
        options.ConsumerStopTimeout = TimeSpan.FromSeconds(10);
    });
builder.Services.AddOptions<HostOptions>()
    .Configure(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1));

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
