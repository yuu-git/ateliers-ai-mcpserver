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

### 1. リポジトリのクローン
```bash
git clone https://github.com/yuu-git/ateliers-ai-mcpserver.git
cd ateliers-ai-mcpserver
```

### 2. Personal Access Token設定

#### 2-1. GitHubでPATを作成

1. GitHub → Settings → Developer settings → Personal access tokens → Fine-grained tokens
2. **Generate new token** をクリック
3. 設定：
   - **Token name**: `ateliers-ai-mcpserver`（任意）
   - **Repository access**: Public repositories (All repositories)
   - **Permissions**:
     - **Contents**: Read and write
     - **Metadata**: Read-only
4. **Generate token** をクリック
5. トークンをコピー（一度しか表示されません）

#### 2-2. ローカル設定ファイル作成

テンプレートをコピー：
```bash
cp appsettings.local.json.sample appsettings.local.json
```

**Windows (コマンドプロンプト)**:
```cmd
copy appsettings.local.json.sample appsettings.local.json
```

エディタで開いてPATを書き換え：
```bash
# VS Code
code appsettings.local.json

# Notepad
notepad appsettings.local.json
```

内容:
```json
{
  "GitHub": {
    "PersonalAccessToken": "github_pat_11AAAAAA..." // コピーしたPATを貼り付け
  }
}
```

**注意**: `appsettings.local.json` は `.gitignore` で除外されており、Gitにコミットされません。

### 3. ビルド
```bash
dotnet restore
dotnet build
```
