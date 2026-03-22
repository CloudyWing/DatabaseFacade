# DatabaseFacade

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE.md)
[![Build and Deploy Documentation](https://github.com/CloudyWing/DatabaseFacade/actions/workflows/docfx-publish.yml/badge.svg)](https://github.com/CloudyWing/DatabaseFacade/actions/workflows/docfx-publish.yml)
[![Publish to NuGet](https://github.com/CloudyWing/DatabaseFacade/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/CloudyWing/DatabaseFacade/actions/workflows/nuget-publish.yml)

DatabaseFacade 是用來簡化 ADO.NET 操作流程的資料庫工具，本身不具有操作資料庫的能力，需搭配對應的資料庫 Library 使用。

寫這個套件不是要讓大家使用，只是因為某個念想，想把過往寫得Respository [DatabaseClients](https://github.com/CloudyWing/DatabaseClients) 完善，但後面發現命名不太適合，所以就重新開一個新的。

目前操作 ADO.NET 已經有一個很強大的套件 [Dapper](https://github.com/DapperLib/Dapper)，如果不使用 Entity Framework 的話，我是建議使用 Dapper，DatabaseFacade 能在程式碼能給有需要的人做為參考就足夠了。

## 支援版本
- net10.0
- netstandard2.0
- net45

## 使用教學
首先先在 NuGet 安裝你要使用的資料庫 Client，例如：「System.Data.SqlClient」、「Microsoft.Data.SqlClient」，然後在應用程式啟用時，設定 FacadeConfiguration，至少要設定對應的 DbFactory 和連線字串。
```csharp
FacadeConfiguration.SetConfiguration(SqlClientFactory.Instance, "{資料庫連線字串}");
```
實際使用(以 SQL Server 語法為例)
```csharp
using CommandExecutor executor = new CommandExecutor();
executor.CommandText = "SELECT * FROM Table WHERE Id IN @Id";
executor.Parameters.Add("Id", new int[] { 1, 2, 3 });
DataTable dt = executor.CreateDataTable();
```

### 非同步 API

如果是在 ASP.NET Core、BackgroundService，或其他本來就以 `async` / `await` 為主的流程中使用，也可以直接呼叫非同步版本 API：

- `CreateDataReaderAsync`
- `CreateDataTableAsync`
- `QueryScalarAsync`
- `ExecuteAsync`

所有非同步方法都保留原本的同步 API，並支援 `CancellationToken`。

```csharp
using CommandExecutor executor = new CommandExecutor();
executor.CommandText = "SELECT * FROM Table WHERE Id IN @Id";
executor.Parameters.Add("Id", new int[] { 1, 2, 3 });
DataTable dt = await executor.CreateDataTableAsync(cancellationToken);
```

如果需要逐筆讀取資料，可以直接使用 `CreateDataReaderAsync` 回傳的 `DbDataReader`：

```csharp
using CommandExecutor executor = new CommandExecutor();
executor.CommandText = "SELECT Id, Name FROM Table WHERE Id IN @Id";
executor.Parameters.Add("Id", new int[] { 1, 2, 3 });

using DbDataReader reader = await executor.CreateDataReaderAsync(cancellationToken: cancellationToken);
while (await reader.ReadAsync(cancellationToken)) {
    Console.WriteLine($"{reader["Id"]}, {reader["Name"]}");
}
```

如果搭配 Transaction 使用，作法與同步版本相同，只是把執行資料庫作業的 API 換成 Async 版本即可。

### FacadeConfiguration可設定項目

| 屬性 | 型別 | 用途 |
| --- | --- | --- |
| DefaultDbProviderFactory | DbProviderFactory | 用來建立 ADO.NET 相關物件 |
| DefaultConnectionString | string | 資料庫連線字串 |
| DefaultCommandTimeout | int | Command 的 Timeout 時間，預設為 30 秒 |
| DefaultKeepConnection | bool | 執行完 SQL 語法後，是否要關閉 SqlConnection |
| DefaultIsolationLevel | IsolationLevel | Transaction 的層級鎖，預設為 ReadCommitted |
| ParameterNamePrefix | string | 參數型別為 IEnumerable 時，產生的 ParameterName Prefix，預設「CloudyWing」 |
| OnCommandCreating | Action<ParameterCollection, string?> | 可在建立 DbCommand 前加入自訂程式碼 |
| OnCommandCreated | Action<IDbCommand> | 可在建立 DbCommand 後加入自訂程式碼 |

## Documentation

完整文檔請參考：<https://cloudywing.github.io/DatabaseFacade/>

或查看以下主要文章：

- **[入門指南](./docs/articles/getting-started.md)**
- **[建立 DbParameters 的方法](./docs/articles/creating-parameters.md)**
- **[執行資料庫作業 API 後，初始化 Command 資訊](./docs/articles/resetting-command-state.md)**
- **[Transaction 使用方法](./docs/articles/transactions.md)**
- **[非同步操作](./docs/articles/async-operations.md)**
- **[建立 SQL 語法 Log](./docs/articles/sql-logging.md)**

## License
This project is MIT [licensed](./LICENSE.md).
