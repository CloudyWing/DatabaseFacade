### [CloudyWing.DatabaseFacade](CloudyWing.DatabaseFacade.md 'CloudyWing.DatabaseFacade')

## ParameterCollection Class

The parameter collection.

```csharp
public sealed class ParameterCollection : System.Collections.ObjectModel.KeyedCollection<string, CloudyWing.DatabaseFacade.ParameterMetadata>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Collections.ObjectModel.Collection&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.ObjectModel.Collection-1 'System.Collections.ObjectModel.Collection`1')[ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing.DatabaseFacade.ParameterMetadata')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.ObjectModel.Collection-1 'System.Collections.ObjectModel.Collection`1') &#129106; [System.Collections.ObjectModel.KeyedCollection&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.ObjectModel.KeyedCollection-2 'System.Collections.ObjectModel.KeyedCollection`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.ObjectModel.KeyedCollection-2 'System.Collections.ObjectModel.KeyedCollection`2')[ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing.DatabaseFacade.ParameterMetadata')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.ObjectModel.KeyedCollection-2 'System.Collections.ObjectModel.KeyedCollection`2') &#129106; ParameterCollection
### Methods

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(CloudyWing.DatabaseFacade.ParameterMetadata)'></a>

## ParameterCollection.Add(ParameterMetadata) Method

Adds the specified metadata.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Add(CloudyWing.DatabaseFacade.ParameterMetadata metadata);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(CloudyWing.DatabaseFacade.ParameterMetadata).metadata'></a>

`metadata` [ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing.DatabaseFacade.ParameterMetadata')

The metadata.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object)'></a>

## ParameterCollection.Add(string, object) Method

Adds the specified parameter name.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Add(string parameterName, object value);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object).parameterName'></a>

`parameterName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Name of the parameter.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object).value'></a>

`value` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The value.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType)'></a>

## ParameterCollection.Add(string, object, DbType) Method

Adds the specified parameter name.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Add(string parameterName, object value, System.Data.DbType dbType);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType).parameterName'></a>

`parameterName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Name of the parameter.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType).value'></a>

`value` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The value.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType).dbType'></a>

`dbType` [System.Data.DbType](https://docs.microsoft.com/en-us/dotnet/api/System.Data.DbType 'System.Data.DbType')

Type of the database.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,byte,byte)'></a>

## ParameterCollection.Add(string, object, DbType, byte, byte) Method

Adds the specified parameter name.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Add(string parameterName, object value, System.Data.DbType dbType, byte precision, byte scale);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,byte,byte).parameterName'></a>

`parameterName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Name of the parameter.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,byte,byte).value'></a>

`value` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The value.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,byte,byte).dbType'></a>

`dbType` [System.Data.DbType](https://docs.microsoft.com/en-us/dotnet/api/System.Data.DbType 'System.Data.DbType')

Type of the database.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,byte,byte).precision'></a>

`precision` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')

The precision.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,byte,byte).scale'></a>

`scale` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')

The scale.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,int)'></a>

## ParameterCollection.Add(string, object, DbType, int) Method

Adds the specified parameter name.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Add(string parameterName, object value, System.Data.DbType dbType, int size);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,int).parameterName'></a>

`parameterName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Name of the parameter.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,int).value'></a>

`value` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The value.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,int).dbType'></a>

`dbType` [System.Data.DbType](https://docs.microsoft.com/en-us/dotnet/api/System.Data.DbType 'System.Data.DbType')

Type of the database.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,int).size'></a>

`size` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The size.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,System.Data.ParameterDirection)'></a>

## ParameterCollection.Add(string, object, DbType, ParameterDirection) Method

Adds the specified parameter name.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Add(string parameterName, object value, System.Data.DbType dbType, System.Data.ParameterDirection direction);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,System.Data.ParameterDirection).parameterName'></a>

`parameterName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Name of the parameter.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,System.Data.ParameterDirection).value'></a>

`value` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The value.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,System.Data.ParameterDirection).dbType'></a>

`dbType` [System.Data.DbType](https://docs.microsoft.com/en-us/dotnet/api/System.Data.DbType 'System.Data.DbType')

Type of the database.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(string,object,System.Data.DbType,System.Data.ParameterDirection).direction'></a>

`direction` [System.Data.ParameterDirection](https://docs.microsoft.com/en-us/dotnet/api/System.Data.ParameterDirection 'System.Data.ParameterDirection')

The direction.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(System.Data.IDbDataParameter)'></a>

## ParameterCollection.Add(IDbDataParameter) Method

Adds the specified parameter.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection Add(System.Data.IDbDataParameter parameter);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.Add(System.Data.IDbDataParameter).parameter'></a>

`parameter` [System.Data.IDbDataParameter](https://docs.microsoft.com/en-us/dotnet/api/System.Data.IDbDataParameter 'System.Data.IDbDataParameter')

The parameter.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(CloudyWing.DatabaseFacade.ParameterMetadata[])'></a>

## ParameterCollection.AddRange(ParameterMetadata[]) Method

Adds the range.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection AddRange(params CloudyWing.DatabaseFacade.ParameterMetadata[] parameters);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(CloudyWing.DatabaseFacade.ParameterMetadata[]).parameters'></a>

`parameters` [ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing.DatabaseFacade.ParameterMetadata')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The parameters.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(object)'></a>

## ParameterCollection.AddRange(object) Method

Adds the range.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection AddRange(object obj);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(object).obj'></a>

`obj` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The object.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Collections.Generic.IDictionary_string,object_)'></a>

## ParameterCollection.AddRange(IDictionary<string,object>) Method

Adds the range.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection AddRange(System.Collections.Generic.IDictionary<string,object> pairs);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Collections.Generic.IDictionary_string,object_).pairs'></a>

`pairs` [System.Collections.Generic.IDictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2 'System.Collections.Generic.IDictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2 'System.Collections.Generic.IDictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2 'System.Collections.Generic.IDictionary`2')

The pairs.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

#### Exceptions

[System.ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentNullException 'System.ArgumentNullException')  
pairs

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Collections.Generic.IEnumerable_CloudyWing.DatabaseFacade.ParameterMetadata_)'></a>

## ParameterCollection.AddRange(IEnumerable<ParameterMetadata>) Method

Adds the range.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection AddRange(System.Collections.Generic.IEnumerable<CloudyWing.DatabaseFacade.ParameterMetadata> parameters);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Collections.Generic.IEnumerable_CloudyWing.DatabaseFacade.ParameterMetadata_).parameters'></a>

`parameters` [System.Collections.Generic.IEnumerable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1 'System.Collections.Generic.IEnumerable`1')[ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing.DatabaseFacade.ParameterMetadata')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1 'System.Collections.Generic.IEnumerable`1')

The parameters.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

#### Exceptions

[System.ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentNullException 'System.ArgumentNullException')  
parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Collections.Generic.IEnumerable_System.Data.IDbDataParameter_)'></a>

## ParameterCollection.AddRange(IEnumerable<IDbDataParameter>) Method

Adds the range.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection AddRange(System.Collections.Generic.IEnumerable<System.Data.IDbDataParameter> parameters);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Collections.Generic.IEnumerable_System.Data.IDbDataParameter_).parameters'></a>

`parameters` [System.Collections.Generic.IEnumerable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1 'System.Collections.Generic.IEnumerable`1')[System.Data.IDbDataParameter](https://docs.microsoft.com/en-us/dotnet/api/System.Data.IDbDataParameter 'System.Data.IDbDataParameter')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1 'System.Collections.Generic.IEnumerable`1')

The parameters.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

#### Exceptions

[System.ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentNullException 'System.ArgumentNullException')  
parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Data.IDbDataParameter[])'></a>

## ParameterCollection.AddRange(IDbDataParameter[]) Method

Adds the range.

```csharp
public CloudyWing.DatabaseFacade.ParameterCollection AddRange(params System.Data.IDbDataParameter[] parameters);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterCollection.AddRange(System.Data.IDbDataParameter[]).parameters'></a>

`parameters` [System.Data.IDbDataParameter](https://docs.microsoft.com/en-us/dotnet/api/System.Data.IDbDataParameter 'System.Data.IDbDataParameter')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The parameters.

#### Returns
[ParameterCollection](CloudyWing.DatabaseFacade.ParameterCollection.md 'CloudyWing.DatabaseFacade.ParameterCollection')  
The self.

<a name='CloudyWing.DatabaseFacade.ParameterCollection.GetCommandExecutor()'></a>

## ParameterCollection.GetCommandExecutor() Method

Gets the command executor.

```csharp
public CloudyWing.DatabaseFacade.CommandExecutor GetCommandExecutor();
```

#### Returns
[CommandExecutor](CloudyWing.DatabaseFacade.CommandExecutor.md 'CloudyWing.DatabaseFacade.CommandExecutor')  
The command executor.