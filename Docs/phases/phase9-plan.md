# Phase 9 Plan: Docusaurus Integration

**Phase:** 9  
**開始予定:** Phase 8完了後（2026年2月予定）  
**目標:** Notion→Docusaurus の記事変換フローを構築し、ナレッジベース自動化を完成

---

## Phase 9 の目標

### 🎯 ビジョン

**完全なナレッジ管理ワークフローの実現**

```
思考 → Notion（Phase 7-8）→ 整理 → Docusaurus（Phase 9）→ 公開
```

Phase 7-8で構築した「思考のバッファ」を、
Phase 9で「公式ドキュメント・ブログ」として自動的に記事化する。

**ゴール:**
- Notionの「draft」アイデアからDocusaurus記事を生成
- フロントマター自動設定
- 公開後、Notionのステータス更新
- マルチクライアント対応（VS Code, Visual Studio, Claude Desktop）

### 📦 達成目標

1. **Docusaurus記事作成ツール実装**
   - 記事作成・更新・削除
   - フロントマター自動生成

2. **Notion→Docusaurus変換フロー**
   - Notion IdeasからDocusaurus記事生成
   - カテゴリ・タグの自動マッピング

3. **公開ワークフロー**
   - 記事公開後のNotion更新
   - Git操作との連携

4. **v1.0.0 リリース準備**
   - 全機能統合完了
   - ドキュメント完備

---

## 実装計画

### Step 1: Docusaurusファイル構造管理

#### 1.1 Docusaurusプロジェクト構造

**対象リポジトリ:** AteliersDev

```
AteliersDev/
├── docs/                    # 技術ドキュメント
│   ├── csharp/
│   ├── github-guidelines/
│   └── ...
├── blog/                    # ブログ記事
│   └── YYYY-MM-DD-title.md
└── docusaurus.config.js
```

#### 1.2 フロントマター仕様

**Docs記事:**
```yaml
---
id: article-id
title: Article Title
sidebar_label: Short Title
sidebar_position: 1
description: Article description
tags:
  - tag1
  - tag2
---
```

**Blog記事:**
```yaml
---
slug: article-slug
title: Blog Post Title
authors:
  - name: Konno Yuu
    title: Software Engineer
    url: https://github.com/yuu-git
    image_url: https://github.com/yuu-git.png
tags: [tag1, tag2]
date: 2025-12-01
---
```

---

### Step 2: Docusaurus記事作成ツール

#### 2.1 MCPツール実装

**create_docusaurus_article**
```csharp
[McpTool(
    Name = "create_docusaurus_article",
    Description = "Docusaurus記事を作成します。"
)]
public async Task<string> CreateDocusaurusArticle(
    string title,
    string content,
    string type,  // "docs" or "blog"
    string? category = null,  // docs: サブディレクトリ, blog: null
    string[]? tags = null,
    string? description = null,
    int? sidebarPosition = null,  // docs only
    DateTime? date = null  // blog only
)
```

**update_docusaurus_article**
```csharp
[McpTool(
    Name = "update_docusaurus_article",
    Description = "Docusaurus記事を更新します。"
)]
public async Task<string> UpdateDocusaurusArticle(
    string filePath,
    string? title = null,
    string? content = null,
    string[]? tags = null,
    string? description = null
)
```

**delete_docusaurus_article**
```csharp
[McpTool(
    Name = "delete_docusaurus_article",
    Description = "Docusaurus記事を削除します。"
)]
public async Task<string> DeleteDocusaurusArticle(
    string filePath
)
```

**list_docusaurus_articles**
```csharp
[McpTool(
    Name = "list_docusaurus_articles",
    Description = "Docusaurus記事一覧を取得します。"
)]
public async Task<string> ListDocusaurusArticles(
    string type,  // "docs" or "blog"
    string? category = null
)
```

---

### Step 3: Notion→Docusaurus変換

#### 3.1 変換フロー設計

```
Notion Ideas（Status: "draft"）
    ↓
1. Notion APIで取得
    ↓
2. カテゴリ・タグをマッピング
    ↓
3. Docusaurus記事生成
    ↓
4. Git操作（commit & push）
    ↓
5. Notionステータス更新（"published"）
```

#### 3.2 MCPツール実装

**notion_to_docusaurus**
```csharp
[McpTool(
    Name = "notion_to_docusaurus",
    Description = "Notion IdeasからDocusaurus記事を生成します。"
)]
public async Task<string> NotionToDocusaurus(
    string notionIdeaId,
    string docusaurusType,  // "docs" or "blog"
    string? category = null,
    bool autoCommit = true,
    bool updateNotionStatus = true
)
```

**bulk_notion_to_docusaurus**
```csharp
[McpTool(
    Name = "bulk_notion_to_docusaurus",
    Description = "複数のNotion IdeasをDocusaurus記事に一括変換します。"
)]
public async Task<string> BulkNotionToDocusaurus(
    string notionFilter,  // Status: "draft", Category: "技術" など
    string docusaurusType,
    bool autoCommit = true
)
```

#### 3.3 マッピング設定

**appsettings.json:**
```json
{
  "Docusaurus": {
    "RepositoryKey": "AteliersDev",
    "DocsPath": "docs",
    "BlogPath": "blog",
    "CategoryMapping": {
      "技術": "docs/technical",
      "C#": "docs/csharp",
      "GitHub": "docs/github-guidelines",
      "個人": "blog"
    },
    "Author": {
      "Name": "Konno Yuu",
      "Title": "Software Engineer",
      "Url": "https://github.com/yuu-git",
      "ImageUrl": "https://github.com/yuu-git.png"
    }
  }
}
```

---

### Step 4: 公開ワークフロー統合

#### 4.1 Git連携

**publish_docusaurus_article**
```csharp
[McpTool(
    Name = "publish_docusaurus_article",
    Description = "Docusaurus記事を公開します（commit & push）。"
)]
public async Task<string> PublishDocusaurusArticle(
    string filePath,
    string? commitMessage = null
)
```

#### 4.2 Notion連携

**publish_notion_idea_to_docusaurus**
```csharp
[McpTool(
    Name = "publish_notion_idea_to_docusaurus",
    Description = "Notion IdeasをDocusaurusに公開し、Notionステータスを更新します。"
)]
public async Task<string> PublishNotionIdeaToDocusaurus(
    string notionIdeaId,
    string docusaurusType,
    string? category = null,
    string? commitMessage = null
)
```

#### 4.3 完全な公開フロー

```csharp
public async Task<PublishResult> PublishFlow(string notionIdeaId)
{
    // 1. Notion Ideasから取得
    var idea = await notionService.GetIdea(notionIdeaId);
    
    // 2. Docusaurus記事生成
    var article = ConvertToDocusaurusArticle(idea);
    
    // 3. ファイル作成
    await docusaurusService.CreateArticle(article);
    
    // 4. Git操作（commit & push）
    await gitService.CommitAndPush("記事公開: " + idea.Title);
    
    // 5. Notionステータス更新
    await notionService.UpdateIdeaStatus(notionIdeaId, "published");
    
    return new PublishResult { Success = true };
}
```

---

### Step 5: ナレッジベース自動化

#### 5.1 会話→記事フロー

**conversation_to_article**
```csharp
[McpTool(
    Name = "conversation_to_article",
    Description = "会話内容からDocusaurus記事を生成します。"
)]
public async Task<string> ConversationToArticle(
    string conversationSummary,
    string title,
    string type,  // "docs" or "blog"
    string? category = null,
    string[]? tags = null,
    bool saveToNotion = true,  // Notionにも保存
    bool publishImmediately = false
)
```

**使用例:**
```
Claude: 今日の会話からC#のDateTime拡張メソッドについて記事を書きましょう。

User: お願いします。

Claude: [conversation_to_article 実行]
→ 会話内容を整理
→ Docusaurus記事生成
→ Notionにも保存（後で編集可能）
→ 必要ならすぐ公開
```

---

## 技術要件

### Docusaurus

**バージョン:**
- Docusaurus 3.x

**フロントマター処理:**
- YAML パーサー
- MDX サポート

**ファイル操作:**
- AteliersDev リポジトリへの書き込み
- Git操作との連携

---

## 期待される成果

### 1. 完全なナレッジワークフロー

**Before Phase 9:**
```
アイデア → Notionにメモ
↓
手動で整理
↓
手動でDocusaurus記事作成
↓
手動でコミット・プッシュ
```

**After Phase 9:**
```
アイデア → Notionにメモ（Phase 7）
↓
整理・検索（Phase 8）
↓
Claudeに「この記事を公開して」
↓
自動でDocusaurus記事生成・公開
↓
Notionステータス更新
```

### 2. マルチクライアント対応

```
VS Code でコード書きながら
↓
「今の実装をドキュメント化して公開」
↓
自動で記事作成・公開
```

### 3. v1.0.0 達成

Phase 9完了時点で、以下が統合完了：
- Git操作（Phase 5）
- マルチクライアント（Phase 6）
- Notion基礎（Phase 7）
- Notion拡張（Phase 8）
- Docusaurus統合（Phase 9）

→ **完全な個人ナレッジ管理システム**

---

## リスクと対策

### リスク1: フロントマター生成の複雑性

**対策:**
- テンプレート機能
- 自動補完機能
- 検証機能

### リスク2: NotionとDocusaurusの構造差異

**対策:**
- 柔軟なマッピング設定
- カスタマイズ可能な変換ルール
- 手動調整の余地を残す

### リスク3: Git操作との連携

**対策:**
- Phase 5のGitTools活用
- トランザクション的な処理
- ロールバック機能

---

## v1.0.0 リリース準備

Phase 9完了後、v1.0.0としてリリース：

### 完了基準

1. **機能完成度**
   - 全Phase（5-9）の機能が統合動作
   - マルチクライアント対応完了
   - ドキュメント完備

2. **品質基準**
   - 全ツールのテスト完了
   - パフォーマンス最適化
   - エラーハンドリング強化

3. **ドキュメント基準**
   - セットアップガイド完備
   - トラブルシューティング完備
   - ベストプラクティス文書化

---

## 完了基準

### 必須条件
- ✅ Docusaurus記事作成ツール実装
- ✅ Notion→Docusaurus変換実装
- ✅ 公開ワークフロー実装
- ✅ ナレッジベース自動化実装
- ✅ 3つのIDE（VS Code, Visual Studio, Claude Desktop）で動作確認
- ✅ ドキュメント完備

### テスト項目
- ✅ Docusaurus記事CRUD × 3 IDE = 12テスト
- ✅ Notion→Docusaurus変換 × 3 IDE = 6テスト
- ✅ 公開フロー × 3 IDE = 6テスト
- ✅ 会話→記事変換 × 3 IDE = 3テスト
- 合計: 27テスト

### ドキュメント
- ✅ `Docs/setup/docusaurus.md` - Docusaurus統合セットアップガイド
- ✅ `Docs/phases/phase9-plan.md` - このファイル
- ✅ `Docs/workflows/publishing.md` - 公開ワークフロー手順
- ✅ README更新（v1.0.0完成版）

---

## リリース計画

### Phase 9 完了後 = v1.0.0

**タグ:** v1.0.0

**リリースノート内容:**
```markdown
# v1.0.0: Complete Knowledge Management System

## Major Features
- Docusaurus integration (article creation, publishing)
- Notion to Docusaurus conversion flow
- Conversation to article automation
- Complete multi-client support (Claude Desktop, VS Code, Visual Studio)

## Integrated Systems
- Git Operations (Phase 5)
- Multi-Client Support (Phase 6)
- Notion Basic (Phase 7)
- Notion Extended (Phase 8)
- Docusaurus Integration (Phase 9)

## Documentation
- Complete setup guides for all IDEs
- Notion integration guide
- Docusaurus publishing workflow
- Best practices and troubleshooting

## Breaking Changes
None

## What's Next
- Future phases for additional integrations
- Community feedback and improvements
- Additional features based on usage
```

---

## 参考資料

- [Docusaurus Documentation](https://docusaurus.io/)
- [Docusaurus Frontmatter](https://docusaurus.io/docs/api/plugins/@docusaurus/plugin-content-docs#markdown-front-matter)
- [Docusaurus Blog](https://docusaurus.io/docs/blog)
- [MDX](https://mdxjs.com/)
