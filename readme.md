## 指示
以下の設計書に従い、コンソールアプリケーションを作成してください。
- Design/01-LogicDetective-概要設計.md
- Design/02-LogicDetective-基本設計.md
- Design/03-LogicDetective-詳細設計.md
- Design/04-LogicDetective-コンソール設計.md

以下の設計書は無視してください。まずはコンソールアプリケーションを作成します。
- 05-LogicDetective-画面設計.md

## プロトタイプ
以下に既にプロトタイプを作成済みですが、まだまだ未完成です。
- LogicDetective : プロトタイプコード
- LogicDetective.Tests : テストコード

### 開発ルール
- プロトタイプは必ずしも設計書に従っていません。必ず、設計書を正として下さい。
- プロトタイプは別ブランチに保存済みですので、いくらでも壊して構いません。
- コーディングスタイルはプロトタイプに従ってください。

## 開発コマンド
- dotnet run --project LogicDetective/LogicDetective.csproj
- dotnet run --project LogicDetective/LogicDetective.csproj --diagnose-hint
