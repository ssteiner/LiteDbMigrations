using System.Reflection;

namespace LiteDbMigrations.Extensions
{
    internal static class LiteDbMigrationHelpers
    {

        public static IEnumerable<Type> GetMigrationTypes(this Assembly assembly)
        {
            return assembly.GetUsableTypes().Where(u => IsMigrationType(u));
        }

        private static IEnumerable<Type> GetUsableTypes(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException r)
            {
                return r.Types?.Where(u => u != null)!;
            }
        }

        public static MigrationAttribute GetMigrationAttribute(this Type type)
        {
            var attribute = type.GetCustomAttribute<MigrationAttribute>(true);
            return attribute == null
                ? throw new InvalidOperationException("MissingThe migration must be decorated with a [Migration] attribute")
                : attribute;
        }


        public static readonly Func<Type, bool> IsMigrationType = t => typeof(IDatabaseMigration).IsAssignableFrom(t)
            && !t.IsAbstract && t.GetCustomAttribute<MigrationAttribute>() != null;

    }
}
