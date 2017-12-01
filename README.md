# DbModelFramework

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
var myModel = MyModel.Get(typeof(MyModel).GetProperty("Property1"), "MyValue");
```
