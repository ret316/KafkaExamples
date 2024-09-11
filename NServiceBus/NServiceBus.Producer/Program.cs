using NServiceBus;
using NServiceBus.Producer.Components;
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
var transport = endpointConfiguration.UseTransport(new KafkaTransport());
//transport.ConnectionString("localhost:9092");  // Адрес брокера Kafka


var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
builder.Services.AddSingleton(endpointInstance);

//builder.Host.UseNServiceBus((context) =>
//{
//    var endpointConfiguration = new EndpointConfiguration("nservicebus");

//    var transport = endpointConfiguration
//        .UseTransport(new KafkaTransport());
//    transport.ConnectionString("localhost:9092");

//    endpointConfiguration.EnableInstallers();

//    return endpointConfiguration;
//});

var app = builder.Build();

//app.Lifetime.ApplicationStopping.Register(async () =>
//{
//    await endpointInstance.Stop();
//});

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
