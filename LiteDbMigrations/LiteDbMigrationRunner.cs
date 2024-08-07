using LiteDB;
using Microsoft.Extensions.Logging;

namespace LiteDbMigrations
{
    public class LiteDbMigrationRunner
    {
        private readonly ILiteDatabase db;
        private readonly LiteDbMigrationOptions options;
        private readonly ILogger<LiteDbMigrationRunner> logger;

        public LiteDbMigrationRunner(LiteDbMigrationOptions options, ILogger<LiteDbMigrationRunner> logger)
        {
            if (options.Database != null)
                db = options.Database;
            else if (options.ConnectionString != null)
                db = new LiteDatabase(options.ConnectionString);
            else
                throw new ArgumentException("Neither Database nor ConnectionString have been provided");
            this.options = options;
            this.logger = logger;
        }

        public void Run()
        {
            RunMigrations();
        }

        private void RunMigrations()
        {
            var migrations = MigrationLocator.FindAllMigrations(options);
            var migrationRecordStore = options.MigrationRecordStore ?? new MigrationRecordStore(db, options);
            logger.LogInformation("Starting migrations");
            var currentDbVersion = migrationRecordStore.GetMostRecentMigration();
            Direction direction = Direction.Up;

            long? toVersion = options.ToVersion ?? migrations.OrderByDescending(x => x.Attribute.Version).FirstOrDefault()?.Attribute.Version;

            if (toVersion.HasValue)
            {
                if (toVersion == currentDbVersion.Version)
                {
                    logger.LogInformation("Current database version matches desired version {version}. Migrations complete", toVersion);
                    return;
                }
                if (currentDbVersion.Version > toVersion)
                    direction = Direction.Down;

                logger.LogInformation("Going to migrate from current version {currentVersion} to {desiredVersion}", currentDbVersion.Version, toVersion);

                IEnumerable<MigrationWithAttribute> migrationsToRun = direction == Direction.Up
                    ? migrations.OrderBy(u => u.Attribute.Version).Where(u => u.Attribute.Version <= toVersion && u.Attribute.Version > currentDbVersion.Version)
                    : migrations.OrderByDescending(u => u.Attribute.Version).Where(u => u.Attribute.Version > toVersion && u.Attribute.Version <= currentDbVersion.Version);
                var migrationExecuted = false;
                foreach (var migration in migrationsToRun)
                {
                    ExecuteMigration(direction, migration, migrationRecordStore);
                    migrationExecuted = true;
                }
                if (migrationExecuted)
                    db.Checkpoint();
            }
            logger.LogInformation("Migrations complete");
        }

        private void ExecuteMigration(Direction direction, MigrationWithAttribute migrationWithAttribute, IMigrationRecordStore migrationRecordStore)
        {
            var migration = migrationWithAttribute.Migration();
            logger.LogInformation("Executing Migration {migrationName} to version {version}, direction {direction}", migration.GetType().Name, migrationWithAttribute.Attribute.Version, direction);
            var useTransaction = direction == Direction.Up ? migrationWithAttribute.Attribute.UseTransactionOnUp : migrationWithAttribute.Attribute.UseTransactionOnDown;
            if (useTransaction)
                db.BeginTrans();
            if (direction == Direction.Up)
            {
                migration.Up(db);
                var upRecord = new MigrationRecord { Id = ObjectId.NewObjectId().ToString(), Version = migrationWithAttribute.Attribute.Version, MigrationId = migration.GetType().Name };
                migrationRecordStore.Store(upRecord);
            }
            else
            {
                migration.Down(db);
                migrationRecordStore.Delete(migrationWithAttribute.Attribute.Version);
            }
            if (useTransaction)
                db.Commit();
            logger.LogInformation("Migration {migrationName} to version {version}, direction {direction} complete", migration.GetType().Name, migrationWithAttribute.Attribute.Version, direction);
        }

        enum Direction { Up, Down }
    }
}
