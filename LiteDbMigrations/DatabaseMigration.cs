using LiteDB;
using LiteDbMigrations.Extensions;

namespace LiteDbMigrations
{
    public abstract class DatabaseMigration(): IDatabaseMigration
    {
        public abstract void Up(ILiteDatabase db);

        public abstract void Down(ILiteDatabase db);

        public static int RemoveProperty(ILiteDatabase db, string propertyName, string collectionName)
        {
            var col = db.GetCollection(collectionName);
            var documents = col.FindAll();
            List<BsonDocument> newDocuments = [];
            foreach (var doc in documents)
            {
                if (doc.Remove(propertyName))
                    newDocuments.Add(doc);
            }
            return UpdateInChunks(col, newDocuments);
        }

        public static int RemoveProperties(ILiteDatabase db, IEnumerable<string> propertyNames, string collectionName)
        {
            var col = db.GetCollection(collectionName);
            var documents = col.FindAll();
            List<BsonDocument> newDocuments = [];
            foreach (var doc in documents)
            {
                bool remove = false;
                foreach (var propertyName in propertyNames)
                    remove &= doc.Remove(propertyName);
                if (remove)
                    newDocuments.Add(doc);
            }
            return UpdateInChunks(col, newDocuments);
        }

        public static int UpdateInChunks<T>(ILiteCollection<T> col, IEnumerable<T> updatedObjects)
        {
            int nbRows = 0;
            var chunks = updatedObjects.SplitList(1000);
            if (chunks != null)
            {
                foreach (var chunk in chunks)
                    nbRows =+ col.Update(chunk);
            }
            return nbRows;
        }

    }

    public interface IDatabaseMigration
    {
        public void Up(ILiteDatabase db);

        public void Down(ILiteDatabase db);
    }
}
