using CA.MCP.Core.Models;
using ConvertApiDotNet.Services;
using Microsoft.Extensions.Logging;

namespace CA.MCP.Core.Services
{
    public class OpenApiSchemaService : IOpenApiSchemaService
    {
        private readonly ILogger<OpenApiSchemaService>? _logger;
        private readonly ConverterCatalog _converterCatalog;

        public OpenApiSchemaService(ILogger<OpenApiSchemaService>? logger = null)
        {
            _logger = logger;
            _converterCatalog = new ConverterCatalog();
        }


        /// <summary>
        /// Retrieves information about the available conversion path between two specified formats.
        /// </summary>
        /// <param name="fromFormat"></param>
        /// <param name="toFormat"></param>
        /// <returns></returns>
        public async Task<ConversionInfoResult> GetConversionInfo(string fromFormat, string toFormat)
        {
            try
            {
                var openApiInfo = _converterCatalog.GetConverter(fromFormat, toFormat);
                if (openApiInfo != null)
                {
                    var result = new ConversionInfoResult
                    {
                        Converter = openApiInfo,
                        ErrorMessage = null
                    };

                    return result;
                }

                _logger?.LogError("Conversion path info could not be retrieved for {FromFormat} to {ToFormat}", fromFormat, toFormat);

                return new ConversionInfoResult
                {
                    ErrorMessage = "Conversion path info could not be retrieved."
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to get conversion info for {FromFormat} to {ToFormat}", fromFormat, toFormat);

                var errorInfo = new ConversionInfoResult
                {
                    ErrorMessage = ex.Message
                };

                return errorInfo;
            }
        }


        /// <summary>
        /// Retrieves a list of available converters that match the specified tags.
        /// </summary>
        /// <param name="tags">A list of tags used to filter the available converters. Only converters associated with all specified tags
        /// are returned. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ConversionInfoResultList with
        /// the matching converters, or an error message if the retrieval fails.</returns>
        public async Task<ConversionInfoResultList> GetConvertersByTags(List<string> tags)
        {
            try
            {
                var openApiInfo = _converterCatalog.GetConvertersByTags(tags);
                if (openApiInfo != null)
                {
                    var result = new ConversionInfoResultList
                    {
                        Converter = openApiInfo,
                        ErrorMessage = null
                    };

                    return result;
                }

                _logger?.LogError("Failed to retrieve converters for tags: {Tags}", tags);

                return new ConversionInfoResultList
                {
                    ErrorMessage = "No converters found for the specified tags."
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError("Failed to retrieve converters for tags: {Tags}", tags);

                var errorInfo = new ConversionInfoResultList
                {
                    ErrorMessage = ex.Message
                };

                return errorInfo;
            }
        }


        /// <summary>
        /// Searches for available converters that match the specified search terms.
        /// </summary>
        /// <param name="terms">A list of search terms used to filter the available converters. Each term is matched against converter
        /// metadata. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ConversionInfoResultList with
        /// matching converter information if found; otherwise, an error message is provided.</returns>
        public async Task<ConversionInfoResultList> SearchConverters(List<string> terms)
        {
            try
            {
                var openApiInfo = _converterCatalog.SearchConverters(terms.ToArray());
                if (openApiInfo != null)
                {
                    var result = new ConversionInfoResultList
                    {
                        Converter = openApiInfo,
                        ErrorMessage = null
                    };

                    return result;
                }

                _logger?.LogError("No converters found for search terms: {Terms}", terms);

                return new ConversionInfoResultList
                {
                    ErrorMessage = "No converters found for the specified search terms."
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to search converters for terms: {Terms}", terms);

                var errorInfo = new ConversionInfoResultList
                {
                    ErrorMessage = ex.Message
                };

                return errorInfo;
            }
        }
    }
}