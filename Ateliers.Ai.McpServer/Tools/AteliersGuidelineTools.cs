using ModelContextProtocol.Server;
using System.ComponentModel;
using Ateliers.Ai.McpServer.Services;

namespace Ateliers.Ai.McpServer.Tools;

/// <summary>
/// Ateliers AI Assistants ガイドライン参照ツール
/// </summary>
[McpServerToolType]
public class AteliersGuidelineTools
{
    private readonly GitHubService _gitHubService;

    public AteliersGuidelineTools(GitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    /// <summary>
    /// ガイドラインファイルを読み取る
    /// </summary>
    [McpServerTool]
    [Description("Read a guideline file from ateliers-ai-assistants repository")]
    public async Task<string> ReadGuideline(
        [Description("Relative path to the guideline file (e.g., 'README.md', 'guidelines/coding.md')")]
        string filePath)
    {
        try
        {
            var content = await _gitHubService.GetFileContentAsync("AteliersAiAssistants", filePath);
            return content;
        }
        catch (FileNotFoundException ex)
        {
            return $"File not found: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error reading file: {ex.Message}";
        }
    }

    /// <summary>
    /// ガイドラインファイルの一覧を取得
    /// </summary>
    [McpServerTool]
    [Description("List all guideline markdown files in ateliers-ai-assistants repository")]
    public async Task<string> ListGuidelines()
    {
        try
        {
            var files = await _gitHubService.ListFilesAsync(
                "AteliersAiAssistants",
                directory: "",
                extension: ".md"
            );

            if (files.Count == 0)
            {
                return "No markdown files found.";
            }

            return string.Join("\n", files.OrderBy(f => f));
        }
        catch (Exception ex)
        {
            return $"Error listing files: {ex.Message}";
        }
    }
}