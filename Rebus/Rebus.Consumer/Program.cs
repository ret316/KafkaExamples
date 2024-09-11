using Rebus.Config;
using Rebus.Consumer;
using Rebus.Consumer.Components;
using Rebus.Handlers;
using Rebus.Kafka;
using Rebus.Routing.TypeBased;
using Rebus.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddRebus(configure =>
{
    return configure
        .Transport(t => t.UseKafka(
            "localhost:9092", // Адрес Kafka-брокера
            "rebus-topic") // Название топика
        )
        .Routing(r => r.TypeBased().MapAssemblyOf<RebusMessage>("rebus-topic"));
});

builder.Services.AddScoped<IHandleMessages<RebusMessage>, RebusConsumer>();

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
