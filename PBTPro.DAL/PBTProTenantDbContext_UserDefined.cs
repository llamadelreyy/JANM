using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using PBTPro.DAL.Services;

namespace PBTPro.DAL
{
    public partial class PBTProTenantDbContext : DbContext
    {
        private readonly string _tenantSchema;

        public PBTProTenantDbContext()
        {
            _tenantSchema = "tenant";
        }

        public PBTProTenantDbContext(string tenantSchema)
        {
            _tenantSchema = tenantSchema;
        }

        public PBTProTenantDbContext(DbContextOptions<PBTProTenantDbContext> options)
            : base(options)
        {
            _tenantSchema = "tenant";
        }

        public PBTProTenantDbContext(DbContextOptions<PBTProTenantDbContext> options, string tenantSchema)
            : base(options)
        {
            _tenantSchema = tenantSchema;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                var test = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite());
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply the schema dynamically to all tables in the model
            if (!string.IsNullOrEmpty(_tenantSchema))
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    entity.SetSchema(_tenantSchema);
                }
            }

            #region PostGIS Function
            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
            .GetMethod(nameof(PostGISFunctions.ST_IsValid), new[] { typeof(Geometry) }))
            .HasName("st_isvalid");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_Within), new[] { typeof(Geometry), typeof(Geometry) }))
                .HasName("st_within");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_MakeEnvelope), new[] { typeof(double), typeof(double), typeof(double), typeof(double), typeof(int) }))
                .HasName("st_makeenvelope");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_AsGeoJSON), new[] { typeof(Geometry) }))
                .HasName("st_asgeojson");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_Transform), new[] { typeof(Geometry), typeof(int) }))
                .HasName("st_transform");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_Buffer), new[] { typeof(Geometry), typeof(int) }))
                .HasName("st_buffer");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_Intersects), new[] { typeof(Geometry), typeof(Geometry) }))
                .HasName("st_intersects");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_Union), new[] { typeof(Geometry) }))
                .HasName("st_union");

            modelBuilder.HasDbFunction(typeof(PostGISFunctions)
                .GetMethod(nameof(PostGISFunctions.ST_Collect), new[] { typeof(Geometry) }))
                .HasName("st_collect");

            #endregion 

        }
    }
}
