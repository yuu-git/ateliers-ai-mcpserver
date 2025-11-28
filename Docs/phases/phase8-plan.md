# Phase 8 Plan: Notion Extended Features

**Phase:** 8  
**開始予定:** Phase 7完了後（2026年1月予定）  
**目標:** Notion機能を拡張し、より高度な情報管理を実現

---

## Phase 8 の目標

### 🎯 ビジョン

**Phase 7で構築した基盤を拡張**

```
Phase 7: Tasks + Ideas（基礎）
    ↓
Phase 8: Bookmarks + 検索強化 + タグ管理（拡張）
    ↓
Phase 9: Docusaurus統合（連携）
```

Phase 7で「思考のバッファ」ができた。
Phase 8で「情報の整理・検索」を強化する。

### 📦 達成目標

1. **Bookmarks管理実装**
   - 「あとで読む」リスト管理
   - URL保存・分類

2. **検索機能強化**
   - 全文検索
   - 複合フィルタ
   - 関連アイテム取得

3. **タグ・カテゴリ管理**
   - タグ一覧・管理
   - カテゴリ階層化
   - 横断検索

4. **Notionビュー操作**
   - カンバン・カレンダー対応
   - ビュー切り替え

---

## 実装計画

### Step 1: Bookmarks管理

#### 1.1 Notionデータベース設計

**Bookmarks Database プロパティ:**
```
- Title (タイトル): Title型
- URL: URL型
- Description (説明): Rich text型
- Category (カテゴリ): Select型 ("技術記事" / "ニュース" / "リファレンス" / "その他")
- Tags (タグ): Multi-select型
- Status (ステータス): Select型 ("未読" / "読了" / "参考用")
- Added By (追加元): Select型 ("Claude" / "ChatGPT" / "Copilot" / "手動")
- Priority (優先度): Select型 ("高" / "中" / "低")
- Created (作成日): Created time型
```

#### 1.2 MCPツール実装

**add_bookmark**
```csharp
[McpTool(
    Name = "add_bookmark",
    Description = "Notionに「あとで読む」ブックマークを追加します。"
)]
public async Task<string> AddBookmark(
    string url,
    string? title = null,
    string? description = null,
    string? category = null,
    string[]? tags = null,
    string? priority = "中",
    string? addedBy = null
)
```

**list_bookmarks**
```csharp
[McpTool(
    Name = "list_bookmarks",
    Description = "Notionのブックマーク一覧を取得します。"
)]
public async Task<string> ListBookmarks(
    string? status = null,
    string? category = null,
    string? priority = null,
    int? limit = 20
)
```

**update_bookmark_status**
```csharp
[McpTool(
    Name = "update_bookmark_status",
    Description = "ブックマークのステータスを更新します（未読→読了など）。"
)]
public async Task<string> UpdateBookmarkStatus(
    string bookmarkId,
    string status
)
```

---

### Step 2: 検索機能強化

#### 2.1 全文検索実装

**search_all_notion**
```csharp
[McpTool(
    Name = "search_all_notion",
    Description = "Notion全体（Tasks, Ideas, Bookmarks）を横断検索します。"
)]
public async Task<string> SearchAllNotion(
    string keyword,
    string[]? databases = null,  // 検索対象DB指定
    int? limit = 20
)
```

#### 2.2 複合フィルタ検索

**advanced_task_search**
```csharp
[McpTool(
    Name = "advanced_task_search",
    Description = "複数条件でタスクを検索します。"
)]
public async Task<string> AdvancedTaskSearch(
    string? status = null,
    string? priority = null,
    DateTime? dueDateFrom = null,
    DateTime? dueDateTo = null,
    string[]? tags = null,
    string? createdBy = null,
    int? limit = 20
)
```

**advanced_idea_search**
```csharp
[McpTool(
    Name = "advanced_idea_search",
    Description = "複数条件でアイデアを検索します。"
)]
public async Task<string> AdvancedIdeaSearch(
    string? keyword = null,
    string? category = null,
    string[]? tags = null,
    string? status = null,
    DateTime? createdFrom = null,
    DateTime? createdTo = null,
    int? limit = 20
)
```

#### 2.3 関連アイテム取得

**get_related_items**
```csharp
[McpTool(
    Name = "get_related_items",
    Description = "指定したアイテムに関連するTasks, Ideas, Bookmarksを取得します。"
)]
public async Task<string> GetRelatedItems(
    string itemId,
    string itemType,  // "task" / "idea" / "bookmark"
    string[]? relationTypes = null  // "tag" / "category" / "keyword"
)
```

---

### Step 3: タグ・カテゴリ管理

#### 3.1 タグ管理ツール

**list_all_tags**
```csharp
[McpTool(
    Name = "list_all_tags",
    Description = "Notion全体で使用されているタグ一覧を取得します。"
)]
public async Task<string> ListAllTags(
    string? database = null  // 特定DBのタグのみ
)
```

**get_items_by_tag**
```csharp
[McpTool(
    Name = "get_items_by_tag",
    Description = "指定したタグが付いたすべてのアイテムを取得します。"
)]
public async Task<string> GetItemsByTag(
    string tag,
    string[]? databases = null
)
```

**suggest_tags**
```csharp
[McpTool(
    Name = "suggest_tags",
    Description = "内容からタグを提案します（AI支援）。"
)]
public async Task<string> SuggestTags(
    string content,
    int? maxSuggestions = 5
)
```

#### 3.2 カテゴリ管理ツール

**list_all_categories**
```csharp
[McpTool(
    Name = "list_all_categories",
    Description = "使用されているカテゴリ一覧を取得します。"
)]
public async Task<string> ListAllCategories(
    string database  // "tasks" / "ideas" / "bookmarks"
)
```

**reorganize_categories**
```csharp
[McpTool(
    Name = "reorganize_categories",
    Description = "カテゴリの統合・整理を行います。"
)]
public async Task<string> ReorganizeCategories(
    string database,
    string oldCategory,
    string newCategory
)
```

---

### Step 4: Notionビュー操作

#### 4.1 ビュー取得・切り替え

**get_database_views**
```csharp
[McpTool(
    Name = "get_database_views",
    Description = "データベースの利用可能なビュー一覧を取得します。"
)]
public async Task<string> GetDatabaseViews(
    string database
)
```

**switch_view**
```csharp
[McpTool(
    Name = "switch_view",
    Description = "データベースのビューを切り替えます。"
)]
public async Task<string> SwitchView(
    string database,
    string viewType  // "table" / "board" / "calendar" / "list"
)
```

#### 4.2 カンバンビュー操作

**move_task_in_board**
```csharp
[McpTool(
    Name = "move_task_in_board",
    Description = "カンバンボードでタスクを移動します（ステータス変更）。"
)]
public async Task<string> MoveTaskInBoard(
    string taskId,
    string toStatus
)
```

#### 4.3 カレンダービュー操作

**get_calendar_events**
```csharp
[McpTool(
    Name = "get_calendar_events",
    Description = "期限付きタスクをカレンダー形式で取得します。"
)]
public async Task<string> GetCalendarEvents(
    DateTime? from = null,
    DateTime? to = null
)
```

---

## 技術要件

### Notion API拡張機能

**Search API:**
- POST /v1/search
- 全文検索サポート

**Filter & Sort:**
- 複合フィルタ実装
- ソート条件の組み合わせ

**Database Query:**
- POST /v1/databases/{database_id}/query
- 高度なフィルタ構文

---

## 期待される成果

### 1. 情報の整理・検索効率化

**Before Phase 8:**
```
「あの記事どこだっけ？」
→ 手動でNotion検索
→ タグが散らかっている
```

**After Phase 8:**
```
Claudeに「Rust関連のブックマーク一覧出して」
→ 即座に関連情報取得
→ タグで横断検索可能
```

### 2. ブックマーク管理の統合

```
技術記事を見つける
↓
「Notionにあとで読むとして追加」
↓
優先度・カテゴリ自動分類
↓
読了後、関連タスク・アイデアと連携
```

### 3. Phase 9への準備

- Notionに蓄積された情報を整理
- タグ・カテゴリで構造化
- Docusaurus記事化の下準備完了

---

## リスクと対策

### リスク1: 検索パフォーマンス

**対策:**
- キャッシュ機構実装
- ページネーション対応
- レート制限内での最適化

### リスク2: タグの散乱

**対策:**
- タグ提案機能でガイド
- 定期的な統合・整理支援
- 使用頻度の可視化

### リスク3: ビュー操作の複雑化

**対策:**
- よく使うビューのショートカット
- ビュー切り替えのプリセット
- ドキュメント充実

---

## Phase 9 への準備

Phase 8完了後、Phase 9（Docusaurus統合）で以下が可能に：

### 1. Notion→Docusaurus変換フロー
```
Notion Ideas（draft状態）
    ↓
タグ・カテゴリで整理済み
    ↓
Docusaurus記事として生成
    ↓
フロントマター自動設定
```

### 2. 情報の流れ
```
アイデア → Notion（Phase 7）
    ↓
整理・検索 → Notion拡張（Phase 8）
    ↓
記事化 → Docusaurus（Phase 9）
    ↓
公開 → ateliers.dev
```

---

## 完了基準

### 必須条件
- ✅ Bookmarks管理実装
- ✅ 検索機能強化（全文検索・複合フィルタ）
- ✅ タグ・カテゴリ管理実装
- ✅ Notionビュー操作実装
- ✅ 3つのIDE（VS Code, Visual Studio, Claude Desktop）で動作確認
- ✅ ドキュメント更新

### テスト項目
- ✅ Bookmarks（3ツール）× 3 IDE = 9テスト
- ✅ 検索機能（5ツール）× 3 IDE = 15テスト
- ✅ タグ管理（3ツール）× 3 IDE = 9テスト
- ✅ ビュー操作（3ツール）× 3 IDE = 9テスト
- 合計: 42テスト

### ドキュメント
- ✅ `Docs/setup/notion.md` 更新（拡張機能追加）
- ✅ `Docs/phases/phase8-plan.md` - このファイル
- ✅ README更新（Notion拡張機能の説明追加）

---

## リリース計画

### Phase 8 完了後

**タグ:** v0.8.0

**リリースノート内容:**
```markdown
# v0.8.0: Notion Extended Features

## New Features
- Bookmarks management ("Read Later" list)
- Advanced search (full-text, multi-filter, related items)
- Tag and category management
- Notion view operations (board, calendar, list)

## Improvements
- Enhanced search performance
- Better organization tools
- Preparation for Docusaurus integration

## Documentation
- Updated Notion setup guide
- Advanced search examples
- Tag management best practices

## Breaking Changes
None
```

---

## 参考資料

- [Notion API Search](https://developers.notion.com/reference/post-search)
- [Notion API Query Database](https://developers.notion.com/reference/post-database-query)
- [Notion API Filters](https://developers.notion.com/reference/post-database-query-filter)
- [Notion Views](https://www.notion.so/help/views)
