using LiteDbMigrations.Extensions;
using System.Reflection;

namespace LiteDbMigrations
{
    internal class MigrationLocator
    {
        internal static IEnumerable<MigrationWithAttribute> FindAllMigrations(LiteDbMigrationOptions options)
        {
            IEnumerable<Assembly> assembliesToSearch = options.Assemblies!.Count > 0 ? options.Assemblies : GetMigrationAssembliesFromCurrentApplication();
            List<MigrationWithAttribute> availableMigrations = [];
            foreach (var assembly in assembliesToSearch)
            {
                availableMigrations.AddRange(assembly.GetMigrationTypes()
                    .Select(x => new MigrationWithAttribute(() => options.MigrationResolver.Resolve(x), x.GetMigrationAttribute())));
            }
            return availableMigrations;
        }

        private static IEnumerable<Assembly> GetMigrationAssembliesFromCurrentApplication()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                return [entryAssembly];
            return [];
            //return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
