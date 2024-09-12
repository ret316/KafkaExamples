using MassTransit;
using MassTransit.Consumer;
using MassTransit.Consumer.Components;
using MassTransit.Shared;

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

builder.Services.AddMassTransit(options =>
{
    options.UsingInMemory();

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
                e.UseInMemoryOutbox(context);
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
