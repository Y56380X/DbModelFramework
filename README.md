# DbModelFramework

## Getting started

### Environment
Download NuGet-Package

### Inject InjectionContainer
As a minimum requirement you have to define a new injection container with at least a DbRequirements class as a part. You can use one of the predifined DbRequirements (DbModelFramework.Sqlite, DbModelFramework.MySql, DbModelFramework.MsSql).
The DbRequirements class has to export the type DbModelFramework.DbRequirements.

Your DbRequirements class:
```C#
[Export(typeof(DbModelFramework.DbRequirements))]
class DbRequirements : DbModelFramework.Sqlite.DbRequirements
{
	private static readonly string ConnectionString = new SqliteConnectionStringBuilder { DataSource = "database.db" }.ConnectionString;

	public override IDbConnection CreateDbConnection()
	{
		var connection = new SqliteConnection(ConnectionString);
		connection.Open();

		return connection;
	}
}
```

Injection container creation (System.Composition is required):
```C#
var configuration = new ContainerConfiguration();
configuration.WithPart<DbRequirements>();

DbModelFramework.DependencyInjection.InjectionContainer = configuration.CreateContainer();
```

## Use the models

### Define a new model class
```C#
class MyModel : Model<MyModel>
{
	public string Property1 { get; set; }
	public string Property2 { get; set; }
}
```

### Create a new model instance (insert in db)
```C#
var myModel = MyModel.Create();
```

### Get all model data
```C#
var myModels = MyModel.Get();
```

### Get all model data which fits in the condition
```C#
var myModels = MyModel.Get(model => model.Property1 == "MyValue");
```

### Get model data by primary key
```C#
var myModel = MyModel.Get(1); // Get model data with primary key 1
```

### Save changes of a models data (update in db)
```C#
myModel.Save();
```

### Delete model data (delete from db)
```C#
myModel.Delete();
```
