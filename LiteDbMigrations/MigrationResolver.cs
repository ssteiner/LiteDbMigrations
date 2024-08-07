namespace LiteDbMigrations
{
    public interface IMigrationResolver
    {
        DatabaseMigration Resolve(Type migrationType);
    }

    internal class DependencyInjectionMigrationResolver(IServiceProvider serviceProvider) : IMigrationResolver
    {
        public DatabaseMigration Resolve(Type migrationType)
        {
            return (DatabaseMigration)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(serviceProvider, migrationType);
        }
    }

    internal class BuildInMigrationResolver : IMigrationResolver
    {
        public DatabaseMigration Resolve(Type migrationType)
        {
            return (DatabaseMigration)Activator.CreateInstance(migrationType)!;
        }
    }
}
