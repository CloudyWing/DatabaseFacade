### [CloudyWing\.DatabaseFacade](CloudyWing.DatabaseFacade.md 'CloudyWing\.DatabaseFacade')

## ParameterMetadata Class

The parameter metadata\.

```csharp
public sealed class ParameterMetadata : System.ICloneable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ParameterMetadata

Implements [System\.ICloneable](https://learn.microsoft.com/en-us/dotnet/api/system.icloneable 'System\.ICloneable')

### See Also
- [System\.ICloneable](https://learn.microsoft.com/en-us/dotnet/api/system.icloneable 'System\.ICloneable')
### Constructors

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ParameterMetadata()'></a>

## ParameterMetadata\(\) Constructor

Initializes a new instance of the [ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing\.DatabaseFacade\.ParameterMetadata') class\.

```csharp
public ParameterMetadata();
```

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ParameterMetadata(CloudyWing.DatabaseFacade.ParameterMetadata)'></a>

## ParameterMetadata\(ParameterMetadata\) Constructor

Initializes a new instance of the [ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing\.DatabaseFacade\.ParameterMetadata') class\.

```csharp
internal ParameterMetadata(CloudyWing.DatabaseFacade.ParameterMetadata from);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ParameterMetadata(CloudyWing.DatabaseFacade.ParameterMetadata).from'></a>

`from` [ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing\.DatabaseFacade\.ParameterMetadata')

From\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ParameterMetadata(System.Data.IDbDataParameter)'></a>

## ParameterMetadata\(IDbDataParameter\) Constructor

Initializes a new instance of the [ParameterMetadata](CloudyWing.DatabaseFacade.ParameterMetadata.md 'CloudyWing\.DatabaseFacade\.ParameterMetadata') class\.

```csharp
internal ParameterMetadata(System.Data.IDbDataParameter from);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ParameterMetadata(System.Data.IDbDataParameter).from'></a>

`from` [System\.Data\.IDbDataParameter](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbdataparameter 'System\.Data\.IDbDataParameter')

From\.
### Properties

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.DbType'></a>

## ParameterMetadata\.DbType Property

Gets or sets the type of the database\.

```csharp
public System.Nullable<System.Data.DbType> DbType { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Data\.DbType](https://learn.microsoft.com/en-us/dotnet/api/system.data.dbtype 'System\.Data\.DbType')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')
The type of the database\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.Direction'></a>

## ParameterMetadata\.Direction Property

Gets or sets the direction\.

```csharp
public System.Data.ParameterDirection Direction { get; set; }
```

#### Property Value
[System\.Data\.ParameterDirection](https://learn.microsoft.com/en-us/dotnet/api/system.data.parameterdirection 'System\.Data\.ParameterDirection')
The direction\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ParameterName'></a>

## ParameterMetadata\.ParameterName Property

Gets or sets the name of the parameter\.

```csharp
public string ParameterName { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
The name of the parameter\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.Precision'></a>

## ParameterMetadata\.Precision Property

Gets or sets the precision\.

```csharp
public System.Nullable<byte> Precision { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')
The precision\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.Scale'></a>

## ParameterMetadata\.Scale Property

Gets or sets the scale\.

```csharp
public System.Nullable<byte> Scale { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte 'System\.Byte')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')
The scale\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.Size'></a>

## ParameterMetadata\.Size Property

Gets or sets the size\.

```csharp
public System.Nullable<int> Size { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')
The size\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.SourceColumn'></a>

## ParameterMetadata\.SourceColumn Property

Gets or sets the source column\.

```csharp
public string SourceColumn { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
The source column\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.SourceVersion'></a>

## ParameterMetadata\.SourceVersion Property

Gets or sets the source version\.

```csharp
public System.Nullable<System.Data.DataRowVersion> SourceVersion { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Data\.DataRowVersion](https://learn.microsoft.com/en-us/dotnet/api/system.data.datarowversion 'System\.Data\.DataRowVersion')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')
The source version\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.Value'></a>

## ParameterMetadata\.Value Property

Gets or sets the value\.

```csharp
public object Value { get; set; }
```

#### Property Value
[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')
The value\.
### Methods

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ApplyParameter(System.Data.IDbDataParameter)'></a>

## ParameterMetadata\.ApplyParameter\(IDbDataParameter\) Method

Applies the parameter\.

```csharp
public void ApplyParameter(System.Data.IDbDataParameter to);
```
#### Parameters

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.ApplyParameter(System.Data.IDbDataParameter).to'></a>

`to` [System\.Data\.IDbDataParameter](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbdataparameter 'System\.Data\.IDbDataParameter')

To\.

<a name='CloudyWing.DatabaseFacade.ParameterMetadata.Clone()'></a>

## ParameterMetadata\.Clone\(\) Method

Creates a new object that is a copy of the current instance\.

```csharp
public object Clone();
```

Implements [Clone\(\)](https://learn.microsoft.com/en-us/dotnet/api/system.icloneable.clone 'System\.ICloneable\.Clone')

#### Returns
[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')
A new object that is a copy of this instance\.