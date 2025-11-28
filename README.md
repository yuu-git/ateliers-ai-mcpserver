# Ateliers AI MCP Server

C#/.NETで実装したModel Context Protocol（MCP）サーバー。
Claude Desktop向けに**GitHub/ローカルファイル統合操作**と**技術記事参照**を提供。

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

ateliers.devの技術資産（コーディングガイドライン、技術記事、開発リポジトリ）をClaude Desktopから直接参照・編集できるMCPサーバー。

### 主な特徴

- ✅ **ローカル優先アクセス** - LocalPath設定時は高速なローカルファイル操作、未設定時はGitHub API経由
- ✅ **Git統合** - AutoPull/AutoPush対応、ファイル変更の自動コミット＆プッシュ
- ✅ **汎用ファイル操作** - 読み取り/書き込み/削除/リネーム/コピー/バックアップの完全CRUD
- ✅ **記事専門ツール** - ateliers.dev技術記事の検索・一覧・読み取り（Frontmatter自動除去）
- ✅ **複数リポジトリ対応** - 設定ファイルで柔軟なリポジトリ管理

## バージョン履歴

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

- .NET 10.0 SDK
- Claude Desktop
- Git（AutoPull/AutoPush使用時）
- GitHub Personal Access Token（オプション：GitHub API/Git Push使用時）

## セットアップ

### 1. リポジトリのクローン

```bash
git clone https://github.com/yuu-git/ateliers-ai-mcpserver.git
cd ateliers-ai-mcpserver
```

### 2. 設定ファイルの作成

#### 2-1. appsettings.local.json作成

テンプレートをコピー：

```bash
# Linux/macOS
cp Ateliers.Ai.McpServer/appsettings.local.json.sample Ateliers.Ai.McpServer/appsettings.local.json

# Windows (PowerShell)
Copy-Item Ateliers.Ai.McpServer/appsettings.local.json.sample Ateliers.Ai.McpServer/appsettings.local.json
```

#### 2-2. LocalPath設定（推奨）

ローカルファイルシステムから高速アクセスしたい場合は、LocalPathを設定：

```json
{
  "Repositories": {
    "PublicNotes": {
      "LocalPath": "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-public-notes",
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
- Git統合による自動コミット＆プッシュ

#### 2-3. Git設定（AutoPull/AutoPush使用時）

Git統合を使用する場合、認証情報を設定：

**グローバル設定（推奨）:**

```json
{
  "GitHub": {
    "Token": "github_pat_11AAAAAA...",
    "Email": "your-email@example.com",
    "Username": "your-github-username"
  }
}
```

**リポジトリ固有設定（オプション）:**

```json
{
  "Repositories": {
    "PublicNotes": {
      "GitHubToken": "github_pat_notes_specific_token",
      "GitEmail": "notes@example.com",
      "GitUsername": "your-username",
      "AutoPull": true,
      "AutoPush": true
    }
  }
}
```

**認証情報の優先順位:**
1. リポジトリ固有のToken/Email/Username
2. グローバルのToken/Email/Username

**注意**: `appsettings.local.json` は `.gitignore` で除外されており、Gitにコミットされません。

### 3. ビルド

```bash
dotnet restore
dotnet build --configuration Release
```

### 4. Claude Desktop設定

Claude Desktopの設定ファイル（`claude_desktop_config.json`）にMCPサーバーを追加：

**設定ファイルの場所:**
- Windows: `%APPDATA%\Claude\claude_desktop_config.json`
- macOS: `~/Library/Application Support/Claude/claude_desktop_config.json`

**設定例:**

```json
{
  "mcpServers": {
    "ateliers-mcp-server": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\Ateliers.Ai.McpServer.csproj",
        "--configuration",
        "Release"
      ]
    }
  }
}
```

**または実行ファイルを直接指定:**

```json
{
  "mcpServers": {
    "ateliers-mcp-server": {
      "command": "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\bin\\Release\\net10.0\\Ateliers.Ai.McpServer.exe"
    }
  }
}
```

### 5. Claude Desktop再起動

Claude Desktopを再起動すると、MCPサーバーが利用可能になります。

## 使い方

### ファイル読み取り

```
Services/GitHubService.cs を読んで
```

Claude が自動的に `read_repository_file` ツールを使用します。

### ファイル編集（Git統合）

```
README.mdのバージョン履歴を更新して
```

AutoPush=true の場合、Claude が以下を実行：
1. AutoPull確認→リモートの最新を取得
2. `read_repository_file` でREADME.mdを読み取り
3. 内容を更新
4. `edit_repository_file` で保存（自動バックアップ作成）
5. Git commit & push（自動）

### 記事検索

```
GitHub Actionsに関する記事を探して
```

Claude が `search_articles` で記事を検索し、関連記事を提示します。

## Git統合機能

### AutoPull/AutoPush

リポジトリごとに設定可能：

```json
{
  "Repositories": {
    "PublicNotes": {
      "AutoPull": true,   // 書き込み前に自動プル
      "AutoPush": true    // 書き込み後に自動プッシュ
    }
  }
}
```

### コンフリクト処理

マージコンフリクト検出時はエラーで停止し、手動解決を促します：

```
❌ Pull failed: Merge conflict detected. Please resolve manually:
1. Navigate to repository
2. Run: git status
3. Resolve conflicts
4. Run: git add . && git commit
```

### コミットメッセージ

デフォルト: `Update {filePath} via MCP`

将来的にカスタマイズ可能な実装予定。

## トラブルシューティング

### MCPサーバーが認識されない

1. Claude Desktopを完全に再起動
2. `claude_desktop_config.json` のパスが正しいか確認
3. ビルドエラーがないか確認

### ファイルが読み取れない

1. `appsettings.local.json` のLocalPathが正しいか確認
2. GitHub API使用時はPATが設定されているか確認
3. ファイルパスが正しいか確認（相対パスで指定）

### Git Push が失敗する

1. GitHub Token が正しく設定されているか確認
2. Tokenに Contents: Write 権限があるか確認
3. リポジトリがGitで初期化されているか確認
4. リモートブランチが設定されているか確認（`git remote -v`）

### ツールが見つからない

1. Claude Desktopを再起動
2. 最新版にビルドし直す
3. ログを確認（`%APPDATA%\Claude\logs\`）

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

## ライセンス

MIT License

## 関連リンク

- [ateliers.dev](https://ateliers.dev) - 技術ブログ
- [ateliers-ai-assistants](https://github.com/yuu-git/ateliers-ai-assistants) - AIコーディングガイドライン
- [Model Context Protocol](https://modelcontextprotocol.io/) - MCP公式サイト

## 今後の予定

### Phase 6: Docusaurus統合
- 記事作成ツール（create_blog_post, create_doc_article）
- Frontmatter自動生成
- 会話→記事変換機能
- **v1.0.0目標**: Docusaurus + MCP完全統合

### Phase 7以降
- SQLServer/SQLite統合
- 役割別MCPサーバー分割（coding, docs, productivity）
- VoicePeak CLI統合
- Docker化（配布オプション）
