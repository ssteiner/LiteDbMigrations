# LiteDB Migrations

## Quick Start

Add the LiteDbMigrations.csproj to your project.

## Introduction

LiteDbMigrations is a migration framework for [LiteDB](https://www.litedb.org/) that helps you to keep your documents and collections in sync with your application. It is heavily influenced by approaches seen in [Raven Migrations](https://github.com/migrating-ravens/RavenMigrations) and [Mongo.Migration](https://github.com/SRoddis/Mongo.Migration).

I needed a migration framework for LiteDb, and of course I initially looked at [LiteDB.Migration](https://github.com/JKamsker/LiteDB.Migration). I'm trying to keep my data classes free of dependencies, and having previously worked with EFCore and the beforementioned migration frameworks for [RavenDB](https://ravendb.net/) and [MongoDB](https://www.mongodb.com/), I ended up writing my own little migration framework.

## Migration Concept

You define your own migrations by implementing `IDatabaseMigration` which provides an `Up` and a `Down` method and give you access to the entire `ILiteDatabase`, so you can execute whatever operation you may want as part of a migration. In addition, every migration needs to be tagged with a `[Migration(versionnumber)]` tag containing the version. I'm using long so you can use timestamps but feel free to just number up from 1.

To help with common Tasks, you can derive from `DatabaseMigration`, which provides you with additional helper methods to update documents: `RemoveProperty`/`RemoveProperties` to remove a single / multiple properties from a collection of documents (I haven't found a way to `unset` a property yet - ping me if you know one) as well as Updating a list of document in chunks (`UpdateInChunks`).

The sample project provides 5 migration samples that creates/inserts documents, modifies all documents in a collection, drops collections, renames properties, and 'moves' properties.

Executed migrations are stored in a collection in your database (by default: _migrations, and configurable) - similar to the __EFMigrationsHistory table in EFCore. So there's no document version for individual documents.

### A Migration

A Migration looks like this

``` c#
// migration number
[Migration(20240731100610)]
// every migration must implement IDatabaseMigration or derive from DatabaseMigration
internal class AddNullableBool : DatabaseMigration
{
    private readonly string collectionName = LiteDbConstants.MigrationObjectCollection;

    // operations to perform when migrating up to this version
    public override void Up(ILiteDatabase db)
    {
        var col = db.GetCollection(collectionName);
        int nbRows = col.UpdateMany("{ IsDefault: true }", "_id > 0");
    }

    // undoes the migration
    public override void Down(ILiteDatabase db)
    {
        int nbRows = RemoveProperty(db, nameof(MigrationObject_V2.IsDefault), collectionName);
    }
}

```

This sample would add an `IsDefault` column to every document with `_id > 0` in the collection name `collectionName` and set it to true. On going down, the IsDefault property is removed using the helper function `RemoveProperty` (available when deriving from DatabaseMigration, or feel free to write your own (better?) version.

There's two way you can run migrations - in your standard ASP.NET Core app (or any app with an apphost really - see the `IocMigration` class)

``` c#
// In Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Add the MigrationRunner into the dependency injection container.
    services.AddLiteDbMigrations(options =>
    {
        options.ToVersion = 1;
    });
}

public void Configure(IApplicationBuilder app, ...)
{
    // Run pending migrations.
    var migrationService = app.ApplicationServices.GetRequiredService<LiteDbMigrationRunner>();
    migrationService.Run();
}
```

Or you can do things the hard (and manual way - an working example is provided in the `ManualMigration` class
``` c#
// Skip dependency injection and run the migrations.

var options = new LiteDbMigrationOptions
{
    ToVersion = 1
    ConnectionString = configuration.GetValue<string>("LiteDbConnectionString"), 
};

ILogger<LiteDbMigrationRunner> myLogger = new NullLogger<LiteDbMigrationRunner>();
var migrationRunner = new LiteDbMigrationRunner(options, logger);
migrationRunner.Run();

```

### Migration Runner

The `LiteDbMigrationRunner` is in charge of performing migrations. It first determines the current version of the database by help of an `IMigrationRecordStore` (you can wire your own and provide it in the `LiteDbMigrationOptions` if you'd like). The `IMigrationRecordStore` loads and stores `MigrationRecords` in your database so we know which version it currently has. The collection used is defined in `LiteDbMigrationOptions.MigrationRecordsCollectionName`.
It then determines available Migrations by using an `IMigrationResolver`. By default, all classes implenenting `IDatabaseMigration` and having an `Migration` attribute in the current assembly will be used. You can change this by providing your own list of assemblies using the `LiteDbMigrationOptions.Assemblies`.

Based on the determined current version and the desired version (defined in `LiteDbMigrationOptions.ToVersion`), the runner defines which migrations to execute and in what order (up or down). If `ToVersion` is null, that indicates 'execute all migrations', whereas 0 means: 'undo all migrations'.

Every `DatabaseMigration` is executed in its own transaction, you shouldn't get stuck in the middle. There's a catch though: certain LiteDB operations cannot run on an active transaction: `DropCollection` will fail. Hence I've added the ability to disable transaction for a given Migration's `Up`/`Down` method by setting `UseTransactionOnUp` / `UseTransactionOnDown` to `true`. This is used in the sample to bypass transactions in the `InitialPopulate` and `AddAndPopulateNewTable` migrations. 

The LiteDbMigrationRunner has 3 ways of determining which database to use:

1) Using `LiteDbMigrationOptions.Database`
2) Creating its own `ILiteDatabase` using the connection string in `LiteDbMigrationOptions.ConnectionString`
3) In IOC Mode, if neither #1 and #2 are available, it'll try to get an `ILiteDatabase` from the IOC container.

### Solution structure

I've used your standard structure that you may know from EFCore or Raven.Migrations:

```
\Migrations
    - 001_FirstMigration.cs
    - 002_SecondMigration.cs
    - 003_ThirdMigration.cs
```

## Contributing

I'm open to your ideas and contributions. Let me know if you have an idea you'd like to see integrated. Feel like writing some units test? You'd be very welcome.

## Thanks

The guys behind [RavenMigrations](https://github.com/migrating-ravens/RavenMigrations) and [Mongo.Migrations](https://github.com/SRoddis/Mongo.Migration) whose work inspired me and gave me the confidence to say 'hey, I can bring these concepts to LiteDB'. And of course [Mauricio David](https://github.com/mbdavid) for creating LiteDB in the first place.
