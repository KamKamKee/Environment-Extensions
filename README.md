# Environment Extentions

A package for Unity. EExtensions is a collection of different features for different usecases I personally like to use in my projects without having to write them over and over again.

Most notable features are [Properties](#properties), [List Expressions](#list-expressions) and [Data Persistance](#data-persistance). They are complete game changers for me and introduce workflows I couldn't achieve in basic Unity.

**AI disclaimer**: This package was entirely handcrafted. No generative AI was used to create any of the assets or the documentation.

# Table of contents

The following features/scripts are included in EExtensions.

- [Cooldown](#cooldown)
	- [isReady](#propertybool-cooldownisready)
	- [Initialize()](#cooldowninitialize)
	- [Trigger()](#cooldowntrigger)
	- [TriggerWithCooldown()](#cooldowntriggerwithcooldownfloat-duration)
	- [ForceReady()](#cooldownforceready)
- [Data Persistance](#data-persistance)
	- [DataPersistance](#datapersistacne)
	- [CustomPersistance](#custompersistancet)
	- [SimplePath](#simplepath)
- [List Expressions](#list-expressions)
	- [Join()](#joinstring-inbetween)
	- [Printable()](#printable)
	- [ForEachR()](#foreachrtfunct-string-function)
	- [Filter()](#filtertfunct-bool-function)
	- [Filtered()](#filteredtfunct-bool-function)
	- [Map()](#maptfunct-t-function)
	- [Mapped()](#mappedtfunct-t-func)
	- [SwitchMapped()](#switchmappedtout-tinfunctout-tin-transformationfunction)
	- [Tap()](#taptactionlistt-tapfunction)
	- [Chaining](#chaining)
- [Math Extensions](#math-extensions)
	- [float.IsInRange()](#floatisinrangefloat-min-float-max)
	- [float.IsArround()](#floatisarroundfloat-target-float-radius-float-offset)
	- [float.IsArroundHigher()](#floatisarroundhigherfloat-target-float-radius)
	- [float.IsArroundLower()](#floatisarroundlowerfloat-target-float-radius)
	- [LayerMask.IncludesLayer()](#layermaskincludeslayerint-layer)
	- [float.Range()](#floatrangefloat-endvalue)
	- [float.RangeArround()](#floatrangearroundfloat-radius-float-offset)
- [Properties](#properties)
	- [Property](#propertyt)
		- [Reading](#reading-a-property)
		- [Setting](#setting-a-property)
		- [Updating](#updating-a-property)
		- [Subscribing](#subscribing-to-a-property)
		- [Manual Notification](#manual-notification)
	- [Computed](#computedt)
	- [PropertyBehaviour](#propertybehaviour)
	- [SubscriptionTopic](#subscriptiontopict)
- [UI](#ui)
	- [CanvasGroupSwitch](#canvasgroupswitch)
- [Vector Utility](#vector-utility)
	- [Vector3.Multiply()](#vector3multiplyfloat-x-float-y-float-z)
	- [Vector3.Multiplied()](#vector3multipliedfloat-x-float-y-float-z)
	- [Vector3.Flat()](#vector3flat)
	- [Vector3.FlatV2()](#vector3flatv2)
	- [Vector3.WithMagnitude()](#vector3withmagnitudefloat-magnitude)
	- [Vector3.SetMagnitude()](#vector3setmagnitude)
	- [Vector2.RandomFloatFromRange()](#vector2randomfloatfromrange)
	- [Vector2.RandomIntFromRange()](#vector2randomintfromrange)

# Documentation

All features are already explained in the package itself. You will see them when hovering over the functions/classes with your mouse. The documentation will explain each feature in more detail and provides examples for some cases.


## Cooldown

Implementing the cooldown is easy, although it requires a certain workflow.

```cs
using EExtensions;
using EExtensions.Properties;
using UnityEngine;

public class CooldownExample : PropertyBehaviour
{
	[SerializeField] private Cooldown fiveSecondCooldown = new(5.0f);

	void Awake()
	{
		fiveSecondCooldown.Initialize();
	}
	
	void Start()
	{
		SubscribeClean(fiveSecondCooldown.isReady, isReady =>
		{
			if (isReady)
				OnReady();

			else
				OnTriggered();
		});

        fiveSecondCooldown.Trigger();
	}

    private void Update()
    {
		print(fiveSecondCooldown.ProgessInTime());
    }

    private void OnTriggered()
	{
		Debug.Log("Triggered");
	}

	private void OnReady()
	{
		Debug.Log("Ready");
	}
}
```

In order for the Cooldown to work properly, you'll have to initialize it. Best approach would be to use it as in the example shown. Initialize in Awake so the Cooldown can be used. Subscribe to the isReady Property.

### Property\<bool> Cooldown.isReady
Boolean Property that updates to *true* when the Cooldown is inactive. *false* when the Cooldown was triggered.

### Cooldown.Initialize()
Creates the isReady Property on the Cooldown. Now everything can be used. You should do that in Awake or Start, depending on your needs.

### Cooldown.Trigger()
Activates the cooldown and sets the isReady property to *false*. After the time (you configured in the constructor) is up, isReady will go back to *true*.

### Cooldown.TriggerWithCooldown(float duration)
Sets the new duration of the Coodown and triggers it afterwards.

### Cooldown.ForceReady()
Readys the cooldown immediately


## Data Persistance


You can choose to either store information raw in a binary format or as json in a binary format. You cannot edit any of them in the standard windows editor, although the json version will be readable and it's value will be exposed to the user. The key benefit behind json is default values on load. When retrieving data that is incomplete or on an older version, the missing spots will just be filled in with their default values.

For storing and retrieving data, there are a few static methods all inside the **DataPersistance** static class.

### DataPersistacne


**.SaveClass\<T>(T classToSave, SimplePath path)**
stores a class formatted in binary

**.SaveClassToJson\<T>(T classToSave, SimplePath path)**
stores a class formatted in json, then binary

**.SaveJson\<T>(string data, SimplePath path)**
store json data formatted in binary

**.LoadClass\<T>(SimplePath path)**
retrieves a class from raw format

**.LoadClassFromJson\<T>(SimplePath path)**
retrieves a class from json format

**.LoadJson(SimplePath path)**
retrieves pure json data from json format


### CustomPersistance\<T>


Is a class that references a file to store and retrieve data from. Initializing it takes in a path to the target file, and the default value in case the file doesn't exist.

Currently it is not possible to decide between raw or json. Json is default by now.

```cs
using EExtensions.DataPersistance;
using UnityEngine;

public class PersistanceExample : MonoBehaviour
{
	public class ExampleData
	{
		public string name = "";
		public float currencyAmount = 0;
		public int currentLevel = 0;

        public override string ToString()
        {
			return $"name: '{name}', currencyAmount: '{currencyAmount}', currentLevel: '{currentLevel}'";
        }
	}

	static SimplePath exampleFilePath = "Examples/Example File.data";
    static SimplePath exampleIntFilePath = "Examples/Example Int File.data";
    private CustomPersistence<ExampleData> exampleFilePersister;
	private CustomPersistence<int> exampleSingleIntFilePersister;

    private void Awake()
    {
		exampleFilePersister = new(Application.dataPath + exampleFilePath, new ExampleData());
		exampleSingleIntFilePersister = new(Application.dataPath + exampleIntFilePath, 0);

		Debug.Log($"Before | data: {exampleFilePersister.data}, integer: {exampleSingleIntFilePersister.data}");

		SetDataToDefault();
		IncreaseInteger();
		UpdateData();

        Debug.Log($"After | data: {exampleFilePersister.data}, integer: {exampleSingleIntFilePersister.data}");
    }

    public void SetDataToDefault()
	{
		exampleFilePersister.Update(data => new ExampleData());
		exampleSingleIntFilePersister.Update(value => 0);

		Debug.Log($"SetDataToDefault() integerFile: {exampleSingleIntFilePersister.data}, dataFile: {exampleFilePersister.data}");
	}

	public void IncreaseInteger()
	{
		exampleSingleIntFilePersister.Update(value => value + 1);

        Debug.Log($"IncreaseInteger() integerFile: {exampleSingleIntFilePersister.data}");
	}

	public void UpdateData()
	{
		exampleFilePersister.Update(data =>
		{
			data.name = "John Doe";
			data.currencyAmount = 231.50f;
			data.currentLevel = 2;
		});

        Debug.Log($"UpdateData() dataFile: {exampleFilePersister.data}");
	}
}
```

You are not supposed to edit data from the CustomPersistance persister. You can access it and read it, but modifying it should be done with the according method.

The .Update() method will take in a function. It will pass the current data as the argument. Inside the code block you can now safely edit the data. You can also return data. The return value will be taken as the new data and stored in the target file. Here's two examples for this behavior:

```cs
// Updating values inside a persisted class
CustomPersistance<ExampleData> persister = new("Path/To/File");
persister.Update(data => { 
	data.name = "Example";
	data.currentLevel++;
	data.WhateverMethodYouWantToImplement();
});
```

```cs
// Updating the entire persisted value
CustomPersistance<int> persister = new("Path/To/File");
persister.Update(data => 5); // Sets the data to '5'
persister.Update(data => data + 2) // Adds a value of '2' to the data
```


### SimplePath


Should be used instead of path strings and a lot of useful features.

Can be converted from a string. Back into a string.
```cs
SimplePath simplePath = "C:/Path/To/My/Folder";
Debug.Log(simplePath); // "C:/Path/To/My/Folder"
```

Will clean up inconsistencies or issues in a path.
```cs
SimplePath simplePath = "C:\\Path//To/My\\Folder";
Debug.Log(simplePath); // "C:/Path/To/My/Folder"
```

You can add parts.
```cs
SimplePath simplePath = "C:/Path/To/My/Folder";
simplePath += "File"; // "C:/Path/To/My/Folder/File"
```

You can subtract parts.
```cs
SimplePath simplePath = "C:/Path/To/My/Folder";
simplePath -= 2; // "C:/Path/To"
simplePath--;	 // "C:/Path"
```

Parts can be accessed with the indexer. 0 = first, -1 = last. Just like in Python.
```cs
SimplePath simplePath = "C:/Path/To/My/Folder";
Debug.Log(simplePath[0]);  // "C:"
Debug.Log(simplePath[1]);  // "Path"
Debug.Log(simplePath[-1]); // "Folder"
```


## List Expressions


Is a collection of convenience extensions for the System.Collections.Generic List\<T>. The idea was to create a workflow similar to JavaScript's arrays. 

### Join(string inbetween)
Returns a string combining all current elements of a list. The elements will be seperated by the inbetween string.

```cs
List<int> myList = new() { 1, 2, 4, 8, 16, 32 };
Debug.Log(myList.Join(" - ")); // 1 - 2 - 4 - 8 - 16 - 32
```

### Printable()
Returns a string to print the list in a readable way. You can add an optional displayName.

```cs
List<int> myList = new() { 1, 2, 4, 8, 16, 32 };
List<int> myListEmpty = new() { };
List<string> myStringList = new() { "This", "is", null, "an", "example" };

Debug.Log(myList.Printable("int list")); 		  // List int list { 1, 2, 4, 8, 16, 32 }
Debug.Log(myListEmpty.Printable());	 			  // List { EMPTY }
Debug.Log(myStringList.Printable("string list")); // List string list { This, is, [NULL], an, example }
```

### ForEachR\<T>(Func\<T, string> function)

In case you want to use the .ForEach() function with continue and break. Although it always requires a `return "";` at the end.

```cs
List<int> myList = new() { 1, 2, 4, 8, 16, 32 };

myList.ForEachR(item =>
{
    if (item == 8)
        return "continue";

    if (item == 32)
        return "break";

    Debug.Log(item);

    return "";
});
```

```cs
>> 1
>> 2
>> 4
>> 16
```

### Filter\<T>(Func\<T, bool> function)

Iterates through the items. You can decide which one should stay and which one should go. Returning true will have it stay and returning false will remove the item from the list.

```cs
List<string> myStringList = new() { "This", "is", null, "an", "example" };

myStringList.Filter(item =>
{
    return item != null;
});

Debug.Log(myStringList.Printable()); // List { This, is, an, example }
```

### Filtered\<T>(Func\<T, bool> function)

Will do the same as Filter(), but it won't override the original list. This will return the new filtered list. You will find an example regarding this behaviour in [Chaining](#chaining).

### Map\<T>(Func\<T, T> function)

Iterates through the items. You can modify each element and return the modified value that will take it's place.

```cs
List<int> myList = new() { 1, 2, 4, 8, 16, 32 };

myList.Map(item =>
{
    if (item >= 8)
        return item * 2;

    return item;
});

Debug.Log(myList.Printable()); // List { 1, 2, 4, 16, 32, 64 }
```

```cs
List<string> myStringList = new() { "This", "is", "an", "example" };

myStringList.Map(item => item + " test");

Debug.Log(myStringList.Printable()); // List { This test, is test, an test, example test }
```

### Mapped\<T>(Func\<T, T> func)

Will do the same as Map(), but it won't override the original list. This will return the new filtered list. You will find an example regarding this behaviour in [Chaining](#chaining).

### SwitchMapped\<TOut, TIn>(Func\<Tout, TIn> transformationFunction)

Transforms the list into a different type. With the function you can define the algorithm which will convert the different types. This method will not modify the original list, as this is not possible. Instead the modified list will be returned.

This will convert the string list into an integer list using the string's length for each item.
```cs
List<string> myStringList = new() { "This", "is", "an", "example" };

List<int> switchedList = myStringList.SwitchMapped(item => item.Length);

Debug.Log(switchedList.Printable()); // List { 4, 2, 2, 7 }
```

This will convert the integer list into a string list by converting the values and adding something for better understanding purposes.
```cs
List<int> myList = new() { 1, 2, 4, 8, 16, 32 };

List<string> switchedList = myList.SwitchMapped(item => item + " as a string");

Debug.Log(switchedList.Printable()); // List { 1 as a string, 2 as a string, 4 as a string, 8 as a string, 16 as a string, 32 as a string }
```

### Tap\<T>(Action\<List\<T>> tapFunction)

Will execute one function for the entire list. It will pass the list as an argument. This comes in handy especially for debugging purposes. Examples are on the next documentation point, covering chaining.

### Chaining

You can easily chain together most of the methods explained above to quickly transform list data to your needs. Almost all methods return the list you modified, so you can reuse it right after.

```cs
List<string> myStringList = new() { "This", "is", null, "an", "example" };

List<int> stringLengths = myStringList
    .Filter(item => item != null)
    .Map(item => item.ToUpper())
    .Tap(list => Debug.Log(list.Printable("After Filter and Map")))
    .SwitchMapped(item => item.Count())
    .Tap(List => Debug.Log(List.Printable("Result")));

Debug.Log(myStringList.Printable("Original"));
Debug.Log(stringLengths.Printable("Switched"));
```

```
>> List After Filter and Map { THIS, IS, AN, EXAMPLE }
>> List Result { 4, 2, 2, 7 }
>> List Original { THIS, IS, AN, EXAMPLE }
>> List Switched { 4, 2, 2, 7 }
```

As you can see, the original List was modified up to the point an "ed" function was used. In this case SwitchMapped(), as this will not continue to modify the original list. Let's see the same code but with only "ed" versions.

```cs
List<string> myStringList = new() { "This", "is", null, "an", "example" };

List<int> stringLengths = myStringList
    .Filtered(item => item != null)
    .Mapped(item => item.ToUpper())
    .Tap(list => Debug.Log(list.Printable("After Filter and Map")))
    .SwitchMapped(item => item.Count())
    .Tap(List => Debug.Log(List.Printable("Result")));

Debug.Log(myStringList.Printable("Original"));
Debug.Log(stringLengths.Printable("Switched"));
```

```
>> List After Filter and Map { THIS, IS, AN, EXAMPLE }
>> List Result { 4, 2, 2, 7 }
>> List Original { This, is, [NULL], an, example }
>> List Switched { 4, 2, 2, 7 }
```

The fourth log shows the myStringList. It was not modified this time.


## Math Extensions

A collection of useful checks and conversions for math related stuff.

Most float extensions are also applicable to integers.


### float.IsInRange(float min, float max)
Returns true if the value is inside the range, including min and max.

```cs
15.5f.IsInRange(12.0f, 17.0f); // true
15.5f.IsInRange(12.0f, 15.0f); // false
15.5f.IsInRange(15.5f, 20.0f); // true
15.5f.IsInRange(15.6f, 20.0f); // false
```

### float.IsArround(float target, float radius, float offset)
Returns true if the value is inside the range of a target within a radius. Offset is optional.

```cs
15.5f.IsArround(16.0f, 1.0f) // Range 15.0 - 17.0 >> true
15.5f.IsArround(16.0f, 0.5f) // Range 15.5 - 16.5 >> true
15.5f.IsArround(17.0f, 1.25f) // Range 15.75 - 18.25 >> false
15.5f.IsArround(15.0f, 0.25f) // Range 14.75 - 15.25 >> false
```

### float.IsArroundHigher(float target, float radius)
Does the same as IsArround() but cuts off the lower half.

```cs
15.5f.IsArround(16.0f, 1.0f) // Range 16.0 - 17.0 >> false
15.5f.IsArround(16.0f, 0.5f) // Range 16.0 - 16.5 >> false
15.5f.IsArround(17.0f, 1.25f) // Range 17.0 - 18.25 >> false
15.5f.IsArround(15.0f, 0.25f) // Range 15.0 - 15.25 >> false
```

### float.IsArroundLower(float target, float radius)
Does the same as IsArround() but cuts off the upper half.

```cs
15.5f.IsArround(16.0f, 1.0f) // Range 15.0 - 16.0 >> true
15.5f.IsArround(16.0f, 0.5f) // Range 15.5 - 16.0 >> true
15.5f.IsArround(17.0f, 1.25f) // Range 15.75 - 17.0 >> false
15.5f.IsArround(15.0f, 0.25f) // Range 14.75 - 15.0 >> false
```

### LayerMask.IncludesLayer(int layer)
Returns true if a LayerMask includes a specific layer.

### float.Range(float endValue)
Returns a Vector2 representing a range. X is the min value and y is the max value. float.Range() uses the base value as the min value and takes in a parameter for the max value.

```cs
Vector2 newRange = 10.0f.Range(20.0f); // Vector2 (10, 20)
```

Usage of this feature is further explained in [Vector Utility](#vector-utility).

### float.RangeArround(float radius, float offset)
Same as float.Range(). This one creates the range using a radius. It uses the base value as an anchor point and takes in a parameter for the radius and an optional offset.

```cs
Vector2 newRange = 10.0f.RangeArround(5.0f); // Vector2 (5, 15)
```

Usage of this feature is further explained in [Vector Utility](#vector-utility).


## Properties


Introduces react style signals to Unity/C#. Properties are value wrappers that can be subscribed to. Updates can be made and subscribers will be notified.

Since I have worked with Angular, I loved the workflow signals allowed for. Properties try to mimic that exact workflow.

### Property\<T>

Simple properties can be defined with a value type that should be stored. They hold a current value that can be modified and read through various functions. If you require a more lightweight version, please refer to the [SubscriptionTopic](#subscriptiontopict).

```cs
// Regular property
using EExtensions.Properties;
using UnityEngine;

public class PropertyExample : MonoBehaviour
{
    private Property<int> integerProperty;

    private void Awake()
    {
        integerProperty = new(0, this);
    }
}
```

Creating a property is simple. Specify the type and initialize the property in the Awake function. In the constructor, pass the initial value and the parent object. This reference will be used for the property to know when it should be disposed and can be marked as inactive.

A regular property always needs a parent MonoBehaviour. If you want a static property that does not have to be disposed or you want to implement a property outside of a MonoBehaviour scope, use Property\<T>.Static() factory method.

```cs
// Static property
using EExtensions.Properties;
using UnityEngine;

public class PropertyExample : MonoBehaviour
{
    public static Property<int> integerProperty = Property<int>.Static(0);
}
```

You only have to pass initial value and no parent. If your parent class **can** be destroyed, use .Dispose() to dispose of the property manually.

```cs
// Property without a MonoBehaviour parent
using EExtensions.Properties;

public class PropertyExample
{
    private Property<int> integerProperty;

    public PropertyExample()
    {
        integerProperty = Property<int>.Static(0);
		/* In some cases, initializing the property inside of the constructor can be problematic
		 * One example would be the Cooldown class you can also find in this package.
		 */
    }

	public Initialize()
	{
		integerProperty = Property<int>.Static(0);
		/* Cooldown e.g. uses this method.
		 * That's why Initialize() must be called in awake when using a cooldown.
		 * Choose between the two methods depending on your needs.
		 */
	}

    ~PropertyExample()
    {
        integerProperty.Dispose();
    }
}
```

#### Reading a property

Now let's read the current value of a property. For that use the .Get() method. It will return the value. If you store a class inside a property, you will be able to edit the class's values **although you should not do that**. More on that later when covering the .Set() and .Update() methods.

```cs
int currentValue = integerProperty.Get(); // 0
```

Alternatively an implicit conversion is possible but doesn't work with all value types. If you're checking agains a boolean or number property, this should work just fine, but classes in properties do not work as smoothly.

```cs
if (booleanProperty.Get() && integerProperty.Get() == 5) { ... }
```

```cs
if (booleanProperty && integerProperty == 5) { ... }
```

#### Setting a property

This one is easy.

This property will be used for the following examples:
```cs
private Property<int> integerProperty;

private void Awake()
{
	integerProperty = new(0, this);
}
```

**.Set(T newValue)**
will check the new value agains the old value, updates it and if a change is current, all subscribers will be notified.

```cs
private void Start()
{
	integerProperty.Set(5); // Subscriber notification
	integerProperty.Set(5); // No subscriber notification
}
```

**.SetLazy(T newValue)**
won't check if a change is current. The value will be updated and a notification will be sent.

```cs
private void Start()
{
	integerProperty.SetLazy(5); // Subscriber notification
	integerProperty.SetLazy(5); // Another Subscriber notification
}
```

**.SetQuiet(T newValue)**
sets the new value without sending a notification. I don't know when you would ever need that, but I'm pretty sure there is some unique use case to that. Refer to [manual notification](#manual-notification) to expand on this.

```cs
private void Start()
{
	integerProperty.SetQuiet(5); // No subscriber notification
	integerProperty.SetQuiet(5); // No subscriber notification
}
```

#### Updating a property

This one is not so easy but useful.

**.Update(Func\<T, T> updateFunction)**
provides the current value to a function. Return the updated value that should be stored in the property. The updated value will be checked against the current value. It will be replaced and subscribers will be notified if changes are current.

```cs
private void Start()
{
    integerProperty.Update(value => value + 5); // Subscriber notification (5)
    integerProperty.Update(value => value + 5); // Subscriber notification (10)
    integerProperty.Update(value => value + 0); // No subscriber notification
}
```

**.UpdateLazy(Func\<T, T> updateFunction)**
won't check if a change is current. The value will be updated and a notification will be sent.

```cs
private void Start()
{
    integerProperty.UpdateLazy(value => value + 5); // Subscriber notification (5)
    integerProperty.UpdateLazy(value => value + 5); // Subscriber notification (10)
    integerProperty.UpdateLazy(value => value + 0); // Another subscriber notification (10)
}
```

**.UpdateQuiet(Func\<T, T> updateFunction)**
updates the current value without sending a notification. I don't know when you would ever need that, but I'm pretty sure there is some unique use case to that. Refer to [manual notification](#manual-notification) to expand on this.

```cs
private void Start()
{
    integerProperty.UpdateQuiet(value => value + 5); // No subscriber notification
    integerProperty.UpdateQuiet(value => value + 5); // No subscriber notification
}
```

---

Now we've covered most generic value types. Class values *can* be modified in the same way, but it seems rather inconvenient. For this there is a another way to update a property. Setting it still works normally by using the .Set() method.

This class will be used for the following examples:
```cs
public class ExampleData
{
    public string name = "";
    public float currencyAmount = 0;
    public int currentLevel = 0;

    public void DoSomething()
    {
        Debug.Log("Did something");
    }

    public void ModifySomething()
    {
        currentLevel = 1;
    }

    public override string ToString()
    {
        return $"name: '{name}', currencyAmount: '{currencyAmount}', currentLevel: '{currentLevel}'";
    }
}
```

This property will be used for the following examples:
```cs
private Property<ExampleData> dataProperty;

private void Awake()
{
    dataProperty = new(new ExampleData(), this);
}
```

**.Update(Action\<T> updateFunctionNoReturn)**
takes in a function and passes the current value as the argument. This time no return value is expected. Inside the code block you can edit the values of the original data as you like. No checks of the same type will be done here. This function is always lazy and will always notify subscribers.

```cs
private void Start()
{
    dataProperty.Update(data =>
    {
        data.name = "John Doe";
        data.currencyAmount = 215.50f;
        data.currentLevel = 2;
    });
}
```

This updated all values. You can do whatever you want in there.

```cs
private void Start()
{
    dataProperty.Update(data =>
    {
        data.currencyAmount += 55.00f;
    });
}
```

Regular methods of your data can be called normally by .Get()

```cs
private void Start()
{
    dataProperty.Get().DoSomething();
}
```

Methods that modify anything inside the own value itself should be used in .Update() because of the subscription notifications.

```cs
private void Start()
{
    dataProperty.Get().ModifySomething(); // No subscriber notification

	// Subscriber notification
    dataProperty.Update(data => 
    {
        data.ModifySomething();
    });
}
```

#### Subscribing to a property

Whenever a property changes it's value, an event is invoked. Subscribing to a property allows the receive a callback from this event with different callback values depending on what you need.

```cs
private Property<int> integerProperty;

private void Awake()
{
    integerProperty = new(0, this);
}

private void Start()
{
    integerProperty.Subscribe(Callback);
}

private void Callback(int currentValue)
{
    Debug.Log($"{currentValue}");
}
```

The example's method Callback() will be called every time, the value of integerProperty changed. There are three different function types accepted.

```cs
private void Start()
{
    integerProperty.Subscribe(CallbackOneValue);
    integerProperty.Subscribe(CallbackTwoValues);
    integerProperty.Subscribe(CallbackNoValue);
}

private void CallbackOneValue(int currentValue)
{
    Debug.Log($"Value changed: {currentValue}");
}

private void CallbackTwoValues(int previousValue, int currentValue)
{
    Debug.Log($"Value changed: {previousValue} >> {currentValue}");
}

private void CallbackNoValue()
{
    Debug.Log($"Value changed");
}
```

A function with no parameters will be called whenever a change happened, but won't provide any value information through the parameters.

A function with one parameter will provide the current value as it's argument.

A function with two parameters will provide the previous as the first and the current value as the second argument.

You can of course use lambda functions instead of class member functions.

```cs
private void Start()
{
    integerProperty.Subscribe(
		currentValue => Debug.Log($"Value changed: {currentValue}")
	);

	integerProperty.Subscribe(
		(previousValue, currentValue) => Debug.Log($"Value changed: {previousValue} >> {currentValue}")
	);

	integerProperty.Subscribe(
		() => Debug.Log($"Value changed")
	);
}
```

To see this working in action, you can connect a coroutine to change the property's value over time.

```cs
private Property<int> integerProperty;

private void Awake()
{
    integerProperty = new(0, this);
}

private void Start()
{
    integerProperty.Subscribe(
        currentValue => Debug.Log($"Value changed: {currentValue}")
    );

    StartCoroutine(UpdateValueOverTime());
}

private IEnumerator UpdateValueOverTime()
{
    while (true)
    {
        yield return new WaitForSeconds(1.0f);

        integerProperty.Update(value => value + 1);
    }
}
```

```
>> Value changed: 1
>> Value changed: 2
>> Value changed: 3
>> Value changed: 4
...
```

These subscriptions will live on forever. Whenever the subscribed property changes it's value, it will try to call these methods, regardless if their parents exist or not.

To avoid running into this problem, you can store the subscription data and unsubscribe whenever the parent is destroyed. The automation of this behaviour is documented in the [PropertyBehaviour](#propertybehaviour) section.

```cs
private Property<int> integerProperty;
private IGenericSubscription integerPropertyCallbackSubscription;

private void Awake()
{
    integerProperty = new(0, this);
}

private void Start()
{
    integerPropertyCallbackSubscription = integerProperty.Subscribe(
        currentValue => Debug.Log($"Value changed: {currentValue}")
    );

    StartCoroutine(UpdateValueOverTime());
}

private IEnumerator UpdateValueOverTime()
{
    while (true)
    {
        yield return new WaitForSeconds(1.0f);

        integerProperty.Update(value => value + 1);
    }
}

private void OnDestroy()
{
    integerPropertyCallbackSubscription.Unsubscribe();
}
```

Property subscriptions also provide the ability to resubscribe to the property if needed.

Here is a little example for that:

```cs
private Property<int> prop;
private IGenericSubscription subscription;

private void Awake()
{
    prop = new(0, this);
}

private void Start()
{
    subscription = prop.Subscribe(currentValue =>
    {
        Debug.Log($"value: {currentValue}");

        if (currentValue == 4)
        {
            Debug.Log("Unsubscribing...");
            subscription.Unsubscribe();
        }
    });

    StartCoroutine(UpdateValueOverTime());
}

private IEnumerator UpdateValueOverTime()
{
    while (true)
    {
        yield return new WaitForSeconds(1.0f);

        prop.Update(value => value + 1);

        if (prop == 10)
        {
            subscription.Resubscribe();
            Debug.Log("Resubscribed");
        }
    }
}
```

```
>> value: 1
>> value: 2
>> value: 3
>> value: 4
>> Unsubscribing...
>> Resubscribed
>> value: 11
>> value: 12
>> value: 13
...
```

#### Manual notification

In case you need to send notifications to subscribers by yourself. There is a simple functionality for that. This can be used with the lazy functions of the property, that don't send a notification by themselves.

**.Notify()**
notifies every subscriber of a change. Both current value and previous value arguments will be filled with the property's current value.

**.Notify(T previousValue)**
notifies every subscriber of a change. The current value argument will be filled with the property's current value and the previous value argument will be filled with your provided previous value.

```cs
private Property<int> integerProperty;

private void Awake()
{
	integerProperty = new(0, this);
}

private void Start()
{
	integerProperty.SetQuiet(5); // No subscriber notification
	integerProperty.SetQuiet(5); // No subscriber notification
	integerProperty.Notify(); // Subscriber notification
}
```

### Computed\<T>

_**Still Work in Progress...**_

### PropertyBehaviour

Derives from MonoBehaviour and is the intended way to use properties. The PropertyBehaviour class automates subscription management. You can create a new PropertyBehaviour script in your project browser by using `Context Menu` > `Create` > `PropertyBehaviour script`.

PropertyBehaviour provides a few functions to manage any ongoing subscriptions.

**.SubscribeClean\<T>(Property\<T> property, Action\<T, T> callbackFunction)**
registers the subscription for automated management. Just like subscribing to the property directly, your callback function can have zero, one or two parameters. The rules are the same as in [regular subscriptions](#subscribing-to-a-property).

```cs
public class NewPropertyBehaviourScript : PropertyBehaviour
{
	private Property<int> integerProperty;

	// Use this for initialization. Non-static properties should be initialized here
	void Awake()
	{
		integerProperty = new(0, this);
	}
	
	// All property subscriptions should go here
	void Start()
	{
		SubscribeClean(integerProperty, value => Debug.Log(value));
	}
}
```

Registered subscriptions will be unsubscribed when the gameobject is destroyed. They will be paused when the gameobject is deactivated and resumed when the gameobject is activated again. Whenever the subscriptions are resumed, they are also checked, if they are still valid. If the property behind the subscription was disposed, the subscription will be removed.

With these function you can manually manage ongoing subscriptions:

```cs
public class NewPropertyBehaviourScript : PropertyBehaviour
{
	void Start()
	{
		PauseAllSubscriptions();

		ResumeAllSubscriptions();

		CleanupSubscriptions();

		UnsubscribeAll();

		UnsubscribeFrom(integerProperty);
	}
}
```

Because some functionality is provided through MonoBehaviour functions, you will have to override these to use them. Make sure to always include the base.Function() to have automatic subscription management.

```cs
public class NewPropertyBehaviourScript : PropertyBehaviour
{
	// Subscriptions will be resumed here
	protected override void OnEnable()
    {
        base.OnEnable();

		// Your code
    }

	// Subscriptions will be paused here
    protected override void OnDisable()
    {
        base.OnDisable();

		// Your code
    }

	// Subscriptions will be unsubscribed here
    protected override void OnDestroy()
    {
        base.OnDestroy();

		// Your code
    }
}
```

You can also use these checks:

```cs
public class NewPropertyBehaviourScript : PropertyBehaviour
{
	void Start()
	{
		IGenericSubscription subscription = FindGenericSubscriptionFrom(integerProperty);

		bool anyActive = AnySubscriptionsActive();
	}
}
```

**.FindGenericSubscriptionFrom\<T>(Property\<T> property)**
will return a generic subscription object if you have subscribed to the specified property earlier using .SubscribeClean(). If no such subscription was found, null will be returned.

### SubscriptionTopic\<T>

_**Still Work in Progress...**_


## UI


The UI section requires the [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween) package. Available at [OpenUPM](https://openupm.com/packages/com.kyrylokuzyk.primetween/) and in the [Unity Asset Store](https://assetstore.unity.com/packages/tools/animation/primetween-high-performance-animations-and-sequences-252960). It's great and entirely free.

### CanvasGroupSwitch

Is a component that should be used with a CanvasGroup. It provides functionality to smoothly activate and deactivate the CanvasGroup.

**.Activate()**
Transitions the alpha value to 1. You can also provide a new fade speed.

**.Deactivate()**
Transitions the alpha value to 0. You can also provide a new fade speed.


## Vector Utility


This includes a small bunch of little extensions for vectors.

### Vector3.Multiply(float x, float y, float z)

Modifies the vector and multiplies it on each axis.

### Vector3.Multiplied(float x, float y, float z)

Returns a new vector that was multiplied on each axis.

### Vector3.Flat()

Returns a vector without the y axis. Internally this is just a wrapper for Vector3.Multiply(y: 0)

### Vector3.FlatV2()

Does the same as .Flat() but returns a Vector2 instead.

```
Vector3					Vector2
			x => x
			y
			z => y
```

### Vector3.WithMagnitude(float magnitude)

Returns a vector with the modified target magnitude.

### Vector3.SetMagnitude()

Modifies the vector to match the target magnitude.

### Vector3.DistanceTo(Vector3 other)

Just a wrapper for Vector3.Distance(). You can also use x, y, z values instead of another vector.

### Vector2.RandomFloatFromRange()

Here the Vector2 is used as a range not as a vector. X represents the min value and y represents the max value. Both inclusive. It returns a random float value.

### Vector2.RandomIntFromRange()

Same here. This one returns a random integer value. Also min and max both inclusive.