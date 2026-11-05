# 查詢結果的取得方式

`CommandExecutor` 提供多種取得查詢結果的 API，依資料量與使用方式選擇。

| 方法 | 回傳型別 | 執行時機 | 適用情境 |
| --- | --- | --- | --- |
| `CreateDataReader` / `CreateDataReaderAsync` | `IDataReader` / `DbDataReader` | 立即執行，讀取器由呼叫端釋放 | 大量資料逐筆處理 |
| `CreateDataTable` / `CreateDataTableAsync` | `DataTable` | 立即執行並載入第一個結果集 | 需要 `DataTable` 的既有程式 |
| `CreateDataSet` / `CreateDataSetAsync` | `DataSet` | 立即執行並載入所有結果集 | 一次查詢多個結果集 |
| `Query<T>` | `IReadOnlyList<T>` | 立即執行並轉換所有資料列 | 轉換成自訂型別 |
| `QueryAsync<T>`（僅 .NET 10） | `IAsyncEnumerable<T>` | 延遲執行，列舉時逐筆讀取 | 非同步串流處理 |
| `QueryScalar<T>` / `QueryScalarAsync<T>` | `T?` | 立即執行 | 取得單一值並轉型 |

## 轉換成自訂型別

`Query<T>` 會立即執行指令，把第一個結果集的每一列交給對應函式轉換，回傳前就已關閉讀取器並依 `KeepConnection` 釋放連線。讀取器使用 `CommandBehavior.Default`，對應函式可以用任意順序讀取欄位。

```csharp
using CommandExecutor executor = new CommandExecutor();

IReadOnlyList<Customer> customers = executor
  .SetCommandText("SELECT Id, Name FROM Customers WHERE Id IN @Ids")
  .Parameters.Add("Ids", new int[] { 1, 2, 3 })
    .GetCommandExecutor()
  .Query(record => new Customer {
    Id = record.GetInt32(0),
    Name = record.GetString(1)
  });
```

.NET 10 可使用 `QueryAsync<T>` 以 `await foreach` 逐筆讀取。`QueryAsync<T>` 為延遲執行，開始列舉時才依當下的 `CommandText` 與 `Parameters` 建立指令，執行後立即套用 `ResetItems`，列舉結束時關閉讀取器。

```csharp
await foreach (Customer customer in executor
  .SetCommandText("SELECT Id, Name FROM Customers")
  .QueryAsync(record => new Customer {
    Id = record.GetInt32(0),
    Name = record.GetString(1)
  }, cancellationToken: cancellationToken)) {
  Console.WriteLine(customer.Name);
}
```

> [!NOTE]
> `KeepConnection` 為 `true` 時，列舉期間請不要在同一個 `CommandExecutor` 執行其他指令，除非資料庫 provider 支援 Multiple Active Result Sets。

## 取得單一值並轉型

`QueryScalar<T>` 會把 `QueryScalar` 的結果轉成指定型別。查無資料或值為 `DBNull` 時回傳 `T` 的預設值，數值轉換使用 `CultureInfo.InvariantCulture`，列舉型別接受數值或名稱，`Guid` 接受字串或 16 位元組的陣列。

```csharp
int count = executor
  .SetCommandText("SELECT COUNT(1) FROM Customers")
  .QueryScalar<int>();

DateTime? lastLogin = await executor
  .SetCommandText("SELECT MAX(LoginAt) FROM LoginLogs WHERE CustomerId = @Id")
  .Parameters.Add("Id", 1)
    .GetCommandExecutor()
  .QueryScalarAsync<DateTime?>(cancellationToken: cancellationToken);
```

## 載入多個結果集

`CreateDataSet` 會把每個結果集載入成一個 `DataTable`，命名規則與 `DbDataAdapter` 相同，依序為 `Table`、`Table1`、`Table2`。沒有欄位的結果集會略過。

```csharp
DataSet dataSet = executor
  .SetCommandText("SELECT * FROM Customers; SELECT * FROM Orders")
  .CreateDataSet();

DataTable customers = dataSet.Tables["Table"];
DataTable orders = dataSet.Tables["Table1"];
```

`CreateDataTableAsync` 與 `CreateDataSetAsync` 以非同步方式讀取資料列後，再交給 `DataTable.Load` 建立 `DataTable`，欄位名稱、`AllowDBNull`、主鍵與資料列狀態都與同步版本相同。資料列會先暫存在記憶體，大量資料請改用 `CreateDataReaderAsync` 或 `QueryAsync<T>`。

## DataReader 的資源生命週期

`CreateDataReader` 回傳的讀取器由呼叫端負責釋放。底層的 `DbCommand` 會在讀取器關閉後，於下一次建立指令或釋放 `CommandExecutor` 時釋放。

- `KeepConnection` 為 `false` 時，讀取器加上 `CommandBehavior.CloseConnection`，連線改由讀取器負責關閉，`Connection` 變成 `null`。讀取器尚未關閉時，後續指令會使用新的連線，釋放 `CommandExecutor` 也不會關閉讀取器。
- `KeepConnection` 為 `true` 時，讀取器使用 `CommandExecutor` 持有的連線，釋放 `CommandExecutor` 會一併關閉讀取器。

```csharp
public IDataReader OpenCustomerReader() {
  // KeepConnection 為 false，讀取器在 executor 釋放後仍可讀取，關閉讀取器時一併關閉連線
  using CommandExecutor executor = new CommandExecutor(false);
  return executor.SetCommandText("SELECT Id, Name FROM Customers").CreateDataReader();
}
```
