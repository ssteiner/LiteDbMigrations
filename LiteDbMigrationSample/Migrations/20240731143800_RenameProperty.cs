using LiteDB;
using LiteDbMigrations;
using LiteDbMigrationSample.Models;

namespace LiteDbMigrationSample.Migrations
{
    [Migration(20240731143800)]
    internal class RenameProperty : DatabaseMigration
    {
        private readonly string collectionName = LiteDbConstants.MigrationObjectCollection;

        public override void Up(ILiteDatabase db)
        {
            var script = "{ IntIdentifier2: $.IntIdentifier }";
            var filter = $"IntIdentifier != null";

            var col = db.GetCollection<MigrationObject_V3>(collectionName);
            int nbRows = col.UpdateMany(script, filter);
            RemoveProperty(db, nameof(MigrationObject_V3.IntIdentifier), collectionName);
        }

        public override void Down(ILiteDatabase db)
        {
            var script = "{ IntIdentifier: $.IntIdentifier2 }";
            var filter = $"IntIdentifier2 != null";

            var col = db.GetCollection<MigrationObject_V4>(collectionName);
            int nbRows = col.UpdateMany(script, filter);
            RemoveProperty(db, nameof(MigrationObject_V4.IntIdentifier2), collectionName);
        }
    }
}
