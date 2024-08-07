namespace LiteDbMigrationSample.Models
{
    public class MigrationObject_V4 : MigrationObject_V2
    {
        public int? IntIdentifier2 { get; set; }

        public string OtherObjectId { get; set; }
    }
}
