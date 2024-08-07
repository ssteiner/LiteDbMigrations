namespace LiteDbMigrations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MigrationAttribute(long version) : Attribute
    {
        public long Version { get; set; } = version;

        public string Description { get; set; } = string.Empty;

        public bool UseTransactionOnUp { get; set; } = true;

        public bool UseTransactionOnDown { get; set; } = true;
    }
}
