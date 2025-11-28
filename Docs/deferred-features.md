# Deferred Features

全Phaseを通じて見送られた機能の一覧。

---

## Phase 5 見送り機能

### 1. ブランチ操作
**Phase:** 5  
**見送り日:** 2025-11-28  
**実装予定:** Phase 6以降（必要性確認後）

**理由:**
- 基本Git操作のテストを優先
- YAGNI原則（You Aren't Gonna Need It）
- ブランチ戦略が固まっていない

**必要なツール:**
- `create_branch(repositoryKey, branchName, fromBranch?)`
- `switch_branch(repositoryKey, branchName)`
- `list_branches(repositoryKey)`
- `delete_branch(repositoryKey, branchName)`

**実装時間:** 30分程度

**実装条件:**
- Phase 6で明確なブランチ戦略が必要になった場合
- または実験的ブランチ作業が発生した場合

---

### 2. AutoPull/AutoPush 機能
**Phase:** 5  
**見送り日:** 2025-11-28  
**実装予定:** Phase 7以降（再検討）

**理由:**
- 1ファイル変更 = 1コミット問題
- Git履歴汚染リスク
- ユーザーのコミットタイミング制御不能
- 単体Git操作の動作確認を優先

**設計上の問題:**
```
edit_repository_file (AutoPush=true) → 1コミット
edit_repository_file (AutoPush=true) → 1コミット
edit_repository_file (AutoPush=true) → 1コミット
結果: 3ファイル = 3コミット（Git履歴汚染）
```

**Phase 7での再検討ポイント:**
- Docusaurus統合での実際の必要性確認
- バッチコミット機能の実装
- 除外パターン（excludePatterns）対応
- CI/CD統合との兼ね合い
- デフォルト設定（false推奨）

**実装方針（再検討後）:**
- 用途: 記事自動公開、CI/CD統合
- 複数ファイル変更をまとめる仕組み
- 明示的有効化必要（デフォルトfalse）

**現状:**
- 設定項目は実装済み（`AutoPull`, `AutoPush`）
- RepositoryToolsの呼び出しをコメントアウト
- Phase 7で再検討予定

---

## Phase 6 見送り機能

### 3. Docusaurus統合
**Phase:** 6  
**見送り日:** 2025-11-28  
**実装予定:** Phase 7

**理由:**
- MCP汎用化（Multi-Client Integration）を優先
- VS Code, Visual Studio統合により、Docusaurus以外にも適用可能に
- マルチクライアント対応完了後に実装した方が、すべてのIDEで使える

**Phase 6の方針変更:**
```
変更前: Phase 6 = Docusaurus統合
変更後: Phase 6 = MCP Multi-Client Integration
```

**Phase 7での実装内容:**
- 記事作成ツール（frontmatter自動生成）
- 記事更新ツール
- 記事一覧取得
- カテゴリ管理
- ナレッジベース自動化（会話→記事）

**Phase 7実装タイミング:**
- Phase 6完了後（VS Code, Visual Studio統合完了後）
- マルチクライアント対応が安定してから

**Phase 7での利点:**
```
どのIDEからでもDocusaurus記事作成が可能
↓
VS Code → ateliers-ai-mcpserver → Docusaurus記事作成
Visual Studio → ateliers-ai-mcpserver → Docusaurus記事作成
Claude Desktop → ateliers-ai-mcpserver → Docusaurus記事作成
```

---

## 今後の追加ルール

新しい機能を見送る場合、以下の情報を記録してください：

```markdown
### [機能名]
**Phase:** [Phase番号]
**見送り日:** YYYY-MM-DD
**実装予定:** [Phase番号または条件]

**理由:**
- [見送った理由1]
- [見送った理由2]

**実装条件:**
- [どういう条件で実装するか]

**現状:**
- [現在の状態、準備済みの内容]
```
