using Ateliers.Ai.McpServer.Configuration;
using Microsoft.Extensions.Options;
using System.Text;

namespace Ateliers.Ai.McpServer.Services;

/// <summary>
/// ローカルMarkdownファイルへのメモ管理サービス
/// </summary>
public class NotesService
{
    private readonly string _basePath;

    public NotesService(IOptions<AppSettings> settings)
    {
        try
        {
            // Dictionary方式でNotesリポジトリ設定を取得
            if (!settings.Value.Repositories.TryGetValue("Notes", out var notesSettings))
            {
                var errorMsg = $"Notes repository is not configured in appsettings.json. Available keys: {string.Join(", ", settings.Value.Repositories.Keys)}";
                throw new InvalidOperationException(errorMsg);
            }

            // 相対パスを絶対パスに解決
            var localPath = notesSettings.LocalPath
                ?? throw new InvalidOperationException("Notes.LocalPath is not configured");

            _basePath = Path.Combine(AppContext.BaseDirectory, localPath);

            // パス情報をログ出力（デバッグ用）
            Console.Error.WriteLine($"[NotesService] Base directory: {AppContext.BaseDirectory}");
            Console.Error.WriteLine($"[NotesService] Local path: {localPath}");
            Console.Error.WriteLine($"[NotesService] Full path: {_basePath}");

            // 初回実行時にディレクトリ構造を自動作成
            InitializeNotesDirectory();

            Console.Error.WriteLine($"[NotesService] Initialization completed successfully");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[NotesService] Initialization failed: {ex.Message}");
            Console.Error.WriteLine($"[NotesService] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// notes/ディレクトリ構造の初期化
    /// </summary>
    private void InitializeNotesDirectory()
    {
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
            CreateInitialStructure();
        }
    }

    /// <summary>
    /// 初期ディレクトリとファイルの作成
    /// </summary>
    private void CreateInitialStructure()
    {
        // ディレクトリ作成
        Directory.CreateDirectory(Path.Combine(_basePath, "todo"));
        Directory.CreateDirectory(Path.Combine(_basePath, "ideas"));
        Directory.CreateDirectory(Path.Combine(_basePath, "snippets", "csharp"));
        Directory.CreateDirectory(Path.Combine(_basePath, "snippets", "python"));
        Directory.CreateDirectory(Path.Combine(_basePath, "snippets", "sql"));

        // 初期ファイル作成
        CreateInitialFile(
            Path.Combine(_basePath, "todo", "current.md"),
            """
            <!-- ⚠️ このファイルはMCPツール経由で自動更新されます -->
            <!-- 手動編集する場合は、フォーマットを崩さないよう注意してください -->

            # Current TODO

            ## In Progress

            <!-- ここにTODOが追加されます -->

            ## Completed

            <!-- 完了したTODOは手動でここに移動できます -->

            """);

        CreateInitialFile(
            Path.Combine(_basePath, "ideas", "technical.md"),
            """
            <!-- ⚠️ このファイルはMCPツール経由で自動更新されます -->

            # Technical Ideas

            <!-- 技術的なアイデアがここに追加されます -->

            """);

        CreateInitialFile(
            Path.Combine(_basePath, "ideas", "article.md"),
            """
            <!-- ⚠️ このファイルはMCPツール経由で自動更新されます -->

            # Article Ideas

            <!-- 記事のアイデアがここに追加されます -->

            """);

        CreateInitialFile(
            Path.Combine(_basePath, "ideas", "project.md"),
            """
            <!-- ⚠️ このファイルはMCPツール経由で自動更新されます -->

            # Project Ideas

            <!-- プロジェクトのアイデアがここに追加されます -->

            """);
    }

    /// <summary>
    /// 初期ファイルの作成（既存ファイルは上書きしない）
    /// </summary>
    private static void CreateInitialFile(string path, string content)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, content);
        }
    }

    /// <summary>
    /// TODOを追加
    /// </summary>
    public async Task<string> AddTodoAsync(string content)
    {
        // NOTE: このサンプル実装では排他制御を省略しています
        // 実用版では以下を検討してください:
        // - SemaphoreSlim による排他制御
        // - FileShare.None でのファイルロック
        // - 同時書き込みの検出とリトライ

        var path = Path.Combine(_basePath, "todo", "current.md");
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var todoItem = $"- [ ] {content} (追加: {timestamp})";

        // バックアップ作成
        if (File.Exists(path))
        {
            File.Copy(path, $"{path}.backup", overwrite: true);
        }

        try
        {
            // ファイル読み込み
            var lines = (await File.ReadAllLinesAsync(path)).ToList();

            // "## In Progress" セクションを探す
            var insertIndex = lines.FindIndex(l => l.StartsWith("## In Progress"));
            if (insertIndex == -1)
            {
                return "❌ ファイルフォーマットが正しくありません（'## In Progress' セクションが見つかりません）";
            }

            // 空行をスキップして挿入位置を決定
            insertIndex += 1;
            while (insertIndex < lines.Count && string.IsNullOrWhiteSpace(lines[insertIndex]))
            {
                insertIndex++;
            }

            // TODO挿入
            lines.Insert(insertIndex, todoItem);

            // ファイル書き込み
            await File.WriteAllLinesAsync(path, lines);

            return "✅ TODOを追加しました";
        }
        catch (IOException ex)
        {
            return $"⚠️ ファイル書き込みに失敗しました: {ex.Message}";
        }
        catch (UnauthorizedAccessException ex)
        {
            return $"❌ アクセス権限がありません: {ex.Message}";
        }
    }

    /// <summary>
    /// TODO一覧を取得
    /// </summary>
    public async Task<string> ListTodosAsync()
    {
        var path = Path.Combine(_basePath, "todo", "current.md");

        try
        {
            if (!File.Exists(path))
            {
                return "📝 TODOファイルがまだ作成されていません";
            }

            var content = await File.ReadAllTextAsync(path);

            // "## In Progress" セクションのTODOのみ抽出
            var lines = content.Split('\n');
            var inProgressSection = false;
            var todos = new List<string>();

            foreach (var line in lines)
            {
                if (line.StartsWith("## In Progress"))
                {
                    inProgressSection = true;
                    continue;
                }
                if (line.StartsWith("## ") && inProgressSection)
                {
                    break; // 次のセクションに到達
                }
                if (inProgressSection && line.TrimStart().StartsWith("- [ ]"))
                {
                    todos.Add(line.Trim());
                }
            }

            if (todos.Count == 0)
            {
                return "📝 現在TODOはありません";
            }

            return $"📋 現在のTODO ({todos.Count}件):\n\n" + string.Join("\n", todos);
        }
        catch (IOException ex)
        {
            return $"⚠️ ファイル読み込みに失敗しました: {ex.Message}";
        }
    }

    /// <summary>
    /// アイデアを追加
    /// </summary>
    public async Task<string> AddIdeaAsync(string category, string content)
    {
        // カテゴリの検証
        var validCategories = new[] { "technical", "article", "project" };
        if (!validCategories.Contains(category))
        {
            return $"❌ 無効なカテゴリです。使用可能: {string.Join(", ", validCategories)}";
        }

        var path = Path.Combine(_basePath, "ideas", $"{category}.md");
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var ideaSection = $"""

            ## {timestamp}

            {content}

            ---

            """;

        // バックアップ作成
        if (File.Exists(path))
        {
            File.Copy(path, $"{path}.backup", overwrite: true);
        }

        try
        {
            // ファイルの末尾に追加
            await File.AppendAllTextAsync(path, ideaSection);

            return $"✅ {category} アイデアを追加しました";
        }
        catch (IOException ex)
        {
            return $"⚠️ ファイル書き込みに失敗しました: {ex.Message}";
        }
        catch (UnauthorizedAccessException ex)
        {
            return $"❌ アクセス権限がありません: {ex.Message}";
        }
    }

    /// <summary>
    /// アイデア一覧を取得
    /// </summary>
    public async Task<string> ListIdeasAsync(string category)
    {
        // カテゴリの検証
        var validCategories = new[] { "technical", "article", "project" };
        if (!validCategories.Contains(category))
        {
            return $"❌ 無効なカテゴリです。使用可能: {string.Join(", ", validCategories)}";
        }

        var path = Path.Combine(_basePath, "ideas", $"{category}.md");

        try
        {
            if (!File.Exists(path))
            {
                return $"📝 {category} アイデアファイルがまだ作成されていません";
            }

            var content = await File.ReadAllTextAsync(path);
            return content;
        }
        catch (IOException ex)
        {
            return $"⚠️ ファイル読み込みに失敗しました: {ex.Message}";
        }
    }

    public async Task<string> SaveSnippetAsync(string language, string name, string code, string? description = null)
    {
        // 言語の検証
        var validLanguages = new[] { "csharp", "python", "sql" };
        if (!validLanguages.Contains(language))
        {
            return $"❌ 無効な言語です。使用可能: {string.Join(", ", validLanguages)}";
        }

        // ファイル名のサニタイズ（安全な文字のみ許可）
        var safeName = string.Concat(name.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'));
        if (string.IsNullOrEmpty(safeName))
        {
            return "❌ 無効なファイル名です";
        }

        var path = Path.Combine(_basePath, "snippets", language, $"{safeName}.md");
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // StringBuilderで構築
        var sb = new StringBuilder();
        sb.AppendLine($"# {name}");
        sb.AppendLine();
        sb.AppendLine($"追加日: {timestamp}");
        sb.AppendLine();

        if (description != null)
        {
            sb.AppendLine("## 説明");
            sb.AppendLine();
            sb.AppendLine(description);
            sb.AppendLine();
        }

        sb.AppendLine("## コード");
        sb.AppendLine();
        sb.AppendLine($"```{language}");
        sb.AppendLine(code);
        sb.AppendLine("```");
        sb.AppendLine();

        var snippetContent = sb.ToString();

        // バックアップ作成（既存ファイルの場合）
        if (File.Exists(path))
        {
            File.Copy(path, $"{path}.backup", overwrite: true);
        }

        try
        {
            await File.WriteAllTextAsync(path, snippetContent);
            return $"✅ {language} スニペット '{safeName}' を保存しました";
        }
        catch (IOException ex)
        {
            return $"⚠️ ファイル書き込みに失敗しました: {ex.Message}";
        }
        catch (UnauthorizedAccessException ex)
        {
            return $"❌ アクセス権限がありません: {ex.Message}";
        }
    }
}