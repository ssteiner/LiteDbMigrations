using LiteDbMigrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LiteDbMigrationSample
{
    internal class ManualMigration
    {
        private readonly IConfiguration configuration;
        internal ManualMigration()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();
        }
        internal void Migrate()
        {
            var options = new LiteDbMigrationOptions
            {
                //ToVersion = 0, 0 => initial version
                ConnectionString = configuration.GetValue<string>("LiteDbConnectionString"), 
            };
            var logger = GetMigrationLogger();
            var migrationRunner = new LiteDbMigrationRunner(options, logger);
            migrationRunner.Run();
        }

        private ILogger<LiteDbMigrationRunner> GetMigrationLogger()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LiteDbMigrationSample.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger<LiteDbMigrationRunner> logger = loggerFactory.CreateLogger<LiteDbMigrationRunner>();
            return logger;
        }
    }
}
