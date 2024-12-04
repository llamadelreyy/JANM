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

    public class PremisMarkerViewModel
    {
        public int gid { get; set; }
        public string? lot { get; set; }
        public string? cukai_status { get; set; }
        public string? lesen_status { get; set; }

        public JsonDocument? geom { get; set; }
    }
}
