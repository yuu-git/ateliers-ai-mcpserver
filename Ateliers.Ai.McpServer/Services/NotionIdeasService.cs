using Microsoft.Extensions.Configuration;
using Notion.Client;

namespace Ateliers.Ai.McpServer.Services;

/// <summary>
/// Notion Ideas データベースの操作を担当するサービス
/// </summary>
public class NotionIdeasService
{
    private readonly NotionService _notionService;
    private readonly IConfiguration _configuration;

    public NotionIdeasService(NotionService notionService, IConfiguration configuration)
    {
        _notionService = notionService;
        _configuration = configuration;
    }

    /// <summary>
    /// Ideas Database IDを取得
    /// </summary>
    private string GetIdeasDatabaseId() => _notionService.GetDatabaseId("Ideas");

    /// <summary>
    /// アイデアを追加
    /// </summary>
    public async Task<string> AddIdeaAsync(
        string title,
        string? content = null,
        string[]? categories = null,
        string[]? tags = null,
        string? relatedLinks = null,
        string? createdBy = null)
    {
        var databaseId = GetIdeasDatabaseId();

        var properties = new Dictionary<string, PropertyValue>
        {
            ["Name"] = new TitlePropertyValue
            {
                Title = new List<RichTextBase>
                {
                    new RichTextText { Text = new Text { Content = title } }
                }
            }
        };

        // Category (Multi-select)
        if (categories != null && categories.Length > 0)
        {
            properties["Category"] = new MultiSelectPropertyValue
            {
                MultiSelect = categories.Select(cat => new SelectOption { Name = cat }).ToList()
            };
        }

        // Tags
        if (tags != null && tags.Length > 0)
        {
            properties["Tags"] = new MultiSelectPropertyValue
            {
                MultiSelect = tags.Select(tag => new SelectOption { Name = tag }).ToList()
            };
        }

        // Status (デフォルト: アイデア)
        properties["Status"] = new SelectPropertyValue { Select = new SelectOption { Name = "アイデア" } };

        // Related Links
        if (!string.IsNullOrWhiteSpace(relatedLinks))
        {
            properties["Related Links"] = new UrlPropertyValue { Url = relatedLinks };
        }

        // Created By
        if (!string.IsNullOrWhiteSpace(createdBy))
        {
            properties["Created By"] = new SelectPropertyValue { Select = new SelectOption { Name = createdBy } };
        }

        var request = new PagesCreateParameters
        {
            Parent = new DatabaseParentInput { DatabaseId = databaseId },
            Properties = properties
        };

        // Content をページブロックとして追加
        if (!string.IsNullOrWhiteSpace(content))
        {
            request.Children = new List<IBlock>
            {
                new ParagraphBlock
                {
                    Paragraph = new ParagraphBlock.Info
                    {
                        RichText = new List<RichTextBase>
                        {
                            new RichTextText { Text = new Text { Content = content } }
                        }
                    }
                }
            };
        }

        var page = await _notionService.Client.Pages.CreateAsync(request);
        return $"Idea created successfully: {title} (ID: {page.Id})";
    }

    /// <summary>
    /// アイデアを検索
    /// </summary>
    public async Task<string> SearchIdeasAsync(
        string? keyword = null,
        string[]? categories = null,
        string[]? tags = null,
        int limit = 10)
    {
        var databaseId = GetIdeasDatabaseId();
        var filters = new List<Filter>();

        // キーワード検索（タイトル）
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            filters.Add(new TitleFilter("Name", contains: keyword));
        }

        // カテゴリフィルタ
        if (categories != null && categories.Length > 0)
        {
            foreach (var category in categories)
            {
                filters.Add(new MultiSelectFilter("Category", contains: category));
            }
        }

        // タグフィルタ
        if (tags != null && tags.Length > 0)
        {
            foreach (var tag in tags)
            {
                filters.Add(new MultiSelectFilter("Tags", contains: tag));
            }
        }

        var queryParams = new DatabasesQueryParameters
        {
            PageSize = limit
        };

        if (filters.Count > 0)
        {
            queryParams.Filter = filters.Count == 1
                ? filters[0]
                : new CompoundFilter { And = filters };
        }

        var response = await _notionService.Client.Databases.QueryAsync(databaseId, queryParams);

        if (response.Results.Count == 0)
        {
            return "No ideas found.";
        }

        var ideas = response.Results.Select(page =>
        {
            var props = (page as Page)?.Properties;
            var ideaTitle = page != null && props.ContainsKey("Name") && props["Name"] is TitlePropertyValue titleProp
                ? string.Join("", titleProp.Title.Select(t => t.PlainText))
                : "Untitled";

            var categoryList = page != null && props.ContainsKey("Category") && props["Category"] is MultiSelectPropertyValue catProp
                ? string.Join(", ", catProp.MultiSelect.Select(c => c.Name))
                : "未設定";

            return $"- {ideaTitle} (カテゴリ: {categoryList}, ID: {page.Id})";
        });

        return $"Ideas ({response.Results.Count}):\n" + string.Join("\n", ideas);
    }

    /// <summary>
    /// アイデアを更新
    /// </summary>
    public async Task<string> UpdateIdeaAsync(
        string ideaId,
        string? title = null,
        string? content = null,
        string[]? categories = null,
        string[]? tags = null,
        string? status = null,
        string? relatedLinks = null)
    {
        var properties = new Dictionary<string, PropertyValue>();

        if (!string.IsNullOrWhiteSpace(title))
        {
            properties["Name"] = new TitlePropertyValue
            {
                Title = new List<RichTextBase>
                {
                    new RichTextText { Text = new Text { Content = title } }
                }
            };
        }

        if (categories != null && categories.Length > 0)
        {
            properties["Category"] = new MultiSelectPropertyValue
            {
                MultiSelect = categories.Select(cat => new SelectOption { Name = cat }).ToList()
            };
        }

        if (tags != null && tags.Length > 0)
        {
            properties["Tags"] = new MultiSelectPropertyValue
            {
                MultiSelect = tags.Select(tag => new SelectOption { Name = tag }).ToList()
            };
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            properties["Status"] = new SelectPropertyValue { Select = new SelectOption { Name = status } };
        }

        if (!string.IsNullOrWhiteSpace(relatedLinks))
        {
            properties["Related Links"] = new UrlPropertyValue { Url = relatedLinks };
        }

        var request = new PagesUpdateParameters { Properties = properties };
        var page = await _notionService.Client.Pages.UpdateAsync(ideaId, request);

        // Content 更新時はページブロックを追加
        if (!string.IsNullOrWhiteSpace(content))
        {
            await _notionService.Client.Blocks.AppendChildrenAsync(new BlockAppendChildrenRequest
            {
                BlockId = ideaId,
                Children = new List<IBlockObjectRequest>
                {
                    new ParagraphBlockRequest
                    {
                        Paragraph = new ParagraphBlockRequest.Info
                        {
                            RichText = new List<RichTextBase>
                            {
                                new RichTextText { Text = new Text { Content = content } }
                            }
                        }
                    }
                }
            });
        }

        return $"Idea updated successfully (ID: {page.Id})";
    }
}
