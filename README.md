# DbModelFramework

```C#
var configuration = new ContainerConfiguration();
configuration.WithPart<DbConnection>();

DbModelFramework.DependencyInjection.InjectionContainer = configuration.CreateContainer();
```

```C#
class MyModel : Model<Car>
{
	public string Property1 { get; set; }
	public string Property2 { get; set; }
}
```

```C#
var myModel = MyModel.Create();
```

```C#
var myModels = MyModel.Get();
```

```C#
var myModels = MyModel.Get(model => model.Property1 == "MyValue");
```

```C#
myModel.Save();
```

```C#
myModel.Delete();
```
