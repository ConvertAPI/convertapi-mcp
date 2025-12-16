using CA.MCP.Core.Infrastructure;
using CA.MCP.Core.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

/// <summary>
/// Provides methods for dynamic file format conversion using the ConvertAPI service. Supports OpenAPI-driven parameter
/// validation and a wide range of conversion options.
/// </summary>
/// <remarks>The <see cref="ConvertTool"/> class enables developers to perform file format conversions with
/// extensive parameter validation based on the live OpenAPI specification. It supports various formats and parameters,
/// including options such as page size, orientation, quality, and more. This class is designed to interact with the
/// ConvertAPI service and validate parameters dynamically to ensure compatibility with the current API
/// schema.</remarks>
[McpServerToolType]
internal class ConvertTool
{

    [McpServerTool, Description(@"Dynamic ConvertAPI call with OpenAPI-driven parameter validation. 

RECOMMENDED WORKFLOW:
1. First, call 'GetConversionParameters' to discover supported parameters for your specific fromFormat->toFormat conversion
2. Review the available parameters, their types, constraints, and descriptions
3. Then call this 'convert' tool with your chosen parameters

This tool supports all parameters from the live OpenAPI specification including PageSize, PageOrientation, File, StoreFile, Quality, etc. Parameters are validated against the current schema before conversions.

REQUIRED PARAMETERS:
- fromFormat: Source format (e.g., 'docx', 'xlsx', 'jpg')
- toFormat: Target format (e.g., 'pdf', 'png', 'html')

EXAMPLE USAGE:
First check: get_conversion_parameters(fromFormat='pdf', toFormat='jpg')
Then convert: convert(fromFormat='pdf', toFormat='jpg', parameters={'PageRange':'1-3', 'ImageResolution':'300'})")]
    public async Task<string[]> Convert(
                [Description("From format (e.g., 'docx', 'xlsx', 'jpg')")] string fromFormat,
                [Description("To format (e.g., 'pdf', 'png', 'html')")] string toFormat,
                [Description("Conversion parameters")] Dictionary<string, string>? parameters = null,
                [Description("Files to convert")] Dictionary<string, string>? fileParameters = null,
                [Description("Specify directory where files to be converted will be stored")] string? outputDirectory = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fromFormat))
                return new[] { Helpers.BuildErrorResponse("fromFormat is required (e.g., 'docx', 'xlsx', 'jpg').", "INVALID_ARGUMENT") };
            if (string.IsNullOrWhiteSpace(toFormat))
                return new[] { Helpers.BuildErrorResponse("toFormat is required (e.g., 'pdf', 'png', 'html').", "INVALID_ARGUMENT") };


            // Determine output directory from first file parameter if not specified
            string? firstFileDir = null;
            if (fileParameters != null)
            {
                foreach (var kv in fileParameters)
                {
                    if (!string.IsNullOrWhiteSpace(kv.Value) && File.Exists(kv.Value))
                    {
                        firstFileDir = Path.GetDirectoryName(kv.Value);
                        break;
                    }
                }
            }

            outputDirectory ??= Path.Combine(firstFileDir ?? Directory.GetCurrentDirectory(), "converted_output");

            var apiSecret = Environment.GetEnvironmentVariable("CONVERTAPI_SECRET")
                ?? throw new InvalidOperationException("ConvertAPI secret is not configured. Set CONVERTAPI_SECRET environment variable.");

            var convertService = new ConvertService(apiSecret);
            var outputs = await convertService.ConvertAsync(
                fromFormat,
                toFormat,
                outputDirectory,
                parameters,
                fileParameters);

            return outputs.Select(url => Helpers.BuildSuccessResponse(new[] { url.ToString() })).ToArray();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Conversion failed: {ex.Message}");
            return new[] { Helpers.BuildErrorResponse("Conversion operation failed", "OPERATION_FAILED", ex.Message) };
        }
    }
}