# 入門指南

## 安裝

在 NuGet 安裝對應的資料庫 Client，例如：

```
dotnet add package Microsoft.Data.SqlClient
```

## 基本設定

在應用程式啟動時呼叫 `FacadeConfiguration.SetConfiguration`，至少需設定 `DbProviderFactory` 和連線字串。

```csharp
using CloudyWing.DatabaseFacade;
using Microsoft.Data.SqlClient;

FacadeConfiguration.SetConfiguration(SqlClientFactory.Instance, "your-connection-string");
```

## 基本查詢

```csharp
using CloudyWing.DatabaseFacade;

using CommandExecutor executor = new CommandExecutor();
executor.CommandText = "SELECT * FROM Customers WHERE Id IN @Id";
executor.Parameters.Add("Id", new int[] { 1, 2, 3 });
DataTable dt = executor.CreateDataTable();
```

## 基本非同步查詢

```csharp
using CloudyWing.DatabaseFacade;

using CommandExecutor executor = new CommandExecutor();
executor.CommandText = "SELECT * FROM Customers WHERE Id IN @Id";
executor.Parameters.Add("Id", new int[] { 1, 2, 3 });
DataTable dt = await executor.CreateDataTableAsync(cancellationToken);
```

If you need to process data sequentially, use `CreateDataReaderAsync` with `DbDataReader.ReadAsync`. See [Async Operations](async-operations.md) for more examples.

## 執行非查詢指令

```csharp
using CommandExecutor executor = new CommandExecutor();
executor.SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
    .Parameters.Add("Id", 1)
    .Add("Name", "小明")
    .GetCommandExecutor()
    .Execute();
```
