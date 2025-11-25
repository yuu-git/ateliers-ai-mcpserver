namespace Ateliers.Ai.McpServer.Services;

/// <summary>
/// ローカルファイルシステム操作サービス
/// </summary>
public class LocalFileService
{
    /// <summary>
    /// ファイルを読み取る
    /// </summary>
    public async Task<string> ReadFileAsync(string basePath, string filePath)
    {
        var fullPath = Path.Combine(basePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return await File.ReadAllTextAsync(fullPath);
    }

    /// <summary>
    /// ファイル一覧を取得
    /// </summary>
    public async Task<List<string>> ListFilesAsync(
        string basePath,
        string directory = "",
        string? extension = null)
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

    /// <summary>
    /// 新規ファイルを作成
    /// </summary>
    public async Task CreateFileAsync(string basePath, string filePath, string content)
    {
        var fullPath = Path.Combine(basePath, filePath);

        if (File.Exists(fullPath))
        {
            throw new InvalidOperationException($"File already exists: {filePath}");
        }

        var directory = Path.GetDirectoryName(fullPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(fullPath, content);
    }

    /// <summary>
    /// 既存ファイルを更新
    /// </summary>
    public async Task UpdateFileAsync(string basePath, string filePath, string content, bool createBackup = true)
    {
        var fullPath = Path.Combine(basePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        // バックアップ作成
        if (createBackup)
        {
            File.Copy(fullPath, $"{fullPath}.backup", overwrite: true);
        }

        await File.WriteAllTextAsync(fullPath, content);
    }

    /// <summary>
    /// ファイルを削除
    /// </summary>
    public void DeleteFile(string basePath, string filePath, bool createBackup = true)
    {
        var fullPath = Path.Combine(basePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        // .backupファイルはバックアップを作らない
        var shouldBackup = createBackup && !filePath.EndsWith(".backup");

        if (shouldBackup)
        {
            File.Copy(fullPath, $"{fullPath}.backup", overwrite: true);
        }

        File.Delete(fullPath);
    }

    /// <summary>
    /// ファイルをリネーム
    /// </summary>
    public void RenameFile(string basePath, string oldFilePath, string newFilePath)
    {
        var oldFullPath = Path.Combine(basePath, oldFilePath);
        var newFullPath = Path.Combine(basePath, newFilePath);

        if (!File.Exists(oldFullPath))
        {
            throw new FileNotFoundException($"File not found: {oldFilePath}");
        }

        if (File.Exists(newFullPath))
        {
            throw new InvalidOperationException($"Destination file already exists: {newFilePath}");
        }

        var newDirectory = Path.GetDirectoryName(newFullPath);
        if (newDirectory != null && !Directory.Exists(newDirectory))
        {
            Directory.CreateDirectory(newDirectory);
        }

        File.Move(oldFullPath, newFullPath);
    }
    /// <summary>
    /// ファイルをコピー
    /// </summary>
    public void CopyFile(string basePath, string sourceFilePath, string destFilePath, bool overwrite = false)
    {
        var sourceFullPath = Path.Combine(basePath, sourceFilePath);
        var destFullPath = Path.Combine(basePath, destFilePath);

        if (!File.Exists(sourceFullPath))
        {
            throw new FileNotFoundException($"Source file not found: {sourceFilePath}");
        }

        if (File.Exists(destFullPath) && !overwrite)
        {
            throw new InvalidOperationException($"Destination file already exists: {destFilePath}");
        }

        var destDirectory = Path.GetDirectoryName(destFullPath);
        if (destDirectory != null && !Directory.Exists(destDirectory))
        {
            Directory.CreateDirectory(destDirectory);
        }

        File.Copy(sourceFullPath, destFullPath, overwrite);
    }

    /// <summary>
    /// ファイルのバックアップを作成
    /// </summary>
    public void BackupFile(string basePath, string filePath, string? backupSuffix = null)
    {
        var fullPath = Path.Combine(basePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var backupPath = backupSuffix != null
            ? $"{fullPath}.{backupSuffix}"
            : $"{fullPath}.backup";

        File.Copy(fullPath, backupPath, overwrite: true);
    }
}