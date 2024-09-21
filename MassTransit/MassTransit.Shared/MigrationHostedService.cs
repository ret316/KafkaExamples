﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransit.Shared;

public class MigrationHostedService<TDbContext> :
    IHostedService
    where TDbContext : DbContext
{
    readonly ILogger<MigrationHostedService<TDbContext>> _logger;
    readonly IServiceScopeFactory _scopeFactory;
    TDbContext? _context;
    IServiceScope? _scope;

    public MigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<MigrationHostedService<TDbContext>> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Applying migrations for {DbContext}", TypeCache<TDbContext>.ShortName);

        _scope = _scopeFactory.CreateScope();

        _context = _scope.ServiceProvider.GetRequiredService<TDbContext>();

        await _context.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}