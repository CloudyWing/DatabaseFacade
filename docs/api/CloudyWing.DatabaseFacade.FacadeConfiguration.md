### [CloudyWing\.DatabaseFacade](CloudyWing.DatabaseFacade.md 'CloudyWing\.DatabaseFacade')

## FacadeConfiguration Class

The facade configuration\.

```csharp
public static class FacadeConfiguration
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; FacadeConfiguration
### Properties

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.DefaultCommandTimeout'></a>

## FacadeConfiguration\.DefaultCommandTimeout Property

Gets or sets the default command timeout\.

```csharp
public static int DefaultCommandTimeout { get; set; }
```

#### Property Value

[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')
The default command timeout\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.DefaultConnectionString'></a>

## FacadeConfiguration\.DefaultConnectionString Property

Gets or sets the default connection string\.

```csharp
public static string DefaultConnectionString { get; set; }
```

#### Property Value

[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
The default connection string\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.DefaultDbProviderFactory'></a>

## FacadeConfiguration\.DefaultDbProviderFactory Property

Gets or sets the default database provider factory\.

```csharp
public static System.Data.Common.DbProviderFactory DefaultDbProviderFactory { get; set; }
```

#### Property Value

[System\.Data\.Common\.DbProviderFactory](https://learn.microsoft.com/en-us/dotnet/api/system.data.common.dbproviderfactory 'System\.Data\.Common\.DbProviderFactory')
The default database provider factory\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.DefaultIsolationLevel'></a>

## FacadeConfiguration\.DefaultIsolationLevel Property

Gets or sets the default isolation level\.

```csharp
public static System.Data.IsolationLevel DefaultIsolationLevel { get; set; }
```

#### Property Value

[System\.Data\.IsolationLevel](https://learn.microsoft.com/en-us/dotnet/api/system.data.isolationlevel 'System\.Data\.IsolationLevel')
The default isolation level\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.DefaultKeepConnection'></a>

## FacadeConfiguration\.DefaultKeepConnection Property

Gets or sets a value indicating whether \[default keep connection\]\.

```csharp
public static bool DefaultKeepConnection { get; set; }
```

#### Property Value

[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')
`true` if \[default keep connection\]; otherwise, `false`\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.OnCommandCreated'></a>

## FacadeConfiguration\.OnCommandCreated Property

Gets or sets the on command created\.

```csharp
public static System.Action<System.Data.IDbCommand> OnCommandCreated { get; set; }
```

#### Property Value

[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.Data\.IDbCommand](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbcommand 'System\.Data\.IDbCommand')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')
The on command created\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.OnCommandCreating'></a>

## FacadeConfiguration\.OnCommandCreating Property

Gets or sets the on command creating\.

```csharp
public static System.Action<CloudyWing.DatabaseFacade.ParameterCollection,string> OnCommandCreating { get; set; }
```

#### Property Value

[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing\.DatabaseFacade\.ParameterCollection')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')
The on command creating\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.ParameterNamePrefix'></a>

## FacadeConfiguration\.ParameterNamePrefix Property

Gets or sets the parameter name prefix\.
            if parameter value is `IEnumerable`，parameter name will be converted to `{ParameterNamePrefix}_{parameter name}_{serial number}`

```csharp
public static string ParameterNamePrefix { get; set; }
```

#### Property Value

[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
The parameter name prefix\.
### Methods

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.SetConfiguration(System.Data.Common.DbProviderFactory,string)'></a>

## FacadeConfiguration\.SetConfiguration\(DbProviderFactory, string\) Method

Sets the configuration\.

```csharp
public static void SetConfiguration(System.Data.Common.DbProviderFactory dbProviderFactory, string connectionString);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.SetConfiguration(System.Data.Common.DbProviderFactory,string).dbProviderFactory'></a>

`dbProviderFactory` [System\.Data\.Common\.DbProviderFactory](https://learn.microsoft.com/en-us/dotnet/api/system.data.common.dbproviderfactory 'System\.Data\.Common\.DbProviderFactory')

The database provider factory\.

<a name='CloudyWing.DatabaseFacade.FacadeConfiguration.SetConfiguration(System.Data.Common.DbProviderFactory,string).connectionString'></a>

`connectionString` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The connection string\.