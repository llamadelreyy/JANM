using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using PBTPro.DAL.Services;

namespace PBTPro.DAL
{
    public partial class PBTProDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int> //DbContext
    {
        private string? _currentUser;
        public PBTProDbContext()
        {
        }

        public PBTProDbContext(string RunUser)
        {
            _currentUser = RunUser;
        }

        public PBTProDbContext(DbContextOptions options) : base(options)
        {
        }

        public PBTProDbContext(DbContextOptions<PBTProDbContext> options, UserResolverService userService)
            : base(options)
        {
            _currentUser = userService.GetUser();
        }

        public DbSet<ApplicationUserRole> UserRoles { get; set; }

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
            modelBuilder.HasPostgresExtension("postgis");

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

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("users", "core");
                entity.Ignore(u => u.NormalizedEmail);
                entity.Ignore(u => u.NormalizedUserName);

                // Default Identity fields
                entity.Property(u => u.Id).HasColumnName("user_id");
                entity.Property(u => u.UserName).HasColumnName("user_name");
                entity.Property(u => u.PasswordHash).HasColumnName("pwd_hash");
                entity.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(u => u.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.Salt).HasColumnName("salt");
                entity.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
                entity.Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
                entity.Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled"); 
                entity.Property(e => e.LockoutEnd)
                .HasPrecision(6)
                .HasDefaultValueSql("NULL::timestamp with time zone")
                .HasComment("Timestamp indicating when the user lockout ends, if applicable (used for account lockout after failed login attempts).")
                .HasColumnName("lockout_end");
                entity.Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
                entity.Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");
                entity.Property(u => u.PhoneNumber).HasColumnName("phoneno");

                // Custom fields
                entity.Property(u => u.IdNo).HasColumnName("idno");
                entity.Property(u => u.IdTypeId).HasColumnName("id_type_id");
                entity.Property(u => u.PwdUpdateAt).HasColumnName("pwd_update_at").HasColumnType("timestamp without time zone");
                entity.Property(u => u.LastLogin).HasColumnName("last_login").HasColumnType("timestamp without time zone");
                entity.Property(u => u.VerificationCode).HasColumnName("verification_code");
                entity.Property(u => u.UserStatusId).HasColumnName("user_status_id");
                entity.Property(u => u.IsDeleted).HasColumnName("is_deleted");
                entity.Property(u => u.dept_id).HasColumnName("dept_id");
                entity.Property(u => u.dept_name).HasColumnName("dept_name");
                entity.Property(u => u.div_id).HasColumnName("div_id");
                entity.Property(u => u.div_name).HasColumnName("div_name");
                entity.Property(u => u.unit_id).HasColumnName("unit_id");
                entity.Property(u => u.unit_name).HasColumnName("unit_name");
                entity.Property(u => u.full_name).HasColumnName("full_name");

                // Auditing fields
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Timestamp indicating when the user record was created.")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.CreatorId)
                    .HasComment("ID of the user who created this record.")
                    .HasColumnName("creator_id");
                entity.Property(e => e.ModifiedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Timestamp indicating when the user record was last modified.")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified_at");
                entity.Property(e => e.ModifierId)
                    .HasComment("ID of the user who last modified this record.")
                    .HasColumnName("modifier_id");
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("roles", "core");
                entity.Ignore(r => r.NormalizedName);
                entity.Ignore(r => r.ConcurrencyStamp);

                entity.Property(r => r.Id).HasColumnName("role_id");
                entity.Property(r => r.Name).HasColumnName("role_name");
                entity.Property(r => r.RoleDesc).HasColumnName("role_desc");
                entity.Property(r => r.IsDefaultRole).HasColumnName("is_default_role");
                entity.Property(r => r.IsTenant).HasColumnName("is_tenant");
                entity.Property(r => r.IsDeleted).HasColumnName("is_deleted");

                // Auditing fields
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Timestamp indicating when the user record was created.")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.CreatorId)
                    .HasComment("ID of the user who created this record.")
                    .HasColumnName("creator_id");
                entity.Property(e => e.ModifiedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Timestamp indicating when the user record was last modified.")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified_at");
                entity.Property(e => e.ModifierId)
                    .HasComment("ID of the user who last modified this record.")
                    .HasColumnName("modifier_id");
            });

            modelBuilder.Ignore<IdentityUserRole<int>>();
            modelBuilder.Entity<ApplicationUserRole>(entity =>
            {
                entity.ToTable("user_roles", "core");

                //entity.HasKey(r => r.UserRoleId);
                entity.HasKey(r => new { r.UserId, r.RoleId });

                entity.Property(r => r.UserRoleId)
                .HasColumnName("user_role_id")
                .ValueGeneratedOnAdd();

                entity.Property(r => r.RoleId).HasColumnName("role_id");
                entity.Property(r => r.UserId).HasColumnName("user_id");
                entity.Property(r => r.IsDeleted).HasColumnName("is_deleted");

                // Auditing fields
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Timestamp indicating when the user record was created.")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.CreatorId)
                    .HasComment("ID of the user who created this record.")
                    .HasColumnName("creator_id");
                entity.Property(e => e.ModifiedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Timestamp indicating when the user record was last modified.")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified_at");
                entity.Property(e => e.ModifierId)
                    .HasComment("ID of the user who last modified this record.")
                    .HasColumnName("modifier_id");

                //entity.HasOne<ApplicationUser>()
                //.WithMany(u => u.ApplicationUserRoles)
                //.HasForeignKey(r => r.UserId)
                //.OnDelete(DeleteBehavior.Restrict);

                //entity.HasOne<ApplicationRole>()
                //    .WithMany(r => r.ApplicationUserRoles)
                //    .HasForeignKey(r => r.RoleId)
                //    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<IdentityUserRole<int>>().HasKey(r => new { r.UserId, r.RoleId });

            modelBuilder.Ignore<IdentityUserLogin<int>>();
            modelBuilder.Ignore<IdentityUserToken<int>>();
            modelBuilder.Ignore<IdentityUserClaim<int>>();
            modelBuilder.Ignore<IdentityRoleClaim<int>>();
        }
    }
}
