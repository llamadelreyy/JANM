using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using PBTPro.DAL.Models;
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

        public virtual DbSet<view_premis_detail> view_premis_details { get; set; }

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

            var intervalToStringConverter = new ValueConverter<NpgsqlInterval?, string>(
        interval => interval.HasValue ? interval.Value.ToString() : null,
        str => string.IsNullOrEmpty(str) ? (NpgsqlInterval?)null : null);

            modelBuilder.Entity<view_premis_detail>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("view_premis_details", _tenantSchema);

                entity.Property(e => e.codeid_premis).HasColumnType("character varying");
                entity.Property(e => e.license_accno).HasColumnType("character varying");
                entity.Property(e => e.license_business_address).HasMaxLength(255);
                entity.Property(e => e.license_business_name).HasMaxLength(100);
                entity.Property(e => e.license_district_code).HasMaxLength(10);
                entity.Property(e => e.license_doc_support).HasColumnType("character varying");
                entity.Property(e => e.license_duration).HasColumnType("character varying");
                entity.Property(e => e.license_g_activity_1).HasColumnType("character varying");
                entity.Property(e => e.license_g_activity_2).HasColumnType("character varying");
                entity.Property(e => e.license_g_activity_3).HasColumnType("character varying");
                entity.Property(e => e.license_g_signbboard_1).HasColumnType("character varying");
                entity.Property(e => e.license_g_signbboard_2).HasColumnType("character varying");
                entity.Property(e => e.license_g_signbboard_3).HasColumnType("character varying");
                entity.Property(e => e.license_lot).HasColumnType("character varying");
                entity.Property(e => e.license_owner_addess).HasMaxLength(255);
                entity.Property(e => e.license_owner_disctict_code).HasMaxLength(20);
                entity.Property(e => e.license_owner_email).HasMaxLength(100);
                entity.Property(e => e.license_owner_icno).HasMaxLength(40);
                entity.Property(e => e.license_owner_name).HasMaxLength(100);
                entity.Property(e => e.license_owner_state_code).HasMaxLength(10);
                entity.Property(e => e.license_owner_telno).HasMaxLength(40);
                entity.Property(e => e.license_ssmno).HasColumnType("character varying");
                entity.Property(e => e.license_state_code).HasMaxLength(10);
                entity.Property(e => e.license_status_view).HasMaxLength(40);
                entity.Property(e => e.license_type).HasColumnType("character varying");
                entity.Property(e => e.premis_floor).HasColumnType("character varying");
                entity.Property(e => e.premis_geom).HasColumnType("geometry(PointZM,4326)");
                entity.Property(e => e.premis_gkeseluruh).HasMaxLength(255);
                entity.Property(e => e.premis_lot).HasColumnType("character varying");
                entity.Property(e => e.tax_accno).HasColumnType("character varying");
                entity.Property(e => e.tax_address).HasColumnType("character varying");
                entity.Property(e => e.tax_district_code).HasMaxLength(10);
                entity.Property(e => e.tax_owner_addess).HasMaxLength(255);
                entity.Property(e => e.tax_owner_disctict_code).HasMaxLength(20);
                entity.Property(e => e.tax_owner_email).HasMaxLength(100);
                entity.Property(e => e.tax_owner_icno).HasMaxLength(40);
                entity.Property(e => e.tax_owner_name).HasMaxLength(100);
                entity.Property(e => e.tax_owner_state_code).HasMaxLength(10);
                entity.Property(e => e.tax_owner_telno).HasMaxLength(40);
                entity.Property(e => e.tax_state_code).HasMaxLength(10);
                entity.Property(e => e.tax_status_view).HasMaxLength(100);
            });
        }
    }
}
