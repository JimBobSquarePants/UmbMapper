<h1 align="center">
    <img src="https://raw.githubusercontent.com/JimBobSquarePants/UmbMapper/develop/build/assets/logo/umbmapper-256.png" alt="UmbMapper" width="175"/>
    <br>
    UmbMapper
    <br><br>
</h1>


**This repository contains a blazingly-fast, really simple to use, convention-based published content mapper for Umbraco.**

## What does it do?

UmbMapper maps `IPublishedContent` instances from the Umbraco Published Content Cache to strongly typed classes. It does so in a very efficient manner with very little overhead.

So far it's made up of the following libraries

- [**UmbMapper**](https://www.nuget.org/packages/UmbMapper) - The main mapping library, Maps all default Umbraco dataTypes and almost anything that uses a `PropertyValueConverter` to POCO equivalents.
- [**UmbMapper.ArcheType**](https://www.nuget.org/packages/UmbMapper.ArcheType) - Allows the mapping of ArcheType models to POCO equivalents.
- [**UmbMapper.NuPickers**](https://www.nuget.org/packages/UmbMapper.NuPickers) - Allows the mapping of NuPicker models to POCO equivalents.
- [**UmbMapper.PublishedContentModelFactory**](https://www.nuget.org/packages/UmbMapper.PublishedContentModelFactory) - Allows the mapping of models using the Umbraco PublishedContentFactory.
- [**UmbMapper.Vorto**](https://www.nuget.org/packages/UmbMapper.Vorto) - Allows the mapping of Vorto models to POCO equivalents.

## Consuming The Libraries

Nightlies are available on [Myget](https://www.myget.org/gallery/umbmapper) with battle tested releases available on [Nuget](https://www.nuget.org/packages/UmbMapper).

Samples project login (Check it out!)

- Username : admin
- Password : umbmapper!

## How it works - The API

> *A POCO should be exactly that.* - Rudyard Kipling

Models are clean without any inheritance requirements. They require no additional attribution to determine mapping logic.

#### Mapping a Simple Class

Here's an example class, nothing fancy...

``` csharp
public class SimplePublishedItem
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; }

    public virtual DateTime CreateDate { get; set; }

    public virtual string Url { get; set; }

    // RTE
    public virtual IHtmlString BodyText { get; set; }

    // Media Picker
    public virtual ImageCropDataSet Image { get; set; }

   // Content Picker
    public virtual ComplexPublishedItem Target { get; set; }
}
```

You'll have noticed the `virtual` keyword there. It's only required when we want to lazy map a property so it's not essential. (more on that later)

#### Registering a Simple Class Mapping

Since this class requires no specialization, and, you've used appropriate convention to ensure property names match the property aliases in the document type, mapping is super simple.

``` csharp
// For simple classes you don't need to create a mapper. 
// The registry will automatically create one based on the default conventional logic.
// Mappers added this way automatically use lazy mapping for `virtual` properties.
UmbMapperRegistry.AddMapperFor<SimplePublishedItem>;
```

#### Mapping a Complex Class

Convention-based mapping like that will work most of the time but sometimes you need more granular control.

This more complex class demonstrates that. In this example i've added an enum, an additional property requiring an alias, and also a property that does not exist on the document type.


``` csharp
public class ComplexPublishedItem : SimplePublishedItem
{
    // Not an editable property in the backoffice, mapped from the "name" property
    public virtual string Slug { get; set; }

   // This wil fallback to "createDate" is there have been no updates
    public virtual DateTime UpdateDate { get; set; }

    // An enum, your type would be a dropdown
    public virtual PlaceOrder PlaceOrder { get; set; }
}
```

#### Registering a Complex Class Mapping

For this class, due to specialization, you need to create a mapping class to tell UmbMapper what to do. This class inherits from `UmbMapperConfig<T>` where `T` is the class you want to map to.

``` csharp
public class ComplexPublishedItemMap : UmbMapperConfig<ComplexPublishedItem>
{
    public ComplexPublishedItemMap()
    {
        // Map all the properties as lazy properties
        this.MapAll().ForEach(x => x.AsLazy());

        // Map from the resolved "name" property in the class
        this.AddMap(p => p.Slug).MapFromInstance((instance, content) => instance.Name.ToLowerInvariant());

        // Try "updateDate" and fallback to "createDate"
        this.AddMap(p => p.UpdateDate).SetAlias(p => p.UpdateDate, p => p.CreateDate);

        // Set our enum mapper (built in)
        this.AddMap(p => p.PlaceOrder).SetMapper<EnumPropertyMapper>();
    }
}
```

Registering a custom mapper is as simple as follows.

``` csharp
// Add the mapper we created to the registry
UmbMapperRegistry.AddMapper(new ComplexPublishedItemMap());
```

### Configuration Options

The `AddMap()` method and subsequent methods called in the mapper constructor each return a `PropertyMap<T>` where `T` is the class you want to map to. This allows us to use a simple fluent API to configure each property map.

The various mapping configuration options are as follows:

- `AddMap()` Instructs the mapper to map the property.
- `AddMappings()` Instructs the mapper to map the collection of properties.
- `MapAll()` Instructs the mapper to map all the the properties in the class.
- `MapFromInstance()` Instructs the mapper to map from the given `Func<T, IPublishedContent, object>` where `T` is the current object instance.
- `Ignore()` Removes a property from the collection of property maps.
- `SetAlias()` Instructs the mapper what aliases to look for in the document type. The order given is the checking order. Case-insensitive.
- `SetMapper()` Instructs the mapper what specific `IPropertyMapper` implementation to use for mapping the property. All properties are initially automatically mapped using the `UmbracoPropertyMapper`.
- `SetCulture()` Instructs the mapper what culture to use when mapping values. Defaults to the current culture contained withing the `UmbracoContext`.
- `AsRecursive()` Instructs the mapper to recursively traverse up the document tree looking for a value to map.
- `AsLazy()` Instructs the mapper to map the property lazily using dynamic proxy generation.

### Mappers

Available `IPropertyMapper`implementations all inherit from the `PropertyMapperBase` class and are as follows:

- `UmbracoPropertyMapper` The default mapper, maps directly from Umbraco's Published Content Cache via `GetPropertyValue`. Runs automatically.
- `EnumPropertyMapper` Maps to enum values. Can handle both integer and string values.
- `DocTypeFactoryPropertyMapper` Allows mapping from mixed `IPublishedContent` sources sharing common properties. Inherits `FactoryPropertyMapperBase`.
- `CsvPropertyMapper` Allows mapping of comma separated string values to arrays of strings, integrals, and real types. Values are automatically clamped and rounded.
- `UmbracoPickerPropertyMapper` Maps from all the Umbraco built-in legacy pickers. Not required for any of the pickers from v7.6+

These mappers handle most use cases since they utilize Umbraco's `PropertyValueConverter` API. Additional mappers can be easily created though. Check the source for examples.

Specialist mappers for Archetype, NuPickers, and Vorto are available also via installing the additional packages.

- `ArchetypeFactoryPropertyMapper` Maps all Archetype properties
- `NuPickerPropertyMapper` Maps NuPicker properties
- `NuPickerEnumPropertyMapper` Maps from a NuPicker value to an enum
- `VortoPropertyMapper` Maps all Vorto properties

### Calling a Mapper

There are four extension methods that have been added to the `IPublishedContent` interface providing compile-time and run-time creation and mapping variants. Their signatures are as follows:

``` csharp
// Map a collection of a compile-time known type instances.
public static IEnumerable<T> MapTo<T>(this IEnumerable<IPublishedContent> content)
    where T : class

// Map a collection of a run-time known type instances.
public static IEnumerable<object> MapTo(this IEnumerable<IPublishedContent> content, Type type)

// Map a single compile-time known type instance.
public static T MapTo<T>(this IPublishedContent content)
    where T : class

// Map a single run-time known type instance.
public static object MapTo(this IPublishedContent content, Type type)
```

It's also possible to map to existing classes.

``` csharp
// Map to a single compile-time known type instance.
public static void MapTo<T>(this IPublishedContent content, T destination)
    where T : class

// Map to a single run-time known type instance.
public static void MapTo(this IPublishedContent content, object destination)
```

> Note: It's recommended that you use the following method to create your target instances as this allows the creation of proxy classes which allow lazy loading.

``` csharp
// Creates an empty instance of the given type.
public static T CreateEmpty<T>()
    where T : class

// Creates an empty instance of the given type passing the published 
// content to the constructor.
public static T CreateEmpty<T>(IPublishedContent content)
    where T : class
```

## IPublishedContentModelFactory

As of v7.1.4, Umbraco ships with using a default model factory for `IPublishedContent`.
For more information about the [IPublishedContentModelFactory](https://github.com/zpqrtbnk/Zbu.ModelsBuilder/wiki/IPublishedContentModelFactory) please see the "Zbu.ModelsBuilder" wiki:

UmbMapper comes with an implementation that can be configured as following.

``` csharp
using UmbMapper.PublishedContentModelFactory;

public class ConfigurePublishedContentModelFactory : ApplicationEventHandler
{
	protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
	{
		// Important! This has to occur after mapper registration
		var factory = new UmbMapperPublishedContentModelFactory();
		PublishedContentModelFactoryResolver.Current.SetFactory(factory);
	}
}
```

> Note: It's recommended that you use lazy property mapping when using the `UmbMapperPublishedContentModelFactory` as it ensures that any `PropertyValueConverter`implementations 
that have `UmbracoContext` based requirements may fail otherwise.

## Performance

**UmbMapper is blazingly quick. No other measurable mapper can match it's performance**. The underpinning logic is simple also and requires very little run-time work as the rules are already determined at compile-time.

Additional performance boosting can be delivered using lazy mapping.

Check out the benchmarks in the solution.

<img src="https://raw.githubusercontent.com/JimBobSquarePants/UmbMapper/develop/build/assets/benchmark.JPG" alt="UmbMapper Benchmarks"/>
   

## Dynamic Proxies and Lazy Mapping

> *Any sufficiently advanced technology is indistinguishable from magic.*  - Gandalf the Grey

If a mapper is configured using the `AsLazy()` instruction and the class contains properties using the `virtual` keyword, the mapper will generate a dynamic proxy class at run-time to represent the type we are mapping to. This class actually inherits our target class and you'll be able to see it by attaching a debugger.

Any properties configured with lazy mapping are not actually mapped until you specifically call the getter on the property. (Via means of `MethodInfo` interception using terribly complicated `Reflection.Emit`) this means that we can map large collections with very limited overheads.
