using CA.MCP.Core.Models;
using ConvertApiDotNet;
using ConvertApiDotNet.Exceptions;
using Microsoft.Extensions.Logging;
using CA.MCP.Core.Infrastructure;

namespace CA.MCP.Core.Services
{
    public class ConvertService
    {
        private readonly ConvertApi _convertApi;
        private readonly ILogger<ConvertService>? _logger;

        public ConvertService(string apiSecret, ILogger<ConvertService>? logger = null)
        {
            _logger = logger;   
            
            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentException("ConvertAPI secret cannot be null or empty.", nameof(apiSecret));
            
            _convertApi = new ConvertApi(apiSecret);
        }

        /// <summary>
        /// Converts a file from one format to another using the specified parameters.
        /// </summary>
        /// <remarks>This method sends a request to the ConvertAPI service to perform the file conversion.
        /// The source and target formats must be specified in the <paramref name="clientRequest"/>. If the conversion
        /// is successful, the resulting files are downloaded and saved locally, and their paths are returned.</remarks>
        /// <param name="clientRequest">The parameters for the conversion request, including source and target formats, and any additional options.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An array of file urls representing the converted files.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="clientRequest"/> does not specify a valid source format or target format.</exception>
        /// <exception cref="ConvertApiException">Thrown if the API request fails or the response indicates an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the API response does not contain any files.</exception>
        public async Task<string> ConvertAsync(ClientRequestParamsRemote clientRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientRequest.FromFormat))
                {
                    _logger?.LogError("Source format is required.");
                    return Helpers.BuildErrorResponse("Source format is required.", "INVALID_ARGUMENT", "FromFormat parameter cannot be null or empty.");
                }
                
                if (string.IsNullOrWhiteSpace(clientRequest.ToFormat))
                {
                    _logger?.LogError("Target format is required.");
                    return Helpers.BuildErrorResponse("Target format is required.", "INVALID_ARGUMENT", "ToFormat parameter cannot be null or empty.");
                }

                cancellationToken.ThrowIfCancellationRequested();

                _logger?.LogInformation("Converting from {FromFormat} to {ToFormat} using ConvertAPI SDK",
                    clientRequest.FromFormat, clientRequest.ToFormat);

                var convertParams = new List<ConvertApiBaseParam>();

                if (clientRequest.Parameters != null)
                {
                    foreach (var param in clientRequest.Parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(param.Key) && param.Value != null)
                        {
                            convertParams.Add(new ConvertApiParam(param.Key, param.Value));
                            _logger?.LogDebug("Added parameter: {Key} = {Value}", param.Key, param.Value);
                        }
                    }
                }

                var result = await _convertApi.ConvertAsync(
                    clientRequest.FromFormat.ToLowerInvariant(),
                    clientRequest.ToFormat.ToLowerInvariant(),
                    convertParams.ToArray()
                );

                if (result.Files == null || result.Files.Length == 0)
                {
                    _logger?.LogError("No files returned from ConvertAPI.");
                    return Helpers.BuildErrorResponse("No files returned from ConvertAPI.", "NO_FILES_RETURNED", "The conversion completed but did not produce any output files.");
                }

                var urls = result.Files.Select(f => f.Url.ToString()).ToArray();
                
                _logger?.LogInformation("Conversion completed successfully. {FileCount} file(s) generated.", urls.Length);
                
                return Helpers.BuildSuccessResponse(urls);
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogWarning("Conversion operation was cancelled.");
                return Helpers.BuildErrorResponse("Conversion operation was cancelled.", "OPERATION_CANCELLED", ex.Message);
            }
            catch (ConvertApiException ex)
            {
                _logger?.LogError(ex, "ConvertAPI error occurred during conversion from {FromFormat} to {ToFormat}",
                    clientRequest.FromFormat, clientRequest.ToFormat);
                return Helpers.BuildErrorResponse("ConvertAPI service error occurred.", "API_ERROR", ex.Response);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error during conversion from {FromFormat} to {ToFormat}",
                    clientRequest.FromFormat, clientRequest.ToFormat);
                return Helpers.BuildErrorResponse("Conversion operation failed.", "OPERATION_FAILED", ex.Message);
            }
        }
    

        /// <summary>
        /// Dynamically converts files using ConvertAPI REST endpoints based on OpenAPI specification.
        /// Supports any conversion format and parameters defined in the ConvertAPI OpenAPI schema.
        /// </summary>
        /// <param name="fromFormat">Source format (e.g., "docx", "xlsx", "jpg").</param>
        /// <param name="toFormat">Target format (e.g., "pdf", "png", "html").</param>
        /// <param name="outputDirectory">Directory to save converted files.</param>
        /// <param name="parameters">String parameters for the conversion (e.g., quality, page size).</param>
        /// <param name="fileParameters">File parameters with their local file paths.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>JSON string containing array of output file paths or error response.</returns>
        public async Task<string> ConvertAsync(
            string fromFormat,
            string toFormat,
            string outputDirectory,
            Dictionary<string, string>? parameters = null,
            Dictionary<string, string>? fileParameters = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromFormat))
                {
                    _logger?.LogError("Source format is required.");
                    return Helpers.BuildErrorResponse("Source format is required.", "INVALID_ARGUMENT", "fromFormat parameter cannot be null or empty.");
                }
                
                if (string.IsNullOrWhiteSpace(toFormat))
                {
                    _logger?.LogError("Target format is required.");
                    return Helpers.BuildErrorResponse("Target format is required.", "INVALID_ARGUMENT", "toFormat parameter cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(outputDirectory))
                {
                    _logger?.LogError("Output directory is required.");
                    return Helpers.BuildErrorResponse("Output directory is required.", "INVALID_ARGUMENT", "outputDirectory parameter cannot be null or empty.");
                }

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to create output directory: {OutputDirectory}", outputDirectory);
                    return Helpers.BuildErrorResponse("Failed to create output directory.", "DIRECTORY_ERROR", $"Unable to create directory '{outputDirectory}': {ex.Message}");
                }

                _logger?.LogInformation("Converting from {FromFormat} to {ToFormat} using ConvertAPI SDK",
                    fromFormat, toFormat);

                var convertParams = new List<ConvertApiBaseParam>();

                // Add string parameters
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(param.Key) && param.Value != null)
                        {
                            convertParams.Add(new ConvertApiParam(param.Key, param.Value));
                            _logger?.LogDebug("Added parameter: {Key} = {Value}", param.Key, param.Value);
                        }
                    }
                }

                // Add file parameters
                if (fileParameters != null)
                {
                    foreach (var fileParam in fileParameters)
                    {
                        if (string.IsNullOrWhiteSpace(fileParam.Key) || string.IsNullOrWhiteSpace(fileParam.Value))
                            continue;

                        if (!File.Exists(fileParam.Value))
                        {
                            _logger?.LogError("File not found for parameter '{Key}': {FilePath}", fileParam.Key, fileParam.Value);
                            return Helpers.BuildErrorResponse(
                                $"File for parameter '{fileParam.Key}' not found.", 
                                "FILE_NOT_FOUND", 
                                $"The file '{fileParam.Value}' does not exist.");
                        }

                        convertParams.Add(new ConvertApiFileParam(fileParam.Key, fileParam.Value));
                        _logger?.LogDebug("Added file parameter: {Key} = {FileName}", fileParam.Key, fileParam.Value);
                    }
                }

                var result = await _convertApi.ConvertAsync(
                    fromFormat.ToLowerInvariant(),
                    toFormat.ToLowerInvariant(),
                    convertParams.ToArray()
                );

                if (result.Files == null || result.Files.Length == 0)
                {
                    _logger?.LogError("No files returned from ConvertAPI.");
                    return Helpers.BuildErrorResponse("No files returned from ConvertAPI.", "NO_FILES_RETURNED", "The conversion completed but did not produce any output files.");
                }

                // Download and save the result files
                var savedPaths = new List<string>();

                foreach (var file in result.Files)
                {
                    try
                    {
                        var filePath = Path.Combine(outputDirectory, file.FileName);

                        _logger?.LogInformation("Downloading file: {FileName} from {Url}", file.FileName, file.Url);

                        using (var httpClient = new HttpClient())
                        {
                            var fileBytes = await httpClient.GetByteArrayAsync(file.Url, cancellationToken);
                            await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);
                        }

                        savedPaths.Add(filePath);
                        _logger?.LogInformation("Saved file: {FilePath}", filePath);
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger?.LogError(ex, "Failed to download file: {FileName} from {Url}", file.FileName, file.Url);
                        return Helpers.BuildErrorResponse(
                            $"Failed to download file '{file.FileName}'.", 
                            "DOWNLOAD_ERROR", 
                            ex.Message);
                    }
                    catch (IOException ex)
                    {
                        _logger?.LogError(ex, "Failed to save file: {FilePath}", Path.Combine(outputDirectory, file.FileName));
                        return Helpers.BuildErrorResponse(
                            $"Failed to save file '{file.FileName}'.", 
                            "FILE_WRITE_ERROR", 
                            ex.Message);
                    }
                }

                _logger?.LogInformation("Conversion completed successfully. {FileCount} file(s) saved.", savedPaths.Count);

                return Helpers.BuildSuccessResponse(savedPaths.ToArray());
            }
            catch (OperationCanceledException ex)
            {
                _logger?.LogWarning("Conversion operation was cancelled.");
                return Helpers.BuildErrorResponse("Conversion operation was cancelled.", "OPERATION_CANCELLED", ex.Message);
            }
            catch (ConvertApiException ex)
            {
                _logger?.LogError(ex, "ConvertAPI error occurred during conversion from {FromFormat} to {ToFormat}",
                    fromFormat, toFormat);
                return Helpers.BuildErrorResponse("ConvertAPI service error occurred.", "API_ERROR", ex.Response);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error during conversion from {FromFormat} to {ToFormat}",
                    fromFormat, toFormat);
                return Helpers.BuildErrorResponse("Conversion operation failed.", "OPERATION_FAILED", ex.Message);
            }
        }
    }
}