# Phase 10: MCP Server Split Plan

**作成日:** 2024-11-29  
**目的:** Phase 10以降のリファクタリング計画を記録  
**背景:** Phase 7-9でツール数増加に伴うモノリシック問題の解決

---

## 現状の問題点

### Phase 9完了時点でのツール数

```
ateliers-ai-mcpserver (モノリシック構成)
├── GitTools: 7ツール (Phase 5)
├── RepositoryTools: 8ツール (Phase 5)
├── AteliersDevTools: 3ツール (Phase 2-3)
├── NotionTools: 7ツール (Phase 7)
├── NotionExtendedTools: 14ツール (Phase 8)
└── DocusaurusTools: 4ツール (Phase 9)

合計: 43ツール
```

### 問題1: Tool Budget超過

**MCP公式ベストプラクティスより:**
> "Tool Budget is our internal term for the number of tools an agent can handle effectively"

- エージェントが効果的に扱えるツール数には認知的限界がある
- 40+ツールは選択肢が多すぎて使いにくい
- ユーザーが適切なツールを見つけにくい

### 問題2: モノリシック・アンチパターン

**MCP公式ベストプラクティスより:**
> "Each MCP server should have one clear, well-defined purpose"
> "❌ Monolithic Anti-Pattern: Mega-Server"

- 単一のMCPサーバーに複数の責任が混在
- GitHub、Notion、Docusaurusは独立した関心事
- 一つの変更が全体に影響する可能性

### 問題3: 設定ファイル衝突リスク

**将来的なシナリオ:**
```
VS Code設定:
{
  "mcpServers": {
    "ateliers-ai-mcpserver": { ... },
    "slack-mcp-server": { ... },        // 他のMCPも使う
    "azure-mcp-server": { ... }
  }
}
```

各MCPサーバーが `githubsettings.local.json` を使っていたら：
- GitHub Tokenが複数箇所に分散
- トークン更新時に複数ファイル修正が必要
- 一貫性の崩壊リスク

---

## MCP公式ベストプラクティス

### 原則1: 単一責任（Single Responsibility）

**公式推奨:**
> "A single Host can connect to multiple Servers simultaneously via different Clients"

```
✅ 推奨パターン:

VS Code (Host)
├── ateliers-github-mcp (7ツール)
├── ateliers-notion-mcp (21ツール)
└── ateliers-docusaurus-mcp (4ツール)

Visual Studio (Host)
├── ateliers-github-mcp (7ツール)
└── ateliers-notion-mcp (21ツール)

Claude Desktop (Host)
├── ateliers-github-mcp (7ツール)
├── ateliers-notion-mcp (21ツール)
└── ateliers-docusaurus-mcp (4ツール)
```

各サーバーは**単一の関心事**に集中、ホストは必要なサーバーを組み合わせる。

### 原則2: ツール数管理

**推奨ツール数:**
- 小規模サーバー: 5-10ツール
- 中規模サーバー: 10-20ツール
- 大規模サーバー: 20-30ツール（要グループ化）

### 原則3: モジュール性

**公式推奨:**
> "New Servers can be added to the ecosystem without requiring changes to existing Hosts"

- 新機能は新サーバーとして追加
- 既存サーバーに影響を与えない
- 独立したバージョン管理

---

## リファクタリング戦略

### Option A: 独立MCPサーバー（推奨）

**構造:**
```
# 各サーバーは完全に独立したリポジトリ・プロジェクト

ateliers-github-mcp/
├── Ateliers.Ai.Mcp.GitHub/
│   ├── Program.cs
│   ├── Services/GitHubService.cs
│   ├── Tools/GitHubTools.cs
│   ├── appsettings.json
│   ├── githubsettings.json
│   └── githubsettings.local.json
├── README.md
└── .gitignore

ateliers-notion-mcp/
├── Ateliers.Ai.Mcp.Notion/
│   ├── Program.cs
│   ├── Services/NotionService.cs
│   ├── Tools/NotionTools.cs
│   ├── appsettings.json
│   ├── notionsettings.json
│   └── notionsettings.local.json
├── README.md
└── .gitignore

ateliers-docusaurus-mcp/
├── Ateliers.Ai.Mcp.Docusaurus/
│   ├── Program.cs
│   ├── Services/DocusaurusService.cs
│   ├── Tools/DocusaurusTools.cs
│   ├── appsettings.json
│   ├── docusaurussettings.json
│   └── docusaurussettings.local.json
├── README.md
└── .gitignore
```

**✅ メリット:**
1. **MCP公式ベストプラクティス準拠**
2. **完全な独立性** - 設定衝突なし
3. **シンプル** - 各サーバーは理解しやすい
4. **Tool Budget自然管理** - ホストが必要なサーバーだけ接続
5. **個別バージョン管理** - GitHub v1.0、Notion v2.0など独立

**❌ デメリット:**
1. コード重複（共通ロジックの再実装）
2. プロジェクト数増加（3リポジトリ）
3. 各サーバーの個別メンテナンス

---

### Option B: 共有ライブラリ + 合成サーバー

**構造:**
```
# モノレポ or マルチリポジトリ

Ateliers.Ai.Mcp.Core/           (共通基盤)
├── ConfigurationService.cs
├── McpServerBase.cs
└── Extensions/

Ateliers.Ai.Mcp.GitHub/         (GitHubライブラリ)
├── Services/GitHubService.cs
├── Tools/GitHubTools.cs
└── GitHubMcpServerExtensions.cs

Ateliers.Ai.Mcp.Notion/         (Notionライブラリ)
├── Services/NotionService.cs
├── Tools/NotionTools.cs
└── NotionMcpServerExtensions.cs

Ateliers.Ai.Mcp.Docusaurus/     (Docusaurusライブラリ)
├── Services/DocusaurusService.cs
├── Tools/DocusaurusTools.cs
└── DocusaurusMcpServerExtensions.cs

# 個別サーバー or 合成サーバー
Ateliers.Ai.Mcp.GitHub.Server/
Ateliers.Ai.Mcp.Notion.Server/
Ateliers.Ai.Mcp.Docusaurus.Server/
Ateliers.Ai.Mcp.Composed/       (オプション: 設定で組み合わせ)
```

**実装例:**
```csharp
// Ateliers.Ai.Mcp.Composed/Program.cs
var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;

// 設定に基づいて必要なサーバーだけ登録
if (config.GetValue<bool>("EnableGitHub"))
    builder.Services.AddGitHubMcpServer();

if (config.GetValue<bool>("EnableNotion"))
    builder.Services.AddNotionMcpServer();

if (config.GetValue<bool>("EnableDocusaurus"))
    builder.Services.AddDocusaurusMcpServer();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
```

**✅ メリット:**
1. **コード再利用** - 共通ロジックを1箇所に
2. **柔軟な組み合わせ** - 設定で制御
3. **NuGetパッケージ化可能** - ライブラリとして配布
4. **各ライブラリは個別テスト可能**

**❌ デメリット:**
1. 実装複雑性が高い
2. 合成サーバーの管理コスト
3. ライブラリ間の依存管理
4. MCP標準的なパターンではない

---

## 推奨アプローチ: Option A（独立MCPサーバー）

### 理由

1. **MCP公式ベストプラクティスに完全準拠**
   - 単一責任原則
   - モジュール性
   - 設定衝突完全回避

2. **シンプルさ**
   - 各サーバーは独立、理解しやすい
   - 新規参加者でも把握しやすい

3. **将来性**
   - 新サービス追加時は新サーバー作成だけ
   - 既存サーバーに影響なし
   - 個別のバージョン管理・リリース可能

4. **実運用での柔軟性**
   - VS CodeではGitHub+Notion
   - VisualStudioではGitHubのみ
   - Claude DesktopではすべてON
   - など、ホストごとに自由に組み合わせ

---

## 移行手順（Phase 10実装時）

### Step 1: 新リポジトリ作成

```bash
# 3つの新リポジトリを作成
cd C:\Projects\OnlineRepos\yuu-git\
mkdir ateliers-github-mcp
mkdir ateliers-notion-mcp
mkdir ateliers-docusaurus-mcp

# 各リポジトリでGit初期化
cd ateliers-github-mcp
git init
# GitHub リモート追加
# 初回コミット
```

### Step 2: GitHubサーバー抽出

**ateliers-github-mcp/Ateliers.Ai.Mcp.GitHub/Program.cs:**
```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

// ログ無効化
builder.Logging.ClearProviders();

// 設定ファイル読み込み
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("githubsettings.json", optional: true)
    .AddJsonFile("githubsettings.local.json", optional: true);

// MCPサーバー登録
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
await app.RunAsync();
```

**抽出するファイル:**
- `Services/GitHubService.cs`
- `Tools/GitTools.cs` → `GitHubTools.cs`
- `Tools/RepositoryTools.cs`
- `githubsettings.json`（既存から分離）

**テスト:**
```bash
dotnet run --project Ateliers.Ai.Mcp.GitHub
```

### Step 3: Notionサーバー抽出

**ateliers-notion-mcp/Ateliers.Ai.Mcp.Notion/Program.cs:**
```csharp
// GitHubサーバーと同様の構造
```

**抽出するファイル:**
- `Services/NotionService.cs`
- `Tools/NotionTools.cs`（Phase 7-8で実装）
- `notionsettings.json`

### Step 4: Docusaurusサーバー抽出

**ateliers-docusaurus-mcp/Ateliers.Ai.Mcp.Docusaurus/Program.cs:**
```csharp
// GitHubサーバーと同様の構造
```

**抽出するファイル:**
- `Services/DocusaurusService.cs`（Phase 9で実装）
- `Tools/AteliersDevTools.cs` → `DocusaurusTools.cs`
- `Tools/DocusaurusPublishingTools.cs`（Phase 9で実装）
- `docusaurussettings.json`

### Step 5: クライアント設定更新

**VS Code (.vscode/mcp.json):**
```json
{
  "mcpServers": {
    "ateliers-github": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Projects/OnlineRepos/yuu-git/ateliers-github-mcp/Ateliers.Ai.Mcp.GitHub/Ateliers.Ai.Mcp.GitHub.csproj"
      ]
    },
    "ateliers-notion": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Projects/OnlineRepos/yuu-git/ateliers-notion-mcp/Ateliers.Ai.Mcp.Notion/Ateliers.Ai.Mcp.Notion.csproj"
      ]
    },
    "ateliers-docusaurus": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Projects/OnlineRepos/yuu-git/ateliers-docusaurus-mcp/Ateliers.Ai.Mcp.Docusaurus/Ateliers.Ai.Mcp.Docusaurus.csproj"
      ]
    }
  }
}
```

**Visual Studio (.mcp.json):**
```json
{
  "servers": {
    "ateliers-github": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Projects/OnlineRepos/yuu-git/ateliers-github-mcp/Ateliers.Ai.Mcp.GitHub/Ateliers.Ai.Mcp.GitHub.csproj"
      ]
    },
    "ateliers-notion": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/Projects/OnlineRepos/yuu-git/ateliers-notion-mcp/Ateliers.Ai.Mcp.Notion/Ateliers.Ai.Mcp.Notion.csproj"
      ]
    }
  }
}
```

**Claude Desktop:**
```json
{
  "mcpServers": {
    "ateliers-github": {
      "command": "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-github-mcp\\Ateliers.Ai.Mcp.GitHub\\bin\\Release\\net10.0\\Ateliers.Ai.Mcp.GitHub.exe",
      "args": []
    },
    "ateliers-notion": {
      "command": "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-notion-mcp\\Ateliers.Ai.Mcp.Notion\\bin\\Release\\net10.0\\Ateliers.Ai.Mcp.Notion.exe",
      "args": []
    },
    "ateliers-docusaurus": {
      "command": "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-docusaurus-mcp\\Ateliers.Ai.Mcp.Docusaurus\\bin\\Release\\net10.0\\Ateliers.Ai.Mcp.Docusaurus.exe",
      "args": []
    }
  }
}
```

### Step 6: 統合テスト

各クライアントで動作確認：
1. ✅ VS Code - 3サーバーすべてツール認識
2. ✅ Visual Studio - 2サーバーツール認識
3. ✅ Claude Desktop - 3サーバーすべてツール認識

### Step 7: 旧リポジトリのアーカイブ

```bash
# ateliers-ai-mcpserver を archived にする
# README.md に移行先を明記
```

**README.md更新:**
```markdown
# ⚠️ This repository is archived

**このリポジトリはアーカイブされました。**

Phase 10のリファクタリングにより、以下の独立したMCPサーバーに分割されました：

- [ateliers-github-mcp](https://github.com/yuu-git/ateliers-github-mcp)
- [ateliers-notion-mcp](https://github.com/yuu-git/ateliers-notion-mcp)
- [ateliers-docusaurus-mcp](https://github.com/yuu-git/ateliers-docusaurus-mcp)

詳細は [Phase 10 Migration Guide](./Docs/refactoring/phase10-migration-guide.md) を参照してください。
```

---

## Phase 7-9で気をつけること

リファクタリングを容易にするため、以下を意識してコード作成：

### 1. ツールの明確な分離

**良い例:**
```csharp
// NotionTools.cs
public static class NotionTools
{
    // Notionツールのみ
}

// DocusaurusTools.cs  
public static class DocusaurusTools
{
    // Docusaurusツールのみ
}
```

**悪い例:**
```csharp
// MixedTools.cs
public static class MixedTools
{
    // NotionとDocusaurusが混在
}
```

### 2. 設定ファイルの分離

**既に実装済み:**
```
githubsettings.json
githubsettings.local.json
notionsettings.json
notionsettings.local.json
docusaurussettings.json (Phase 9で追加)
docusaurussettings.local.json (Phase 9で追加)
```

この方針を継続する。

### 3. Serviceクラスの独立性

**良い例:**
```csharp
// NotionService.cs
public class NotionService
{
    private readonly IConfiguration _configuration;
    
    public NotionService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // Notion関連のみ、外部依存なし
}
```

**悪い例:**
```csharp
// NotionService.cs
public class NotionService
{
    private readonly GitHubService _githubService; // 依存してる！
}
```

### 4. 名前空間の整理

**Phase 7-9での名前空間:**
```csharp
namespace Ateliers.Ai.McpServer.Tools.Notion
{
    // Notionツール
}

namespace Ateliers.Ai.McpServer.Tools.Docusaurus
{
    // Docusaurusツール
}

namespace Ateliers.Ai.McpServer.Services.Notion
{
    // Notionサービス
}
```

**Phase 10での移行:**
```csharp
// ateliers-notion-mcp
namespace Ateliers.Ai.Mcp.Notion.Tools
{
    // Notionツール
}

namespace Ateliers.Ai.Mcp.Notion.Services
{
    // Notionサービス
}
```

名前空間を明確に分けておくと、Phase 10でのコピー＆ペーストが容易。

---

## 共通コードの扱い

Phase 10で独立サーバー化した際、共通ロジックの扱い方：

### Option 1: コード重複を許容（推奨）

**方針:**
- 各サーバーに必要なコードをコピー
- 共通化しない

**理由:**
- 完全な独立性
- 各サーバーが自己完結
- 依存関係なし

**適用範囲:**
- ConfigurationService（設定読み込み）
- エラーハンドリング基盤
- ログ基盤

### Option 2: NuGetパッケージ化

**方針:**
- 共通ロジックを `Ateliers.Ai.Mcp.Core` として NuGet化
- 各サーバーが参照

**理由:**
- コード重複回避
- 共通ロジックの一元管理

**適用範囲:**
- 本当に共通で変更頻度が低いもの
- 例：MCPサーバー基盤クラス

**注意:**
- 過度な共通化は避ける
- 各サーバーの独立性を優先

---

## リファクタリングのタイミング

### トリガー条件（いずれか満たしたらPhase 10開始）

1. **ツール数が40超えた**
   - Phase 9完了時点で43ツール予想
   - これは既に超過

2. **Tool Budget問題が実際に発生**
   - ユーザーが「ツールが多すぎて選べない」
   - エージェントが誤ったツールを頻繁に選択

3. **複数MCPサーバーの組み合わせニーズ**
   - VS CodeでSlack MCPと組み合わせたい
   - 設定ファイル衝突が実際に発生

4. **メンテナンス性の問題**
   - 一つの変更が広範囲に影響
   - テストが困難

### 優先順位

**Phase 9完了後、すぐに Phase 10 を実施することを推奨。**

**理由:**
- 既にツール数は40超え確実
- MCP公式ベストプラクティスに準拠
- 将来の拡張が容易

---

## 各サーバーの想定ツール数（Phase 9完了時）

### ateliers-github-mcp
```
GitTools: 7ツール
- commit_repository
- push_repository
- pull_repository
- create_tag
- push_tag
- create_and_push_tag
- commit_and_push_repository

RepositoryTools: 8ツール
- list_repository_files
- read_repository_file
- add_repository_file
- edit_repository_file
- delete_repository_file
- copy_repository_file
- rename_repository_file
- backup_repository_file

合計: 15ツール
```

### ateliers-notion-mcp
```
NotionBasicTools (Phase 7): 7ツール
- add_task
- update_task
- list_tasks
- complete_task
- add_idea
- search_ideas
- update_idea

NotionExtendedTools (Phase 8): 14ツール
- add_bookmark
- list_bookmarks
- update_bookmark_status
- search_all_notion
- advanced_task_search
- advanced_idea_search
- get_related_items
- list_all_tags
- get_items_by_tag
- suggest_tags
- list_all_categories
- reorganize_categories
- get_database_views
- move_task_in_board

合計: 21ツール
```

### ateliers-docusaurus-mcp
```
AteliersDevTools (Phase 2-3): 3ツール
- list_articles
- read_article
- search_articles

DocusaurusPublishingTools (Phase 9): 4ツール
- create_docusaurus_article
- update_docusaurus_article
- notion_to_docusaurus
- publish_notion_idea_to_docusaurus

合計: 7ツール
```

**総合計: 43ツール → 分割後: 15 + 21 + 7**

---

## 完了基準（Phase 10）

### 必須条件

- ✅ 3つの独立MCPサーバー作成完了
  - ateliers-github-mcp
  - ateliers-notion-mcp
  - ateliers-docusaurus-mcp

- ✅ 各サーバーの動作確認
  - 単体で起動・動作
  - ツールが正しく認識される

- ✅ マルチクライアント統合テスト
  - VS Code: 3サーバー接続
  - Visual Studio: 2サーバー接続
  - Claude Desktop: 3サーバー接続

- ✅ ドキュメント更新
  - 各サーバーのREADME.md
  - セットアップガイド
  - 移行ガイド

- ✅ 旧リポジトリのアーカイブ
  - archived状態にする
  - 移行先を明記

---

## 参考資料

- [MCP Core Architecture](https://modelcontextprotocol.io/docs/concepts/architecture)
- [MCP Best Practices](https://modelcontextprotocol.info/docs/best-practices/)
- [Docker MCP Server Best Practices](https://www.docker.com/blog/mcp-server-best-practices/)
- [15 Best Practices for Building MCP Servers](https://thenewstack.io/15-best-practices-for-building-mcp-servers-in-production/)

---

## 補足：Phase 10を急がない理由

Phase 7-9の実装中は以下を優先：

1. **機能実装に集中**
   - Notion統合（Phase 7-8）
   - Docusaurus統合（Phase 9）
   - まずは動かす

2. **Phase 9完了後に判断**
   - 実際のツール数を確認
   - 実運用での問題を確認
   - その上でリファクタリング実施

3. **段階的な移行**
   - 一度にすべて分割しない
   - まずGitHubサーバーを分離
   - 動作確認後、Notion、Docusaurusと順次分離

**焦らず、確実に。**

---

**作成日:** 2024-11-29  
**次回更新:** Phase 9完了時（リファクタリング直前）
