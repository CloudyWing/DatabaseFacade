# 建立 DbParameters 的方法

## 基本作法
```csharp
// sql = SELECT * FROM Table WHERE Id = @Id AND Ids IN @Ids

// 正常使用
executor.Parameters.Add("Id", 1);

// 遇到 IN 時可傳入 IEnumerable 型別，實際會變成 @{Prefix}_Ids_{流水號}
executor.Parameters.Add("Ids", new int[] { 1, 2 });
```
## 使用 ParameterMetadata
```csharp
建立一筆
executor.Parameters.Add(new ParameterMetadata {
    ParameterName = "Id",
    Value = 1
});

建立多筆
executor.Parameters.AddRange(param1, param2);
executor.Parameters.AddRange(new ParameterMetadata[] { param1, param2 });
```

## 使用 IDbDataParameter
```csharp
建立一筆
SqlParameter param1 = new SqlParameter("Id", id);
executor.Parameters.Add(param1);

建立多筆
executor.Parameters.AddRange(param1, param2);
executor.Parameters.AddRange(new SqlParameter[] { param1, param2 });
```

## 使用 Dictionary
```csharp
executor.Parameters.AddRange(new Dictionary<string, object> {
    ["Id"] = id,
    ["Name"] = name
}
```

## 使用 object
```csharp
executor.Parameters.AddRange(new {
    Id = id,
    Name = name
}
```

## Method Chaining
```csharp
int result = executor.SetCommandText("INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)")
                .Parameters.Add("Id", 10).Add("Name", "新增測試").GetCommandExecutor()
                .Execute();
```