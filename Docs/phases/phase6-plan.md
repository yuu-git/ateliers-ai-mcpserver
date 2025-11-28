# Phase 6 Plan: MCP Multi-Client Integration

**Phase:** 6  
**開始予定:** 2025-11-28  
**目標:** ateliers-ai-mcpserver を複数のIDEから利用可能にする

---

## Phase 6 の目標

### 🎯 ビジョン

MCPの真価 = **「入力源に依存しない機能提供」**

```
Claude Desktop  ┐
VS Code Copilot ├─→ ateliers-ai-mcpserver ─→ 同じ機能・同じツール
VS Copilot      ┘
```

どのIDEを使っていても、同じMCPサーバーから同じツールにアクセスできる環境を構築する。

### 📦 達成目標

1. **VS Code統合完了**
   - `.vscode/mcp.json` サンプル作成
   - Agent Modeから全ツール利用可能
   - セットアップガイド完備

2. **Visual Studio統合完了**
   - `.mcp.json` サンプル作成
   - CodeLens統合確認
   - Agent Modeから全ツール利用可能
   - セットアップガイド完備

3. **ドキュメント整備完了**
   - 各IDE用セットアップガイド
   - トラブルシューティングガイド
   - README更新（マルチクライアント対応の説明）

---

## 実装計画

### Step 1: VS Code統合

#### 1.1 設定ファイル作成

**ファイル:** `.vscode/mcp.json.sample`

```json
{
  "servers": {
    "ateliers-ai-mcpserver": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\Ateliers.Ai.McpServer.csproj"
      ],
      "env": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
  }
}
```

**変更点:**
- `command`: プロジェクトパスをユーザー環境に合わせて変更
- `env`: 環境変数設定（必要に応じて）

#### 1.2 セットアップガイド作成

**ファイル:** `Docs/setup/vscode.md`

**内容:**
- 前提条件（VS Code 1.102+, GitHub Copilot拡張機能）
- インストール手順
- `.vscode/mcp.json` 設定方法
- Agent Mode有効化手順
- ツール確認方法
- トラブルシューティング

#### 1.3 動作テスト

**テスト項目:**
- [x] Agent Mode起動確認
- [x] ツール一覧表示確認
- [x] GitTools動作確認（commit_and_push_repository）
- [x] RepositoryTools動作確認（read_repository_file）
- [x] AteliersDevTools動作確認（read_article）
- [ ] GitHubTools動作確認（list_repositories）

---

### Step 2: Visual Studio統合

#### 2.1 設定ファイル作成

**ファイル:** `.mcp.json.sample`

```json
{
  "servers": {
    "ateliers-ai-mcpserver": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Projects\\OnlineRepos\\yuu-git\\ateliers-ai-mcpserver\\Ateliers.Ai.McpServer\\Ateliers.Ai.McpServer.csproj"
      ]
    }
  }
}
```

**配置場所:**
- `[SOLUTIONDIR]\.vs\mcp.json` - Visual Studio専用（ソリューション固有）
- `[SOLUTIONDIR]\.mcp.json` - ソース管理対象
- `[SOLUTIONDIR]\.vscode\mcp.json` - VS Codeと共有
- `%USERPROFILE%\.mcp.json` - グローバル設定

#### 2.2 セットアップガイド作成

**ファイル:** `Docs/setup/visual-studio.md`

**内容:**
- 前提条件（Visual Studio 2022 17.14+, GitHub Copilot）
- インストール手順
- `.mcp.json` 設定方法
- CodeLens操作方法
- Agent Mode有効化手順
- ツール確認方法
- トラブルシューティング

#### 2.3 動作テスト

**テスト項目:**
- [ ] CodeLens表示確認
- [ ] Agent Mode起動確認
- [ ] ツール一覧表示確認
- [ ] GitTools動作確認
- [ ] RepositoryTools動作確認
- [ ] AteliersDevTools動作確認
- [ ] GitHubTools動作確認

---

### Step 3: Claude Desktop対応確認

#### 3.1 既存設定の検証

**ファイル:** `claude_desktop_config.json` (参考用)

**確認事項:**
- 現在の設定が正しく動作しているか
- 他IDE設定との整合性

#### 3.2 セットアップガイド作成

**ファイル:** `Docs/setup/claude-desktop.md`

**内容:**
- 前提条件
- インストール手順
- `claude_desktop_config.json` 設定方法
- ツール確認方法
- トラブルシューティング

---

### Step 4: ドキュメント整備

#### 4.1 README.md更新

**追加内容:**
- マルチクライアント対応の説明
- 対応IDE一覧（Claude Desktop, VS Code, Visual Studio）
- クイックスタートガイドへのリンク
- セットアップガイドへのリンク

#### 4.2 トラブルシューティングガイド作成

**ファイル:** `Docs/troubleshooting.md`

**内容:**
- よくある問題と解決方法
- IDE別の問題
- 認証問題
- ネットワーク問題
- ログ確認方法
- 問題報告方法

#### 4.3 Docsフォルダー構造更新

```
Docs/
├── README.md
├── setup/                          # 新規追加
│   ├── claude-desktop.md
│   ├── vscode.md
│   └── visual-studio.md
├── troubleshooting.md              # 新規追加
├── phases/
│   ├── phase5-handover.md
│   ├── phase6-plan.md              # このファイル
│   ├── phase7-plan.md              # 新規: Notion基礎統合
│   ├── phase8-plan.md              # 新規: Notion拡張
│   └── phase9-plan.md              # 新規: Docusaurus統合
├── decisions/
│   └── 2025-11-28-git-tools-design.md
└── deferred-features.md
```

---

## 技術要件

### VS Code

**必須:**
- VS Code 1.102以降
- GitHub Copilot拡張機能
- .NET 8 SDK

**Transport:**
- stdio (ローカル実行)

**設定ファイル:**
- `.vscode/mcp.json`

### Visual Studio

**必須:**
- Visual Studio 2022 17.14以降
- GitHub Copilot拡張機能
- .NET 8 SDK

**Transport:**
- stdio (ローカル実行)

**設定ファイル:**
- `.mcp.json` (複数の配置場所をサポート)

### Claude Desktop

**必須:**
- Claude Desktop最新版
- .NET 8 SDK

**Transport:**
- stdio (ローカル実行)

**設定ファイル:**
- `claude_desktop_config.json`

---

## 期待される成果

### 1. ユーザー体験の向上

**Before Phase 6:**
```
Claude Desktopだけで使える
↓
IDEを切り替えると機能が使えない
```

**After Phase 6:**
```
どのIDEでも同じツールが使える
↓
シームレスな開発体験
```

### 2. MCPの真価を実証

- 入力源に依存しない機能提供
- プラットフォーム非依存のアーキテクチャ
- 統一されたツール体験

### 3. 将来の拡張性

Phase 6完了後、以下が可能に：
- 新しいIDEの追加が容易
- ツール追加がすべてのクライアントに反映
- 設定管理の一元化

---

## リスクと対策

### リスク1: IDE間の挙動差異

**対策:**
- 各IDEで十分なテストを実施
- IDE別のトラブルシューティング作成
- 問題発見時は設定調整で対応

### リスク2: 設定の複雑化

**対策:**
- サンプル設定ファイルを提供
- セットアップガイドを詳細に記述
- よくある問題をドキュメント化

### リスク3: パフォーマンス問題

**対策:**
- stdio transportは軽量で高速
- 必要に応じてログ出力削減
- .NET 8のパフォーマンス最適化活用

---

## Phase 7 への準備

Phase 6完了後、Phase 7（Notion基礎統合）で以下が可能に：

1. **どのIDEからでもタスク・アイデア管理**
   ```
   VS Code → ateliers-ai-mcpserver → Notion Tasks追加
   Visual Studio → ateliers-ai-mcpserver → Notion Ideas追加
   Claude Desktop → ateliers-ai-mcpserver → Notion参照・更新
   ```

2. **思考のバッファとしてのNotion**
   ```
   1. アイデア発生（任意のIDE）
   2. Notionに即座にメモ（MCP経由）
   3. Gitコミット不要な軽量管理
   4. 後で整理・記事化
   ```

3. **Phase 8以降への基盤**
   - Notion拡張機能（Bookmarks、検索強化）
   - Docusaurus統合（Notion→記事変換フロー）
   - v1.0.0 リリース準備

---

## 完了基準

### 必須条件
- [x] VS Code統合完了（設定ファイル・ドキュメント）
- [ ] Visual Studio統合完了
- [ ] Claude Desktop動作確認
- [ ] 全ツール動作確認（3つのIDE）
- [x] セットアップガイド（VS Code）
- [ ] セットアップガイド（Visual Studio, Claude Desktop）
- [ ] トラブルシューティングガイド
- [ ] README更新

### テスト項目
- [x] GitTools（7ツール）× VS Code = 7テスト
- [x] RepositoryTools（6ツール）× VS Code = 6テスト
- [x] AteliersDevTools（3ツール）× VS Code = 3テスト
- [ ] GitHubTools（3ツール）× VS Code = 3テスト
- [ ] 全ツール × Visual Studio
- [ ] 全ツール × Claude Desktop

### ドキュメント
- [ ] `Docs/setup/claude-desktop.md`
- [x] `Docs/setup/vscode.md`
- [ ] `Docs/setup/visual-studio.md`
- [ ] `Docs/troubleshooting.md`
- [ ] `README.md` 更新
- [x] `.vscode/mcp.json.sample`
- [ ] `.mcp.json.sample`

---

## リリース計画

### Phase 6 完了後

**タグ:** v0.6.0

**リリースノート内容:**
```markdown
# v0.6.0: MCP Multi-Client Integration

## New Features
- VS Code integration with Agent Mode
- Visual Studio integration with CodeLens
- Multi-client support (Claude Desktop, VS Code, Visual Studio)
- Comprehensive setup guides for each IDE

## Documentation
- Setup guides for Claude Desktop, VS Code, Visual Studio
- Troubleshooting guide
- Updated README with multi-client architecture

## Breaking Changes
None

## Migration Guide
See Docs/setup/ for client-specific setup instructions
```

---

## 次のステップ（Phase 7予告）

Phase 6完了後、Phase 7（Notion基礎統合）で実装予定：

### 1. Notion API接続基盤
- 認証システム構築（Personal Access Token / OAuth対応）
- appsettings.json設定
- Notion API C# クライアント実装

### 2. Tasks管理（CRUD操作）
- `add_task` - タスク追加
- `update_task` - タスク更新
- `list_tasks` - タスク一覧取得（フィルタ対応）
- `complete_task` - タスク完了

### 3. Ideas管理（CRUD操作）
- `add_idea` - アイデア追加
- `search_ideas` - アイデア検索
- `update_idea` - アイデア更新

### 4. マルチクライアント対応
- VS Code, Visual Studio, Claude Desktop から同じNotionツール利用
- コミット不要な軽量情報管理
- 「思考のバッファ」として機能

**Phase 8以降への展望:**
- Phase 8: Notion拡張（Bookmarks、検索強化、タグ管理）
- Phase 9: Docusaurus統合（Notion→記事変換フロー）
- v1.0.0: 完全な個人ナレッジ管理システム

---

## 参考資料

- [VS Code MCP Documentation](https://code.visualstudio.com/docs/copilot/customization/mcp-servers)
- [Visual Studio MCP Documentation](https://learn.microsoft.com/en-us/visualstudio/ide/mcp-servers)
- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [MCP C# SDK](https://devblogs.microsoft.com/dotnet/build-a-model-context-protocol-mcp-server-in-csharp/)
