# File Tag Editor

A Windows Forms application for editing audio file metadata (Title, Year, Comments) with automatic year defaulting and file timestamp preservation.

## Features

- Edit audio metadata for various formats (WAV, etc.)
- Automatic year defaulting to current year if missing
- Preserves original file timestamps after metadata changes
- Single executable distribution options
- Built with TagLibSharp for robust audio format support

## Requirements

- .NET 8.0 Desktop Runtime (for framework-dependent builds)
- Windows operating system
- For development: .NET 8.0 SDK

## Development

### Building for Development
```bash
dotnet build
```

### Running in Development
```bash
dotnet run
```

## Distribution Builds

### Release Build
Creates a single executable that requires .NET 8 Desktop Runtime on target machines:

```bash
dotnet publish FileTagEditor.csproj -c Release --no-self-contained -p:PublishSingleFile=true
```

**Output**: `bin\Release\net8.0-windows\win-x64\publish\FileTagEditor.exe` (~658 KB)
**Requirements**: .NET 8 Desktop Runtime on target machine
**Benefits**: Small file size, uses system libraries

## VS Code Tasks

Use `Ctrl+Shift+P` → "Tasks: Run Task" and select:
- `publish-release` - Release build
- `build` - Development build (or use `Ctrl+Shift+B`)

## Usage

1. Run the application
2. Select an audio file when prompted
3. Edit metadata fields in the grid
4. Click Save to apply changes
5. Original file timestamps are preserved
