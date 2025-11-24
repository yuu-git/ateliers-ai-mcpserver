using Ateliers.Ai.McpServer.Configuration;
using Microsoft.Extensions.Options;
using Octokit;

public class GitHubNotesService
{
    private readonly GitHubClient _client;
    private readonly AppSettings _settings;

    public GitHubNotesService(IOptions<AppSettings> settings)
    {
        _settings = settings.Value;

        var productHeader = new ProductHeaderValue("Ateliers-AI-McpServer");
        _client = new GitHubClient(productHeader);

        // PAT認証
        if (_settings.GitHub.AuthenticationMode == "PersonalAccessToken")
        {
            var tokenAuth = new Credentials(_settings.GitHub.PersonalAccessToken);
            _client.Credentials = tokenAuth;
        }
    }

    /// <summary>
    /// ファイル作成または更新
    /// </summary>
    public async Task<string> CreateOrUpdateFileAsync(
        string repositoryName,
        string path,
        string content,
        string commitMessage)
    {
        if (!_settings.Repositories.TryGetValue(repositoryName, out var repo))
        {
            return $"❌ Repository '{repositoryName}' not configured";
        }

        var owner = repo.GitHub.Owner;
        var name = repo.GitHub.Name;
        var branch = repo.GitHub.Branch;

        try
        {
            // ファイルが存在するか確認
            IReadOnlyList<RepositoryContent> existingFile;
            try
            {
                existingFile = await _client.Repository.Content.GetAllContents(owner, name, path);
            }
            catch (NotFoundException)
            {
                existingFile = null;
            }

            // 作成 or 更新
            if (existingFile == null)
            {
                // 新規作成
                var createRequest = new CreateFileRequest(commitMessage, content, branch);
                await _client.Repository.Content.CreateFile(owner, name, path, createRequest);
                return $"✅ Created: {path}";
            }
            else
            {
                // 更新
                var sha = existingFile[0].Sha;
                var updateRequest = new UpdateFileRequest(commitMessage, content, sha, branch);
                await _client.Repository.Content.UpdateFile(owner, name, path, updateRequest);
                return $"✅ Updated: {path}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ GitHub API error: {ex.Message}";
        }
    }
}