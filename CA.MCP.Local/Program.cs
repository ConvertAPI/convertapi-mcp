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

ConvertApi.ApiBaseUri = Environment.GetEnvironmentVariable("CONVERTAPI_BASE_URI");

await builder.Build().RunAsync();
