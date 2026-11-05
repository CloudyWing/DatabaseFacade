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

如果需要逐筆處理資料，可搭配 `CreateDataReaderAsync` 與 `DbDataReader.ReadAsync` 使用，更多範例請參考[非同步操作](async-operations.md)，其他取得查詢結果的方式請參考[查詢結果的取得方式](querying-results.md)。

## 執行非查詢指令

```csharp
using CommandExecutor executor = new CommandExecutor();
executor.SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
    .Parameters.Add("Id", 1)
    .Add("Name", "小明")
    .GetCommandExecutor()
    .Execute();
```

## 執行緒安全

- `FacadeConfiguration` 的設定值是整個應用程式共用的靜態狀態，沒有同步機制，請在應用程式啟動時設定一次，不要在其他執行緒執行指令時修改。
- `CommandExecutor` 不是執行緒安全的型別，內部持有連線、交易與指令狀態，請每個工作單位使用一個實例，不要在多個並行作業間共用。
