# 執行資料庫作業 API 後，初始化 Command 資訊

## 資料庫作業 API：
* IDataReader CreateDataReader(ResetItems thenReset = ResetItems.All, CommandBehavior behavior = CommandBehavior.SequentialAccess)
* DataTable CreateDataTable(ResetItems thenReset = ResetItems.All)
* object QueryScalar(ResetItems thenReset = ResetItems.All)
* int Execute(ResetItems thenReset = ResetItems.All)

## ResetItems
Flags Enum，每個資料庫異動 API 皆有這個參數，當資料庫異動完畢後，會依照這個參數，決定初始化的 Command Info，總共如下：
| 名稱 | 初始值 | 說明 |
| -------- | -------- | -------- |
| None |  | 不異動
| CommandText | Null | |
| CommandTimeout | DefaultCommandTimeout | |
| CommandType | CommandType.Text | |
| Parameters | Clear() | |
| TextAndParameters | | CommandText和Parameters初始化 |
| All | | 全部初始化 |

## 用途
在某些情況下，執行完資料庫作業後，會需要保留原資料，例如有時要新增多筆資料要時，CommandText 是相同的，只有 Parameters 需要給予新值。
```csharp
executor.CommandText = "INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)";
executor.Parameters.AddRange(new {
    Id = 1,
    Name = "小明",
});
// 只初始化 Parameters，CommandText仍保留
executor.Execute(ResetItems.Parameters);
executor.Parameters.AddRange(new {
    Id = 2,
    Name = "小王",
});
executor.Execute(ResetItems.Parameters);
```
