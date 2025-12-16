using CA.MCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.MCP.Core.Services
{
    public interface IOpenApiSchemaService
    {
        Task<ConversionInfoResult> GetConversionInfo(string fromFormat, string toFormat);
        Task<ConversionInfoResultList> GetConvertersByTags(List<string> tags);
        Task<ConversionInfoResultList> SearchConverters(List<string> terms);
    }
}
