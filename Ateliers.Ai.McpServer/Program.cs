using Ateliers.Ai.McpServer.Configuration;
using Ateliers.Ai.McpServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

// 設定をDIコンテナに登録
builder.Services.Configure<AppSettings>(builder.Configuration);

// メモリキャッシュを追加
builder.Services.AddMemoryCache();

// GitHubServiceをシングルトンとして登録
builder.Services.AddSingleton<GitHubService>();

// NotesServiceをシングルトンとして登録
builder.Services.AddSingleton<NotesService>();

// MCPサーバー設定
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
await app.RunAsync();