using LiteDB;
using LiteDbMigrations;
using LiteDbMigrationSample.Models;

namespace LiteDbMigrationSample.Migrations
{
    [Migration(20240731100000, UseTransactionOnDown = false)]
    internal class InitialPopulate() : DatabaseMigration()
    {
        private readonly string collectionName = LiteDbConstants.MigrationObjectCollection;
        public override void Up(ILiteDatabase db)
        {
            var col = db.GetCollection<MigrationObject>(collectionName, BsonAutoId.ObjectId);
            List<MigrationObject> migrationObjects = [
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "one" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "two" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "three" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "four" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "five" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "six" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "seven" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "eight" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "nine" },
                new MigrationObject { Id = ObjectId.NewObjectId().ToString(), Name = "ten" },
                ];
            int nbInserted = col.InsertBulk(migrationObjects);

        }
        public override void Down(ILiteDatabase db)
        {
            db.GetCollection<MigrationObject>(collectionName).DeleteAll();
            var dropped = db.DropCollection(collectionName);
        }
    }
}
