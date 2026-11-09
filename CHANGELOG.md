# Changelog

## v0.3.0 (2026-11-09)

### Breaking Changes

- 參數值為 `null` 時，改以 `DBNull.Value` 傳給 provider。原本 provider 會將 `null` 視為未提供參數值，現在會寫入資料庫 `NULL`。
- `byte[]` 與 `char[]` 參數改為與 `string` 相同，視為單一參數值交給 provider，不再展開成 `IN` 清單。
- `CommandType` 為 `StoredProcedure` 或 `TableDirect` 時，改為將所有參數直接加入指令，集合值不展開，原樣交給 provider（可用於資料表值參數）。原本在這兩種 `CommandType` 下參數會被略過。
- 指令執行完成後，`Output`、`InputOutput` 與 `ReturnValue` 參數的值會寫回 `ParameterMetadata.Value`，再依 `ResetItems` 清空參數。`CreateDataReader`、`CreateDataReaderAsync` 與 .NET 10 的 `QueryAsync<T>` 則在讀取器關閉後，於呼叫 `ParameterCollection.GetValue<T>`、建立下一個指令或釋放 executor 時寫回。
- `CreateDataReader` 與 `CreateDataReaderAsync` 的連線與 `DbCommand` 歸屬調整如下。
  - `KeepConnection` 為 `false` 時，連線交由讀取器的 `CommandBehavior.CloseConnection` 管理，`Connection` 變為 `null`。
  - 讀取器尚未關閉時，後續指令改用新連線，釋放 executor 也不會關閉讀取器的連線。
  - 讀取器對應的 `DbCommand` 會在讀取器關閉後，於下一次建立指令或釋放 executor 時釋放。
- `CreateDataTableAsync` 改為先將資料列暫存於記憶體，再以 `DataTable.Load` 建立 `DataTable`。欄位命名、`AllowDBNull`、`Unique` 與主鍵規則，以及載入後的資料列狀態（`Unchanged`），皆與 `CreateDataTable` 一致。
- 移除 `CommandExecutor` 的 finalizer。`CommandExecutor` 不持有 unmanaged 資源，未呼叫 `Dispose` 的實例不再進入 finalization 佇列，請以 `using` 或 .NET 10 的 `await using` 明確釋放連線與其他資源。

### New Features

- 新增 `CreateDataSet` 與 `CreateDataSetAsync`，將每個結果集載入為一個 `DataTable`，命名規則與 `DbDataAdapter` 相同。
- 新增輕量對應 API。
  - `Query<T>`：立即執行，以對應函式轉換第一個結果集的所有資料列。對應函式為 `null` 時拋出 `ArgumentNullException`。
  - `QueryAsync<T>`（.NET 10）：以 `IAsyncEnumerable<T>` 延遲執行並逐筆讀取。
  - `QueryScalar<T>` 與 `QueryScalarAsync<T>`：處理 `DBNull` 並轉換為指定型別。
- 新增交易便利方法 `BeginTransactionAsync`、`Commit`、`Rollback`、`CommitAsync` 與 `RollbackAsync`。完成交易後會釋放並清除 `Transaction`，沒有進行中的交易時拋出 `InvalidOperationException`。
- 新增 `ParameterCollection.GetValue<T>`，取得轉型後的參數值，`DBNull` 時回傳預設值。參數名稱為 `null` 時拋出 `ArgumentNullException`，參數不存在時拋出 `KeyNotFoundException`，無法轉型時拋出 `InvalidCastException`。
- 新增 `CommandExecutor` 個別實例的 `CommandCreating` 與 `CommandCreated` 回呼，執行順序為先呼叫 `FacadeConfiguration` 的全域回呼，再呼叫實例回呼。
- `CommandExecutor` 在 .NET 10 新增 `IAsyncDisposable` 實作，以非同步方式釋放交易、讀取器指令與連線。
- 新增 `KeepConnectionRequiredException(string, Exception)` 建構子，可接受任意 `Exception` 作為內部例外。

### Bug Fixes

- 修正開啟連線或建立指令失敗時未釋放資源的問題。開啟連線失敗或未設定連線字串時，會釋放已建立的連線並將 `Connection` 設為 `null`。建立 `DbCommand` 後設定參數或執行 `CommandCreated` 回呼失敗時，會釋放該 `DbCommand`。
- 修正 `AddRange` 傳入含 indexer 的物件時拋出例外，改為只取可讀取的公開執行個體屬性。
- 修正參數名稱前綴含 `$` 時，展開 `IN` 清單會產生錯誤 SQL。

### Performance

- 依參數名稱快取比對 `CommandText` 用的 Regex，快取上限為 1,024 筆，避免每次執行都重新建立。Regex 同時改用 `CultureInvariant`，比對結果不受建立時的執行緒文化特性影響。
- `AddRange` 從物件建立參數時，依型別快取可讀取的屬性清單。

### Documentation

- 新增 NuGet 套件專用 README，並改以絕對連結指向儲存庫文件。
- 補充新增 API 的使用說明與參數處理規則。
- 修正非同步範例的取消權杖傳遞，補齊參數回寫與資源釋放的說明。
- 修正 SQL Log 範例的數值參數取值與 Oracle 參數比對 Regex。
- 補充執行緒安全、參數名稱大小寫與交易狀態的 XML 文件說明。

### Build and CI

- 新增私用建置套件 `Microsoft.CodeAnalysis.PublicApiAnalyzers`，以 v0.2.0 的公開 API 建立 `PublicAPI.Shipped.txt` 基準，追蹤後續公開 API 變更。此套件不是消費端的執行階段相依。
- 啟用 `snupkg` 符號套件、Source Link 相關設定，以及以 0.2.0 為基準的 PackageValidation。
- 移除 `GeneratePackageOnBuild`，套件改由發佈流程的 `dotnet pack` 產生。
- `LangVersion` 明訂為 14，自原始碼建置需使用支援 C# 14 的 SDK。
- CI 改用 devops-templates 的 reusable workflow，於 ubuntu 與 windows 以 .NET 8 與 10 SDK 執行建置、測試與打包。
- 發佈改為 push 版本標籤時以 Trusted Publishing 發佈 NuGet，發佈前檢查 nupkg 版號與標籤一致。移除 `nuget-publish.yml` 與 API key 發佈流程。
- 文件網站改以 GitHub Pages artifact 部署，GitHub Pages 的 Source 需設為 GitHub Actions。
- 新增 Dependabot 設定。
- 測試專案升級 `SQLitePCLRaw.lib.e_sqlite3` 至 2.1.14 以排除已知弱點，僅影響測試專案。

## v0.2.0 (2026-03-16)

### New Features

- 新增非同步資料庫操作 API，並保留既有同步 API：
  - `CreateDataReaderAsync`
  - `CreateDataTableAsync`
  - `QueryScalarAsync`
  - `ExecuteAsync`
- 非同步 API 全面支援 `CancellationToken`，可直接整合既有 `async` / `await` 流程。
- 目標框架更新為 `netstandard2.0`、`net45`、`net10.0`；啟用 Nullable Reference Types。
- 版本管理改採 MinVer；文件系統改為 DocFX。
- 補強公開 API 的英文 XML 註解；更新 README 與 DocFX 文件，新增〈非同步操作〉說明。

### Bug Fixes

- 改善 `CommandExecutor` 內部連線與命令建立流程，統一同步與非同步執行路徑。
- 修正參數名稱比對邏輯，對動態產生的參數名稱採用較安全的規則處理。
- 強化資源釋放流程，釋放 `CommandExecutor` 時會一併整理交易與連線資源。
