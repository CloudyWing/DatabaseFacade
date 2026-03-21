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
