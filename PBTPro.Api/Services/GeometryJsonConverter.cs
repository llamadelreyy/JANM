using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

public class GeometryJsonConverter : JsonConverter<Geometry>
{
    public override Geometry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // For this example, we're not implementing the deserialization.
        throw new NotImplementedException("Deserialization of GeoJSON is not implemented.");
    }

    public override void Write(Utf8JsonWriter writer, Geometry value, JsonSerializerOptions options)
    {
        if (value == null || value.IsEmpty)
        {
            writer.WriteNullValue();
            return;
        }

        // Start by writing the GeoJSON representation
        var geoJsonWriter = new GeoJsonWriter();
        var geoJson = geoJsonWriter.Write(value);

        try
        {
            // Parse the GeoJSON string into a JSON document
            using (JsonDocument jsonDocument = JsonDocument.Parse(geoJson))
            {
                // Here, we can traverse the document and manually prevent serialization of circular references
                // For example, we can avoid deeply nested coordinates by not serializing certain structures.
                WriteJsonWithoutCycles(writer, jsonDocument.RootElement);
            }
        }
        catch (JsonException ex)
        {
            // Handle any JSON parsing issues here
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
    }

    private void WriteJsonWithoutCycles(Utf8JsonWriter writer, JsonElement root)
    {
        // Example of breaking the cycle by writing a flat JSON structure without nested coordinate references
        writer.WriteStartObject();
        if (root.TryGetProperty("type", out var typeProp))
        {
            writer.WriteString("type", typeProp.GetString());
        }
        if (root.TryGetProperty("coordinates", out var coordsProp))
        {
            writer.WriteStartArray("coordinates");

            // Here, we might limit the depth of the coordinates or handle them differently to avoid cycles
            foreach (var coord in coordsProp.EnumerateArray())
            {
                // You can apply your own logic to handle how the coordinates are serialized
                writer.WriteStartArray();
                writer.WriteNumberValue(coord[0].GetDouble());
                writer.WriteNumberValue(coord[1].GetDouble());
                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }
        writer.WriteEndObject();
    }
}