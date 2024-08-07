using LiteDB;
using LiteDbMigrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiteDbMigrationSample
{
    internal class IocMigration
    {
        private readonly IConfiguration configuration;
        internal IocMigration()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", false, true)
                .Build();
        }
        internal void Migrate()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            // add and configure migrations. 
            // you could also instantiate the database and provide it to the options
            // normally this would run in your Startup class, just add using LiteDbMigrations to get the extension method
            builder.Services.AddLiteDbMigrations(options => 
            {
                options.ToVersion = null; // 0 => initial version, null => run all migrations that have been found
                //options.Database = ... if you already have an ILiteDatabase, you can provide it here
            });

            ILiteDatabase db = new LiteDatabase(configuration.GetValue<string>("LiteDbConnectionString"));
            builder.Services.AddSingleton(db);

            using IHost host = builder.Build();
            // you need a MigrationRunner to migrate, normally this would run in your Startup class in the Configure method
            var migration = host.Services.GetRequiredService<LiteDbMigrationRunner>();
            migration.Run();
        }
    }
}
