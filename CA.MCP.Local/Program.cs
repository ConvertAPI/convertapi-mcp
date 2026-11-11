using CA.MCP.Core.Services;
using CA.MCP.Core.Tools;
using ConvertApiDotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<InformationTool>()
    .WithTools<ConvertTool>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ConvertService>();
builder.Services.AddSingleton<IOpenApiSchemaService, OpenApiSchemaService>();

// Optional override; the ConvertAPI library defaults to https://v2.convertapi.com
var baseUri = Environment.GetEnvironmentVariable("CONVERTAPI_BASE_URI");
if (!string.IsNullOrWhiteSpace(baseUri))
    ConvertApi.ApiBaseUri = baseUri;

await builder.Build().RunAsync();
