using NServiceBus;
using NServiceBus.Consumer.Components;
using NServiceBus.Transport.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Настройка NServiceBus
var endpointConfiguration = new EndpointConfiguration("nservicebus-topic");

// Использование Kafka как транспортного механизма
var transport = endpointConfiguration.UseTransport<KafkaTransport>();
transport.ConnectionString("localhost:9092");  // Адрес брокера Kafka

var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
builder.Services.AddSingleton(endpointInstance);

var app = builder.Build();

app.MapDefaultEndpoints();

app.Lifetime.ApplicationStopping.Register(async () =>
{
    await endpointInstance.Stop();
});

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
