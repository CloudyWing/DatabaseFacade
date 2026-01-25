### [CloudyWing\.DatabaseFacade](CloudyWing.DatabaseFacade.md 'CloudyWing\.DatabaseFacade')

## CommandExecutor Class

The command executor\.

```csharp
public sealed class CommandExecutor : System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CommandExecutor

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')
### Constructors

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(string,System.Nullable_bool_)'></a>

## CommandExecutor\(string, Nullable\<bool\>\) Constructor

Initializes a new instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing\.DatabaseFacade\.CommandExecutor') class\.

```csharp
public CommandExecutor(string connStr, System.Nullable<bool> keepConnection=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(string,System.Nullable_bool_).connStr'></a>

`connStr` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The connection string\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(string,System.Nullable_bool_).keepConnection'></a>

`keepConnection` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The keep connection\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_)'></a>

## CommandExecutor\(DbProviderFactory, string, Nullable\<bool\>\) Constructor

Initializes a new instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing\.DatabaseFacade\.CommandExecutor') class\.

```csharp
public CommandExecutor(System.Data.Common.DbProviderFactory providerFactory, string connStr, System.Nullable<bool> keepConnection=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_).providerFactory'></a>

`providerFactory` [System\.Data\.Common\.DbProviderFactory](https://learn.microsoft.com/en-us/dotnet/api/system.data.common.dbproviderfactory 'System\.Data\.Common\.DbProviderFactory')

The provider factory\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_).connStr'></a>

`connStr` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The connection string\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_).keepConnection'></a>

`keepConnection` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The keep connection\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Nullable_bool_)'></a>

## CommandExecutor\(Nullable\<bool\>\) Constructor

Initializes a new instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing\.DatabaseFacade\.CommandExecutor') class\.

```csharp
public CommandExecutor(System.Nullable<bool> keepConnection=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Nullable_bool_).keepConnection'></a>

`keepConnection` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The keep connection\.
### Properties

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandText'></a>

## CommandExecutor\.CommandText Property

Gets or sets the command text\.

```csharp
public string CommandText { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
The command text\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandTimeout'></a>

## CommandExecutor\.CommandTimeout Property

Gets or sets the command timeout\.

```csharp
public int CommandTimeout { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')
The command timeout\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandType'></a>

## CommandExecutor\.CommandType Property

Gets or sets the type of the command\.

```csharp
public System.Data.CommandType CommandType { get; set; }
```

#### Property Value
[System\.Data\.CommandType](https://learn.microsoft.com/en-us/dotnet/api/system.data.commandtype 'System\.Data\.CommandType')
The type of the command\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Connection'></a>

## CommandExecutor\.Connection Property

Gets the connection\.

```csharp
public System.Data.IDbConnection Connection { get; private set; }
```

#### Property Value
[System\.Data\.IDbConnection](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbconnection 'System\.Data\.IDbConnection')
The connection\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.ConnectionString'></a>

## CommandExecutor\.ConnectionString Property

Gets the connection string\.

```csharp
public string ConnectionString { get; private set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
The connection string\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.DbProviderFactory'></a>

## CommandExecutor\.DbProviderFactory Property

Gets the database provider factory\.

```csharp
public System.Data.Common.DbProviderFactory DbProviderFactory { get; private set; }
```

#### Property Value
[System\.Data\.Common\.DbProviderFactory](https://learn.microsoft.com/en-us/dotnet/api/system.data.common.dbproviderfactory 'System\.Data\.Common\.DbProviderFactory')
The database provider factory\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.KeepConnection'></a>

## CommandExecutor\.KeepConnection Property

Gets a value indicating whether \[keep connection\]\.

```csharp
public bool KeepConnection { get; private set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')
`true` if \[keep connection\]; otherwise, `false`\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Parameters'></a>

## CommandExecutor\.Parameters Property

Gets the parameters\.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Parameters { get; }
```

#### Property Value
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing\.DatabaseFacade\.ParameterCollection')
The parameters\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Transaction'></a>

## CommandExecutor\.Transaction Property

Gets the transaction\.

```csharp
public System.Data.IDbTransaction Transaction { get; private set; }
```

#### Property Value
[System\.Data\.IDbTransaction](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbtransaction 'System\.Data\.IDbTransaction')
The transaction\.
### Methods

<a name='CloudyWing.DatabaseFacade.CommandExecutor.BeginTransaction(System.Nullable_System.Data.IsolationLevel_)'></a>

## CommandExecutor\.BeginTransaction\(Nullable\<IsolationLevel\>\) Method

Begins the transaction\.

```csharp
public System.Data.IDbTransaction BeginTransaction(System.Nullable<System.Data.IsolationLevel> level=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.BeginTransaction(System.Nullable_System.Data.IsolationLevel_).level'></a>

`level` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Data\.IsolationLevel](https://learn.microsoft.com/en-us/dotnet/api/system.data.isolationlevel 'System\.Data\.IsolationLevel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The level\.

#### Returns
[System\.Data\.IDbTransaction](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbtransaction 'System\.Data\.IDbTransaction')
The object representing the new transaction\.

#### Exceptions

[KeepConnectionRequiredException](CloudyWing.DatabaseFacade.KeepConnectionRequiredException.md 'CloudyWing\.DatabaseFacade\.KeepConnectionRequiredException')

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataReader(CloudyWing.DatabaseFacade.ResetItems,System.Data.CommandBehavior)'></a>

## CommandExecutor\.CreateDataReader\(ResetItems, CommandBehavior\) Method

Creates the data reader\.

```csharp
public System.Data.IDataReader CreateDataReader(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All, System.Data.CommandBehavior behavior=System.Data.CommandBehavior.SequentialAccess);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataReader(CloudyWing.DatabaseFacade.ResetItems,System.Data.CommandBehavior).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing\.DatabaseFacade\.ResetItems')

The then reset\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataReader(CloudyWing.DatabaseFacade.ResetItems,System.Data.CommandBehavior).behavior'></a>

`behavior` [System\.Data\.CommandBehavior](https://learn.microsoft.com/en-us/dotnet/api/system.data.commandbehavior 'System\.Data\.CommandBehavior')

The behavior\.

#### Returns
[System\.Data\.IDataReader](https://learn.microsoft.com/en-us/dotnet/api/system.data.idatareader 'System\.Data\.IDataReader')
The data reader\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataTable(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor\.CreateDataTable\(ResetItems\) Method

Creates the data table\.

```csharp
public System.Data.DataTable CreateDataTable(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataTable(CloudyWing.DatabaseFacade.ResetItems).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing\.DatabaseFacade\.ResetItems')

The then reset\.

#### Returns
[System\.Data\.DataTable](https://learn.microsoft.com/en-us/dotnet/api/system.data.datatable 'System\.Data\.DataTable')
The data table\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Dispose()'></a>

## CommandExecutor\.Dispose\(\) Method

Performs application\-defined tasks associated with freeing, releasing, or resetting unmanaged resources\.

```csharp
public void Dispose();
```

Implements [Dispose\(\)](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable.dispose 'System\.IDisposable\.Dispose')

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Execute(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor\.Execute\(ResetItems\) Method

Executes the specified then reset\.

```csharp
public int Execute(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Execute(CloudyWing.DatabaseFacade.ResetItems).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing\.DatabaseFacade\.ResetItems')

The then reset\.

#### Returns
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')
The number of rows affected\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Initialize(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor\.Initialize\(ResetItems\) Method

Initializes the specified items\.

```csharp
public void Initialize(CloudyWing.DatabaseFacade.ResetItems items=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Initialize(CloudyWing.DatabaseFacade.ResetItems).items'></a>

`items` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing\.DatabaseFacade\.ResetItems')

The items\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.QueryScalar(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor\.QueryScalar\(ResetItems\) Method

Queries the scalar\.

```csharp
public object QueryScalar(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.QueryScalar(CloudyWing.DatabaseFacade.ResetItems).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing\.DatabaseFacade\.ResetItems')

The then reset\.

#### Returns
[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')
The first column of the first row in the resultset\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.SetCommandText(string,System.Nullable_System.Data.CommandType_)'></a>

## CommandExecutor\.SetCommandText\(string, Nullable\<CommandType\>\) Method

Sets the command text\.

```csharp
public CloudyWing.DatabaseFacade.CommandExecutor SetCommandText(string commadText, System.Nullable<System.Data.CommandType> commandType=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.SetCommandText(string,System.Nullable_System.Data.CommandType_).commadText'></a>

`commadText` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The commad text\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.SetCommandText(string,System.Nullable_System.Data.CommandType_).commandType'></a>

`commandType` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Data\.CommandType](https://learn.microsoft.com/en-us/dotnet/api/system.data.commandtype 'System\.Data\.CommandType')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

Type of the command\. Set property `CommandType` only if parameter `commandType` is not null\.

#### Returns
[CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing\.DatabaseFacade\.CommandExecutor')
The self\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.SetCommandTimeout(int)'></a>

## CommandExecutor\.SetCommandTimeout\(int\) Method

Sets the command timeout\.

```csharp
public CloudyWing.DatabaseFacade.CommandExecutor SetCommandTimeout(int second);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.SetCommandTimeout(int).second'></a>

`second` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The second\.

#### Returns
[CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing\.DatabaseFacade\.CommandExecutor')
The self\.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.~CommandExecutor()'></a>

## CommandExecutor\.~CommandExecutor\(\) Method

Finalizes an instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing\.DatabaseFacade\.CommandExecutor') class\.

```csharp
~CommandExecutor();
```