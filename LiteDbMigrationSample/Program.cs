using LiteDbMigrationSample;

ManualMigration manualMigration = new();
manualMigration.Migrate();

IocMigration iocMigration = new();
iocMigration.Migrate();
