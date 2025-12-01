using Ateliers.Ai.McpServer.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Ateliers.Ai.McpServer.Tools;

/// <summary>
/// Notion Tasks/Ideas/Reading List管理MCPツール
/// </summary>
[McpServerToolType]
public class NotionTools
{
    private readonly NotionTasksService _notionTasksService;
    private readonly NotionIdeasService _notionIdeasService;
    private readonly NotionReadingListService _notionReadingListService;

    public NotionTools(
        NotionTasksService notionTasksService, 
        NotionIdeasService notionIdeasService,
        NotionReadingListService notionReadingListService)
    {
        _notionTasksService = notionTasksService;
        _notionIdeasService = notionIdeasService;
        _notionReadingListService = notionReadingListService;
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
- 技術記事URLを保存したい時（use add_to_reading_list）

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
        [Description("日付: 期限日 or 実行予定日時（オプション）")] DateTime? dueDate = null,
        [Description("場所: 自宅/オフィス/Zoom/Google Meet/Teams/客先/外出先/その他（オプション）")] string? location = null,
        [Description("タグ配列（オプション）")] string[]? tags = null,
        [Description("登録元: Claude/GPT/Copilot/手動（オプション）")] string? registrant = null)
    {
        try
        {
            return await _notionTasksService.AddTaskAsync(title, description, status, priority, dueDate, location, tags, registrant);
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
        [Description("新しい日付（オプション）")] DateTime? dueDate = null,
        [Description("新しい場所（オプション）")] string? location = null,
        [Description("新しいタグ配列（オプション）")] string[]? tags = null)
    {
        try
        {
            return await _notionTasksService.UpdateTaskAsync(taskId, title, description, status, priority, dueDate, location, tags);
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
- 技術記事URLを保存したい時（use add_to_reading_list）

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
        [Description("タグ配列（オプション）")] string[]? tags = null,
        [Description("関連リンク（オプション）")] string? link = null,
        [Description("登録元: Claude/GPT/Copilot/手動（オプション）")] string? registrant = null)
    {
        try
        {
            return await _notionIdeasService.AddIdeaAsync(title, content, tags, link, registrant);
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
- タグでフィルタしたい時
- キーワードでアイデアを探したい時

EXAMPLES:
✓ 'MCPに関するアイデアを検索'
✓ '技術タグのアイデアを表示'
✓ 'AI関連のアイデアを探して'

RELATED TOOLS:
- add_idea: アイデア追加
- update_idea: アイデア更新")]
    public async Task<string> SearchIdeas(
        [Description("検索キーワード（オプション）")] string? keyword = null,
        [Description("タグフィルタ配列（オプション）")] string[]? tags = null,
        [Description("取得件数上限（デフォルト: 10）")] int limit = 10)
    {
        try
        {
            return await _notionIdeasService.SearchIdeasAsync(keyword, tags, limit);
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
- ステータスやタグを更新したい時
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
        [Description("新しいタグ配列（オプション）")] string[]? tags = null,
        [Description("新しいステータス（オプション）")] string? status = null,
        [Description("新しい関連リンク（オプション）")] string? link = null)
    {
        try
        {
            return await _notionIdeasService.UpdateIdeaAsync(ideaId, title, content, tags, status, link);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    #endregion

    #region Reading List Management

    [McpServerTool]
    [Description(@"Notionにリーディングリスト（あとで読む）を追加します。

WHEN TO USE:
- 技術記事、書籍、動画などを「あとで読む/見る」リストに追加したい時
- URLを保存して後で参照したい時
- 学習資料を管理したい時
- ウェビナーや配信セミナーを登録したい時

DO NOT USE WHEN:
- 今すぐ読む/実行するタスクの時（use add_task）
- URLのないアイデアの時（use add_idea）

EXAMPLES:
✓ 'このMCP記事、いつか読みたいからリーディングリストに追加'
✓ 'この技術書、優先度高で追加して'
✓ '12/10 14:00のウェビナーをリーディングリストに追加'
✗ '今日この記事読む'（use add_task）

RELATED TOOLS:
- list_reading_list: リーディングリスト一覧
- update_reading_list_status: ステータス更新")]
    public async Task<string> AddToReadingList(
        [Description("資料のタイトル（必須）")] string title,
        [Description("URL（オプション）")] string? link = null,
        [Description("種類: 記事/書籍/動画/論文/コード例/その他（オプション）")] string? type = null,
        [Description("ステータス: 未読/完了（デフォルト: 未読）")] string? status = "未読",
        [Description("優先度: 高/中/低（デフォルト: 中）")] string? priority = "中",
        [Description("日付: 期限日 or 開催日時 or 配信日時（オプション）")] DateTime? date = null,
        [Description("再参照フラグ（デフォルト: false）")] bool reference = false,
        [Description("タグ配列（オプション）")] string[]? tags = null,
        [Description("登録元: Claude/GPT/Copilot/手動（オプション）")] string? registrant = null,
        [Description("読後メモ（オプション）")] string? notes = null,
        [Description("資料の概要（オプション）")] string? description = null,
        [Description("著者名（オプション）")] string? author = null)
    {
        try
        {
            return await _notionReadingListService.AddToReadingListAsync(
                title, link, type, status, priority, date, 
                reference, tags, registrant, notes, description, author);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    [McpServerTool]
    [Description(@"リーディングリスト一覧を取得します。

WHEN TO USE:
- リーディングリストを確認したい時
- 未読資料を見たい時
- 優先度でフィルタしたい時

EXAMPLES:
✓ '未読のリーディングリストを表示'
✓ '優先度が高い資料を見せて'

RELATED TOOLS:
- add_to_reading_list: リーディングリスト追加
- update_reading_list_status: ステータス更新")]
    public async Task<string> ListReadingList(
        [Description("ステータスフィルタ（オプション）")] string? status = null,
        [Description("優先度フィルタ（オプション）")] string? priority = null,
        [Description("取得件数上限（デフォルト: 20）")] int limit = 20)
    {
        try
        {
            return await _notionReadingListService.ListReadingListAsync(status, priority, limit);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    [McpServerTool]
    [Description(@"リーディングリストのステータスを更新します。

WHEN TO USE:
- 資料を読み終わった時
- ステータスを「完了」に変更したい時

EXAMPLES:
✓ 'リーディングリストIDxxxを完了にして'
✓ 'この資料読み終わったので完了にして'

RELATED TOOLS:
- add_to_reading_list: リーディングリスト追加
- list_reading_list: リーディングリスト一覧")]
    public async Task<string> UpdateReadingListStatus(
        [Description("Reading List ID（必須）")] string readingListId,
        [Description("新しいステータス: 未読/完了")] string status,
        [Description("完了日（オプション、status=完了時に設定）")] DateTime? completedDate = null)
    {
        try
        {
            return await _notionReadingListService.UpdateReadingListStatusAsync(readingListId, status, completedDate);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
    }

    #endregion
}
