using ModelContextProtocol.Server;
using System.ComponentModel;
using Ateliers.Ai.McpServer.Services;

namespace Ateliers.Ai.McpServer.Tools;

/// <summary>
/// AteliersAiMcpServer リポジトリ操作ツール
/// </summary>
[McpServerToolType]
public class AteliersAiMcpServerTools
{
    private readonly GitHubService _gitHubService;

    public AteliersAiMcpServerTools(GitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    /// <summary>
    /// リポジトリ内の全ファイル一覧を取得
    /// </summary>
    [McpServerTool]
    [Description("List all files in ateliers-ai-mcpserver repository")]
    public async Task<string> ListAllFiles()
    {
        try
        {
            var files = await _gitHubService.ListFilesAsync(
                "AteliersAiMcpServer",
                directory: ""
            );

            if (files.Count == 0)
            {
                return "No files found.";
            }

            return string.Join("\n", files.OrderBy(f => f));
        }
        catch (Exception ex)
        {
            return $"Error listing files: {ex.Message}";
        }
    }
}
