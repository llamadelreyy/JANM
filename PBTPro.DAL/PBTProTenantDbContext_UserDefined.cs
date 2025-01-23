using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        }
    }
}
