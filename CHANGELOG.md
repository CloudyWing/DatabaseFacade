# Changelog

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
