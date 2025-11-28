using ModelContextProtocol.Server;
using System.ComponentModel;
using Ateliers.Ai.McpServer.Services;
using Ateliers.Ai.McpServer.Configuration;
using Microsoft.Extensions.Options;

namespace Ateliers.Ai.McpServer.Tools;

/// <summary>
/// Git操作ツール（明示的なGit操作）
/// </summary>
[McpServerToolType]
public class GitTools
{
    private readonly GitOperationService _gitOperationService;
    private readonly AppSettings _settings;

    public GitTools(
        GitOperationService gitOperationService,
        IOptions<AppSettings> settings)
    {
        _gitOperationService = gitOperationService;
        _settings = settings.Value;
    }

    [McpServerTool]
    [Description(@"Pull changes from remote repository to local.

WHEN TO USE:
- Before starting work to get latest changes
- After someone else pushed changes
- To sync with remote before committing

DO NOT USE WHEN:
- No remote repository configured
- Working on detached HEAD
- In the middle of merge conflict

EXAMPLES:
✓ 'Pull latest changes from AteliersAiMcpServer'
✓ 'Sync PublicNotes repository with remote'
✓ 'Get remote changes before editing'

RELATED TOOLS:
- push_repository: Push local changes to remote
- commit_repository: Commit local changes first
- commit_and_push_repository: Commit and push in one step")]
    public async Task<string> PullRepository(
        [Description("Repository key: AteliersAiAssistants, AteliersAiMcpServer, AteliersDev, PublicNotes, TrainingMcpServer")]
        string repositoryKey)
    {
        try
        {
            if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
            {
                return $"❌ Repository '{repositoryKey}' not found";
            }

            if (string.IsNullOrEmpty(repoSettings.LocalPath))
            {
                return $"❌ LocalPath not configured for '{repositoryKey}'";
            }

            var result = await _gitOperationService.PullAsync(repositoryKey, repoSettings.LocalPath);

            if (!result.Success)
            {
                if (result.HasConflict)
                    return $"⚠️ Merge conflict:\n{result.Message}";
                
                return $"❌ Pull failed: {result.Message}";
            }

            return $"✅ {result.Message}";
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description(@"Commit all changes in the repository with a message.

WHEN TO USE:
- After making multiple file changes
- Ready to create a commit snapshot
- Before pushing changes to remote
- Creating a logical checkpoint in development

DO NOT USE WHEN:
- No changes to commit
- Want to commit only specific files (use git commands directly)
- In the middle of merge conflict

EXAMPLES:
✓ 'Commit all changes in AteliersAiMcpServer with message Phase 5 complete'
✓ 'Commit changes to PublicNotes with message Update TODO list'
✓ 'Create commit in AteliersDev: Add new blog post'

RELATED TOOLS:
- push_repository: Push commits to remote
- commit_and_push_repository: Commit and push in one step
- pull_repository: Get latest changes before committing")]
    public async Task<string> CommitRepository(
        [Description("Repository key: AteliersAiAssistants, AteliersAiMcpServer, AteliersDev, PublicNotes, TrainingMcpServer")]
        string repositoryKey,
        [Description("Commit message (e.g., 'Phase 5: Add Git integration')")]
        string message)
    {
        try
        {
            if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
            {
                return $"❌ Repository '{repositoryKey}' not found";
            }

            if (string.IsNullOrEmpty(repoSettings.LocalPath))
            {
                return $"❌ LocalPath not configured for '{repositoryKey}'";
            }

            var result = await _gitOperationService.CommitAllAsync(
                repositoryKey, 
                repoSettings.LocalPath, 
                message);

            if (!result.Success)
                return $"❌ Commit failed: {result.Message}";

            if (result.CommitHash == null)
                return $"ℹ️ {result.Message}";

            return $"✅ {result.Message}\nCommit: {result.CommitHash[..7]}";
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description(@"Push committed changes to remote repository.

WHEN TO USE:
- After committing local changes
- Ready to share changes with team
- Backing up commits to remote
- Publishing completed work

DO NOT USE WHEN:
- No commits to push
- Remote repository not configured
- In the middle of merge conflict

EXAMPLES:
✓ 'Push commits from AteliersAiMcpServer to GitHub'
✓ 'Upload local commits to remote PublicNotes'
✓ 'Publish changes to AteliersDev'

RELATED TOOLS:
- commit_repository: Commit changes first
- commit_and_push_repository: Commit and push in one step
- pull_repository: Get latest changes before pushing")]
    public async Task<string> PushRepository(
        [Description("Repository key: AteliersAiAssistants, AteliersAiMcpServer, AteliersDev, PublicNotes, TrainingMcpServer")]
        string repositoryKey)
    {
        try
        {
            if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
            {
                return $"❌ Repository '{repositoryKey}' not found";
            }

            if (string.IsNullOrEmpty(repoSettings.LocalPath))
            {
                return $"❌ LocalPath not configured for '{repositoryKey}'";
            }

            var result = await _gitOperationService.PushAsync(repositoryKey, repoSettings.LocalPath);

            if (!result.Success)
                return $"❌ Push failed: {result.Message}";

            return $"✅ {result.Message}";
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description(@"Commit all changes and push to remote in one operation.

WHEN TO USE:
- Quick workflow: changes → commit → push
- Publishing completed work immediately
- Small incremental changes
- Single logical unit of work ready to share

DO NOT USE WHEN:
- Want to review commit before pushing
- Multiple unrelated changes need separate commits
- In the middle of merge conflict

EXAMPLES:
✓ 'Commit and push AteliersAiMcpServer with message Phase 5 complete'
✓ 'Save and publish PublicNotes changes: Update TODO'
✓ 'Commit and push blog post to AteliersDev'

RELATED TOOLS:
- commit_repository: Commit only (review before push)
- push_repository: Push only (already committed)
- pull_repository: Get latest changes first")]
    public async Task<string> CommitAndPushRepository(
        [Description("Repository key: AteliersAiAssistants, AteliersAiMcpServer, AteliersDev, PublicNotes, TrainingMcpServer")]
        string repositoryKey,
        [Description("Commit message (e.g., 'Phase 5: Add Git integration')")]
        string message)
    {
        try
        {
            if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
            {
                return $"❌ Repository '{repositoryKey}' not found";
            }

            if (string.IsNullOrEmpty(repoSettings.LocalPath))
            {
                return $"❌ LocalPath not configured for '{repositoryKey}'";
            }

            var result = await _gitOperationService.CommitAllAndPushAsync(
                repositoryKey, 
                repoSettings.LocalPath, 
                message);

            if (!result.Success)
                return $"❌ {result.Message}";

            if (result.CommitHash == null)
                return $"ℹ️ {result.Message}";

            return $"✅ {result.Message}\nCommit: {result.CommitHash[..7]}";
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description(@"Create a Git tag (lightweight or annotated).

WHEN TO USE:
- Marking release points (v1.0.0, v0.5.0)
- Creating milestones (phase-5-complete)
- Marking important commits for reference
- Documenting project snapshots

DO NOT USE WHEN:
- Tag name already exists
- No commits to tag (empty repository)

TAG TYPES:
- Lightweight: Just a name pointer (no message)
- Annotated: Includes tagger info, date, message (with message parameter)

EXAMPLES:
✓ 'Create tag v0.5.0 in AteliersAiMcpServer with message Phase 5 complete'
✓ 'Tag milestone phase-5-complete in current commit'
✓ 'Create release tag v1.0.0'

RELATED TOOLS:
- push_tag: Push tag to remote
- create_and_push_tag: Create and push in one step
- commit_repository: Commit before tagging")]
    public async Task<string> CreateTag(
        [Description("Repository key: AteliersAiAssistants, AteliersAiMcpServer, AteliersDev, PublicNotes, TrainingMcpServer")]
        string repositoryKey,
        [Description("Tag name (e.g., 'v0.5.0', 'phase-5-complete')")]
        string tagName,
        [Description("Optional: Tag message for annotated tag (omit for lightweight tag)")]
        string? message = null)
    {
        try
        {
            if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
            {
                return $"❌ Repository '{repositoryKey}' not found";
            }

            if (string.IsNullOrEmpty(repoSettings.LocalPath))
            {
                return $"❌ LocalPath not configured for '{repositoryKey}'";
            }

            var result = await _gitOperationService.CreateTagAsync(
                repositoryKey, 
                repoSettings.LocalPath, 
                tagName, 
                message);

            if (!result.Success)
                return $"❌ {result.Message}";

            var tagType = string.IsNullOrEmpty(message) ? "lightweight" : "annotated";
            return $"✅ {result.Message} ({tagType} tag)";
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description(@"Push a Git tag to remote repository.

WHEN TO USE:
- After creating local tag
- Publishing release tags
- Sharing milestone markers with team
- Making tags visible on GitHub

DO NOT USE WHEN:
- Tag doesn't exist locally
- Remote repository not configured

EXAMPLES:
✓ 'Push tag v0.5.0 to remote AteliersAiMcpServer'
✓ 'Upload tag phase-5-complete to GitHub'
✓ 'Publish release tag v1.0.0'

RELATED TOOLS:
- create_tag: Create tag first
- create_and_push_tag: Create and push in one step")]
    public async Task<string> PushTag(
        [Description("Repository key: AteliersAiAssistants, AteliersAiMcpServer, AteliersDev, PublicNotes, TrainingMcpServer")]
        string repositoryKey,
        [Description("Tag name (e.g., 'v0.5.0')")]
        string tagName)
    {
        try
        {
            if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
            {
                return $"❌ Repository '{repositoryKey}' not found";
            }

            if (string.IsNullOrEmpty(repoSettings.LocalPath))
            {
                return $"❌ LocalPath not configured for '{repositoryKey}'";
            }

            var result = await _gitOperationService.PushTagAsync(
                repositoryKey, 
                repoSettings.LocalPath, 
                tagName);

            if (!result.Success)
                return $"❌ {result.Message}";

            return $"✅ {result.Message}";
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description(@"Create a Git tag and push to remote in one operation.

WHEN TO USE:
- Quick release tagging workflow
- Publishing version milestones immediately
- Creating and sharing project snapshots
- Marking important commits for team reference

DO NOT USE WHEN:
- Want to verify tag locally first
- Tag name already exists
- Remote repository not configured

EXAMPLES:
✓ 'Create and push tag v0.5.0 in AteliersAiMcpServer with message Phase 5 complete'
✓ 'Tag and publish milestone phase-6-start'
✓ 'Create release tag v1.0.0 and push to GitHub'

RELATED TOOLS:
- create_tag: Create tag only (verify before push)
- push_tag: Push existing tag
- commit_and_push_repository: Commit and push changes")]
    public async Task<string> CreateAndPushTag(
        [Description("Repository key: AteliersAiAssistants, AteliersAiMcpServer, AteliersDev, PublicNotes, TrainingMcpServer")]
        string repositoryKey,
        [Description("Tag name (e.g., 'v0.5.0', 'phase-5-complete')")]
        string tagName,
        [Description("Optional: Tag message for annotated tag (omit for lightweight tag)")]
        string? message = null)
    {
        try
        {
            if (!_settings.Repositories.TryGetValue(repositoryKey, out var repoSettings))
            {
                return $"❌ Repository '{repositoryKey}' not found";
            }

            if (string.IsNullOrEmpty(repoSettings.LocalPath))
            {
                return $"❌ LocalPath not configured for '{repositoryKey}'";
            }

            var result = await _gitOperationService.CreateAndPushTagAsync(
                repositoryKey, 
                repoSettings.LocalPath, 
                tagName, 
                message);

            if (!result.Success)
                return $"❌ {result.Message}";

            var tagType = string.IsNullOrEmpty(message) ? "lightweight" : "annotated";
            return $"✅ {result.Message} ({tagType} tag)";
        }
        catch (Exception ex)
        {
            return $"❌ Error: {ex.Message}";
        }
    }
}