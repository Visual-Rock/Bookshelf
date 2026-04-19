using Bookshelf.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.MigrationService;

public class Worker(
    ILogger<Worker> logger,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime applicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Migrating database bookshelf at: {time}", DateTime.Now);

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookshelfContext>();

            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => { await db.Database.MigrateAsync(stoppingToken); });

            logger.LogInformation("Database bookshelf migration completed successfully at: {time}", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database bookshelf migration failed at: {time}", DateTime.Now);
        }
        finally
        {
            applicationLifetime.StopApplication();
        }
    }
}