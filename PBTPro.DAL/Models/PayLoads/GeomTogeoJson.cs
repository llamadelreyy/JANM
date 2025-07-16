using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class LotPolygonViewModel
    {
        public int gid { get; set; }
        public double? objectid { get; set; }
        public string? lot { get; set; }
        public JsonDocument? geom { get; set; }
    }

    public class DunPolygonViewModel
    {
        public int gid { get; set; }
        public string? name { get; set; }
        public JsonDocument? geom { get; set; }
    }

    public class ParlimentPolygonViewModel
    {
        public int gid { get; set; }
        public string? name { get; set; }
        public JsonDocument? geom { get; set; }
    }

    public class AhliMajlisPolygonViewModel
    {
        public int gid { get; set; }
        public string? ahli_majlis { get; set; }
        public string? name { get; set; }
        public string? no_telefon { get; set; }
        public string? penggal { get; set; }
        public string? dun { get; set; }
        public string? ekar { get; set; }
        public JsonDocument? geom { get; set; }
    }

    public class PremisMarkerViewModel
    {
        public string codeid_premis { get; set; }
        public string lot { get; set; }
        public string? category { get; set; }
        public JsonDocument? geom { get; set; }
        public string? tax_status_view { get; set; }
        public int? tax_status_id { get; set; }
        public string? license_status_view { get; set; }
        public int? license_status_id { get; set; }
    }

    public class PremisLicenseFilterModel
    {
        public string types_code { get; set; }
        public DateOnly? start_date { get; set; }
        public DateOnly? end_date { get; set; }
        public List<int>? category_ids { get; set; }
    }
}
