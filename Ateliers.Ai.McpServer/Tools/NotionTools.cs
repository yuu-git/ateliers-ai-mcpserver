using Ateliers.Ai.McpServer.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Ateliers.Ai.McpServer.Tools;

/// <summary>
/// Notion Tasks/Ideas管理MCPツール
/// </summary>
[McpServerToolType]
public class NotionTools
{
    private readonly NotionTasksService _notionTasksService;
    private readonly NotionIdeasService _notionIdeasService;

    public NotionTools(NotionTasksService notionTasksService, NotionIdeasService notionIdeasService)
    {
        _notionTasksService = notionTasksService;
        _notionIdeasService = notionIdeasService;
    }

    #region Tasks Management

    [McpServerTool]
    [Description(@"Notionにタスクを追加します。

WHEN TO USE:
- ユーザーが「タスクを追加」「TODO追加」「やることメモ」と言った時
- 作業項目を記録したい時
- 期限付きのタスクを管理したい時

DO NOT USE WHEN:
- アイデアや構想を記録したい時（use add_idea）
- 技術記事を作成したい時（use create_docusaurus_article）

EXAMPLES:
✓ 'Phase 7の実装をタスクに追加して'
✓ '明日までにドキュメント作成、優先度高でタスク追加'
✗ 'MCPサーバーのアイデアをメモ'（use add_idea）

RELATED TOOLS:
- update_task: タスク更新
- list_tasks: タスク一覧取得
- complete_task: タスク完了")]
    public async Task<string> AddTask(
        [Description("タスクのタイトル（必須）")] string title,
        [Description("タスクの詳細説明（オプション）")] string? description = null,
        [Description("ステータス: 未着手/進行中/完了/保留（デフォルト: 未着手）")] string? status = "未着手",
        [Description("優先度: 高/中/低（デフォルト: 中）")] string? priority = "中",
        [Description("期限日（オプション、ISO 8601形式）")] DateTime? dueDate = null,
        [Description("タグ配列（オプション）")] string[]? tags = null,
        [Description("作成元: Claude/ChatGPT/Copilot/手動（オプション）")] string? createdBy = null)
    {
        try
        {
            return await _notionTasksService.AddTaskAsync(title, description, status, priority, dueDate, tags, createdBy);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    [McpServerTool]
    [Description(@"Notionのタスクを更新します。

WHEN TO USE:
- タスクの内容を変更したい時
- ステータスや優先度を更新したい時
- 期限を変更したい時

DO NOT USE WHEN:
- タスクを完了にするだけの時（use complete_task）
- 新しいタスクを追加する時（use add_task）

EXAMPLES:
✓ 'タスクIDxxxのステータスを進行中に変更'
✓ 'タスクxxxの優先度を高に上げて'

RELATED TOOLS:
- add_task: タスク追加
- complete_task: タスク完了
- list_tasks: タスク一覧")]
    public async Task<string> UpdateTask(
        [Description("タスクID（必須）")] string taskId,
        [Description("新しいタイトル（オプション）")] string? title = null,
        [Description("新しい詳細説明（オプション）")] string? description = null,
        [Description("新しいステータス（オプション）")] string? status = null,
        [Description("新しい優先度（オプション）")] string? priority = null,
        [Description("新しい期限日（オプション）")] DateTime? dueDate = null,
        [Description("新しいタグ配列（オプション）")] string[]? tags = null)
    {
        try
        {
            return await _notionTasksService.UpdateTaskAsync(taskId, title, description, status, priority, dueDate, tags);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    [McpServerTool]
    [Description(@"Notionのタスク一覧を取得します。フィルタ可能。

WHEN TO USE:
- タスク一覧を確認したい時
- 未完了タスクを見たい時
- 期限が近いタスクを確認したい時
- 優先度でフィルタしたい時

EXAMPLES:
✓ '未完了タスクを一覧表示'
✓ '優先度が高いタスクを見せて'
✓ '期限が近いタスクを確認'

RELATED TOOLS:
- add_task: タスク追加
- update_task: タスク更新
- complete_task: タスク完了")]
    public async Task<string> ListTasks(
        [Description("ステータスフィルタ（オプション）")] string? status = null,
        [Description("優先度フィルタ（オプション）")] string? priority = null,
        [Description("期限が近いものだけ（7日以内、オプション）")] bool? dueSoon = null,
        [Description("取得件数上限（デフォルト: 10）")] int limit = 10)
    {
        try
        {
            return await _notionTasksService.ListTasksAsync(status, priority, dueSoon, limit);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    [McpServerTool]
    [Description(@"Notionのタスクを完了にします。

WHEN TO USE:
- タスクが完了した時
- タスクを終了状態にしたい時

EXAMPLES:
✓ 'タスクIDxxxを完了にして'
✓ 'このタスク終わったので完了にして'

RELATED TOOLS:
- add_task: タスク追加
- update_task: タスク更新
- list_tasks: タスク一覧")]
    public async Task<string> CompleteTask(
        [Description("タスクID（必須）")] string taskId)
    {
        try
        {
            return await _notionTasksService.CompleteTaskAsync(taskId);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    #endregion

    #region Ideas Management

    [McpServerTool]
    [Description(@"Notionにアイデアを追加します。

WHEN TO USE:
- アイデアや構想を記録したい時
- ひらめきをメモしたい時
- 技術的なアイデアを保存したい時

DO NOT USE WHEN:
- 作業タスクを追加したい時（use add_task）
- 技術記事を作成したい時（use create_docusaurus_article）

EXAMPLES:
✓ 'MCPサーバーの新機能アイデアをメモ'
✓ 'ビジネスアイデア：AI活用した営業支援ツール'
✗ 'Phase 7の実装タスク追加'（use add_task）

RELATED TOOLS:
- search_ideas: アイデア検索
- update_idea: アイデア更新")]
    public async Task<string> AddIdea(
        [Description("アイデアのタイトル（必須）")] string title,
        [Description("アイデアの内容（オプション）")] string? content = null,
        [Description("カテゴリ配列: 技術/ビジネス/個人/その他（オプション）")] string[]? categories = null,
        [Description("タグ配列（オプション）")] string[]? tags = null,
        [Description("関連リンク（オプション）")] string? relatedLinks = null,
        [Description("作成元: Claude/ChatGPT/Copilot/手動（オプション）")] string? createdBy = null)
    {
        try
        {
            return await _notionIdeasService.AddIdeaAsync(title, content, categories, tags, relatedLinks, createdBy);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    [McpServerTool]
    [Description(@"Notionのアイデアを検索します。

WHEN TO USE:
- アイデアを検索したい時
- カテゴリやタグでフィルタしたい時
- キーワードでアイデアを探したい時

EXAMPLES:
✓ 'MCPに関するアイデアを検索'
✓ '技術カテゴリのアイデアを表示'
✓ 'AI関連のアイデアを探して'

RELATED TOOLS:
- add_idea: アイデア追加
- update_idea: アイデア更新")]
    public async Task<string> SearchIdeas(
        [Description("検索キーワード（オプション）")] string? keyword = null,
        [Description("カテゴリフィルタ配列（オプション）")] string[]? categories = null,
        [Description("タグフィルタ配列（オプション）")] string[]? tags = null,
        [Description("取得件数上限（デフォルト: 10）")] int limit = 10)
    {
        try
        {
            return await _notionIdeasService.SearchIdeasAsync(keyword, categories, tags, limit);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    [McpServerTool]
    [Description(@"Notionのアイデアを更新します。

WHEN TO USE:
- アイデアの内容を変更したい時
- ステータスやカテゴリを更新したい時
- 追加情報を記録したい時

EXAMPLES:
✓ 'アイデアIDxxxのステータスを検討中に変更'
✓ 'アイデアxxxにタグを追加'

RELATED TOOLS:
- add_idea: アイデア追加
- search_ideas: アイデア検索")]
    public async Task<string> UpdateIdea(
        [Description("アイデアID（必須）")] string ideaId,
        [Description("新しいタイトル（オプション）")] string? title = null,
        [Description("新しい内容（オプション）")] string? content = null,
        [Description("新しいカテゴリ配列（オプション）")] string[]? categories = null,
        [Description("新しいタグ配列（オプション）")] string[]? tags = null,
        [Description("新しいステータス（オプション）")] string? status = null,
        [Description("新しい関連リンク（オプション）")] string? relatedLinks = null)
    {
        try
        {
            return await _notionIdeasService.UpdateIdeaAsync(ideaId, title, content, categories, tags, status, relatedLinks);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    #endregion
}
