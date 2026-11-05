# 建立 DbParameters 的方法

## 基本作法

```csharp
// sql = SELECT * FROM Table WHERE Id = @Id AND Ids IN @Ids

// 正常使用
executor.Parameters.Add("Id", 1);

// 遇到 IN 時可傳入 IEnumerable 型別，實際會變成 @{Prefix}_Ids_{流水號}
executor.Parameters.Add("Ids", new int[] { 1, 2 });
```

### 參數值的處理規則

- 值為 `null` 時會以 `DBNull.Value` 傳給資料庫。
- `string`、`byte[]` 與 `char[]` 視為單一值，不會展開成 IN 清單。
- 其他實作 `IEnumerable` 的值會展開成 `(@{Prefix}_Ids_0, @{Prefix}_Ids_1, ...)`。空集合會取代成 `(NULL)`，此時 `IN @Ids` 與 `NOT IN @Ids` 都不會有任何資料符合，需要「排除空清單」的查詢請在呼叫前自行判斷。
- `CommandType` 為 `Text` 時，只有出現在 `CommandText` 中的參數才會加入 DbCommand，比對參數名稱時不分大小寫。
- 參數名稱在 `Parameters` 中以大小寫區分，`Id` 與 `id` 可以同時加入並都會傳給資料庫。SQLite 這類名稱區分大小寫的資料庫會分別繫結；SQL Server 這類名稱不分大小寫的資料庫會因參數重複宣告而失敗，請對同一個參數使用一致的大小寫。

## 使用 ParameterMetadata

```csharp
// 建立一筆
executor.Parameters.Add(new ParameterMetadata {
  ParameterName = "Id",
  Value = 1
});

// 建立多筆
executor.Parameters.AddRange(param1, param2);
executor.Parameters.AddRange(new ParameterMetadata[] { param1, param2 });
```

## 使用 IDbDataParameter

```csharp
// 建立一筆
SqlParameter param1 = new SqlParameter("Id", id);
executor.Parameters.Add(param1);

// 建立多筆
executor.Parameters.AddRange(param1, param2);
executor.Parameters.AddRange(new SqlParameter[] { param1, param2 });
```

## 使用 Dictionary

```csharp
executor.Parameters.AddRange(new Dictionary<string, object> {
  ["Id"] = id,
  ["Name"] = name
});
```

## 使用 object

```csharp
executor.Parameters.AddRange(new {
  Id = id,
  Name = name
});
```

## Method Chaining

```csharp
int result = executor.SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
  .Parameters.Add("Id", 10).Add("Name", "新增測試").GetCommandExecutor()
  .Execute();
```

## 預存程序

`CommandType` 為 `StoredProcedure` 或 `TableDirect` 時，所有參數都會直接加入 DbCommand，集合值也不會展開，會原樣交給資料庫 provider，可用於資料表值參數等用法。

```csharp
executor.SetCommandText("dbo.GetCustomers", CommandType.StoredProcedure)
  .Parameters.Add("Name", "小明");

DataTable dt = executor.CreateDataTable();
```

## Output 參數

`Direction` 為 `Output`、`InputOutput` 或 `ReturnValue` 的參數，會在指令執行完成後把資料庫回傳的值寫回 `ParameterMetadata.Value`，再依 `ResetItems` 初始化，可用 `Parameters.GetValue<T>` 取得轉型後的值，值為 `DBNull` 時回傳預設值。

```csharp
executor.SetCommandText("dbo.CreateCustomer", CommandType.StoredProcedure)
  .Parameters.Add("Name", "小明")
    .Add("Id", null, DbType.Int32, ParameterDirection.Output);

// 預設的 ResetItems.All 會清空 Parameters，要讀取 Output 參數時需保留 Parameters
executor.Execute(ResetItems.All & ~ResetItems.Parameters);
int id = executor.Parameters.GetValue<int>("Id");
```

也可以保留 `ParameterMetadata` 的參考，執行後直接讀取 `Value`：

```csharp
ParameterMetadata total = new ParameterMetadata {
  ParameterName = "Total",
  DbType = DbType.Int32,
  Direction = ParameterDirection.Output
};

executor.SetCommandText("dbo.CountCustomers", CommandType.StoredProcedure)
  .Parameters.Add(total)
  .GetCommandExecutor()
  .Execute();

int count = (int)total.Value!;
```

使用 `CreateDataReader` 時，資料庫要等讀取器關閉後才會回傳 Output 參數，值會在讀取器關閉後，於呼叫 `GetValue`、建立下一個指令或釋放 `CommandExecutor` 時寫回。
