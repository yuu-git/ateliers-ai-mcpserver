using Ateliers.Ai.McpServer.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Ateliers.Ai.McpServer.Tools;

/// <summary>
/// ローカルファイルへのメモ管理用MCPツール群
/// </summary>
[McpServerToolType]
public class LocalNotesTools
{
    private readonly NotesService _notesService;

    public LocalNotesTools(NotesService notesService)
    {
        _notesService = notesService;
    }

    /// <summary>
    /// TODOを追加
    /// </summary>
    [McpServerTool]
    [Description("Add a new TODO item to the current TODO list")]
    public Task<string> AddTodo(
        [Description("TODO content to add")]
        string content)
    {
        return _notesService.AddTodoAsync(content);
    }

    /// <summary>
    /// TODO一覧を取得
    /// </summary>
    [McpServerTool]
    [Description("List all current TODO items in progress")]
    public Task<string> ListTodos()
    {
        return _notesService.ListTodosAsync();
    }

    /// <summary>
    /// アイデアを追加
    /// </summary>
    [McpServerTool]
    [Description("Add a new idea to the specified category (technical, article, or project)")]
    public Task<string> AddIdea(
        [Description("Category: technical, article, or project")]
        string category,
        [Description("Idea content to add")]
        string content)
    {
        return _notesService.AddIdeaAsync(category, content);
    }

    /// <summary>
    /// アイデア一覧を取得
    /// </summary>
    [McpServerTool]
    [Description("List all ideas in the specified category (technical, article, or project)")]
    public Task<string> ListIdeas(
        [Description("Category: technical, article, or project")]
        string category)
    {
        return _notesService.ListIdeasAsync(category);
    }

    /// <summary>
    /// コードスニペットを保存
    /// </summary>
    [McpServerTool]
    [Description("Save a code snippet with optional description")]
    public Task<string> SaveSnippet(
        [Description("Programming language: csharp, python, or sql")]
        string language,
        [Description("Snippet name (will be used as filename)")]
        string name,
        [Description("Code content")]
        string code,
        [Description("Optional description of the snippet")]
        string? description = null)
    {
        return _notesService.SaveSnippetAsync(language, name, code, description);
    }
}