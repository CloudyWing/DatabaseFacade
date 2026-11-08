# 執行資料庫作業 API 後，初始化 Command 資訊

## 資料庫作業 API

- `IDataReader CreateDataReader(ResetItems thenReset = ResetItems.All, CommandBehavior behavior = CommandBehavior.SequentialAccess)`
- `DataTable CreateDataTable(ResetItems thenReset = ResetItems.All)`
- `DataSet CreateDataSet(ResetItems thenReset = ResetItems.All)`
- `object QueryScalar(ResetItems thenReset = ResetItems.All)`
- `T? QueryScalar<T>(ResetItems thenReset = ResetItems.All)`
- `IReadOnlyList<T> Query<T>(Func<IDataRecord, T> map, ResetItems thenReset = ResetItems.All)`
- `int Execute(ResetItems thenReset = ResetItems.All)`

各 API 的 Async 版本也有相同的 `thenReset` 參數。

## ResetItems

Flags Enum，每個資料庫異動 API 皆有這個參數，當資料庫異動完畢後，會依照這個參數，決定初始化的 Command Info。執行失敗時不會初始化。一般 API 會在執行完成後、初始化之前，把 Output 參數寫回 `ParameterMetadata.Value`；`CreateDataReader`、`CreateDataReaderAsync` 與 `QueryAsync<T>` 則是先初始化，待讀取器關閉後才寫回，詳見[建立 DbParameters 的方法](creating-parameters.md)的 Output 參數說明。總共如下：

| 名稱 | 初始值 | 說明 |
| --- | --- | --- |
| None |  | 不異動 |
| CommandText | Null |  |
| CommandTimeout | DefaultCommandTimeout |  |
| CommandType | CommandType.Text |  |
| Parameters | Clear() |  |
| All |  | 全部初始化 |

## 用途

在某些情況下，執行完資料庫作業後，會需要保留原資料，例如有時要新增多筆資料時，CommandText 是相同的，只有 Parameters 需要給予新值。

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
