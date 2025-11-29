using Ateliers.Ai.McpServer.Configuration;
using Ateliers.Ai.McpServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

// UTF-8エンコーディングを明示的に設定
Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

// ログを完全に無効化（stdioの干渉を防ぐ）
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.None);

// 実行ファイルのディレクトリを取得
var assemblyLocation = Assembly.GetExecutingAssembly().Location;
var assemblyDirectory = Path.GetDirectoryName(assemblyLocation)!;

// 設定ファイルを実行ファイルと同じディレクトリから読み込む
builder.Configuration.Sources.Clear();
builder.Configuration
    .SetBasePath(assemblyDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .AddJsonFile("notionsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile("notionsettings.local.json", optional: true, reloadOnChange: true);

// 設定をDIコンテナに登録
builder.Services.Configure<AppSettings>(builder.Configuration);

// AppSettings をシングルトンとして登録（GitOperationService用）
builder.Services.AddSingleton(provider =>
{
    var appSettings = new AppSettings();
    builder.Configuration.Bind(appSettings);
    return appSettings;
});

// メモリキャッシュを追加
builder.Services.AddMemoryCache();

// Serviceをシングルトンとして登録
builder.Services.AddSingleton<GitHubService>();
builder.Services.AddSingleton<NotesService>();
builder.Services.AddSingleton<GitHubNotesService>();
builder.Services.AddSingleton<LocalFileService>();
builder.Services.AddSingleton<GitOperationService>();
builder.Services.AddSingleton<NotionService>();

// MCPサーバー設定
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
await app.RunAsync();
