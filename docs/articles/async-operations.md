# 非同步操作

DatabaseFacade 現在提供對應的非同步 API，讓原本偏向同步流程的用法，也能自然地整合到 `async` / `await` 為主的應用程式中。

## 可用方法

| 同步方法 | 非同步方法 | 回傳型別 |
| --- | --- | --- |
| `CreateDataReader` | `CreateDataReaderAsync` | `Task<DbDataReader>` |
| `CreateDataTable` | `CreateDataTableAsync` | `Task<DataTable>` |
| `QueryScalar` | `QueryScalarAsync` | `Task<object?>` |
| `Execute` | `ExecuteAsync` | `Task<int>` |

所有非同步方法都會保留原本的同步 API，並額外提供 `CancellationToken` 參數。

## 非同步查詢 DataTable

```csharp
using CommandExecutor executor = new CommandExecutor();
executor.CommandText = "SELECT * FROM Customers WHERE Id IN @Ids";
executor.Parameters.Add("Ids", new int[] { 1, 2, 3 });

DataTable dataTable = await executor.CreateDataTableAsync(cancellationToken);
```

## 非同步逐筆讀取資料

如果資料量較大，或想要邊讀邊處理，可以直接使用 `CreateDataReaderAsync`：

```csharp
using CommandExecutor executor = new CommandExecutor();
executor.CommandText = "SELECT Id, Name FROM Customers WHERE Id IN @Ids";
executor.Parameters.Add("Ids", new int[] { 1, 2, 3 });

using DbDataReader reader = await executor.CreateDataReaderAsync(cancellationToken: cancellationToken);
while (await reader.ReadAsync(cancellationToken)) {
  Console.WriteLine($"{reader["Id"]}, {reader["Name"]}");
}
```

## 非同步執行單一值查詢與非查詢命令

```csharp
using CommandExecutor executor = new CommandExecutor();

long count = Convert.ToInt64(await executor
  .SetCommandText("SELECT COUNT(1) FROM Customers")
  .QueryScalarAsync(cancellationToken: cancellationToken));

int affectedRows = await executor
  .SetCommandText("UPDATE Customers SET Name = @Name WHERE Id = @Id")
  .Parameters.Add("Name", "小明")
    .Add("Id", 1)
    .GetCommandExecutor()
  .ExecuteAsync(cancellationToken: cancellationToken);
```

## 與 Transaction 一起使用

Transaction 的開啟方式不變，仍然透過 `BeginTransaction` 進行；只要將實際執行命令的部分改成 Async 版本即可。

```csharp
using CommandExecutor executor = new CommandExecutor(true);
using IDbTransaction transaction = executor.BeginTransaction();

await executor
  .SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
  .Parameters.Add("Id", 1)
    .Add("Name", "小明")
    .GetCommandExecutor()
  .ExecuteAsync(ResetItems.Parameters, cancellationToken);

await executor
  .SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
  .Parameters.Add("Id", 2)
    .Add("Name", "小王")
    .GetCommandExecutor()
  .ExecuteAsync(cancellationToken: cancellationToken);

transaction.Commit();
```

> [!IMPORTANT]
> 若要使用 `BeginTransaction`，請記得將 `KeepConnection` 設為 `true`，否則會拋出 `KeepConnectionRequiredException`。
