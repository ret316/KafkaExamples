using Oakton.Resources;
using Wolverine;
using Wolverine.Consumer.Components;
using Wolverine.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.UseWolverine(options =>
{
    options.ConfigureKafka("localhost:29091")
        .ConfigureClient(o =>
        {
            o.AllowAutoCreateTopics = true;
            o.BootstrapServers = "localhost:29091";
        })
    .AutoProvision();

    options.ListenToKafkaTopic("wolverine-topic")
        .ProcessInline();

    options.Services.AddResourceSetupOnStartup();
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

await app.RunAsync();
