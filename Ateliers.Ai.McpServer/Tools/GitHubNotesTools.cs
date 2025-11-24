using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public class GitHubNotesTools
{
    private readonly GitHubNotesService _service;

    public GitHubNotesTools(GitHubNotesService service)
    {
        _service = service;
    }

    [McpServerTool]
    [Description("Add or update a file in ateliers-public-notes repository")]
    public Task<string> AddPublicNote(
        [Description("File path (e.g., 'todo/current.md', 'ideas/technical.md')")]
        string path,
        [Description("File content")]
        string content,
        [Description("Commit message")]
        string commitMessage = "Update via MCP")
    {
        return _service.CreateOrUpdateFileAsync("PublicNotes", path, content, commitMessage);
    }

    [McpServerTool]
    [Description("Add or update a guideline file in ateliers-ai-assistants")]
    public Task<string> AddGuideline(
        [Description("File path (e.g., 'guidelines/csharp/naming.md')")]
        string path,
        [Description("Guideline content")]
        string content,
        [Description("Commit message")]
        string commitMessage = "Add guideline via MCP")
    {
        return _service.CreateOrUpdateFileAsync("AteliersAiAssistants", path, content, commitMessage);
    }

    [McpServerTool]
    [Description("Add or update a file in ateliers-training-mcpserver-claude repository")]
    public Task<string> AddTrainingDocument(
        [Description("File path (e.g., 'docs/phase4-plan.md')")]
        string path,
        [Description("Document content")]
        string content,
        [Description("Commit message")]
        string commitMessage = "Add document via MCP")
    {
        return _service.CreateOrUpdateFileAsync("TrainingMcpServer", path, content, commitMessage);
    }
}