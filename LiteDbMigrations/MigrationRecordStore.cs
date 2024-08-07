using LiteDB;

namespace LiteDbMigrations
{
    public class MigrationRecordStore(ILiteDatabase db, LiteDbMigrationOptions options) : IMigrationRecordStore
    {
        public void Delete(long version)
        {
            GetMigrationCollection().DeleteMany(u => u.Version == version);
        }

        public MigrationRecord GetMostRecentMigration()
        {
            var latestMigration = GetMigrationCollection().Query().OrderByDescending(u => u.Version).FirstOrDefault();
            return latestMigration ?? new MigrationRecord { Version = 0 };
        }

        public MigrationRecord Load(long version)
        {
            return GetMigrationCollection().FindOne(u => u.Version == version);
        }

        public void Store(MigrationRecord migration)
        {
            GetMigrationCollection().Insert(migration);
        }

        private ILiteCollection<MigrationRecord> GetMigrationCollection()
        {
            if (!string.IsNullOrEmpty(options.MigrationRecordsCollectionName))
                return db.GetCollection<MigrationRecord>(options.MigrationRecordsCollectionName);
            return db.GetCollection<MigrationRecord>();
        }
    }

    public interface IMigrationRecordStore
    {
        MigrationRecord Load(long version);

        /// <summary>
        /// stores the migration
        /// </summary>
        /// <param name="migrationId"></param>
        void Store(MigrationRecord migrationRecord);

        void Delete(long version);

        MigrationRecord GetMostRecentMigration();
    }
}
