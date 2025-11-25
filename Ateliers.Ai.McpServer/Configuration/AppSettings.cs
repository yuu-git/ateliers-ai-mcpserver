namespace Ateliers.Ai.McpServer.Configuration;

/// <summary>
/// アプリケーション設定
/// </summary>
public class AppSettings
{
    /// <summary>GitHub共通設定</summary>
    public GitHubSettings GitHub { get; set; } = new();

    /// <summary>リポジトリ設定（キー：リポジトリ識別子）</summary>
    public Dictionary<string, RepositoryConfig> Repositories { get; set; } = new();
}

/// <summary>
/// GitHub共通設定
/// </summary>
public class GitHubSettings
{
    /// <summary>認証モード（Anonymous or PAT）</summary>
    public string AuthenticationMode { get; set; } = "Anonymous";

    /// <summary>Personal Access Token</summary>
    public string? PersonalAccessToken { get; set; }

    /// <summary>キャッシュ有効期限（分）</summary>
    public int CacheExpirationMinutes { get; set; } = 5;
}

/// <summary>
/// リポジトリ設定
/// </summary>
public class RepositoryConfig
{
    /// <summary>データソース（GitHub or Local）</summary>
    public string Source { get; set; } = "GitHub";

    /// <summary>GitHub設定</summary>
    public GitHubSourceConfig? GitHub { get; set; }

    /// <summary>ローカルパス（設定時はローカル優先）</summary>
    public string? LocalPath { get; set; }

    /// <summary>書き込み前に自動プル</summary>
    public bool AutoPull { get; set; } = false;

    /// <summary>書き込み後に自動プッシュ</summary>
    public bool AutoPush { get; set; } = false;
}

/// <summary>
/// GitHubソース設定
/// </summary>
public class GitHubSourceConfig
{
    /// <summary>リポジトリOwner</summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>リポジトリ名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>ブランチ名</summary>
    public string Branch { get; set; } = "master";
}