using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LiteDbMigrations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLiteDbMigrations(this IServiceCollection services, Action<LiteDbMigrationOptions>? configurationOptions = null, ILiteDatabase? database = null)
        {
            return services.AddSingleton(provider => CreateMigrationRunner(provider, configurationOptions, database));
        }

        private static LiteDbMigrationRunner CreateMigrationRunner(IServiceProvider provider, Action<LiteDbMigrationOptions>? configurationOptions = null, ILiteDatabase? database = null)
        {
            var migrationResolver = new DependencyInjectionMigrationResolver(provider);
            LiteDbMigrationOptions options = new();
            configurationOptions?.Invoke(options);
            options.MigrationResolver = migrationResolver;
            if (database != null)
                options.Database = database;
            else if (string.IsNullOrEmpty(options.ConnectionString))
                options.Database = provider.GetRequiredService<ILiteDatabase>();
            var logger = provider.GetRequiredService<ILogger<LiteDbMigrationRunner>>();
            return new LiteDbMigrationRunner(options, logger);
        }
    }
}
