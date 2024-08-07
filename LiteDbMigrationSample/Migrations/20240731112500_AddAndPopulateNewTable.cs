using LiteDB;
using LiteDbMigrations;
using LiteDbMigrationSample.Models;

namespace LiteDbMigrationSample.Migrations
{
    [Migration(20240731112500, UseTransactionOnDown = false)]
    internal class AddAndPopulateNewTable : DatabaseMigration
    {
        private readonly string collectionName = LiteDbConstants.MigrationObjectCollection;
        private readonly string otherCollectionName = LiteDbConstants.OtherMigrationObjectsCollection;

        public override void Up(ILiteDatabase db)
        {
            var obj1 = new OtherMigrationObject { Id = ObjectId.NewObjectId().ToString(), IntIdentifier = 1, Name = "Number 1" };
            var obj2 = new OtherMigrationObject { Id = ObjectId.NewObjectId().ToString(), IntIdentifier = 2, Name = "Number 2" };
            var obj3 = new OtherMigrationObject { Id = ObjectId.NewObjectId().ToString(), IntIdentifier = 3, Name = "Number 3" };
            List<OtherMigrationObject> migrationObjects = [obj1, obj2, obj3];
            var col = db.GetCollection<OtherMigrationObject>(otherCollectionName, BsonAutoId.ObjectId);
            int nbRows = col.InsertBulk(migrationObjects);

            var migrationObjectsCollection = db.GetCollection<MigrationObject_V3>(collectionName);

            var allMigrationObjects = migrationObjectsCollection.FindAll();
            var rand = new Random();
            List<MigrationObject_V3> updatedObjects = [];
            foreach (var migrationObject in allMigrationObjects)
            {
                var nextIndex = rand.Next(migrationObjects.Count);
                var obj = migrationObjects[nextIndex];

                migrationObject.OtherObjectId = obj.Id;
                updatedObjects.Add(migrationObject);
            }
            nbRows = UpdateInChunks(migrationObjectsCollection, updatedObjects);
        }

        public override void Down(ILiteDatabase db)
        {
            var col = db.GetCollection<MigrationObject_V3>(collectionName);
            int nbRows = col.UpdateMany("{ OtherObjectId: null }", "_id > 0");
            var dropped = db.DropCollection(otherCollectionName);
        }
    }
}
