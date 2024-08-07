namespace LiteDbMigrations
{
    public class MigrationRecord: IMigrationRecord
    {
        public string Id { get; set; }

        public string MigrationId { get; set; }

        public long Version { get; set; }

        public DateTime ExecutedOn { get; set; } = DateTime.Now;
    }

    public interface IMigrationRecord
    {
        string Id { get; }

        long Version { get; }
    }

}
