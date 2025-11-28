# Visual Studio セットアップガイド

ateliers-ai-mcpserver を Visual Studio 2022 の GitHub Copilot から使用するためのセットアップガイドです。

## 前提条件

### 必須

- **Visual Studio 2022**: バージョン 17.14 以降
- **GitHub Copilot 拡張機能**: インストール済み＆有効化済み
- **.NET 8 SDK**: インストール済み

### 確認方法

#### Visual Studio バージョン確認
```
ヘルプ → Microsoft Visual Studio のバージョン情報
→ バージョン番号を確認（17.14 以降）
```

#### GitHub Copilot 確認
```
拡張機能 → 拡張機能の管理 → "GitHub Copilot" で検索
→ インストール済みか確認
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

### Step 2: Visual Studio でソリューションを開く

```bash
start Ateliers.Ai.McpServer.sln
```

または、Visual Studio から `File > Open > Project/Solution` でソリューションファイルを開きます。

### Step 3: MCP設定ファイルを作成

`.mcp.json.sample` をコピーして `.mcp.json` を作成します。

**方法1: コマンドラインから**
```bash
# Windows (PowerShell)
Copy-Item .mcp.json.sample .mcp.json
```

**方法2: Visual Studio から**
1. ソリューションエクスプローラーで `.mcp.json.sample` を右クリック
2. "Copy" を選択
3. ソリューションルートで右クリック → "Paste"
4. ファイル名を `.mcp.json` に変更

**配置場所の選択肢:**

Visual Studio は複数の場所の `.mcp.json` をサポートします：

1. **ソリューションルート** (推奨)
   - `[SOLUTIONDIR]\.mcp.json`
   - チーム共有可能（`.mcp.json.sample` として）

2. **Visual Studio専用**
   - `[SOLUTIONDIR]\.vs\mcp.json`
   - ソリューション固有設定（自動的にgitignore対象）

3. **VS Codeと共有**
   - `[SOLUTIONDIR]\.vscode\mcp.json`
   - VS Codeユーザーとの共用

4. **グローバル設定**
   - `%USERPROFILE%\.mcp.json`
   - すべてのソリューションで使用

### Step 4: 設定内容の確認

`.mcp.json` の内容:

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
      ]
    }
  }
}
```

**ポイント:**
- 相対パスを使用しているため、プロジェクトをどこにクローンしても動作します
- Visual Studioの場合、環境変数は不要です（必要に応じて追加可能）

### Step 5: Visual Studio を再起動

設定を反映するため、Visual Studio を再起動します。

または、ソリューションを閉じて再度開きます。

---

## 動作確認

### CodeLens を確認

コードエディタ上部に CodeLens が表示されるか確認します。

**表示例:**
```csharp
// CodeLens: Ask Copilot | Explain | Generate Tests
public class Program
{
    ...
}
```

### Copilot Chat を開く

**方法1: メニューから**
```
表示 → GitHub Copilot Chat
```

**方法2: ショートカット**
```
Ctrl+Q で検索 → "Copilot Chat" を選択
```

### ツール一覧の確認

Copilot Chat で以下のようなメッセージを送信:

```
@workspace ateliers-ai-mcpserver で使用できるツールを一覧表示してください
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

Copilot Chat で以下を試してみましょう:

```
ateliers-ai-mcpserver の list_repository_files を使って、
このプロジェクトのファイル一覧を表示してください
```

ファイル一覧が返ってくれば、MCPサーバーが正常に動作しています。

---

## Visual Studio 固有の機能

### CodeLens統合

コードエディタ上でCopilotの提案を直接利用できます:

```csharp
public class MyService
{
    // CodeLens: Ask Copilot | Explain | Generate Tests
    public void DoSomething()
    {
        // コード
    }
}
```

**できること:**
- コードの説明を求める
- テストコードを生成
- リファクタリング提案
- MCPツールを使った操作

### Agent Mode

Visual Studio 2022 17.14以降では、Agent Modeが利用可能です。

**有効化方法:**
```
ツール → オプション → GitHub Copilot → Enable Agent Mode
```

**Agent Mode でできること:**
- 複数ステップのタスク実行
- MCPツールの自動選択・実行
- コード生成とファイル操作の統合

### ソリューションエクスプローラー統合

ソリューションエクスプローラーから直接Copilotを呼び出せます:

1. ファイルまたはフォルダを右クリック
2. "Ask Copilot" を選択
3. MCPツールを使った操作を依頼

---

## トラブルシューティング

### ツールが表示されない

**症状:**
Copilot Chat でツール一覧が表示されない

**原因と対策:**

1. **Visual Studio バージョンが古い**
   - Visual Studio 2022 17.14以降にアップデート

2. **GitHub Copilot が無効**
   - 拡張機能の管理で GitHub Copilot を有効化

3. **設定ファイルが読み込まれていない**
   - `.mcp.json` が正しい場所にあるか確認
   - Visual Studio を再起動

4. **.NET SDK がインストールされていない**
   - `dotnet --version` で確認
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) をインストール

### MCPサーバーが起動しない

**症状:**
ツールは表示されるが、実行時にエラーが出る

**確認事項:**

1. **プロジェクトパスの確認**
   ```bash
   # ソリューションルートで実行
   dotnet run --project ./Ateliers.Ai.McpServer/Ateliers.Ai.McpServer.csproj
   ```
   これが成功すれば、Visual Studioからも起動できるはずです。

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

### CodeLens が表示されない

**症状:**
コードエディタでCodeLensが表示されない

**対策:**

1. **CodeLens機能の有効化**
   ```
   ツール → オプション → テキストエディター → すべての言語 → CodeLens
   → "CodeLensを有効にする" にチェック
   ```

2. **GitHub Copilot の CodeLens 有効化**
   ```
   ツール → オプション → GitHub Copilot → Enable CodeLens
   ```

### ログの確認方法

詳細なログを確認したい場合:

```bash
# ターミナルから直接起動してログを確認
dotnet run --project ./Ateliers.Ai.McpServer/Ateliers.Ai.McpServer.csproj
```

MCPサーバーの起動ログやエラーメッセージが表示されます。

---

## よくある質問

### Q1: 複数のソリューションで使用できますか?

**A:** はい、`.mcp.json` を各ソリューションに配置するか、グローバル設定（`%USERPROFILE%\.mcp.json`）を使用できます。

### Q2: VS Code と併用できますか?

**A:** はい、`.vscode\mcp.json` に設定すれば、VS CodeとVisual Studioで同じMCPサーバーを共有できます。

### Q3: Claude Desktop と併用できますか?

**A:** はい、問題なく併用できます。すべてのクライアントから同じMCPサーバーを使用できます。

### Q4: Agent Mode は必須ですか?

**A:** いいえ、Agent ModeなしでもMCPツールは使用できます。Agent Modeは複雑なタスクを自動化したい場合に便利です。

### Q5: 設定を変更したら再起動が必要ですか?

**A:** はい、`.mcp.json` を変更したら Visual Studio の再起動が必要です。

---

## 参考資料

- [Visual Studio MCP Documentation](https://learn.microsoft.com/en-us/visualstudio/ide/mcp-servers)
- [GitHub Copilot for Visual Studio](https://docs.github.com/copilot/using-github-copilot/using-github-copilot-in-visual-studio)
- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [ateliers-ai-mcpserver GitHub](https://github.com/yuu-git/ateliers-ai-mcpserver)

---

## 次のステップ

Visual Studio統合が完了したら:

1. **実際のプロジェクトで使ってみる**
   - Git操作の自動化
   - ファイル操作の効率化
   - CodeLensからのツール呼び出し

2. **他のIDEも試す**
   - VS Code 統合
   - Claude Desktop 統合

3. **Agent Mode を活用**
   - 複数ステップのタスク自動化
   - コード生成とファイル操作の統合

---

**サポートが必要な場合:**
- GitHub Issues: https://github.com/yuu-git/ateliers-ai-mcpserver/issues
- プロジェクトドキュメント: `Docs/` ディレクトリ
