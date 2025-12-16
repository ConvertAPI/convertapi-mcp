# ConvertAPI MCP Server

A [Model Context Protocol (MCP)](https://modelcontextprotocol.io) server that provides AI assistants with powerful file format conversion capabilities through the [ConvertAPI](https://www.convertapi.com/) service. Convert documents, images, spreadsheets, presentations, and more between 200+ file formats with OpenAPI-driven parameter validation.

## Features

- 🔄 **Universal File Conversion** - Convert between 200+ file formats (PDF, DOCX, XLSX, JPG, PNG, HTML, and more)
- ✅ **OpenAPI-Driven Validation** - Dynamic parameter validation against ConvertAPI's live OpenAPI specification
- 🎯 **Comprehensive Parameters** - Supports all ConvertAPI parameters including PageSize, PageOrientation, Quality, StoreFile, etc.
- 🤖 **AI-Ready** - Seamlessly integrates with Claude Desktop, Cline, and other MCP-compatible AI assistants
- 📦 **Local ** - Supports local file operations

## Installation

### Prerequisites

- .NET 9.0 SDK or later
- A ConvertAPI account and API secret ([Get one free](https://www.convertapi.com/))

### Configuration

1. Clone the repository:

git clone https://github.com/ConvertAPI/convertapi-mcp cd ConvertAPI-MCP

2. Set your ConvertAPI secret as an environment variable:

**Windows (PowerShell):**
$env:CONVERTAPI_SECRET = "your_api_secret_here"
$env:CONVERTAPI_BASE_URI = "https://v2.convertapi.com"

**Linux/macOS:**
export CONVERTAPI_SECRET="your_api_secret_here"
export CONVERTAPI_BASE_URI="https://v2.convertapi.com"


3. Build the project:
dotnet build

## Usage

**Configuration:**

Set the following in your application configuration:

**Local Mode (with file download):**
dotnet run --project "CA.MCP.Local"


## Integration with AI Assistants

### Claude Desktop Configuration

Add to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "convertapi": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Path\\To\\CA.MCP.Local\\CA.MCP.Local.csproj",
        "--no-build"
      ],
      "env": {
        "CONVERTAPI_SECRET": "your_api_secret_here",
        "CONVERTAPI_BASE_URI": "https://v2.convertapi.com"
      }
    }
  }
}
```


### Cline (VSCode Extension)

Add to your MCP settings in Cline:

```json
{
  "convertapi": {
    "command": "dotnet",
    "args": [
      "run",
      "--project",
      "/path/to/CA.MCP.Local",
      "--no-build"
    ],
    "env": {
      "CONVERTAPI_SECRET": "your_api_secret_here",
      "CONVERTAPI_BASE_URI": "https://v2.convertapi.com"
    }
  }
}
```

## Available Tools

### `Convert`

Dynamically converts files between formats with OpenAPI-driven parameter validation.

**Parameters:**
- `fromFormat` (required) - Source format (e.g., "docx", "xlsx", "jpg")
- `toFormat` (required) - Target format (e.g., "pdf", "png", "html")
- `parameters` (optional) - Conversion parameters as key-value pairs
- `fileParameters` (optional) - Files to convert with parameter names
- `outputDirectory` (optional) - Directory to save converted files (Local mode only)

**Example Usage in AI Assistant:**

Convert this Word document to PDF:
•	From: docx
•	To: pdf
•	File: C:\Documents\report.docx
•	Parameters: PageSize=A4, PageOrientation=portrait


### `Information`

Provides information about ConvertAPI capabilities, supported formats, and usage guidelines.

## Supported Conversions

ConvertAPI supports 200+ file formats across multiple categories:

- **Documents**: PDF, DOCX, DOC, RTF, TXT, ODT, PAGES
- **Spreadsheets**: XLSX, XLS, CSV, ODS, NUMBERS
- **Presentations**: PPTX, PPT, ODP, KEY
- **Images**: JPG, PNG, GIF, BMP, TIFF, SVG, WEBP, ICO
- **Web**: HTML, MHTML, MHT
- **eBooks**: EPUB, MOBI, AZW3
- **Archives**: ZIP, RAR, 7Z
- And many more...

## Common Conversion Parameters

Depending on the conversion type, you can use parameters such as:

- **PDF Options**: `PageSize`, `PageOrientation`, `MarginTop`, `MarginBottom`, `MarginLeft`, `MarginRight`
- **Image Options**: `Quality`, `ImageWidth`, `ImageHeight`, `ScaleImage`, `ScaleProportions`
- **General**: `StoreFile`, `FileName`, `Timeout`

The server automatically validates parameters against ConvertAPI's OpenAPI specification before conversion.


## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Resources

- [ConvertAPI Documentation](https://www.convertapi.com/doc)
- [Model Context Protocol Specification](https://modelcontextprotocol.io)
- [ConvertAPI .NET SDK](https://github.com/ConvertAPI/convertapi-dotnet)

## Acknowledgments

- Built with the [ModelContextProtocol.NET](https://github.com/modelcontextprotocol/servers) library
- Powered by [ConvertAPI](https://www.convertapi.com/)

## Support

For issues and questions:
- ConvertAPI support: [support@convertapi.com](mailto:support@convertapi.com)
- GitHub Issues: [Report an issue](https://github.com/ConvertAPI/convertapi-mcp/issues)
