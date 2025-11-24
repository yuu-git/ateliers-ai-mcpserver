namespace Ateliers.Ai.McpServer.Configuration;

/// <summary>
/// アプリケーション設定のルート
/// </summary>
public class AppSettings
{
    public GitHubSettings GitHub { get; set; } = new();
    public Dictionary<string, RepositorySettings> Repositories { get; set; } = new();
}

/// <summary>
/// GitHub接続設定
/// </summary>
public class GitHubSettings
{
    /// <summary>
    /// 認証モード: Anonymous または PersonalAccessToken
    /// </summary>
    public string AuthenticationMode { get; set; } = "Anonymous";

    /// <summary>
    /// Personal Access Token (認証モードがPersonalAccessTokenの場合に使用)
    /// </summary>
    public string? PersonalAccessToken { get; set; }

    /// <summary>
    /// キャッシュの有効期限（分）
    /// </summary>
    public int CacheExpirationMinutes { get; set; } = 5;
}

/// <summary>
/// リポジトリ設定
/// </summary>
public class RepositorySettings
{
    /// <summary>
    /// データソース: GitHub または Local
    /// </summary>
    public string Source { get; set; } = "GitHub";

    /// <summary>
    /// GitHub設定
    /// </summary>
    public GitHubRepositorySettings? GitHub { get; set; }

    /// <summary>
    /// ローカルパス設定
    /// </summary>
    public string? LocalPath { get; set; }

    /// <summary>
    /// ディレクトリ設定（Docusaurus用）
    /// </summary>
    public DirectorySettings? Directories { get; set; }
}

/// <summary>
/// GitHubリポジトリ設定
/// </summary>
public class GitHubRepositorySettings
{
    /// <summary>
    /// リポジトリのオーナー
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// リポジトリ名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ブランチ名
    /// </summary>
    public string Branch { get; set; } = "main";
}

/// <summary>
/// ディレクトリ設定
/// </summary>
public class DirectorySettings
{
    /// <summary>
    /// Docsディレクトリ
    /// </summary>
    public string Docs { get; set; } = "docs";

    /// <summary>
    /// Blogディレクトリ
    /// </summary>
    public string Blog { get; set; } = "blog";
}