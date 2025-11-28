# VS Code セットアップガイド

ateliers-ai-mcpserver を VS Code の GitHub Copilot Agent Mode から使用するためのセットアップガイドです。

## 前提条件

### 必須

- **VS Code**: バージョン 1.102 以降
- **GitHub Copilot 拡張機能**: インストール済み＆有効化済み
- **.NET 8 SDK**: インストール済み

### 確認方法

#### VS Code バージョン確認
```
Ctrl+Shift+P → "About" → バージョン番号を確認
```

#### GitHub Copilot 確認
```
拡張機能タブ → "GitHub Copilot" で検索 → インストール済みか確認
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

### Step 2: VS Code でワークスペースを開く

```bash
code .
```

または、VS Code から `File > Open Folder` でプロジェクトフォルダを開きます。

### Step 3: MCP設定ファイルを作成

`.vscode/mcp.json.sample` をコピーして `.vscode/mcp.json` を作成します。

**方法1: コマンドラインから**
```bash
# Windows (PowerShell)
Copy-Item .vscode/mcp.json.sample .vscode/mcp.json

# macOS / Linux
cp .vscode/mcp.json.sample .vscode/mcp.json
```

**方法2: VS Code から**
1. `.vscode/mcp.json.sample` を右クリック
2. "Copy" を選択
3. `.vscode/` フォルダ内で右クリック → "Paste"
4. ファイル名を `mcp.json` に変更

### Step 4: 設定内容の確認

`.vscode/mcp.json` の内容:

```json
{
  "servers": {
    "ateliers-ai-mcpserver": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "./Ateliers.Ai.McpServer/Ateliers.Ai.McpServer.csproj"
      ],
      "env": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
  }
}
```

**ポイント:**
- 相対パスを使用しているため、プロジェクトをどこにクローンしても動作します
- `DOTNET_ENVIRONMENT=Development` で詳細なログが出力されます

### Step 5: VS Code を再起動

設定を反映するため、VS Code を再起動します。

```
Ctrl+Shift+P → "Developer: Reload Window"
```

---

## 動作確認

### Agent Mode を起動

**方法1: コマンドパレットから**
```
Ctrl+Shift+P → "GitHub Copilot: Open Agent Mode"
```

**方法2: Copilot チャットから**
```
Copilot チャット画面で @workspace を入力
```

### ツール一覧の確認

Agent Mode で以下のようなメッセージを送信:

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

Agent Mode で以下を試してみましょう:

```
ateliers-ai-mcpserver の read_article を使って、
docs ディレクトリにある記事のタイトルを教えてください
```

記事タイトルが返ってくれば、MCPサーバーが正常に動作しています。

---

## トラブルシューティング

### ツールが表示されない

**症状:**
Agent Mode でツール一覧が表示されない

**原因と対策:**

1. **VS Code バージョンが古い**
   - VS Code 1.102以降にアップデート

2. **GitHub Copilot が無効**
   - 拡張機能タブで GitHub Copilot を有効化

3. **設定ファイルが読み込まれていない**
   - `.vscode/mcp.json` が存在するか確認
   - VS Code を再起動（`Developer: Reload Window`）

4. **.NET SDK がインストールされていない**
   - `dotnet --version` で確認
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) をインストール

### MCPサーバーが起動しない

**症状:**
ツールは表示されるが、実行時にエラーが出る

**確認事項:**

1. **プロジェクトパスの確認**
   ```bash
   # プロジェクトルートで実行
   dotnet run --project ./Ateliers.Ai.McpServer/Ateliers.Ai.McpServer.csproj
   ```
   これが成功すれば、VS Codeからも起動できるはずです。

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

### ログの確認方法

詳細なログを確認したい場合:

```bash
# ターミナルから直接起動してログを確認
dotnet run --project ./Ateliers.Ai.McpServer/Ateliers.Ai.McpServer.csproj
```

MCPサーバーの起動ログやエラーメッセージが表示されます。

---

## よくある質問

### Q1: 複数のプロジェクトで使用できますか?

**A:** はい、各プロジェクトの `.vscode/mcp.json` に設定を追加できます。

### Q2: Claude Desktop と併用できますか?

**A:** はい、問題なく併用できます。両方から同じMCPサーバーを使用できます。

### Q3: チーム全体で共有できますか?

**A:** `.vscode/mcp.json` はユーザー固有の設定のため、各自が設定する必要があります。
`.vscode/mcp.json.sample` をリポジトリに含めて共有するのがおすすめです。

### Q4: 設定を変更したら再起動が必要ですか?

**A:** はい、`.vscode/mcp.json` を変更したら VS Code の再起動（`Developer: Reload Window`）が必要です。

---

## 参考資料

- [VS Code MCP Documentation](https://code.visualstudio.com/docs/copilot/customization/mcp-servers)
- [GitHub Copilot Documentation](https://docs.github.com/copilot)
- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [ateliers-ai-mcpserver GitHub](https://github.com/yuu-git/ateliers-ai-mcpserver)

---

## 次のステップ

VS Code統合が完了したら:

1. **実際のプロジェクトで使ってみる**
   - Git操作の自動化
   - ファイル操作の効率化
   - 記事作成の自動化

2. **他のIDEも試す**
   - Visual Studio 統合
   - Claude Desktop 統合

3. **カスタマイズ**
   - 独自のツールを追加
   - 環境変数の設定

---

**サポートが必要な場合:**
- GitHub Issues: https://github.com/yuu-git/ateliers-ai-mcpserver/issues
- プロジェクトドキュメント: `Docs/` ディレクトリ
