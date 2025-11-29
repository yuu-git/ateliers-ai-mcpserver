# Phase 8 引継ぎドキュメント

**作成日:** 2025-11-29  
**Phase 7 完了コミット:** 4dedd97  
**Phase 7 タグ:** v0.7.0  
**次のアクション:** Phase 8（Docsタスク洗い出し → Notion移行）

---

## Phase 7 完了状況

### ✅ 実装完了項目

**Step 1: Notion API接続基盤**
- ✅ Notion.Net NuGetパッケージ追加（v4.4.0）
- ✅ notionsettings.json/local.json作成
- ✅ NotionService実装
- ✅ Program.cs設定読み込み＋DI登録

**Step 2: Tasks管理（4ツール）**
- ✅ NotionTasksService実装
- ✅ add_task - タスク追加
- ✅ update_task - タスク更新
- ✅ list_tasks - タスク一覧取得
- ✅ complete_task - タスク完了
- ✅ Descriptionをページブロックとして保存（長文対応）

**Step 3: Ideas管理（3ツール）**
- ✅ NotionIdeasService実装
- ✅ add_idea - アイデア追加
- ✅ search_ideas - アイデア検索
- ✅ update_idea - アイデア更新
- ✅ Contentをページブロックとして保存（長文対応）

**改善項目**
- ✅ try-catchによるエラー詳細表示
- ✅ DI問題修正（static→instance）
- ✅ .gitignore更新（*.local.json除外）

---

## 現在のツール構成（25ツール）

### GitTools（7ツール）
```
- commit_repository
- push_repository
- pull_repository
- create_tag
- push_tag
- create_and_push_tag
- commit_and_push_repository
```

### RepositoryTools（8ツール）
```
- list_repository_files
- read_repository_file
- add_repository_file
- edit_repository_file
- delete_repository_file
- copy_repository_file
- rename_repository_file
- backup_repository_file
```

### AteliersDevTools（3ツール）
```
- list_articles
- read_article
- search_articles
```

### NotionTools（7ツール - Phase 7で追加）
```
Tasks管理:
- add_task
- update_task
- list_tasks
- complete_task

Ideas管理:
- add_idea
- search_ideas
- update_idea
```

**合計: 25ツール**

---

## Phase 8 の新しい計画

### 🎯 目的

**当初計画からの変更**

❌ **旧Phase 8:** Notion拡張機能（Bookmarks等14ツール追加）

✅ **新Phase 8:** Docsのタスク洗い出し → Notion移行

### 📦 達成目標

1. **Docs内のタスク・アイデアの洗い出し**
   - phase7-plan.md
   - phase8-plan.md（旧計画）
   - phase9-plan.md（旧計画）
   - deferred-features.md
   - decisions/

2. **Notionへの移行**
   - タスク → Notion Tasks
   - アイデア → Notion Ideas
   - 既存の add_task, add_idea ツールを活用

3. **計画管理の明確化**
   - Notion: 進行中のタスク・検討中のアイデア
   - Docs: 完了したPhaseの記録・アーキテクチャ決定

4. **新ツール追加なし**
   - 既存の7ツールで実施可能

---

## 実装方針

### Step 1: Docsタスク洗い出し

**対象ファイル:**
```
Docs/phases/phase7-plan.md      # 完了基準の確認
Docs/phases/phase8-plan.md      # 旧計画のアイデア抽出
Docs/phases/phase9-plan.md      # 旧計画のアイデア抽出
Docs/deferred-features.md       # 保留機能の整理
Docs/decisions/                 # 決定事項の記録確認
Docs/refactoring/phase10-server-split-plan.md  # リファクタリング計画
```

**洗い出し方法:**
```
各ファイルから:
1. 未完了のタスク → Notion Tasksに追加
2. 将来の機能候補 → Notion Ideasに追加
3. 完了した項目 → そのまま残す（記録として）
```

### Step 2: Notionへの移行

**Tasks例:**
```
add_task を使用:
- Title: "Phase 9: 独立MCPサーバーへのリファクタリング"
- Status: "未着手"
- Priority: "高"
- Tags: ["Phase9", "リファクタリング", "MCP"]
- Description: "3つの独立MCPサーバーに分割（旧Phase 10計画）"
- Created By: "Claude"
```

**Ideas例:**
```
add_idea を使用:
- Title: "Notion Bookmarks管理機能"
- Category: "技術"
- Tags: ["Notion", "機能拡張", "Phase10候補"]
- Content: "旧Phase 8計画: add_bookmark等3ツール。必要性を見極めてから実装。"
- Status: "検討中"
- Created By: "Claude"
```

### Step 3: Docs整理

**Docsの役割明確化:**

**残すもの（記録として）:**
- 完了したPhaseの計画書（phase7-plan.md等）
- アーキテクチャ決定記録（decisions/）
- セットアップガイド（setup/）
- トラブルシューティング（troubleshooting.md）

**Notionに移行するもの:**
- 未完了のタスク
- 将来の機能候補（アイデア）
- 検討中の改善案

**削除/アーカイブするもの:**
- phase8-plan.md（旧計画 → アイデアとして抽出後、アーカイブ）
- phase9-plan.md（旧計画 → アイデアとして抽出後、アーカイブ）

---

## Phase 8 完了基準

### 必須条件

- ✅ Docs/phases/ 内の旧計画からタスク・アイデアを抽出
- ✅ Docs/deferred-features.md の内容を整理
- ✅ すべてのタスク・アイデアがNotionに登録済み
- ✅ Docsの役割が明確化（記録 vs 進行管理）

### 期待される状態

**Notion Tasks:**
- Phase 9（リファクタリング）のタスク
- その他の実装予定タスク

**Notion Ideas:**
- 旧Phase 8計画の機能（Bookmarks等）
- 旧Phase 9計画の機能（Docusaurus統合等）
- その他の将来的な機能候補

**Docs:**
- 完了済みPhaseの記録
- セットアップガイド
- アーキテクチャ決定記録

---

## Phase 9 以降の計画

### Phase 9: リファクタリング（旧Phase 10）

**目的:**
- 独立MCPサーバーへの分割
- ツール選択問題の解決
- MCP公式ベストプラクティス準拠

**実装内容:**
```
ateliers-github-mcp:     15ツール
ateliers-notion-mcp:      7ツール（Phase 7のみ）
ateliers-docusaurus-mcp:  3ツール

合計: 25ツール（変わらず、構造が改善）
```

**参考:**
- Docs/refactoring/phase10-server-split-plan.md

### Phase 10 以降: 機能拡張（必要に応じて）

**候補（優先順位未定）:**
- Notion Bookmarks管理（旧Phase 8）
- Notion検索機能強化（旧Phase 8）
- Docusaurus記事作成・公開（旧Phase 9）
- SQL Server / SQLite MCP
- その他（Notionで管理）

**判断基準:**
- 実際の使用頻度
- ツール数のバランス
- 実装コスト vs 利便性

---

## 技術的な重要ポイント

### 1. Notionデータベース構造

**Tasks Database:**
```
- Name (Title): タスク名
- Status (Select): 未着手/進行中/完了/保留
- Priority (Select): 高/中/低
- Due Date (Date): 期限
- Tags (Multi-select): タグ
- Created By (Select): Claude/ChatGPT/Copilot/手動
- Description (Page Content): 詳細説明（ページブロック）
```

**Ideas Database:**
```
- Name (Title): アイデア名
- Category (Multi-select): 技術/ビジネス/個人/その他
- Tags (Multi-select): タグ
- Status (Select): アイデア/検討中/実装予定/完了/却下
- Content (Page Content): 内容（ページブロック）
- Related Links (URL): 関連リンク
- Created By (Select): Claude/ChatGPT/Copilot/手動
```

### 2. 長文対応の設計

**重要な改善（Phase 7で実装）:**
```
Description/Content → Propertiesではなく、Childrenブロックとして保存
→ 2000文字制限を回避
→ 改行を含む長文が綺麗に保存される
```

**実装箇所:**
- NotionTasksService.cs
- NotionIdeasService.cs

### 3. NotionプロパティはNotion側で削除済み

**Phase 7で実施:**
- Tasks Database: "Description"プロパティ削除
- Ideas Database: "Content"プロパティ削除

理由: ページブロックに移行したため不要

---

## Phase 8 実装時の注意点

### 1. 既存ツールの活用

**新ツール不要:**
- add_task
- add_idea
- update_task
- update_idea

これらで十分対応可能。

### 2. 分類の方針

**Task vs Idea の判断基準:**

**Task（add_task）:**
- 具体的な実装作業
- 明確な完了基準がある
- スケジュール管理が必要

**Idea（add_idea）:**
- 将来の機能候補
- 検討が必要
- 実装するかどうか未定

### 3. タグ設計

**推奨タグ:**
```
Tasks:
- Phase9, Phase10, リファクタリング, 機能拡張, バグ修正

Ideas:
- Notion, Docusaurus, GitHub, MCP, 機能拡張, 改善案
```

### 4. Created By の設定

**Phase 8での設定:**
```
Docsから移行したもの: "Claude"
Phase 7完了時に追加: "Claude"
手動で追加したもの: "手動"
```

---

## 現在の設定ファイル構成

### appsettings.json（基本設定）
```json
{
  "Repositories": {
    "AteliersAiAssistants": { ... },
    "AteliersAiMcpServer": { ... },
    "AteliersDev": { ... },
    "PublicNotes": { ... },
    "TrainingMcpServer": { ... }
  }
}
```

### appsettings.local.json（ローカル設定、gitignore対象）
```json
{
  // LocalPathの上書き等
}
```

### githubsettings.json / githubsettings.local.json
```json
{
  "GitHubSettings": {
    "DefaultOwner": "yuu-git",
    "Repositories": { ... }
  }
}
```

### notionsettings.json / notionsettings.local.json（Phase 7で追加）
```json
{
  "Notion": {
    "ApiToken": "secret_xxx",  // local.jsonで設定
    "WorkspaceId": "",
    "Databases": {
      "Tasks": "database-id",   // local.jsonで設定
      "Ideas": "database-id"    // local.jsonで設定
    }
  }
}
```

---

## 参考ファイルパス

### 実装ファイル
```
Ateliers.Ai.McpServer/
├── Program.cs
├── Services/
│   ├── NotionService.cs
│   ├── NotionTasksService.cs
│   └── NotionIdeasService.cs
├── Tools/
│   └── NotionTools.cs
├── appsettings.json
├── appsettings.local.json
├── githubsettings.json
├── githubsettings.local.json
├── notionsettings.json
└── notionsettings.local.json
```

### ドキュメント
```
Docs/
├── phases/
│   ├── phase7-plan.md              # Phase 7計画（完了）
│   ├── phase8-plan.md              # 旧Phase 8計画（アイデア抽出対象）
│   └── phase9-plan.md              # 旧Phase 9計画（アイデア抽出対象）
├── refactoring/
│   └── phase10-server-split-plan.md  # Phase 9実施予定（リファクタリング）
├── deferred-features.md            # 保留機能（整理対象）
├── decisions/
│   └── 2025-11-28-git-tools-design.md
├── setup/
│   ├── claude-desktop.md
│   ├── visual-studio.md
│   └── vscode.md
└── troubleshooting.md
```

---

## Phase 8 開始時のチェックリスト

### 環境確認
- [ ] ateliers-ai-mcpserver がビルド・起動可能
- [ ] Claude Desktop でNotionツールが認識される
- [ ] Notion Tasks/Ideas データベースにアクセス可能
- [ ] notionsettings.local.json が正しく設定されている

### ドキュメント確認
- [ ] Docs/phases/phase7-plan.md を確認
- [ ] Docs/phases/phase8-plan.md（旧計画）を確認
- [ ] Docs/phases/phase9-plan.md（旧計画）を確認
- [ ] Docs/deferred-features.md を確認
- [ ] Docs/refactoring/phase10-server-split-plan.md を確認

### 実装準備
- [ ] add_task ツールの動作確認
- [ ] add_idea ツールの動作確認
- [ ] list_tasks でタスク一覧を確認
- [ ] search_ideas でアイデア検索を確認

---

## よくある質問（FAQ）

### Q1. Phase 8で新ツールは追加しないのですか？

A1. **追加しません。**

理由:
- 既存の add_task, add_idea で対応可能
- ツール数を抑制（現在25ツール）
- Phase 9のリファクタリングに集中するため

### Q2. 旧Phase 8・9の機能はどうなりますか？

A2. **Notion Ideasに「検討中」として登録します。**

例:
- "Notion Bookmarks管理機能"（旧Phase 8）
- "Docusaurus記事作成機能"（旧Phase 9）

Phase 10以降で必要性を判断してから実装します。

### Q3. Docsのファイルは削除しますか？

A3. **完了済みの計画書は残します。**

削除/アーカイブ対象:
- phase8-plan.md（旧計画）
- phase9-plan.md（旧計画）

残すもの:
- phase7-plan.md（完了記録として）
- setup/（セットアップガイド）
- decisions/（アーキテクチャ決定記録）

### Q4. Phase 9のリファクタリングはいつ実施しますか？

A4. **Phase 8完了後すぐに実施する予定です。**

Phase 9（リファクタリング）の優先度が高い理由:
- ツール選択問題の早期解決
- Phase 10以降の機能追加が容易になる
- MCP公式ベストプラクティス準拠

---

## トラブルシューティング

### 問題1: Notionツールが認識されない

**確認項目:**
1. notionsettings.local.json が存在するか
2. ApiToken が正しく設定されているか
3. Database ID が正しいか（ページIDではなくデータベースID）
4. Notion Integration が Tasks/Ideas データベースに接続されているか

**解決手順:**
→ Docs/setup/notion.md（存在すれば）参照
→ Phase 7実装時の手順を確認

### 問題2: ページブロックが正しく保存されない

**確認項目:**
1. NotionTasksService.cs / NotionIdeasService.cs で Children ブロックを使用しているか
2. Properties ではなく Childrenブロックとして保存しているか

**Phase 7で修正済み:**
- Description/Content → Childrenブロックに保存
- Propertiesから削除済み

### 問題3: DI（依存性注入）エラー

**確認項目:**
1. Program.cs で NotionTasksService, NotionIdeasService が登録されているか
2. static メソッドではなく instance メソッドになっているか

**Phase 7で修正済み:**
- NotionTools.cs: static → instance に変更
- DI正常動作確認済み

---

## 次のチャットへの引継ぎ事項

### 1. Phase 7 完了確認

**コミット:**
- 4dedd97: Phase 7 完了

**タグ:**
- v0.7.0: Notion基礎統合完了

### 2. Phase 8 の目的

**Docsのタスク洗い出し → Notion移行**

- 新ツール追加なし
- 既存ツール（add_task, add_idea）活用
- 計画管理の明確化

### 3. Phase 9 以降

**Phase 9: リファクタリング（旧Phase 10）**
- 独立MCPサーバーへの分割
- Docs/refactoring/phase10-server-split-plan.md 参照

**Phase 10以降: 必要に応じて機能追加**

### 4. 現在のツール数

**25ツール:**
- GitTools: 7
- RepositoryTools: 8
- AteliersDevTools: 3
- NotionTools: 7

---

## 連絡先・フィードバック

Phase 8実装中に問題や疑問が生じた場合:
1. このドキュメントを参照
2. Phase 7実装時のチャット履歴を確認
3. Docs/troubleshooting.md を確認

---

**引継ぎ準備完了**  
**Phase 8 開始の準備が整いました！**

---

**Document Version:** 1.0  
**Created:** 2025-11-29  
**Author:** Phase 7 完了時点での引継ぎ  
**Next Action:** 新しいチャットで Phase 8 開始
