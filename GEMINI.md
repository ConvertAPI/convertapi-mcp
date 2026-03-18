# ConvertAPI MCP Extension

This extension provides file conversion capabilities via ConvertAPI.

## Capabilities
- Convert files between formats (PDF, DOCX, PNG, JPG, etc.)
- Apply PDF manipulations, like merge, split, compress, redact, print-preflight, extract, apply accessibility tags, etc.
- Handle document, image, and archive processing, web to PDF conversion, email attachment extract, etc.
- Supports cloud-based conversion via ConvertAPI

## When to use this extension
Use this extension whenever:
- A user asks to convert a file from one format to another
- A user needs document transformation (e.g. PDF to Word)
- A user wants to generate dynamic documents using a DOCX or HTML as a template and a JSON object with values to be injected

## Examples
- "Convert this PDF to DOCX"
- "Turn this image into a PDF"
- "Apply textual layer on this scanned PDF"

## Notes
- Requires authentication via OAuth
- All conversions are processed via ConvertAPI backend
- Large files may take longer to process

## MCP Server
Base URL: https://mcp.convertapi.com/
