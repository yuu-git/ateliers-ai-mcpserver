# Ateliers AI MCP Server

C#/.NETで実装したModel Context Protocol（MCP）サーバー。
AIアシスタント向けにGitHub読み書き、ローカルメモ管理、技術ナレッジ参照を提供。

## 概要

`ateliers-training-mcpserver-claude`（v0.3.0）をベースにした実運用版。

### Training版との違い

| 項目 | Training版 | 実運用版 |
|:--|:--|:--|
| 目的 | 学習・サンプル | 実運用 |
| GitHub書き込み | ❌ | ✅ |
| エラーハンドリング | 基本のみ | 改善予定 |
| プラグイン | ❌ | 将来対応 |

## バージョン履歴

- **v1.0.0**: GitHub書き込み対応（ateliers-training-mcpserver-claude v0.3.0ベース）

## 前提条件

- .NET 10.0 SDK
- Claude Desktop
- GitHub Personal Access Token

## セットアップ

（Training版のREADMEから流用）
...