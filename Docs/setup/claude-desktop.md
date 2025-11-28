# Claude Desktop セットアップガイド

ateliers-ai-mcpserver を Claude Desktop から使用するためのセットアップガイドです。

## 前提条件

### 必須

- **Claude Desktop**: 最新版
- **.NET 8 SDK**: インストール済み

### 確認方法

#### Claude Desktop バージョン確認
```
Claude Desktop → Settings → About
→ バージョン番号を確認
```

#### .NET SDK 確認
```bash
dotnet --version
# 8.0.x と表示されればOK
```

---

## セットアップ手順

### Step 1: リポジトリをクローン

```bash
git clone https://github.com/yuu-git/ateliers-ai-mcpserver.git
cd ateliers-ai-mcpserver
```

クローンした場所を覚えておいてください。この後の設定で使用します。

**例:**
- `C:\Projects\ateliers-ai-mcpserver`
- `C:\Users\YourName\source\repos\ateliers-ai-mcpserver`
- `D:\dev\ateliers-ai-mcpserver`

### Step 2: 設定ファイルの場所を確認

Claude Desktopの設定ファイルは以下の場所にあります：

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

**実際のパス例:**
```
C:\Users\YourName\AppData\Roaming\Claude\claude_desktop_config.json
```

### Step 3: 設定ファイルを編集

設定ファイルを開き、`mcpServers` セクションに ateliers-ai-mcpserver の設定を追加します。

**2つの設定方法があります：**

#### 方法1: dotnet run方式（推奨）

開発中のコード変更が即座に反映されます。

```json
{
  "mcpServers": {
    "ateliers-ai-mcpserver": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "[YOUR_CLONE_PATH]\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\Ateliers.Ai.McpServer.csproj"
      ]
    }
  }
}
```

**`[YOUR_CLONE_PATH]` を実際のパスに置き換えてください:**

例: `C:\Projects` にクローンした場合
```json
{
  "mcpServers": {
    "ateliers-ai-mcpserver": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Projects\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\Ateliers.Ai.McpServer.csproj"
      ]
    }
  }
}
```

**重要**: 
- プロジェクトパスは**必ずフルパス**で指定してください。相対パスは使えません。
- パス区切りは `\\` （バックスラッシュ2つ）でエスケープ

#### 方法2: ビルド済みexe方式

起動が高速です。ただし、コード変更時には毎回ビルドが必要です。

**事前にビルド:**
```bash
cd [YOUR_CLONE_PATH]\ateliers-ai-mcpserver
dotnet publish -c Release
```

**設定:**
```json
{
  "mcpServers": {
    "ateliers-ai-mcpserver": {
      "command": "[YOUR_CLONE_PATH]\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\bin\\Release\\net8.0\\Ateliers.Ai.McpServer.exe",
      "args": []
    }
  }
}
```

**例:** `C:\Projects` にクローンした場合
```json
{
  "mcpServers": {
    "ateliers-ai-mcpserver": {
      "command": "C:\\Projects\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\bin\\Release\\net8.0\\Ateliers.Ai.McpServer.exe",
      "args": []
    }
  }
}
```

### Step 4: Claude Desktop を再起動

設定ファイルを保存したら、Claude Desktop を完全に終了して再起動します。

**Windows:**
1. タスクトレイのClaude Desktopアイコンを右クリック
2. "Quit" を選択
3. Claude Desktopを再度起動

---

## 動作確認

### ツール一覧の確認

Claude Desktop で以下のメッセージを送信:

```
ateliers-ai-mcpserver で使用できるツールを一覧表示してください
```

以下のツールが表示されれば成功です:

#### GitTools（Git操作）
- `commit_and_push_repository` - コミット＆プッシュ
- `commit_repository` - コミット
- `push_repository` - プッシュ
- `pull_repository` - プル
- `create_tag` - タグ作成
- `push_tag` - タグプッシュ
- `create_and_push_tag` - タグ作成＆プッシュ

#### RepositoryTools（ファイル操作）
- `add_repository_file` - ファイル追加
- `edit_repository_file` - ファイル編集
- `delete_repository_file` - ファイル削除
- `backup_repository_file` - バックアップ作成
- `copy_repository_file` - ファイルコピー
- `rename_repository_file` - ファイルリネーム
- `list_repository_files` - ファイル一覧
- `read_repository_file` - ファイル読み込み

#### AteliersDevTools（記事操作）
- `list_articles` - 記事一覧
- `search_articles` - 記事検索
- `read_article` - 記事読み込み

#### GitHubTools（GitHub操作）
※ GitHub Personal Access Token が必要

### 簡単な動作テスト

Claude Desktop で以下を試してみましょう:

```
ateliers-ai-mcpserver の read_article を使って、
docs ディレクトリにある記事のタイトルを教えてください
```

記事タイトルが返ってくれば、MCPサーバーが正常に動作しています。

---

## 高度な設定

### 複数のMCPサーバーを使用

Claude Desktopでは複数のMCPサーバーを同時に使用できます：

```json
{
  "mcpServers": {
    "ateliers-ai-mcpserver": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "[YOUR_CLONE_PATH]\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\Ateliers.Ai.McpServer.csproj"
      ]
    },
    "other-mcp-server": {
      "command": "node",
      "args": [
        "path/to/other-server.js"
      ]
    }
  }
}
```

### 環境変数の設定

環境変数が必要な場合（例: GitHub Token）:

```json
{
  "mcpServers": {
    "ateliers-ai-mcpserver": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "[YOUR_CLONE_PATH]\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\Ateliers.Ai.McpServer.csproj"
      ],
      "env": {
        "DOTNET_ENVIRONMENT": "Development",
        "GITHUB_TOKEN": "ghp_your_token_here"
      }
    }
  }
}
```

**セキュリティ注意**: 環境変数に機密情報を直接書くのは推奨されません。代わりに `appsettings.Development.json` を使用してください。

---

## トラブルシューティング

### ツールが表示されない

**症状:**
Claude Desktop でツール一覧が表示されない

**原因と対策:**

1. **設定ファイルのJSON構文エラー**
   - JSONの構文が正しいか確認（カンマ、括弧など）
   - オンラインJSONバリデーターで検証

2. **.NET SDK がインストールされていない**
   - `dotnet --version` で確認
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) をインストール

3. **プロジェクトパスが間違っている**
   - フルパスで指定しているか確認
   - パス区切りは `\\` （エスケープ）
   - プロジェクトファイル（.csproj）が実際に存在するか確認

4. **Claude Desktop が設定を読み込んでいない**
   - Claude Desktop を完全に終了（タスクトレイからQuit）
   - 再起動

### MCPサーバーが起動しない

**症状:**
ツールは表示されるが、実行時にエラーが出る

**確認事項:**

1. **プロジェクトが手動で起動するか確認**
   ```bash
   cd [YOUR_CLONE_PATH]\ateliers-ai-mcpserver
   dotnet run --project .\Ateliers.Ai.McpServer\Ateliers.Ai.McpServer.csproj
   ```
   これが成功すれば、Claude Desktopからも起動できるはずです。

2. **依存関係の復元**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **appsettings.json の確認**
   `Ateliers.Ai.McpServer/appsettings.json` が存在するか確認

### GitHub Tools が使えない

**症状:**
GitHubTools（`list_repositories` など）でエラーが出る

**原因:**
GitHub Personal Access Token が設定されていない

**対策:**
`appsettings.Development.json` を作成してトークンを設定:

```json
{
  "GitHub": {
    "PersonalAccessToken": "ghp_your_token_here"
  }
}
```

**トークンの取得方法:**
1. GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. "Generate new token" → 必要な権限を選択
3. トークンをコピーして `appsettings.Development.json` に貼り付け

### ビルド済みexe方式で「ファイルが見つからない」エラー

**症状:**
`.exe` ファイルのパスが正しいのにエラーが出る

**原因:**
- ビルドされていない
- .NET バージョンが異なる（net8.0 vs net10.0）

**対策:**
1. 正しいバージョンでビルド:
   ```bash
   dotnet publish -c Release
   ```

2. 実際に生成されたパスを確認:
   ```bash
   dir [YOUR_CLONE_PATH]\ateliers-ai-mcpserver\Ateliers.Ai.McpServer\bin\Release\ /s
   ```

3. 見つかった `.exe` ファイルのフルパスを設定に使用

### ログの確認方法

Claude Desktop にはMCPサーバーのログが表示されません。
直接ターミナルから起動してログを確認:

```bash
cd [YOUR_CLONE_PATH]\ateliers-ai-mcpserver
dotnet run --project .\Ateliers.Ai.McpServer\Ateliers.Ai.McpServer.csproj
```

MCPサーバーの起動ログやエラーメッセージが表示されます。

---

## よくある質問

### Q1: VS Code や Visual Studio と併用できますか?

**A:** はい、問題なく併用できます。すべてのクライアントから同じMCPサーバーを使用できます。

### Q2: 設定を変更したら再起動が必要ですか?

**A:** はい、`claude_desktop_config.json` を変更したら Claude Desktop の再起動が必要です。

### Q3: 相対パスは使えますか?

**A:** いいえ、Claude Desktop では相対パスは使えません。必ずフルパスで指定してください。

### Q4: dotnet run方式とビルド済みexe方式、どちらがおすすめですか?

**A:** 
- **開発中**: dotnet run方式（コード変更が即座に反映）
- **本番利用**: ビルド済みexe方式（起動が高速）

### Q5: 複数のプロジェクトで使用できますか?

**A:** はい、Claude Desktop は全システムで共通の設定を使用するため、すべてのプロジェクトで使用できます。

---

## 参考資料

- [Claude Desktop MCP Documentation](https://docs.anthropic.com/claude/docs/model-context-protocol)
- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [ateliers-ai-mcpserver GitHub](https://github.com/yuu-git/ateliers-ai-mcpserver)

---

## 次のステップ

Claude Desktop統合が完了したら:

1. **実際のプロジェクトで使ってみる**
   - Git操作の自動化
   - ファイル操作の効率化
   - 記事作成の自動化

2. **他のIDEも試す**
   - VS Code 統合
   - Visual Studio 統合

3. **マルチクライアントの活用**
   - Claude Desktop で設計
   - VS Code で実装
   - すべてのクライアントから同じツールを使用

---

**サポートが必要な場合:**
- GitHub Issues: https://github.com/yuu-git/ateliers-ai-mcpserver/issues
- プロジェクトドキュメント: `Docs/` ディレクトリ
