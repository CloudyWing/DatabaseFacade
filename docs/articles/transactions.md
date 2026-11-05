# Transaction 使用方法

使用 Transaction 時，請將 `KeepConnection` 設為 true，可從以下方法設定：

- `FacadeConfiguration.DefaultKeepConnection`
- `CommandExecutor` 建構子參數

如果是在 `async` / `await` 流程中使用，Transaction 的建立方式不變，只需要把 `CreateDataTable`、`QueryScalar`、`Execute` 等 API 換成對應的 Async 版本即可。

## 範例

```csharp
using (CommandExecutor executor = new CommandExecutor(true))
using (IDbTransaction tran = executor.BeginTransaction()) {
  executor.CommandText = "INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)";

  executor.Parameters.AddRange(new {
    Id = 1,
    Name = "小明"
  });
  executor.Execute(ResetItems.Parameters);

  executor.Parameters.AddRange(new {
    Id = 2,
    Name = "小王"
  });
  executor.Execute();

  tran.Commit();
}
```

## 使用 CommandExecutor 完成交易

`Commit` 與 `Rollback` 會完成 `CommandExecutor` 目前的交易，接著釋放交易物件，並將 `Transaction` 設為 `null`。沒有進行中的交易時會拋出 `InvalidOperationException`。

```csharp
using CommandExecutor executor = new CommandExecutor(true);
executor.BeginTransaction();

try {
  executor.SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
    .Parameters.AddRange(new { Id = 1, Name = "小明" })
    .GetCommandExecutor()
    .Execute();

  executor.Commit();
} catch {
  executor.Rollback();
  throw;
}
```

## 非同步交易

`BeginTransactionAsync` 回傳 `DbTransaction`，可搭配 `CommitAsync` 與 `RollbackAsync` 使用。

```csharp
using CommandExecutor executor = new CommandExecutor(true);
await executor.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

await executor.SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
  .Parameters.AddRange(new { Id = 1, Name = "小明" })
  .GetCommandExecutor()
  .ExecuteAsync(cancellationToken: cancellationToken);

await executor.CommitAsync(cancellationToken);
```

.NET 10 使用 `DbConnection.BeginTransactionAsync` 與 `DbTransaction.CommitAsync` 等非同步 API。.NET Framework 與 .NET Standard 2.0 的 `DbConnection` 與 `DbTransaction` 沒有非同步交易 API，連線以非同步方式開啟，交易的開始、確認與復原則以同步方式執行。

## 注意事項

- `CommandExecutor` 只記錄最近一次開始的交易。交易尚未完成時再次呼叫 `BeginTransaction`，結果由資料庫 provider 決定，SQL Server 與 SQLite 的 provider 不支援平行或巢狀交易，會拋出 `InvalidOperationException`。
- 交易經由交易物件本身確認或復原後，`Transaction` 屬性在下次讀取時會回傳 `null`。
