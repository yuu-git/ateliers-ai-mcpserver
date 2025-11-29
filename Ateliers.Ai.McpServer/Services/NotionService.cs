using Microsoft.Extensions.Configuration;
using Notion.Client;

namespace Ateliers.Ai.McpServer.Services;

/// <summary>
/// Notion APIとの通信を担当するサービス
/// </summary>
public class NotionService
{
    private readonly INotionClient _client;
    private readonly IConfiguration _configuration;

    public NotionService(IConfiguration configuration)
    {
        _configuration = configuration;

        // Notion APIトークン取得
        var apiToken = configuration["Notion:ApiToken"];
        if (string.IsNullOrWhiteSpace(apiToken))
        {
            throw new InvalidOperationException(
                "Notion API Token is not configured. Please set 'Notion:ApiToken' in notionsettings.local.json");
        }

        // Notion Clientを初期化
        _client = NotionClientFactory.Create(new ClientOptions
        {
            AuthToken = apiToken
        });
    }

    /// <summary>
    /// Notion Clientを取得（内部使用用）
    /// </summary>
    internal INotionClient Client => _client;

    /// <summary>
    /// データベースIDを取得
    /// </summary>
    public string GetDatabaseId(string databaseName)
    {
        var databaseId = _configuration[$"Notion:Databases:{databaseName}"];
        if (string.IsNullOrWhiteSpace(databaseId))
        {
            throw new InvalidOperationException(
                $"Database ID for '{databaseName}' is not configured. Please set 'Notion:Databases:{databaseName}' in notionsettings.local.json");
        }
        return databaseId;
    }
}
