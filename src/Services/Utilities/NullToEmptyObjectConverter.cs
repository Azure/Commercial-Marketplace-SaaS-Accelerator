using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class NullToEmptyObjectConverter<T> : JsonConverter<T> where T : class, new()
{
    public override bool HandleNull => true;

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
