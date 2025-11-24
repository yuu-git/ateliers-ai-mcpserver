using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Ateliers.Ai.McpServer.Services;

namespace Ateliers.Ai.McpServer.Tools;

/// <summary>
/// Ateliers.dev 技術記事参照ツール
/// </summary>
[McpServerToolType]
public class AteliersDevTools
{
    private readonly GitHubService _gitHubService;

    public AteliersDevTools(GitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    /// <summary>
    /// Docusaurus記事を読み取る
    /// </summary>
    [McpServerTool]
    [Description("Read a technical article from ateliers.dev documentation")]
    public async Task<string> ReadArticle(
        [Description("Relative path to the article file (e.g., 'docs/csharp/datetime-extensions.md')")]
        string filePath)
    {
        try
        {
            var content = await _gitHubService.GetFileContentAsync("AteliersDev", filePath);

            // Frontmatterを除去してMarkdown本文のみを返す
            var markdown = RemoveFrontmatter(content);

            return markdown;
        }
        catch (FileNotFoundException ex)
        {
            return $"Article not found: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error reading article: {ex.Message}";
        }
    }

    /// <summary>
    /// 記事の一覧を取得
    /// </summary>
    [McpServerTool]
    [Description("List all technical articles in ateliers.dev documentation")]
    public async Task<string> ListArticles(
        [Description("Directory to search (default: 'docs', can also be 'blog')")]
        string directory = "docs")
    {
        try
        {
            var files = await _gitHubService.ListFilesAsync(
                "AteliersDev",
                directory: directory,
                extension: ".md"
            );

            // .mdxファイルも追加で取得
            var mdxFiles = await _gitHubService.ListFilesAsync(
                "AteliersDev",
                directory: directory,
                extension: ".mdx"
            );

            var allFiles = files.Concat(mdxFiles).OrderBy(f => f).ToList();

            if (allFiles.Count == 0)
            {
                return $"No articles found in '{directory}' directory.";
            }

            return string.Join("\n", allFiles);
        }
        catch (Exception ex)
        {
            return $"Error listing articles: {ex.Message}";
        }
    }

    /// <summary>
    /// キーワードで記事を検索
    /// </summary>
    [McpServerTool]
    [Description("Search technical articles by keyword in ateliers.dev documentation")]
    public async Task<string> SearchArticles(
        [Description("Keyword to search for in article titles and content")]
        string keyword,
        [Description("Directory to search (default: 'docs')")]
        string directory = "docs")
    {
        try
        {
            var files = await _gitHubService.ListFilesAsync(
                "AteliersDev",
                directory: directory,
                extension: ".md"
            );

            var mdxFiles = await _gitHubService.ListFilesAsync(
                "AteliersDev",
                directory: directory,
                extension: ".mdx"
            );

            var allFiles = files.Concat(mdxFiles).ToList();
            var results = new List<string>();

            foreach (var file in allFiles)
            {
                // ファイル名にキーワードが含まれているか
                if (file.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add($"📄 {file} (matched in filename)");
                    continue;
                }

                // ファイル内容を検索
                try
                {
                    var content = await _gitHubService.GetFileContentAsync("AteliersDev", file);
                    var markdown = RemoveFrontmatter(content);

                    if (markdown.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        // マッチした行を抽出（最大3行）
                        var matchedLines = GetMatchedLines(markdown, keyword, maxLines: 3);
                        results.Add($"📄 {file}\n{matchedLines}");
                    }
                }
                catch
                {
                    // ファイル読み取りエラーは無視
                }
            }

            if (results.Count == 0)
            {
                return $"No articles found matching keyword '{keyword}' in '{directory}' directory.";
            }

            return string.Join("\n\n", results);
        }
        catch (Exception ex)
        {
            return $"Error searching articles: {ex.Message}";
        }
    }

    /// <summary>
    /// Frontmatter（---で囲まれたメタデータ）を除去
    /// </summary>
    private string RemoveFrontmatter(string content)
    {
        // Frontmatterのパターン: 先頭の"---"から次の"---"まで
        var pattern = @"^---\s*\n.*?\n---\s*\n";
        var result = Regex.Replace(content, pattern, "", RegexOptions.Singleline);
        return result.Trim();
    }

    /// <summary>
    /// キーワードにマッチした行を抽出
    /// </summary>
    private string GetMatchedLines(string content, string keyword, int maxLines)
    {
        var lines = content.Split('\n');
        var matchedLines = lines
            .Where(line => line.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Take(maxLines)
            .Select(line => $"  > {line.Trim()}");

        return string.Join("\n", matchedLines);
    }
}