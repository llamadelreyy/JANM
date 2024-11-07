using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PBTPro.DAL.Services;

namespace PBTPro.DAL
{
    public partial class PBTProDbContext : IdentityDbContext<ApplicationUser> //DbContext
    {
        private string? _currentUser;
        public PBTProDbContext()
        {
        }

        public PBTProDbContext(string RunUser)
        {
            _currentUser = RunUser;
        }

        public PBTProDbContext(DbContextOptions<PBTProDbContext> options, UserResolverService userService)
            : base(options)
        {
            _currentUser = userService.GetUser();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        var configuration = new ConfigurationBuilder()
        //            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        //            .AddJsonFile("appsettings.json")
        //            .Build();
        //        var test = configuration.GetConnectionString("DefaultConnection");
        //        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite());
        //    }
        //}

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity("PBTPro.DAL.ApplicationUser", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("NVARCHAR2(450)");

                b.Property<int>("AccessFailedCount")
                    .HasColumnType("NUMBER(10)");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("Email")
                    .HasMaxLength(256)
                    .HasColumnType("NVARCHAR2(256)");

                b.Property<bool>("EmailConfirmed")
                    .HasColumnType("NUMBER(1)");

                b.Property<bool>("LockoutEnabled")
                    .HasColumnType("NUMBER(1)");

                b.Property<DateTimeOffset?>("LockoutEnd")
                    .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                b.Property<string>("NormalizedEmail")
                    .HasMaxLength(256)
                    .HasColumnType("NVARCHAR2(256)");

                b.Property<string>("NormalizedUserName")
                    .HasMaxLength(256)
                    .HasColumnType("NVARCHAR2(256)");

                b.Property<string>("PasswordHash")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<bool>("PhoneNumberConfirmed")
                    .HasColumnType("NUMBER(1)");

                b.Property<string>("SecurityStamp")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<bool>("TwoFactorEnabled")
                    .HasColumnType("NUMBER(1)");

                b.Property<string>("UserName")
                    .HasMaxLength(256)
                    .HasColumnType("NVARCHAR2(256)");

                b.Property<string>("Name")
                    .HasMaxLength(50)
                    .HasColumnType("NVARCHAR2(50)");

                b.Property<string>("NetworkId")
                    .HasColumnType("NVARCHAR2(50)");

                b.Property<string>("OfficePhone")
                    .HasColumnType("NVARCHAR2(50)");

                b.Property<string>("Status")
                    .HasMaxLength(10)
                    .HasColumnType("NVARCHAR2(10)");

                b.Property<DateTime?>("LastSeenDtm")
                    .HasColumnType("TIMESTAMP(6)");

                b.Property<string>("CreatedBy")
                .IsRequired(false)
                    .HasMaxLength(30)
                    .HasColumnType("NVARCHAR2(30)");

                b.Property<DateTime>("CreatedDtm")
                .IsRequired(true)
                    .HasPrecision(6)
                    .HasColumnType("TIMESTAMP(6)");

                b.Property<string>("ModifiedBy")
                    .HasColumnType("NVARCHAR2(30)");

                b.Property<DateTime?>("ModifiedDtm")
                    .HasColumnType("TIMESTAMP(6)");

                b.Property<string>("UnitOffice")
                    .HasColumnType("NVARCHAR2(15)");

                b.Property<string>("Department")
                    .HasColumnType("NVARCHAR2(100)");

                b.HasKey("Id");

                b.HasIndex("NormalizedEmail")
                    .HasDatabaseName("EmailIndex");

                b.HasIndex("NormalizedUserName")
                    .IsUnique()
                    .HasDatabaseName("UserNameIndex")
                    .HasFilter("\"NormalizedUserName\" IS NOT NULL");

                b.Property<string>("LoginKey")
                .IsRequired(false)
                    .HasColumnType("NVARCHAR2(50)");

                b.ToTable("AspNetUsers", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
            {
                b.Property<string>("Id")
                .HasColumnType("NVARCHAR2(450)");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("Name")
                    .HasMaxLength(256)
                    .HasColumnType("NVARCHAR2(256)");

                b.Property<string>("NormalizedName")
                    .HasMaxLength(256)
                    .HasColumnType("NVARCHAR2(256)");

                b.HasKey("Id");

                b.HasIndex("NormalizedName")
                    .IsUnique()
                    .HasDatabaseName("RoleNameIndex")
                    .HasFilter("\"NormalizedName\" IS NOT NULL");

                b.ToTable("AspNetRoles", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("NUMBER(10)");

                b.Property<string>("ClaimType")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("RoleId")
                    .IsRequired()
                    .HasColumnType("NVARCHAR2(450)");

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("AspNetRoleClaims", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("NUMBER(10)");

                b.Property<string>("ClaimType")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("NVARCHAR2(450)");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserClaims", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
            {
                b.Property<string>("LoginProvider")
                    .HasColumnType("NVARCHAR2(450)");

                b.Property<string>("ProviderKey")
                    .HasColumnType("NVARCHAR2(450)");

                b.Property<string>("ProviderDisplayName")
                    .HasColumnType("NVARCHAR2(2000)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("NVARCHAR2(450)");

                b.HasKey("LoginProvider", "ProviderKey");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserLogins", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("NVARCHAR2(450)");

                b.Property<string>("RoleId")
                    .HasColumnType("NVARCHAR2(450)");

                b.HasKey("UserId", "RoleId");

                b.HasIndex("RoleId");

                b.ToTable("AspNetUserRoles", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("NVARCHAR2(450)");

                b.Property<string>("LoginProvider")
                    .HasColumnType("NVARCHAR2(450)");

                b.Property<string>("Name")
                    .HasColumnType("NVARCHAR2(450)");

                b.Property<string>("Value")
                    .HasColumnType("NVARCHAR2(2000)");

                b.HasKey("UserId", "LoginProvider", "Name");

                b.ToTable("AspNetUserTokens", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
            {
                b.HasOne("PBTPro.DAL.ApplicationUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
            {
                b.HasOne("PBTPro.DAL.ApplicationUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("PBTPro.DAL.ApplicationUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
            {
                b.HasOne("PBTPro.DAL.ApplicationUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}
