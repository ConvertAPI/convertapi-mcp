using CA.MCP.Core.Infrastructure;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CA.MCP.Core.Models
{
    /// <summary>
    /// Represents the parameters required for a client request to convert data between formats.
    /// </summary>
    /// <remarks>This class encapsulates the source and target formats for the conversion, as well as any
    /// additional parameters that may be required to customize the conversion process.</remarks>
    public class ClientRequestParamsRemote
    {
        [Description("From format (e.g., 'docx', 'xlsx', 'jpg')")] 
        public string FromFormat { get; set; } = string.Empty;

        [Description("To format (e.g., 'pdf', 'png', 'html')")]
        public string ToFormat { get; set; } = string.Empty;

        [Description("Conversion parameters")]
        [JsonConverter(typeof(FlexibleParameterConverter))]
        public Dictionary<string, string>? Parameters { get; set; }
    }
}
