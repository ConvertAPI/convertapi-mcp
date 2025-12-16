using System.Text.Json;

namespace CA.MCP.Core.Infrastructure
{
    public class Helpers
    {
        public static string BuildErrorResponse(string message, string code, string? details = null)
        {
            var error = new
            {
                error = true,
                message,
                code,
                details
            };
            return JsonSerializer.Serialize(error, new JsonSerializerOptions { WriteIndented = true });
        }

        public static string BuildSuccessResponse(string[] urls)
        {
            var response = new
            {
                success = true,
                data = urls
            };
            return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
