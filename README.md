# Ateliers AI MCP Server

C#/.NETで実装したModel Context Protocol（MCP）サーバー。
複数のIDEとAIクライアントから**GitHub/ローカルファイル統合操作**と**技術記事参照**を提供。

---

## ⚠️ ベータ版について

**本プロジェクトは開発中のベータ版です。**

- ✅ 基本的な機能は実装済み
- ❌ 動作の安定性は保証できません
- ⚠️ 個人プロジェクトのため、サポートは限定的です
- 🔧 予告なく仕様変更や破壊的変更が発生する可能性があります

**使用は自己責任でお願いします。**  
本番環境や重要なデータでの使用は推奨しません。

不具合報告や改善提案は[Issues](https://github.com/yuu-git/ateliers-ai-mcpserver/issues)へお願いします。

---

## 概要

ateliers.devの技術資産（コーディングガイドライン、技術記事、開発リポジトリ）を複数のIDEから直接参照・編集できるMCPサーバー。

### 主な特徴

- ✅ **マルチクライアント対応** - Claude Desktop、VS Code、Visual Studioから同じツールを使用可能
- ✅ **ローカル優先アクセス** - LocalPath設定時は高速なローカルファイル操作、未設定時はGitHub API経由
- ✅ **Git統合** - AutoPull/AutoPush対応、ファイル変更の自動コミット＆プッシュ
- ✅ **汎用ファイル操作** - 読み取り/書き込み/削除/リネーム/コピー/バックアップの完全CRUD
- ✅ **記事専門ツール** - ateliers.dev技術記事の検索・一覧・読み取り（Frontmatter自動除去）
- ✅ **複数リポジトリ対応** - 設定ファイルで柔軟なリポジトリ管理

### 対応クライアント

| クライアント | バージョン | ステータス |
|:--|:--|:--|
| Claude Desktop | 最新版 | ✅ 完全サポート |
| VS Code | 1.102+ | ✅ Agent Mode対応 |
| Visual Studio | 2022 17.14+ / 2026 Preview | ✅ Agent Mode対応 |

## バージョン履歴

### v0.6.0（2024-11-29）
- **Phase 6完了**: マルチクライアント統合
- VS Code統合（Agent Mode、相対パス対応）
- Visual Studio統合（Agent Mode、セキュリティ強化）
- Claude Desktop統合確認
- 各クライアント用セットアップガイド作成
- `.vscode/mcp.json.sample` と `.mcp.json.sample` の提供

### v0.5.0（2024-11-28）
- **Phase 5完了**: Git操作統合
- LibGit2Sharp導入
- GitOperationService実装（Pull, Commit, Push, CommitAndPush）
- AutoPull/AutoPush機能実装
- 認証情報階層化（リポジトリ固有 → グローバル）
- コンフリクト検出とエラーハンドリング
- 6つの書き込み系ツールにGit統合

### v0.4.0（2024-11-26）
- **Phase 4完了**: ローカルファイル優先ロジック実装
- LocalFileService新設（完全CRUD + Rename + Copy + Backup）
- RepositoryTools実装（8つの汎用ファイル操作ツール）
- AteliersDevTools改善（記事検索・一覧・読み取り）
- 不要ツール削除とツール説明全面改善（MCPツール選択ガイド準拠）
- appsettings.json設計（Dictionary形式でリポジトリ管理）

### v0.3.0（以前）
- Training版ベース（ateliers-training-mcpserver-claude）
- GitHub読み取り機能
- 基本的なキャッシング

## 対応リポジトリ

以下のリポジトリに対応（appsettings.jsonで設定）：

| リポジトリキー | 説明 | 用途 |
|:--|:--|:--|
| AteliersAiAssistants | コーディングガイドライン | AI向けコーディング規約・サンプル |
| AteliersAiMcpServer | 本MCPサーバー | ソースコード管理 |
| AteliersDev | 技術ブログ | Docusaurus記事・ブログ投稿 |
| PublicNotes | パブリックメモ | TODO・アイデア・スニペット |
| TrainingMcpServer | Training版MCPサーバー | 学習用コードベース |

## 機能一覧

### GitTools（Git操作）

| ツール | 機能 |
|:--|:--|
| `commit_repository` | リポジトリの変更をコミット |
| `push_repository` | コミットをリモートにプッシュ |
| `pull_repository` | リモートから最新を取得 |
| `commit_and_push_repository` | コミット＆プッシュを一括実行 |
| `create_tag` | Gitタグを作成 |
| `push_tag` | タグをリモートにプッシュ |
| `create_and_push_tag` | タグ作成＆プッシュを一括実行 |

### RepositoryTools（汎用ファイル操作 + Git統合）

| ツール | 機能 | Git統合 |
|:--|:--|:--|
| `read_repository_file` | ファイル読み取り | - |
| `list_repository_files` | ファイル一覧取得 | - |
| `add_repository_file` | ファイル新規作成 | AutoPull/AutoPush |
| `edit_repository_file` | ファイル更新（自動バックアップ） | AutoPull/AutoPush |
| `delete_repository_file` | ファイル削除（自動バックアップ） | AutoPull/AutoPush |
| `rename_repository_file` | ファイルリネーム | AutoPull/AutoPush |
| `copy_repository_file` | ファイルコピー | AutoPull/AutoPush |
| `backup_repository_file` | バックアップ作成 | - |

### AteliersDevTools（記事専門）

| ツール | 機能 |
|:--|:--|
| `read_article` | 記事読み取り（Frontmatter自動除去） |
| `list_articles` | 記事一覧取得（.md + .mdx） |
| `search_articles` | キーワード検索（ファイル名・内容） |

## 前提条件

- .NET 8.0 SDK以降
- Git（AutoPull/AutoPush使用時）
- GitHub Personal Access Token（オプション：GitHub API/Git Push使用時）
- 以下のいずれかのクライアント：
  - Claude Desktop（最新版）
  - VS Code（1.102以降）+ GitHub Copilot拡張機能
  - Visual Studio 2022（17.14以降）または 2026 Preview + GitHub Copilot

## クイックスタート

### 1. リポジトリのクローン

```bash
git clone https://github.com/yuu-git/ateliers-ai-mcpserver.git
cd ateliers-ai-mcpserver
```

### 2. 設定ファイルの作成（任意）

基本的な動作にはローカル設定は不要ですが、ローカルファイルアクセスやGit統合を使用する場合は `appsettings.local.json` を作成してください。

詳細は各クライアントのセットアップガイドを参照してください。

### 3. クライアント別セットアップ

使用するクライアントに応じて、以下のガイドを参照してください：

#### Claude Desktop
- 📖 [Claude Desktopセットアップガイド](Docs/setup/claude-desktop.md)
- フルパス設定が必要
- dotnet run方式またはビルド済みexe方式

#### VS Code
- 📖 [VS Codeセットアップガイド](Docs/setup/vscode.md)
- `.vscode/mcp.json.sample` をコピーして使用
- 相対パス対応
- Agent Mode必須

#### Visual Studio
- 📖 [Visual Studioセットアップガイド](Docs/setup/visual-studio.md)
- `.mcp.json.sample` をコピーして使用
- 相対パス対応
- Agent Mode推奨

## 使い方

### ファイル読み取り

```
Services/GitHubService.cs を読んで
```

AIが自動的に `read_repository_file` ツールを使用します。

### ファイル編集（Git統合）

```
README.mdのバージョン履歴を更新して
```

AutoPush=true の場合、AIが以下を実行：
1. AutoPull確認→リモートの最新を取得
2. `read_repository_file` でREADME.mdを読み取り
3. 内容を更新
4. `edit_repository_file` で保存（自動バックアップ作成）
5. Git commit & push（自動）

### 記事検索

```
GitHub Actionsに関する記事を探して
```

AIが `search_articles` で記事を検索し、関連記事を提示します。

### マルチクライアント活用例

```
1. Claude Desktopで設計・仕様検討
2. VS Codeで実装
3. Visual StudioでデバッグとCodeLens活用
   ↓
すべてのクライアントから同じMCPツールを使用
```

## 高度な設定

### ローカルファイルアクセス（LocalPath）

高速なローカルファイルアクセスを有効にするには、`appsettings.local.json` を作成：

```json
{
  "Repositories": {
    "PublicNotes": {
      "LocalPath": "[YOUR_CLONE_PATH]\\ateliers-public-notes",
      "AutoPull": true,
      "AutoPush": true
    }
  }
}
```

**メリット:**
- 10-5000倍高速なファイルアクセス
- GitHub APIレート制限の回避
- リアルタイムな編集フィードバック

### Git統合（AutoPull/AutoPush）

認証情報を設定してGit操作を自動化：

```json
{
  "GitHub": {
    "Token": "github_pat_11AAAAAA...",
    "Email": "your-email@example.com",
    "Username": "your-github-username"
  },
  "Repositories": {
    "PublicNotes": {
      "AutoPull": true,
      "AutoPush": true
    }
  }
}
```

## トラブルシューティング

### 共通の問題

- 📖 [トラブルシューティングガイド](Docs/troubleshooting.md)

### クライアント別の問題

各クライアントのセットアップガイド内にトラブルシューティングセクションがあります：

- [Claude Desktop トラブルシューティング](Docs/setup/claude-desktop.md#トラブルシューティング)
- [VS Code トラブルシューティング](Docs/setup/vscode.md#トラブルシューティング)
- [Visual Studio トラブルシューティング](Docs/setup/visual-studio.md#トラブルシューティング)

## 開発

### プロジェクト構造

```
Ateliers.Ai.McpServer/
├─ Configuration/
│  └─ AppSettings.cs          # 設定クラス
├─ Services/
│  ├─ GitHubService.cs        # GitHub API操作
│  ├─ LocalFileService.cs     # ローカルファイル操作
│  └─ GitOperationService.cs  # Git操作（Pull/Commit/Push）
├─ Tools/
│  ├─ GitTools.cs             # Git操作ツール（Phase 5）
│  ├─ RepositoryTools.cs      # 汎用ファイル操作ツール（Git統合）
│  └─ AteliersDevTools.cs     # 記事専門ツール
├─ Program.cs                 # エントリーポイント
├─ appsettings.json           # 基本設定
└─ appsettings.local.json     # ローカル設定（Git管理外）
```

### ビルド（開発モード）

```bash
dotnet build
dotnet run --project Ateliers.Ai.McpServer
```

### テスト

```bash
dotnet test
```

## ドキュメント

### セットアップガイド

- [Claude Desktop](Docs/setup/claude-desktop.md)
- [VS Code](Docs/setup/vscode.md)
- [Visual Studio](Docs/setup/visual-studio.md)

### Phase計画

- [Phase 5: Git統合](Docs/phases/phase5-handover.md)
- [Phase 6: マルチクライアント統合](Docs/phases/phase6-plan.md)
- [Phase 7: Notion基礎統合](Docs/phases/phase7-plan.md)（計画中）
- [Phase 8: Notion拡張](Docs/phases/phase8-plan.md)（計画中）
- [Phase 9: Docusaurus統合](Docs/phases/phase9-plan.md)（計画中）

## ライセンス

MIT License

## 関連リンク

- [ateliers.dev](https://ateliers.dev) - 技術ブログ
- [ateliers-ai-assistants](https://github.com/yuu-git/ateliers-ai-assistants) - AIコーディングガイドライン
- [Model Context Protocol](https://modelcontextprotocol.io/) - MCP公式サイト

## 今後の予定

### Phase 7: Notion基礎統合（次）
- Notion API接続基盤
- Tasks管理（CRUD操作）
- Ideas管理（CRUD操作）
- 「思考のバッファ」としてのNotion活用

### Phase 8: Notion拡張
- Bookmarks管理（あとで読む）
- 検索機能強化
- タグ・カテゴリ管理

### Phase 9: Docusaurus統合
- 記事作成ツール（create_blog_post, create_doc_article）
- Notion→Docusaurus変換フロー
- Frontmatter自動生成
- 会話→記事変換機能
- **v1.0.0目標**: 完全なナレッジ管理システム

### Phase 10以降
- SQLServer/SQLite統合
- 役割別MCPサーバー分割（coding, docs, productivity）
- VoicePeak CLI統合
- Docker化（配布オプション）
