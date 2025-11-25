# Ateliers AI MCP Server

C#/.NETで実装したModel Context Protocol（MCP）サーバー。
Claude Desktop向けに**GitHub/ローカルファイル統合操作**と**技術記事参照**を提供。

## 概要

ateliers.devの技術資産（コーディングガイドライン、技術記事、開発リポジトリ）をClaude Desktopから直接参照・編集できるMCPサーバー。

### 主な特徴

- ✅ **ローカル優先アクセス** - LocalPath設定時は高速なローカルファイル操作、未設定時はGitHub API経由
- ✅ **汎用ファイル操作** - 読み取り/書き込み/削除/リネーム/コピー/バックアップの完全CRUD
- ✅ **記事専門ツール** - ateliers.dev技術記事の検索・一覧・読み取り（Frontmatter自動除去）
- ✅ **複数リポジトリ対応** - 設定ファイルで柔軟なリポジトリ管理

## バージョン履歴

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

### RepositoryTools（汎用ファイル操作）

| ツール | 機能 | ローカル/GitHub |
|:--|:--|:--|
| `read_repository_file` | ファイル読み取り | 両対応（ローカル優先） |
| `list_repository_files` | ファイル一覧取得 | 両対応（ローカル優先） |
| `add_repository_file` | ファイル新規作成 | ローカルのみ |
| `edit_repository_file` | ファイル更新（自動バックアップ） | ローカルのみ |
| `delete_repository_file` | ファイル削除（自動バックアップ） | ローカルのみ |
| `rename_repository_file` | ファイルリネーム | ローカルのみ |
| `copy_repository_file` | ファイルコピー | ローカルのみ |
| `backup_repository_file` | バックアップ作成 | ローカルのみ |

### AteliersDevTools（記事専門）

| ツール | 機能 |
|:--|:--|
| `read_article` | 記事読み取り（Frontmatter自動除去） |
| `list_articles` | 記事一覧取得（.md + .mdx） |
| `search_articles` | キーワード検索（ファイル名・内容） |

## 前提条件

- .NET 10.0 SDK
- Claude Desktop
- GitHub Personal Access Token（オプション：GitHub API使用時）

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
    "AteliersAiMcpServer": {
      "LocalPath": "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-ai-mcpserver"
    },
    "AteliersAiAssistants": {
      "LocalPath": "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-ai-assistants"
    }
  }
}
```

**メリット:**
- 10-5000倍高速なファイルアクセス
- GitHub APIレート制限の回避
- リアルタイムな編集フィードバック

#### 2-3. Personal Access Token設定（GitHub API使用時）

LocalPath未設定の場合、GitHub APIを使用するためPATが必要です。

**GitHubでPAT作成:**

1. GitHub → Settings → Developer settings → Personal access tokens → Fine-grained tokens
2. **Generate new token** をクリック
3. 設定：
   - **Token name**: `ateliers-ai-mcpserver`
   - **Repository access**: Public repositories (All repositories)
   - **Permissions**:
     - **Contents**: Read and write
     - **Metadata**: Read-only
4. トークンをコピー

**appsettings.local.jsonに追加:**

```json
{
  "GitHub": {
    "AuthenticationMode": "PAT",
    "PersonalAccessToken": "github_pat_11AAAAAA..."
  },
  "Repositories": {
    // LocalPath設定...
  }
}
```

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

### ファイル編集

```
README.mdのバージョン履歴を更新して
```

Claude が以下を実行：
1. `read_repository_file` でREADME.mdを読み取り
2. 内容を更新
3. `edit_repository_file` で保存（自動バックアップ作成）

### 記事検索

```
GitHub Actionsに関する記事を探して
```

Claude が `search_articles` で記事を検索し、関連記事を提示します。

## ローカル優先ロジック

本MCPサーバーは、以下のロジックでファイルアクセスを最適化：

```
1. LocalPath設定あり？
   ↓ YES
   ローカルファイルシステムから読み取り（高速）
   ↓ NO
   GitHub APIから読み取り（キャッシュ付き）
```

**書き込み操作**はローカルのみ対応（Phase 5でGit統合予定）。

## トラブルシューティング

### MCPサーバーが認識されない

1. Claude Desktopを完全に再起動
2. `claude_desktop_config.json` のパスが正しいか確認
3. ビルドエラーがないか確認

### ファイルが読み取れない

1. `appsettings.local.json` のLocalPathが正しいか確認
2. GitHub API使用時はPATが設定されているか確認
3. ファイルパスが正しいか確認（相対パスで指定）

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
│  └─ LocalFileService.cs     # ローカルファイル操作
├─ Tools/
│  ├─ RepositoryTools.cs      # 汎用ファイル操作ツール
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

### Phase 5: Git操作統合
- LibGit2Sharp導入
- AutoPull/AutoPush実装
- コミット＆プッシュ機能

### Phase 6以降
- SQLServer/SQLite統合
- 役割別MCPサーバー分割（coding, docs, productivity）
- VoicePeak CLI統合
