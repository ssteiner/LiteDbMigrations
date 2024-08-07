using LiteDB;
using LiteDbMigrations;
using LiteDbMigrationSample.Models;

namespace LiteDbMigrationSample.Migrations
{
    [Migration(20240731100610)]
    internal class AddNullableBool : DatabaseMigration
    {
        private readonly string collectionName = LiteDbConstants.MigrationObjectCollection;

        public override void Up(ILiteDatabase db)
        {
            var col = db.GetCollection(collectionName);
            int nbRows = col.UpdateMany("{ IsDefault: true }", "_id > 0");
        }

        public override void Down(ILiteDatabase db)
        {
            int nbRows = RemoveProperty(db, nameof(MigrationObject_V2.IsDefault), collectionName);
        }
    }
}
