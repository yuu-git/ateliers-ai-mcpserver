# Ateliers.Ai.McpServer Documentation

このフォルダーには、開発プロセスに関するドキュメントを格納します。

## フォルダー構造

```
Docs/
├── README.md                        # このファイル
├── setup/                           # IDE別セットアップガイド
│   ├── claude-desktop.md           # Claude Desktop用
│   ├── vscode.md                   # VS Code用
│   └── visual-studio.md            # Visual Studio用
├── troubleshooting.md              # トラブルシューティングガイド
├── phases/                          # Phase間引き継ぎドキュメント
│   ├── phase1-handover.md
│   ├── phase2-handover.md
│   ├── phase3-handover.md
│   ├── phase4-handover.md
│   ├── phase5-handover.md
│   └── phase6-plan.md
├── decisions/                       # 設計判断記録（日付付き）
│   └── 2025-11-28-git-tools-design.md
└── deferred-features.md             # 見送り機能リスト
```

## ドキュメントの目的

### setup/ - IDE別セットアップガイド
各IDEでの ateliers-ai-mcpserver セットアップ手順を説明します。
- 前提条件
- インストール手順
- 設定ファイル作成
- 動作確認方法
- トラブルシューティング

**対応IDE:**
- Claude Desktop
- VS Code
- Visual Studio

### troubleshooting.md - トラブルシューティング
よくある問題と解決方法を記録します。
- IDE別の問題
- 認証問題
- ネットワーク問題
- ログ確認方法
- 問題報告方法

### phases/ - Phase間引き継ぎ
各Phaseの完了内容、次Phaseへの見送り事項、優先事項を記録します。
- Phase完了時に作成
- 次Phase開始時の参照資料
- 設計判断の記録
- 技術的な注意事項

### decisions/ - 設計判断記録
重要な設計判断を日付付きで記録します。
- なぜその判断をしたのか
- 代替案は何だったのか
- どのような影響があるのか
- 学んだこと

### deferred-features.md - 見送り機能
全Phaseを通じて見送られた機能を一覧管理します。
- どのPhaseで見送ったか
- なぜ見送ったか
- いつ実装するか（Phase番号または条件）
- 実装条件と現状

## ドキュメント作成ガイドライン

1. **簡潔に** - 必要な情報だけを記載
2. **構造化** - 見出しと箇条書きを活用
3. **追跡可能** - 判断の理由を明記
4. **更新可能** - 状況が変わったら更新

## 運用ルール

- Phase完了時に必ず handover.md を作成
- 重要な設計判断は decisions/ に記録
- 見送り機能は deferred-features.md に追加
- IDE対応追加時は setup/ にガイド追加
- 必要に応じて構造を見直し・拡張

## Phase 6 の重点

Phase 6では **MCP Multi-Client Integration** を実施します。

**目標:**
```
Claude Desktop  ┐
VS Code Copilot ├─→ ateliers-ai-mcpserver ─→ 同じ機能提供
VS Copilot      ┘
```

**実装内容:**
- VS Code統合
- Visual Studio統合
- 各IDE用セットアップガイド作成
- トラブルシューティングガイド作成

詳細は [phases/phase6-plan.md](phases/phase6-plan.md) を参照してください。

## v1.0.0 へのロードマップ

```
Phase 5 (完了) - Git Integration
  ↓
Phase 6 (進行中) - MCP Multi-Client Integration
  ├── VS Code統合
  ├── Visual Studio統合
  └── ドキュメント整備
  ↓
Phase 7 (計画中) - Docusaurus Integration
  ├── 記事管理ツール
  ├── フロントマター処理
  └── ナレッジベース自動化
  ↓
v1.0.0 リリース 🎉
  ↓
Phase 8+ - 細かい調整・機能追加
```

## 参考資料

- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [VS Code MCP Documentation](https://code.visualstudio.com/docs/copilot/customization/mcp-servers)
- [Visual Studio MCP Documentation](https://learn.microsoft.com/en-us/visualstudio/ide/mcp-servers)
- [MCP C# SDK](https://devblogs.microsoft.com/dotnet/build-a-model-context-protocol-mcp-server-in-csharp/)
- [LibGit2Sharp Documentation](https://github.com/libgit2/libgit2sharp)
