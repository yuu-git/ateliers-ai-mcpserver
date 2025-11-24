using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;
using Ateliers.Ai.McpServer.Configuration;

namespace Ateliers.Ai.McpServer.Services;

/// <summary>
/// GitHubリポジトリからのファイル取得サービス
/// </summary>
public class GitHubService
{
    private readonly IGitHubClient _client;
    private readonly IMemoryCache _cache;
    private readonly AppSettings _settings;
    private readonly TimeSpan _cacheExpiration;

    public GitHubService(IOptions<AppSettings> settings, IMemoryCache cache)
    {
        _settings = settings.Value;
        _cache = cache;
        _cacheExpiration = TimeSpan.FromMinutes(_settings.GitHub.CacheExpirationMinutes);

        _client = new GitHubClient(new ProductHeaderValue("AteliersMcpServer"));

        if (_settings.GitHub.AuthenticationMode == "PersonalAccessToken"
            && !string.IsNullOrEmpty(_settings.GitHub.PersonalAccessToken))
        {
            _client.Connection.Credentials = new Credentials(_settings.GitHub.PersonalAccessToken);
        }
    }

    /// <summary>
    /// リポジトリからファイル内容を取得（キャッシュ付き）
    /// </summary>
    /// <param name="repositoryKey">設定ファイル内のリポジトリキー（例: "AteliersAiAssistants"）</param>
    /// <param name="filePath">ファイルパス</param>
    /// <returns>ファイル内容</returns>
    public async Task<string> GetFileContentAsync(string repositoryKey, string filePath)
    {
        // リポジトリ設定を取得
        if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
        {
            throw new ArgumentException($"Repository '{repositoryKey}' not found in configuration.");
        }

        if (repoSettings.Source == "Local" && !string.IsNullOrEmpty(repoSettings.LocalPath))
        {
            // ローカルファイルから取得
            return await GetLocalFileAsync(repoSettings.LocalPath, filePath);
        }

        if (repoSettings.Source == "GitHub" && repoSettings.GitHub != null)
        {
            // GitHubから取得（キャッシュ付き）
            return await GetGitHubFileAsync(
                repoSettings.GitHub.Owner,
                repoSettings.GitHub.Name,
                filePath,
                repoSettings.GitHub.Branch
            );
        }

        throw new InvalidOperationException($"Invalid repository configuration for '{repositoryKey}'.");
    }

    /// <summary>
    /// リポジトリ内のファイル一覧を取得（キャッシュ付き）
    /// </summary>
    /// <param name="repositoryKey">設定ファイル内のリポジトリキー</param>
    /// <param name="directory">ディレクトリパス（省略時はルート）</param>
    /// <param name="extension">拡張子フィルター（例: ".md"、省略時は全て）</param>
    /// <returns>ファイルパスのリスト</returns>
    public async Task<List<string>> ListFilesAsync(
        string repositoryKey,
        string directory = "",
        string? extension = null)
    {
        if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
        {
            throw new ArgumentException($"Repository '{repositoryKey}' not found in configuration.");
        }

        if (repoSettings.Source == "Local" && !string.IsNullOrEmpty(repoSettings.LocalPath))
        {
            return await ListLocalFilesAsync(repoSettings.LocalPath, directory, extension);
        }

        if (repoSettings.Source == "GitHub" && repoSettings.GitHub != null)
        {
            return await ListGitHubFilesAsync(
                repoSettings.GitHub.Owner,
                repoSettings.GitHub.Name,
                directory,
                repoSettings.GitHub.Branch,
                extension
            );
        }

        throw new InvalidOperationException($"Invalid repository configuration for '{repositoryKey}'.");
    }

    /// <summary>
    /// GitHubからファイル内容を取得（キャッシュ付き）
    /// </summary>
    private async Task<string> GetGitHubFileAsync(
        string owner,
        string repo,
        string path,
        string branch)
    {
        var cacheKey = $"github:{owner}/{repo}:{branch}:{path}";

        // キャッシュから取得を試みる
        if (_cache.TryGetValue(cacheKey, out string? cachedContent) && cachedContent != null)
        {
            return cachedContent;
        }

        // GitHubから取得
        try
        {
            var contents = await _client.Repository.Content.GetAllContentsByRef(
                owner,
                repo,
                path,
                branch
            );

            if (contents.Count == 0)
            {
                throw new FileNotFoundException($"File not found: {path}");
            }

            var content = contents[0].Content;

            // キャッシュに保存
            _cache.Set(cacheKey, content, _cacheExpiration);

            return content;
        }
        catch (NotFoundException)
        {
            throw new FileNotFoundException($"File not found: {path} in {owner}/{repo}");
        }
    }

    /// <summary>
    /// GitHub上のファイル一覧を取得（キャッシュ付き）
    /// </summary>
    private async Task<List<string>> ListGitHubFilesAsync(
        string owner,
        string repo,
        string directory,
        string branch,
        string? extension)
    {
        var cacheKey = $"github:list:{owner}/{repo}:{branch}:{directory}:{extension}";

        // キャッシュから取得を試みる
        if (_cache.TryGetValue(cacheKey, out List<string>? cachedList) && cachedList != null)
        {
            return cachedList;
        }

        // GitHubから取得
        var allFiles = new List<string>();

        // 空文字列をnullに正規化（ルートディレクトリの場合）
        var normalizedDirectory = string.IsNullOrEmpty(directory) ? null : directory;

        await CollectFilesRecursivelyAsync(owner, repo, normalizedDirectory, branch, allFiles, extension);

        // キャッシュに保存
        _cache.Set(cacheKey, allFiles, _cacheExpiration);

        return allFiles;
    }

    /// <summary>
    /// GitHub上のファイルを再帰的に収集
    /// </summary>
    private async Task CollectFilesRecursivelyAsync(
        string owner,
        string repo,
        string? path,
        string branch,
        List<string> files,
        string? extension)
    {
        try
        {
            IReadOnlyList<RepositoryContent> contents;

            // ルートディレクトリの場合はパスを省略
            if (string.IsNullOrEmpty(path))
            {
                contents = await _client.Repository.Content.GetAllContentsByRef(
                    owner,
                    repo,
                    branch
                );
            }
            else
            {
                contents = await _client.Repository.Content.GetAllContentsByRef(
                    owner,
                    repo,
                    path,
                    branch
                );
            }

            foreach (var item in contents)
            {
                if (item.Type == ContentType.File)
                {
                    // 拡張子フィルタ
                    if (extension == null || item.Path.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(item.Path);
                    }
                }
                else if (item.Type == ContentType.Dir)
                {
                    // ディレクトリの場合は再帰的に探索
                    await CollectFilesRecursivelyAsync(owner, repo, item.Path, branch, files, extension);
                }
            }
        }
        catch (NotFoundException)
        {
            // ディレクトリが存在しない場合は無視
        }
    }

    /// <summary>
    /// ローカルファイルから取得
    /// </summary>
    private async Task<string> GetLocalFileAsync(string basePath, string filePath)
    {
        var fullPath = Path.Combine(basePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return await File.ReadAllTextAsync(fullPath);
    }

    /// <summary>
    /// ローカルファイル一覧を取得
    /// </summary>
    private async Task<List<string>> ListLocalFilesAsync(
        string basePath,
        string directory,
        string? extension)
    {
        var searchPath = string.IsNullOrEmpty(directory)
            ? basePath
            : Path.Combine(basePath, directory);

        if (!Directory.Exists(searchPath))
        {
            return new List<string>();
        }

        var searchPattern = extension != null ? $"*{extension}" : "*";
        var files = Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(basePath, f).Replace("\\", "/"))
            .ToList();

        return await Task.FromResult(files);
    }
}