namespace LiteDbMigrationSample.Models
{
    public class MigrationObject_V3 : MigrationObject_V2
    {
        public int? IntIdentifier { get; set; }

        public string OtherObjectId { get; set; }
    }
}
