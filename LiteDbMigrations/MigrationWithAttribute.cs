namespace LiteDbMigrations
{
    internal class MigrationWithAttribute
    {
        internal MigrationWithAttribute(Func<DatabaseMigration> migration, MigrationAttribute attribute)
        {
            Migration = migration;
            Attribute = attribute;
        }

        internal Func<DatabaseMigration> Migration { get; set; }

        internal MigrationAttribute Attribute { get; set; }
    }
}
