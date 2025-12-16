using System.Text.Json;
using System.Text.Json.Serialization;

namespace CA.MCP.Core.Infrastructure
{
    public class FlexibleParameterConverter : JsonConverter<Dictionary<string, string>>
    {
        public override Dictionary<string, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                return null;

            var dictionary = new Dictionary<string, string>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dictionary;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException($"Expected property name, got {reader.TokenType}");

                var key = reader.GetString()!;
                reader.Read();

                using var doc = JsonDocument.ParseValue(ref reader);
                var el = doc.RootElement;

                string value = el.ValueKind switch
                {
                    JsonValueKind.String => el.GetString()!,
                    JsonValueKind.Number => el.GetRawText(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Null => string.Empty,
                    JsonValueKind.Array => string.Join(",", el.EnumerateArray().Select(e =>
                        e.ValueKind == JsonValueKind.String ? e.GetString() :
                        e.ValueKind == JsonValueKind.Number || e.ValueKind == JsonValueKind.True || e.ValueKind == JsonValueKind.False
                            ? e.GetRawText()
                            : e.GetRawText())),
                    JsonValueKind.Object => el.GetRawText(),
                    _ => string.Empty
                };

                dictionary[key] = value;
            }

            return dictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value)
                writer.WriteString(kvp.Key, kvp.Value);
            writer.WriteEndObject();
        }
    }
}