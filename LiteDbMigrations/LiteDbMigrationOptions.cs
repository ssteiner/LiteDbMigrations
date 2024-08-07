using LiteDB;
using System.Reflection;

namespace LiteDbMigrations
{
    public class LiteDbMigrationOptions(IMigrationResolver resolver)
    {
        public LiteDbMigrationOptions()
            :this(new BuildInMigrationResolver())
        {

        }

        public long? ToVersion { get; set; }

        public string? ConnectionString { get; set; }

        public ILiteDatabase? Database { get; set; }

        public string MigrationRecordsCollectionName { get; set; } = "_migrations";

        /// <summary>
        /// assemblies containing the migrations to run
        /// if none have been provided, the currently assembly is searched
        /// </summary>
        public List<Assembly>? Assemblies { get; set; } = [];

        public IMigrationResolver MigrationResolver { get; set; } = resolver;

        public IMigrationRecordStore? MigrationRecordStore { get; set; }
    }
}
