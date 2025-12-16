using CA.MCP.Core.Models;
using CA.MCP.Core.Services;
using CA.MCP.Core.Infrastructure;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace CA.MCP.Core.Tools
{

    /// <summary>
    /// Provides methods for retrieving information about available converters and their supported parameters for format
    /// conversions.
    /// </summary>
    /// <remarks>Use the methods of this class to discover which conversion operations are available and to
    /// obtain detailed information about the parameters required for each conversion. This is intended to help clients
    /// construct valid conversion requests and understand the capabilities of the conversion service.</remarks>

    [McpServerToolType]
    public class InformationTool
    {
        private readonly IOpenApiSchemaService _schemaService;

        public InformationTool(IOpenApiSchemaService schemaService)
        {
            _schemaService = schemaService;
        }

        [McpServerTool, Description(@"Get all available parameters with their descriptions and constraints for a specific conversion.

CALL THIS FIRST before using the 'convert' tool to understand which parameters are supported and their requirements.

Returns detailed information about:
- Parameter names
- Data types (string, integer, boolean, etc.)
- Allowed values and constraints
- Parameter descriptions
- Whether parameters are required or optional

This helps you construct valid conversion requests with appropriate parameters.")]
        public async Task<ConversionInfoResult> GetConversionParameters(
            [Description("From format (e.g., 'docx', 'xlsx', 'jpg')")] string fromFormat,
            [Description("To format (e.g., 'pdf', 'png', 'html')")] string toFormat)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromFormat))
                    return new ConversionInfoResult { ErrorMessage = Helpers.BuildErrorResponse("fromFormat is required.", "INVALID_ARGUMENT") };
                if (string.IsNullOrWhiteSpace(toFormat))
                    return new ConversionInfoResult { ErrorMessage = Helpers.BuildErrorResponse("toFormat is required.", "INVALID_ARGUMENT") };

                var result = await _schemaService.GetConversionInfo(fromFormat, toFormat);
                return result;
            }
            catch (Exception ex)
            {
                return new ConversionInfoResult { ErrorMessage = Helpers.BuildErrorResponse("Failed to get conversion parameters.", "OPERATION_FAILED", ex.Message) };
            }
        }

        [McpServerTool, Description(@"Retrieves a list of available converters that match the specified tags.")]
        public async Task<ConversionInfoResultList> GetConvertersByTags(
            [Description("A list of tags used to filter the available converters. Only converters associated with all specified tags are returned. For example: [\"pdf\"]")] List<string> tags)
        {
            try
            {
                var result = await _schemaService.GetConvertersByTags(tags);
                return result;
            }
            catch (Exception ex)
            {
                return new ConversionInfoResultList { ErrorMessage = Helpers.BuildErrorResponse("Failed to get converters by tags.", "OPERATION_FAILED", ex.Message) };
            }
        }

        [McpServerTool, Description(@"Searches for available converters that match the specified search terms.")]
        public async Task<ConversionInfoResultList> SearchConverters(
            [Description("A list of search terms used to filter the available converters. Each term is matched against converter metadata. For example: [\"watermark\", \"pdf\"]")] List<string> terms)
        {
            try
            {
                var result = await _schemaService.SearchConverters(terms);
                return result;
            }
            catch (Exception ex)
            {
                return new ConversionInfoResultList { ErrorMessage = Helpers.BuildErrorResponse("Failed to search converters by terms.", "OPERATION_FAILED", ex.Message) };
            }
        }
    }
}