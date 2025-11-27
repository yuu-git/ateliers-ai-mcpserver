# CLAUDE.MD

This file provides guidance for Claude AI when working with the Ateliers AI MCP Server codebase.

## Project Overview

**Ateliers AI MCP Server** is a Model Context Protocol (MCP) server implementation in C#/.NET that integrates Claude Desktop with GitHub repositories and local file systems. It provides seamless access to technical articles, coding guidelines, and repository files.

### Key Capabilities

- **Dual Access Mode**: Local file system (fast) or GitHub API (flexible)
- **Repository Tools**: Complete CRUD operations for files across multiple repositories
- **Article Tools**: Specialized tools for technical article management
- **Smart Caching**: 5-minute memory cache to reduce API calls and improve performance

### Technology Stack

- .NET 10.0
- Octokit.NET (GitHub API client)
- Model Context Protocol (MCP) - stdio transport
- Dependency Injection pattern

## Architecture

### Layer Structure

```
┌─────────────────────────────────────────┐
│           Claude Desktop                │
│        (MCP Client via stdio)           │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Program.cs (Entry Point)        │
│  - Configuration loading                │
│  - DI container setup                   │
│  - MCP server initialization            │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│          Tools Layer                    │
│  ┌─────────────────────────────────┐   │
│  │  RepositoryTools                │   │
│  │  - Generic file operations      │   │
│  │  - 8 tools for CRUD operations  │   │
│  └─────────────────────────────────┘   │
│  ┌─────────────────────────────────┐   │
│  │  AteliersDevTools               │   │
│  │  - Article-specific operations  │   │
│  │  - Frontmatter removal          │   │
│  │  - Article search               │   │
│  └─────────────────────────────────┘   │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Services Layer                  │
│  ┌─────────────────────────────────┐   │
│  │  LocalFileService               │   │
│  │  - Fast local file access       │   │
│  │  - Complete CRUD + utilities    │   │
│  └─────────────────────────────────┘   │
│  ┌─────────────────────────────────┐   │
│  │  GitHubService                  │   │
│  │  - GitHub API integration       │   │
│  │  - Memory cache (5 min TTL)     │   │
│  │  - Anonymous/PAT auth           │   │
│  └─────────────────────────────────┘   │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│      Configuration Layer                │
│  - AppSettings.cs (typed config)        │
│  - appsettings.json (base config)       │
│  - appsettings.local.json (user config) │
└─────────────────────────────────────────┘
```

### Design Patterns

1. **Dependency Injection**: All services use constructor injection
2. **Repository Pattern**: `GitHubService` and `LocalFileService` abstract data access
3. **Strategy Pattern**: Dynamic selection between local/GitHub access
4. **Cache-Aside Pattern**: Memory cache with 5-minute expiration

### Data Flow: Local-First Logic

```
Request → Check LocalPath configured?
   ├─ YES → LocalFileService (1-10ms)
   └─ NO  → GitHubService
              ├─ Check cache → Hit (1-5ms)
              └─ Cache miss → GitHub API (200-500ms)
```

## Project Structure

```
Ateliers.Ai.McpServer/
├── Configuration/
│   └── AppSettings.cs              # Typed configuration classes
├── Services/
│   ├── GitHubService.cs            # GitHub API operations
│   ├── LocalFileService.cs         # Local file system operations
│   ├── NotesService.cs             # (Legacy) Notes operations
│   └── GitHubNotesService.cs       # (Legacy) GitHub notes
├── Tools/
│   ├── RepositoryTools.cs          # Generic file tools (8 tools)
│   └── AteliersDevTools.cs         # Article-specific tools (3 tools)
├── Program.cs                      # Entry point & DI setup
├── appsettings.json                # Base configuration (committed)
└── appsettings.local.json.sample   # Template for user config
```

## Configuration

### Repository Configuration

Repositories are configured in `appsettings.json` and `appsettings.local.json`:

```json
{
  "Repositories": {
    "AteliersAiMcpServer": {
      "Source": "GitHub",
      "GitHub": {
        "Owner": "yuu-git",
        "Name": "ateliers-ai-mcpserver",
        "Branch": "master"
      },
      "LocalPath": null,          // Set in local config for fast access
      "AutoPull": false,
      "AutoPush": false
    }
  }
}
```

### Supported Repositories

| Key | Description | Primary Use |
|-----|-------------|-------------|
| `AteliersAiAssistants` | Coding guidelines | AI coding standards |
| `AteliersAiMcpServer` | This MCP server | Source code management |
| `AteliersDev` | Tech blog (Docusaurus) | Technical articles |
| `PublicNotes` | Public notes/memos | TODOs, ideas, snippets |
| `TrainingMcpServer` | Training version | Learning codebase |

## MCP Tools Reference

### RepositoryTools (8 tools)

1. **read_repository_file** - Read file content (local-first)
2. **list_repository_files** - List files in directory (local-first)
3. **add_repository_file** - Create new file (local only)
4. **edit_repository_file** - Update file with backup (local only)
5. **delete_repository_file** - Delete file with backup (local only)
6. **rename_repository_file** - Rename/move file (local only)
7. **copy_repository_file** - Copy file (local only)
8. **backup_repository_file** - Manual backup creation (local only)

### AteliersDevTools (3 tools)

1. **read_article** - Read article with frontmatter removal
2. **list_articles** - List all .md/.mdx articles
3. **search_articles** - Search articles by keyword

## Development Guidelines

### When Reading Files

1. **Always use Read tool first** before suggesting modifications
2. **Check line numbers** in Read output to ensure accurate edits
3. **Preserve exact indentation** (tabs/spaces) when using Edit tool
4. **Prefer Read over Bash** commands like `cat`, `head`, `tail`

### When Editing Files

1. **Use Edit tool** instead of `sed`, `awk`, or writing entire files
2. **Provide sufficient context** in `old_string` to make it unique
3. **Preserve whitespace** exactly as shown in Read output
4. **Auto-backup**: `edit_repository_file` automatically creates backups

### When Creating Files

1. **Prefer editing existing files** over creating new ones
2. **Only create files** when explicitly required
3. **Don't create documentation** unless explicitly requested
4. **No emoji** in files unless user requests it

### Code Style

This is a C# project following standard .NET conventions:

- **Naming**: PascalCase for classes/methods, camelCase for local variables
- **Async/await**: Use `Async` suffix for async methods
- **DI**: Constructor injection for all dependencies
- **Error handling**: Try-catch at tool layer, exceptions in service layer
- **Comments**: Only where logic isn't self-evident

### Avoiding Over-Engineering

- **No premature abstractions**: Don't create helpers for one-time operations
- **No unnecessary features**: Only implement what's requested
- **No extra error handling**: Trust internal code, validate at boundaries
- **No backwards compatibility hacks**: Delete unused code completely

## Common Tasks

### Adding a New Repository

1. Add entry to `appsettings.json`:
```json
"NewRepo": {
  "Source": "GitHub",
  "GitHub": {
    "Owner": "owner-name",
    "Name": "repo-name",
    "Branch": "main"
  },
  "LocalPath": null,
  "AutoPull": false,
  "AutoPush": false
}
```

2. Users configure `LocalPath` in their `appsettings.local.json`

### Adding a New Tool

1. Create method in appropriate Tools class (`RepositoryTools` or `AteliersDevTools`)
2. Add `[Tool("tool_name", "description")]` attribute
3. Inject required services via constructor
4. Implement tool logic using services
5. Return user-friendly error messages (not exceptions)

### Testing Changes

```bash
# Build
dotnet build --configuration Release

# Run (requires MCP client or manual testing)
dotnet run --project Ateliers.Ai.McpServer --configuration Release
```

### Reading Logs

Claude Desktop logs are useful for debugging:
- Windows: `%APPDATA%\Claude\logs\`
- macOS: `~/Library/Logs/Claude/`

## Important Patterns

### Local-First Access Pattern

```csharp
// Service layer automatically handles this
var content = await _githubService.GetFileContentAsync(
    repositoryKey,
    filePath,
    branch
);
// Returns local file if LocalPath configured, else GitHub API
```

### Cache Key Convention

```
github:{owner}/{repo}:{branch}:{path}
```

Example: `github:yuu-git/ateliers-ai-mcpserver:master:README.md`

### Frontmatter Removal (Articles)

Articles often have YAML frontmatter:
```markdown
---
title: Article Title
date: 2024-01-01
---

Content starts here...
```

`AteliersDevTools.read_article` automatically removes everything between `---` delimiters.

### Automatic Backups

When editing/deleting files locally:
```
Original: /path/to/file.txt
Backup:   /path/to/file.txt.backup.20240126_143022
```

Format: `{original}.backup.{yyyyMMdd_HHmmss}`

## Error Handling

### Tool Layer (RepositoryTools, AteliersDevTools)

- Catch all exceptions
- Return user-friendly error messages
- Never let exceptions bubble to MCP layer

```csharp
try
{
    return await _service.GetFileAsync(...);
}
catch (FileNotFoundException ex)
{
    return $"Error: File not found: {ex.Message}";
}
catch (Exception ex)
{
    return $"Error: {ex.Message}";
}
```

### Service Layer (GitHubService, LocalFileService)

- Throw specific exceptions (FileNotFoundException, etc.)
- Let tool layer handle user-facing messages
- Log errors for debugging

## Git Workflow

### Current Branch

You are working on: `claude/update-claude-md-011nN9tTFDFLMFLsoNmjX9ZS`

### Git Commands

**Committing:**
```bash
# Check status
git status

# Stage changes
git add <files>

# Commit with descriptive message
git commit -m "$(cat <<'EOF'
feat: Description of change

Additional context if needed
EOF
)"
```

**Pushing:**
```bash
# Push to branch (retry up to 4 times with backoff if network fails)
git push -u origin claude/update-claude-md-011nN9tTFDFLMFLsoNmjX9ZS
```

**Creating PRs:**
```bash
# Use gh CLI
gh pr create --title "Title" --body "$(cat <<'EOF'
## Summary
- Change 1
- Change 2

## Test Plan
- [ ] Test item 1
- [ ] Test item 2
EOF
)"
```

## Performance Characteristics

### Local File Access
- Read: 1-10ms
- Write: 5-20ms
- List: 10-50ms (depending on directory size)

### GitHub API Access
- Without cache: 200-500ms
- With cache hit: 1-5ms
- Cache TTL: 5 minutes

### Rate Limits
- Anonymous: 60 requests/hour
- With PAT: 5,000 requests/hour

## Security Considerations

- `appsettings.local.json` is in `.gitignore` (never committed)
- Personal Access Tokens should only be in `appsettings.local.json`
- Default mode is Anonymous (public repos only)
- Local file operations are restricted to configured LocalPath directories

## Troubleshooting

### MCP Server Not Recognized
1. Check `claude_desktop_config.json` has correct path
2. Verify build succeeded: `dotnet build --configuration Release`
3. Restart Claude Desktop completely

### File Access Issues
1. Verify `LocalPath` in `appsettings.local.json`
2. Check file path is relative to repository root
3. For GitHub access, verify PAT if using private repos

### Tool Not Available
1. Check tool has `[Tool(...)]` attribute
2. Verify class has `[McpServerToolType]` attribute
3. Rebuild and restart Claude Desktop

## Version History

- **v0.4.0** (2024-11-26): Phase 4 - Local-first logic, tool reorganization
- **v0.3.0**: Training version baseline
- **Earlier**: GitHub read functionality, basic caching

## Future Phases

- **Phase 5**: Git operations (LibGit2Sharp, auto-pull/push)
- **Phase 6+**: Database integration, role-based server split, VoicePeak CLI

---

**Last Updated**: 2025-11-27
**For**: Claude AI Assistant
**Purpose**: Codebase navigation and development assistance
