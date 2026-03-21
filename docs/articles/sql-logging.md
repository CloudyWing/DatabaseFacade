# 建立 SQL 語法 Log

## SQL Server 範例

```csharp
FacadeConfiguration.OnCommandCreated = cmd => {
  string sql = cmd.CommandText;
  foreach (IDbDataParameter param in cmd.Parameters) {
    string value;
    switch (param.Value) {
      case DateTime dateTime:
        value = $"'{dateTime:yyyy/MM/dd HH:mm:ss}'";
        break;
      case long:
      case int:
      case short:
      case byte:
      case ulong:
      case uint:
      case ushort:
      case sbyte:
      case float:
      case double:
      case decimal:
        value = param.ToString();
        break;
      case DBNull:
      case null:
        value = "NULL";
        break;
      default:
        value = $"'{param.Value?.ToString()}'";
        break;
    }

    string pattern = string.Format("{0}(?=[\\W])|{0}$", "@" + param.ParameterName);
    sql = Regex.Replace(sql, pattern, value, RegexOptions.IgnoreCase);
  }

  Console.WriteLine(sql);
};
```

## Oracle 範例

```csharp
FacadeConfiguration.OnCommandCreated = cmd => {
  string sql = cmd.CommandText;
  foreach (IDbDataParameter param in cmd.Parameters) {
    string value;
    if (param.Value is null || param.Value is DBNull) {
      value = "NULL";
    } else if (param.Value is DateTime dateTime) {
      value = string.Format(
        "TO_DATE('{0}', '{1}')",
        dateTime.ToString("yyyy/MM/dd HH:mm:ss"),
        "yyyy/mm/dd hh24:mi:ss"
      );
    } else if (param.Value is long || param.Value is int
          || param.Value is short || param.Value is byte
          || param.Value is ulong || param.Value is uint
          || param.Value is ushort || param.Value is sbyte
          || param.Value is float || param.Value is double || param.Value is decimal) {
      value = param.Value.ToString();
    } else {
      value = "'" + param.Value.ToString() + "'";
    }

    string pattern = string.Format("{0}(\\W])|{0}$", ":" + param.ParameterName);
    sql = Regex.Replace(sql, pattern, value, RegexOptions.IgnoreCase);
  }

  Console.WriteLine(sql);
};
```

## 注意事項

1. 上述將 DbCommand 轉換成可執行的 SQL 語法作法僅供參考，有一些特別型別可能無法處理，再自行調整。
2. `Console.WriteLine(sql);` 請自行替換成專案實際寫入 Log 的語法。
