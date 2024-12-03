using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Text.Json;

public static class PostGISFunctions
{
    // ST_Within: Checks if a geometry is within another geometry.
    [DbFunction("st_within")]
    public static bool ST_Within(Geometry geometry1, Geometry geometry2) => throw new NotImplementedException();

    // ST_MakeEnvelope: Creates a rectangular polygon from the specified bounds.
    [DbFunction("st_makeenvelope")]
    public static Geometry ST_MakeEnvelope(double xmin, double ymin, double xmax, double ymax, int srid) => throw new NotImplementedException();

    // ST_IsValid: Checks if a geometry is valid.
    [DbFunction("st_isvalid")]
    public static bool ST_IsValid(Geometry geometry) => throw new NotImplementedException();

    // ST_AsGeoJSON: Converts geometry to GeoJSON format as a string.
    [DbFunction("st_asgeojson")]
    public static string ST_AsGeoJSON(Geometry geometry) => throw new NotImplementedException();

    // ST_Transform: Converts geometry crs
    [DbFunction("st_transform")]
    public static Geometry ST_Transform(Geometry geometry, int srid) => throw new NotImplementedException();

    // ST_buffer: Converts geometry crs
    [DbFunction("st_buffer")]
    public static Geometry ST_Buffer(Geometry geometry, double distance) => throw new NotImplementedException();

    public static JsonDocument ParseGeoJsonSafely(string geoJson)
    {
        if (string.IsNullOrEmpty(geoJson)) return null;

        try
        {
            return JsonDocument.Parse(geoJson);
        }
        catch (JsonReaderException)
        {
            return null;
        }
    }
}