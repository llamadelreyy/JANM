using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

public class GeometryJsonConverter : JsonConverter<Geometry>
{
    public override Geometry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization of GeoJSON is not implemented.");
    }

    public override void Write(Utf8JsonWriter writer, Geometry value, JsonSerializerOptions options)
    {
        if (value == null || value.IsEmpty)
        {
            writer.WriteNullValue();
            return;
        }

        var geoJsonWriter = new GeoJsonWriter();
        var geoJson = geoJsonWriter.Write(value);

        using (JsonDocument jsonDocument = JsonDocument.Parse(geoJson))
        {
            jsonDocument.WriteTo(writer);
        }
    }
}