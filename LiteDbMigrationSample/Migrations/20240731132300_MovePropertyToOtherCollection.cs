using LiteDB;
using LiteDbMigrations;
using LiteDbMigrationSample.Models;

namespace LiteDbMigrationSample.Migrations
{
    [Migration(20240731132300)]
    internal class MovePropertyToOtherCollection : DatabaseMigration
    {
        private readonly string collectionName = LiteDbConstants.MigrationObjectCollection;
        private readonly string otherCollectionName = LiteDbConstants.OtherMigrationObjectsCollection;

        public override void Up(ILiteDatabase db)
        {
            var col = db.GetCollection<MigrationObject_V3>(collectionName);

            var migrationObjectsWithoutIntIdentifier = db.GetCollection<MigrationObject_V3>(collectionName)
                .Query()
                .Where(u => u.IntIdentifier == null)
                .Select(x => new MigrationObject_V3 { Id = x.Id, OtherObjectId = x.OtherObjectId })
                .ToList();

            var otherObjectsWithIdentifier = db.GetCollection<OtherMigrationObject>(otherCollectionName)
                .Query()
                .Where(u => u.IntIdentifier != null)
                .ToList();
            int nbRows = 0;
            foreach (var otherObject in otherObjectsWithIdentifier)
            {
                var script = $"{{ IntIdentifier: {otherObject.IntIdentifier} }}";
                var filter = $"OtherObjectId = '{otherObject.Id}'";
                nbRows += col.UpdateMany(script, filter);
            }
        }

        public override void Down(ILiteDatabase db)
        {
            var col = db.GetCollection<MigrationObject_V3>(collectionName);
            int nbRows = RemoveProperty(db, nameof(MigrationObject_V3.IntIdentifier), collectionName);
        }
    }
}
