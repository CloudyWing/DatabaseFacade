# Transaction 使用方法
使用 Transaction 時，請將 KeepConnection 設為 true，可從以下方法設定
* FacadeConfiguration.DefaultKeepConnection
* CommandExecutor 建構子參數

範例
```csharp
using (CommandExecutor executor = new CommandExecutor(true))
using (IDbTransaction tran = executor.BeginTransact()) {
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