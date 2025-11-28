# Phase 7 Plan: Notion Basic Integration

**Phase:** 7  
**開始予定:** Phase 6完了後（2025-12月予定）  
**目標:** Notionを「思考のバッファ」として活用できるMCP基盤を構築

---

## Phase 7 の目標

### 🎯 ビジョン

**Notionを「思考のバッファ」にする**

```
アイデア発生 → Notionに即座にメモ（MCP経由）→ 後で整理・記事化
タスク発生  → Notionに追加（コミット不要）→ 進捗管理
```

「Git + Docusaurus」は重厚な成果物置き場。
「Notion」は軽量で即座に書ける思考のバッファ。

この役割分担を、MCPを通じて全IDE・全AIクライアントから実現する。

### 📦 達成目標

1. **Notion API接続基盤構築**
   - 認証システム（Personal Access Token）
   - appsettings.json設定
   - C# Notion API クライアント実装

2. **Tasks管理実装（CRUD）**
   - タスク追加・更新・完了
   - 一覧取得・フィルタ機能

3. **Ideas管理実装（CRUD）**
   - アイデア追加・検索・更新
   - タグ・カテゴリ管理

4. **マルチクライアント対応**
   - VS Code, Visual Studio, Claude Desktop から利用可能
   - Phase 6の基盤を活用

---

## 実装計画

### Step 1: Notion API接続基盤

#### 1.1 NuGetパッケージ追加

**必要なパッケージ:**
```xml
<PackageReference Include="Notion.Client" Version="6.x.x" />
```

または、直接 Notion REST API を HttpClient で実装。

#### 1.2 appsettings.json 設定

```json
{
  "Notion": {
    "ApiToken": "",
    "WorkspaceId": "",
    "Databases": {
      "Tasks": "",
      "Ideas": "",
      "Bookmarks": ""
    }
  }
}
```

**appsettings.Development.json** (gitignore対象):
```json
{
  "Notion": {
    "ApiToken": "secret_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "WorkspaceId": "workspace-id-here",
    "Databases": {
      "Tasks": "database-id-for-tasks",
      "Ideas": "database-id-for-ideas",
      "Bookmarks": "database-id-for-bookmarks"
    }
  }
}
```

#### 1.3 NotionService 実装

**クラス構成:**
```
Services/
├── NotionService.cs           # メインサービス
├── Notion/
│   ├── NotionTasksService.cs  # Tasks専用
│   ├── NotionIdeasService.cs  # Ideas専用
│   └── NotionClient.cs        # API通信基盤
```

**認証処理:**
- Personal Access Token をヘッダーに設定
- `Notion-Version: 2022-06-28` ヘッダー必須

---

### Step 2: Tasks管理実装

#### 2.1 Notionデータベース設計

**Tasks Database プロパティ:**
```
- Title (タイトル): Title型
- Status (ステータス): Select型 ("未着手" / "進行中" / "完了" / "保留")
- Priority (優先度): Select型 ("高" / "中" / "低")
- Due Date (期限): Date型
- Tags (タグ): Multi-select型
- Created By (作成元): Select型 ("Claude" / "ChatGPT" / "Copilot" / "手動")
- Description (詳細): Rich text型
- Created (作成日): Created time型
- Last Edited (最終更新): Last edited time型
```

#### 2.2 MCPツール実装

**add_task**
```csharp
[McpTool(
    Name = "add_task",
    Description = "Notionにタスクを追加します。"
)]
public async Task<string> AddTask(
    string title,
    string? description = null,
    string? status = "未着手",
    string? priority = "中",
    DateTime? dueDate = null,
    string[]? tags = null,
    string? createdBy = null
)
```

**update_task**
```csharp
[McpTool(
    Name = "update_task",
    Description = "Notionのタスクを更新します。"
)]
public async Task<string> UpdateTask(
    string taskId,
    string? title = null,
    string? description = null,
    string? status = null,
    string? priority = null,
    DateTime? dueDate = null,
    string[]? tags = null
)
```

**list_tasks**
```csharp
[McpTool(
    Name = "list_tasks",
    Description = "Notionのタスク一覧を取得します。フィルタ可能。"
)]
public async Task<string> ListTasks(
    string? status = null,
    string? priority = null,
    bool? dueSoon = null,  // 期限が近いものだけ
    int? limit = 10
)
```

**complete_task**
```csharp
[McpTool(
    Name = "complete_task",
    Description = "Notionのタスクを完了にします。"
)]
public async Task<string> CompleteTask(string taskId)
```

---

### Step 3: Ideas管理実装

#### 3.1 Notionデータベース設計

**Ideas Database プロパティ:**
```
- Title (タイトル): Title型
- Category (カテゴリ): Select型 ("技術" / "ビジネス" / "個人" / "その他")
- Tags (タグ): Multi-select型
- Status (ステータス): Select型 ("アイデア" / "検討中" / "実装予定" / "完了" / "却下")
- Content (内容): Rich text型
- Related Links (関連リンク): URL型
- Created By (作成元): Select型 ("Claude" / "ChatGPT" / "Copilot" / "手動")
- Created (作成日): Created time型
- Last Edited (最終更新): Last edited time型
```

#### 3.2 MCPツール実装

**add_idea**
```csharp
[McpTool(
    Name = "add_idea",
    Description = "Notionにアイデアを追加します。"
)]
public async Task<string> AddIdea(
    string title,
    string? content = null,
    string? category = null,
    string[]? tags = null,
    string? relatedLink = null,
    string? createdBy = null
)
```

**search_ideas**
```csharp
[McpTool(
    Name = "search_ideas",
    Description = "Notionのアイデアを検索します。"
)]
public async Task<string> SearchIdeas(
    string? keyword = null,
    string? category = null,
    string[]? tags = null,
    int? limit = 10
)
```

**update_idea**
```csharp
[McpTool(
    Name = "update_idea",
    Description = "Notionのアイデアを更新します。"
)]
public async Task<string> UpdateIdea(
    string ideaId,
    string? title = null,
    string? content = null,
    string? category = null,
    string[]? tags = null,
    string? status = null
)
```

---

### Step 4: マルチクライアント統合テスト

#### 4.1 VS Code テスト

**Agent Mode で実行:**
```
"Phase 6作業中のアイデアをNotionにメモして"
→ add_idea 実行確認

"未完了タスクを一覧表示して"
→ list_tasks 実行確認
```

#### 4.2 Visual Studio テスト

**CodeLens経由で実行:**
- add_task 実行
- list_tasks 実行

#### 4.3 Claude Desktop テスト

**対話形式で実行:**
```
"Notion Tasksに「Phase 7完了テスト」というタスクを追加して"
"Notion Ideasから「技術」カテゴリのアイデアを検索して"
```

---

## 技術要件

### Notion API

**バージョン:**
- Notion API Version: 2022-06-28

**認証:**
- Personal Access Token

**レート制限:**
- 3 requests per second per integration

**必要な権限:**
- Read content
- Update content
- Insert content

### データベース準備

Phase 7開始前に、Notion側で以下を準備：

1. **Tasksデータベース作成**
   - プロパティ設定
   - データベースIDを取得

2. **Ideasデータベース作成**
   - プロパティ設定
   - データベースIDを取得

3. **Integrationの作成**
   - Internal Integration作成
   - データベースへのアクセス許可
   - Integration Tokenを取得

---

## 期待される成果

### 1. 思考のバッファとしての活用

**Before Phase 7:**
```
アイデア発生
↓
どこにメモする？
- Gitコミット？重い
- ローカルテキスト？散らかる
- 忘れる
```

**After Phase 7:**
```
アイデア発生
↓
Claudeに「Notionにメモして」
↓
即座にNotion Ideasに追加
↓
後で整理・記事化
```

### 2. タスク管理の効率化

```
VS Codeでコード書きながら
↓
「あとで○○する」と気づく
↓
Copilotに「Notionタスクに追加」
↓
手を止めずにタスク管理
```

### 3. Phase 8・9への準備

- Phase 7でNotionにデータが蓄積される
- Phase 8でBookmarks・検索機能を拡張
- Phase 9でNotion→Docusaurus変換フローを構築

---

## リスクと対策

### リスク1: Notion API レート制限

**対策:**
- リクエストを3 requests/secに制限
- バッチ処理を実装
- エラーハンドリング強化

### リスク2: データベース設計の変更

**対策:**
- プロパティ名をappsettings.jsonで設定可能に
- 柔軟なマッピング機能
- スキーマ変更に対応しやすい実装

### リスク3: 認証情報の管理

**対策:**
- appsettings.Development.jsonをgitignore
- 環境変数での設定もサポート
- ドキュメントで安全な管理方法を説明

---

## Phase 8 への準備

Phase 7完了後、Phase 8（Notion拡張）で以下を実装予定：

### 1. Bookmarks管理
- `add_bookmark` - URL保存（あとで読む）
- `list_bookmarks` - ブックマーク一覧

### 2. 検索機能強化
- 全文検索
- 複合フィルタ
- 関連アイテム取得

### 3. タグ・カテゴリ管理
- タグ一覧取得
- カテゴリ管理
- 階層的な整理

### 4. Notionビュー連携
- カンバンビュー操作
- カレンダービュー操作
- リスト/テーブルビュー切り替え

---

## 完了基準

### 必須条件
- ✅ Notion API接続基盤完成
- ✅ Tasks管理（CRUD）実装
- ✅ Ideas管理（CRUD）実装
- ✅ 3つのIDE（VS Code, Visual Studio, Claude Desktop）で動作確認
- ✅ appsettings.json設定ガイド作成
- ✅ NotionセットアップガイドDocs/setup/notion.md作成

### テスト項目
- ✅ add_task × 3 IDE = 3テスト
- ✅ update_task × 3 IDE = 3テスト
- ✅ list_tasks × 3 IDE = 3テスト
- ✅ complete_task × 3 IDE = 3テスト
- ✅ add_idea × 3 IDE = 3テスト
- ✅ search_ideas × 3 IDE = 3テスト
- ✅ update_idea × 3 IDE = 3テスト
- 合計: 21テスト

### ドキュメント
- ✅ `Docs/setup/notion.md` - Notion統合セットアップガイド
- ✅ `Docs/phases/phase7-plan.md` - このファイル
- ✅ README更新（Notion統合の説明追加）

---

## リリース計画

### Phase 7 完了後

**タグ:** v0.7.0

**リリースノート内容:**
```markdown
# v0.7.0: Notion Basic Integration

## New Features
- Notion API connection and authentication
- Tasks management (add, update, list, complete)
- Ideas management (add, search, update)
- Multi-client support for Notion tools

## Documentation
- Notion setup guide
- appsettings.json configuration guide
- Updated README with Notion integration

## Breaking Changes
None

## Migration Guide
See Docs/setup/notion.md for setup instructions
```

---

## 参考資料

- [Notion API Documentation](https://developers.notion.com/)
- [Notion.NET Client](https://github.com/notion-dotnet/notion-sdk-net)
- [Notion API Rate Limits](https://developers.notion.com/reference/request-limits)
- [Notion Database Properties](https://developers.notion.com/reference/property-object)
