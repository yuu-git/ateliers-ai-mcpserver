# トラブルシューティングガイド

ateliers-ai-mcpserver の一般的な問題と解決方法をまとめたガイドです。

---

## 目次

- [共通の問題](#共通の問題)
- [クライアント別の問題](#クライアント別の問題)
- [認証の問題](#認証の問題)
- [Git操作の問題](#git操作の問題)
- [ログの確認方法](#ログの確認方法)
- [問題の報告方法](#問題の報告方法)

---

## 共通の問題

### MCPサーバーが起動しない

**症状:**
クライアントからMCPサーバーが認識されない、またはツール一覧が表示されない

**原因と対策:**

#### 1. .NET SDKがインストールされていない

**確認方法:**
```bash
dotnet --version
```

**対策:**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) をインストール
- インストール後、ターミナルを再起動

#### 2. プロジェクトがビルドされていない

**確認方法:**
```bash
cd [YOUR_CLONE_PATH]\ateliers-ai-mcpserver
dotnet build
```

**対策:**
```bash
dotnet restore
dotnet build --configuration Release
```

#### 3. 設定ファイルのパスが間違っている

**確認方法:**
- 設定ファイル（`.mcp.json`, `claude_desktop_config.json`など）のパスを確認
- プロジェクトファイル（`.csproj`）が実際に存在するか確認

**対策:**
- フルパスで指定（Claude Desktop、一部のクライアント）
- 相対パスが正しいか確認（VS Code、Visual Studio）
- パス区切りは `\\` （Windowsの場合、エスケープ必要）

#### 4. クライアントが設定を読み込んでいない

**対策:**
- クライアントを完全に再起動
- VS Code: `Developer: Reload Window`
- Visual Studio: ソリューションを閉じて再度開く
- Claude Desktop: タスクトレイから "Quit" → 再起動

---

### ツールは表示されるが、実行時にエラーが出る

**症状:**
ツール一覧には表示されるが、実際にツールを使用するとエラーが発生

**原因と対策:**

#### 1. appsettings.jsonが見つからない

**対策:**
```bash
# プロジェクトルートから確認
ls Ateliers.Ai.McpServer/appsettings.json
```

存在しない場合は、Gitから最新版を取得：
```bash
git pull origin main
```

#### 2. リポジトリ設定が不正

**対策:**
`appsettings.json` のリポジトリ設定を確認：
```json
{
  "Repositories": {
    "AteliersAiMcpServer": {
      "Owner": "yuu-git",
      "Name": "ateliers-ai-mcpserver",
      "LocalPath": ""
    }
  }
}
```

#### 3. 依存関係の問題

**対策:**
```bash
dotnet restore
dotnet build --configuration Release
```

---

### ファイルが読み取れない

**症状:**
`read_repository_file` でファイルが見つからないエラー

**原因と対策:**

#### 1. ファイルパスの指定が間違っている

**対策:**
- パスはリポジトリルートからの相対パスで指定
- 例: `Services/GitHubService.cs`（✅）
- 例: `/Services/GitHubService.cs`（❌ 先頭のスラッシュ不要）

#### 2. LocalPathが設定されていない（GitHub API使用時）

**対策:**
- GitHub Personal Access Token を設定
- または LocalPath を設定してローカルアクセスに切り替え

#### 3. リポジトリがクローンされていない

**対策:**
LocalPath を使用する場合、該当リポジトリをクローン：
```bash
git clone https://github.com/yuu-git/[repository-name].git
```

---

## クライアント別の問題

### Claude Desktop

詳細は [Claude Desktop トラブルシューティング](setup/claude-desktop.md#トラブルシューティング) を参照

**よくある問題:**
- 設定ファイルのパスが絶対パスでない
- JSONの構文エラー
- Claude Desktopの再起動忘れ

### VS Code

詳細は [VS Code トラブルシューティング](setup/vscode.md#トラブルシューティング) を参照

**よくある問題:**
- VS Codeのバージョンが古い（1.102未満）
- GitHub Copilot拡張機能が無効
- `.vscode/mcp.json` が存在しない
- Agent Modeが有効化されていない

### Visual Studio

詳細は [Visual Studio トラブルシューティング](setup/visual-studio.md#トラブルシューティング) を参照

**よくある問題:**
- Visual Studioのバージョンが古い（17.14未満）
- GitHub Copilot拡張機能が無効
- `.mcp.json` と `.vscode/mcp.json` の競合（VS 2026 Preview）
- Agent Modeが有効化されていない

---

## 認証の問題

### GitHub Personal Access Tokenが無効

**症状:**
- GitHub APIを使用するツールでエラー
- Git Push操作が失敗

**対策:**

#### 1. トークンの作成

1. GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. "Generate new token"
3. 必要な権限を選択：
   - `repo`: リポジトリへのフルアクセス
   - `read:user`: ユーザー情報の読み取り

#### 2. トークンの設定

`appsettings.local.json` を作成：
```json
{
  "GitHub": {
    "Token": "ghp_your_token_here",
    "Email": "your-email@example.com",
    "Username": "your-github-username"
  }
}
```

#### 3. トークンの有効期限

**確認方法:**
GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)

**対策:**
- 有効期限が切れている場合は、新しいトークンを生成
- トークンを再生成したら、`appsettings.local.json` を更新

---

### Git認証エラー

**症状:**
```
Authentication failed: Invalid credentials
```

**原因と対策:**

#### 1. トークンの権限不足

**対策:**
- トークンに `repo` 権限があるか確認
- 必要に応じてトークンを再生成

#### 2. トークンの設定場所が間違っている

**対策:**
認証情報の優先順位を確認：
1. リポジトリ固有の設定（`GitHubToken`, `GitEmail`, `GitUsername`）
2. グローバル設定（`GitHub.Token`, `GitHub.Email`, `GitHub.Username`）

```json
{
  "GitHub": {
    "Token": "ghp_global_token",
    "Email": "global@example.com",
    "Username": "global-username"
  },
  "Repositories": {
    "SpecificRepo": {
      "GitHubToken": "ghp_specific_token",
      "GitEmail": "specific@example.com",
      "GitUsername": "specific-username"
    }
  }
}
```

---

## Git操作の問題

### Pull/Push が失敗する

**症状:**
- `pull_repository` や `push_repository` でエラー
- AutoPull/AutoPushが動作しない

**原因と対策:**

#### 1. リモートリポジトリが設定されていない

**確認方法:**
```bash
cd [YOUR_CLONE_PATH]\[repository-name]
git remote -v
```

**対策:**
```bash
git remote add origin https://github.com/yuu-git/[repository-name].git
```

#### 2. ローカルに未コミットの変更がある

**確認方法:**
```bash
git status
```

**対策:**
```bash
# 変更をコミット
git add .
git commit -m "Local changes"

# またはリセット（注意: 変更が失われます）
git reset --hard
```

#### 3. マージコンフリクトが発生

**症状:**
```
❌ Pull failed: Merge conflict detected
```

**対策:**
1. 該当リポジトリに移動
2. `git status` でコンフリクトファイルを確認
3. コンフリクトを手動解決
4. `git add . && git commit`
5. MCPツールから再度操作

---

### コミットメッセージがデフォルトのまま

**症状:**
すべてのコミットが `Update {filePath} via MCP` になる

**現状:**
現在はカスタマイズ不可

**将来の対応:**
Phase 7以降でコミットメッセージのカスタマイズ機能を実装予定

---

## ログの確認方法

### MCPサーバーのログ

#### 方法1: 直接起動してログ確認

ターミナルから直接MCPサーバーを起動：

```bash
cd [YOUR_CLONE_PATH]\ateliers-ai-mcpserver
dotnet run --project .\Ateliers.Ai.McpServer\Ateliers.Ai.McpServer.csproj
```

起動ログやエラーメッセージがリアルタイムで表示されます。

#### 方法2: クライアントのログを確認

**Claude Desktop:**
```
%APPDATA%\Claude\logs\
```

**VS Code:**
```
出力 → GitHub Copilot
```

**Visual Studio:**
```
表示 → 出力 → 出力元: GitHub Copilot
```

---

### Git操作のログ

Git操作の詳細ログを確認：

```bash
cd [YOUR_CLONE_PATH]\[repository-name]

# 最近のコミット履歴
git log --oneline -10

# リモートの状態
git remote -v

# ブランチの状態
git status

# 最後のPull/Push結果
git reflog
```

---

## 問題の報告方法

### 報告前の確認事項

以下の情報を収集してください：

1. **環境情報**
   - OS（Windows / macOS / Linux）
   - .NET SDKバージョン（`dotnet --version`）
   - クライアント（Claude Desktop / VS Code / Visual Studio）
   - クライアントのバージョン

2. **エラー情報**
   - エラーメッセージの全文
   - エラーが発生したツール名
   - 実行したコマンドやプロンプト

3. **設定情報**
   - 使用している設定ファイル（個人情報を除く）
   - appsettings.local.jsonの構成（トークンは除く）

4. **ログ**
   - MCPサーバーのログ
   - クライアントのログ

### 報告先

**GitHub Issues:**
https://github.com/yuu-git/ateliers-ai-mcpserver/issues

**報告テンプレート:**

```markdown
## 環境
- OS: Windows 11
- .NET SDK: 8.0.100
- クライアント: VS Code 1.102

## 問題の説明
[問題の詳細を記述]

## 再現手順
1. [手順1]
2. [手順2]
3. [手順3]

## エラーメッセージ
```
[エラーメッセージの全文]
```

## 期待される動作
[期待していた動作を記述]

## 追加情報
- 設定ファイルの構成
- ログの抜粋
```

---

## よくある質問（FAQ）

### Q1: 複数のクライアントで同時に使用できますか?

**A:** はい、問題なく使用できます。すべてのクライアントから同じMCPサーバーを使用できます。

### Q2: LocalPathと GitHub APIはどちらを使うべきですか?

**A:**
- **LocalPath推奨**: ローカルにリポジトリがある場合（10-5000倍高速）
- **GitHub API**: ローカルにリポジトリがない場合、または読み取り専用の場合

### Q3: Git統合（AutoPull/AutoPush）は必須ですか?

**A:** いいえ、オプションです。ファイルの読み書きだけなら不要ですが、自動コミット＆プッシュを使いたい場合は設定してください。

### Q4: appsettings.local.jsonはどこに作成しますか?

**A:** `Ateliers.Ai.McpServer/appsettings.local.json`
このファイルは `.gitignore` で除外されており、Gitにコミットされません。

### Q5: Visual Studio 2026 Previewで動作しますか?

**A:** はい、動作します。ただし、`.vscode/mcp.json` との競合の可能性があるため、注意が必要です。

### Q6: エラーが解決しない場合は?

**A:** 
1. このトラブルシューティングガイドを確認
2. クライアント別のトラブルシューティングを確認
3. GitHub Issuesで報告

---

## 参考資料

- [Claude Desktop セットアップガイド](setup/claude-desktop.md)
- [VS Code セットアップガイド](setup/vscode.md)
- [Visual Studio セットアップガイド](setup/visual-studio.md)
- [Model Context Protocol 公式](https://modelcontextprotocol.io/)
- [.NET ダウンロード](https://dotnet.microsoft.com/download)
