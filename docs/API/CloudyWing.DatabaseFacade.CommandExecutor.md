### [CloudyWing.DatabaseFacade](CloudyWing.DatabaseFacade.md 'CloudyWing.DatabaseFacade')

## CommandExecutor Class

The command executor.

```csharp
public sealed class CommandExecutor :
System.IDisposable
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; CommandExecutor

Implements [System.IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable 'System.IDisposable')
### Constructors

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(string,System.Nullable_bool_)'></a>

## CommandExecutor(string, Nullable<bool>) Constructor

Initializes a new instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing.DatabaseFacade.CommandExecutor') class.

```csharp
public CommandExecutor(string connStr, System.Nullable<bool> keepConnection=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(string,System.Nullable_bool_).connStr'></a>

`connStr` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The connection string.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(string,System.Nullable_bool_).keepConnection'></a>

`keepConnection` [System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')

The keep connection.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_)'></a>

## CommandExecutor(DbProviderFactory, string, Nullable<bool>) Constructor

Initializes a new instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing.DatabaseFacade.CommandExecutor') class.

```csharp
public CommandExecutor(System.Data.Common.DbProviderFactory providerFactory, string connStr, System.Nullable<bool> keepConnection);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_).providerFactory'></a>

`providerFactory` [System.Data.Common.DbProviderFactory](https://docs.microsoft.com/en-us/dotnet/api/System.Data.Common.DbProviderFactory 'System.Data.Common.DbProviderFactory')

The provider factory.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_).connStr'></a>

`connStr` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The connection string.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Data.Common.DbProviderFactory,string,System.Nullable_bool_).keepConnection'></a>

`keepConnection` [System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')

The keep connection.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Nullable_bool_)'></a>

## CommandExecutor(Nullable<bool>) Constructor

Initializes a new instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing.DatabaseFacade.CommandExecutor') class.

```csharp
public CommandExecutor(System.Nullable<bool> keepConnection=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandExecutor(System.Nullable_bool_).keepConnection'></a>

`keepConnection` [System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')

The keep connection.
### Properties

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandText'></a>

## CommandExecutor.CommandText Property

Gets or sets the command text.

```csharp
public string CommandText { get; set; }
```

#### Property Value
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The command text.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandTimeout'></a>

## CommandExecutor.CommandTimeout Property

Gets or sets the command timeout.

```csharp
public int CommandTimeout { get; set; }
```

#### Property Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
The command timeout.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CommandType'></a>

## CommandExecutor.CommandType Property

Gets or sets the type of the command.

```csharp
public System.Data.CommandType CommandType { get; set; }
```

#### Property Value
[System.Data.CommandType](https://docs.microsoft.com/en-us/dotnet/api/System.Data.CommandType 'System.Data.CommandType')  
The type of the command.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.ConnectionString'></a>

## CommandExecutor.ConnectionString Property

Gets the connection string.

```csharp
public string ConnectionString { get; set; }
```

#### Property Value
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The connection string.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.DbProviderFactory'></a>

## CommandExecutor.DbProviderFactory Property

Gets the database provider factory.

```csharp
public System.Data.Common.DbProviderFactory DbProviderFactory { get; set; }
```

#### Property Value
[System.Data.Common.DbProviderFactory](https://docs.microsoft.com/en-us/dotnet/api/System.Data.Common.DbProviderFactory 'System.Data.Common.DbProviderFactory')  
The database provider factory.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.KeepConnection'></a>

## CommandExecutor.KeepConnection Property

Gets a value indicating whether [keep connection].

```csharp
public bool KeepConnection { get; set; }
```

#### Property Value
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
`true` if [keep connection]; otherwise, `false`.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Parameters'></a>

## CommandExecutor.Parameters Property

Gets the parameters.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Parameters { get; }
```

#### Property Value
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The parameters.
### Methods

<a name='CloudyWing.DatabaseFacade.CommandExecutor.~CommandExecutor()'></a>

## CommandExecutor.~CommandExecutor() Method

Finalizes an instance of the [CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing.DatabaseFacade.CommandExecutor') class.

```csharp
~CommandExecutor();
```

<a name='CloudyWing.DatabaseFacade.CommandExecutor.BeginTransaction(System.Nullable_System.Data.IsolationLevel_)'></a>

## CommandExecutor.BeginTransaction(Nullable<IsolationLevel>) Method

Begins the transaction.

```csharp
public System.Data.IDbTransaction BeginTransaction(System.Nullable<System.Data.IsolationLevel> level=null);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.BeginTransaction(System.Nullable_System.Data.IsolationLevel_).level'></a>

`level` [System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.Data.IsolationLevel](https://docs.microsoft.com/en-us/dotnet/api/System.Data.IsolationLevel 'System.Data.IsolationLevel')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')

The level.

#### Returns
[System.Data.IDbTransaction](https://docs.microsoft.com/en-us/dotnet/api/System.Data.IDbTransaction 'System.Data.IDbTransaction')  
The object representing the new transaction.

#### Exceptions

[KeepConnectionRequiredException](CloudyWing.DatabaseFacade.KeepConnectionRequiredException.md 'CloudyWing.DatabaseFacade.KeepConnectionRequiredException')

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataReader(CloudyWing.DatabaseFacade.ResetItems,System.Data.CommandBehavior)'></a>

## CommandExecutor.CreateDataReader(ResetItems, CommandBehavior) Method

Creates the data reader.

```csharp
public System.Data.IDataReader CreateDataReader(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All, System.Data.CommandBehavior behavior=System.Data.CommandBehavior.SequentialAccess);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataReader(CloudyWing.DatabaseFacade.ResetItems,System.Data.CommandBehavior).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing.DatabaseFacade.ResetItems')

The then reset.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataReader(CloudyWing.DatabaseFacade.ResetItems,System.Data.CommandBehavior).behavior'></a>

`behavior` [System.Data.CommandBehavior](https://docs.microsoft.com/en-us/dotnet/api/System.Data.CommandBehavior 'System.Data.CommandBehavior')

The behavior.

#### Returns
[System.Data.IDataReader](https://docs.microsoft.com/en-us/dotnet/api/System.Data.IDataReader 'System.Data.IDataReader')  
The data reader.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataTable(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor.CreateDataTable(ResetItems) Method

Creates the data table.

```csharp
public System.Data.DataTable CreateDataTable(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.CreateDataTable(CloudyWing.DatabaseFacade.ResetItems).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing.DatabaseFacade.ResetItems')

The then reset.

#### Returns
[System.Data.DataTable](https://docs.microsoft.com/en-us/dotnet/api/System.Data.DataTable 'System.Data.DataTable')  
The data table.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Dispose()'></a>

## CommandExecutor.Dispose() Method

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public void Dispose();
```

Implements [Dispose()](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable.Dispose 'System.IDisposable.Dispose')

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Execute(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor.Execute(ResetItems) Method

Executes the specified then reset.

```csharp
public int Execute(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Execute(CloudyWing.DatabaseFacade.ResetItems).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing.DatabaseFacade.ResetItems')

The then reset.

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
The number of rows affected.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Initialize(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor.Initialize(ResetItems) Method

Initializes the specified items.

```csharp
public void Initialize(CloudyWing.DatabaseFacade.ResetItems items=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.Initialize(CloudyWing.DatabaseFacade.ResetItems).items'></a>

`items` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing.DatabaseFacade.ResetItems')

The items.

<a name='CloudyWing.DatabaseFacade.CommandExecutor.QueryScalar(CloudyWing.DatabaseFacade.ResetItems)'></a>

## CommandExecutor.QueryScalar(ResetItems) Method

Queries the scalar.

```csharp
public object QueryScalar(CloudyWing.DatabaseFacade.ResetItems thenReset=CloudyWing.DatabaseFacade.ResetItems.All);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.CommandExecutor.QueryScalar(CloudyWing.DatabaseFacade.ResetItems).thenReset'></a>

`thenReset` [ResetItems](CloudyWing.DatabaseFacade.ResetItems.md 'CloudyWing.DatabaseFacade.ResetItems')

The then reset.

#### Returns
[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')  
The first column of the first row in the resultset.