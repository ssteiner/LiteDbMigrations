using LiteDB;
using LiteDbMigrations;
using LiteDbMigrationSample.Models;

namespace LiteDbMigrationSample.Migrations
{
    [Migration(20240731112300)]
    public class AddFqdn : DatabaseMigration
    {
        private readonly string collectionName = LiteDbConstants.MigrationObjectCollection;

        public override void Up(ILiteDatabase db)
        {
            var col = db.GetCollection(collectionName);
            int nbRows = col.UpdateMany("{ Fqdn: $.Name }", "_id > 0");
        }

        public override void Down(ILiteDatabase db)
        {
            var col = db.GetCollection(collectionName);
            int nbRows = RemoveProperty(db, nameof(MigrationObject_V2.Fqdn), collectionName);
        }
    }
}
