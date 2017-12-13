# DbModelFramework

## Getting started

### Environment
Download NuGet-Package

### Inject InjectionContainer
As a minimum requirement you have to define a new injection container with at least the sqlite db connection class as a part.
The db connection class has to export the type IDbConnection.

```C#
var configuration = new ContainerConfiguration();
configuration.WithPart<DbConnection>();

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

### Save changes of a models data (update in db)
```C#
myModel.Save();
```

### Delete model data (delete from db)
```C#
myModel.Delete();
```
