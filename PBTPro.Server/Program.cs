using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using DevExpress.DashboardCommon;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using DevExpress.AspNetCore;
//using Blazored.Toast;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using System.Runtime.InteropServices;
using GoogleMapsComponents;
using DevExpress.XtraCharts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PBTPro.DAL;
using Blazored.Toast;
using PBTPro.Services;
using PBTPro.Data;
using PBTPro.DAL.Services;
using Serilog;
using Serilog.Events;
using PBTPro.DAL.Models;



var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("../logs/PBT-.log",
                          rollingInterval: RollingInterval.Day,
                          outputTemplate: "{Timestamp:dd-MMM-yyyy HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                          restrictedToMinimumLevel: LogEventLevel.Warning)
            .WriteTo.File("../logs/info/PBT-.log",
                          rollingInterval: RollingInterval.Hour,
                          buffered: true,
                          outputTemplate: "{Timestamp:dd-MMM-yyyy HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                          restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error)
            .Enrich.FromLogContext()
            .CreateLogger();
builder.Logging.AddSerilog(Log.Logger);

// Add this to configure Kestrel with the settings from appsettings.json
// commented due to conflict on deploment
//builder.WebHost.ConfigureKestrel(options => { options.Configure(builder.Configuration.GetSection("Kestrel")); });

//Dashboard
IFileProvider fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredToast();
builder.Services.AddBlazorBootstrap();

var myFirstClass = builder.Configuration.GetSection("GoogleMap:ApiKey").Value;
builder.Services.AddBlazorGoogleMaps(new GoogleMapsComponents.Maps.MapApiLoadOptions(myFirstClass)
{
    Version = "3.58",
    Libraries = "places,visualization,marker"
});

builder.Services.AddDevExpressBlazor();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
builder.Services.AddDevExpressServerSideBlazorReportViewer();
builder.Services.AddControllers();

builder.Services
    .AddResponseCompression()
    .AddDistributedMemoryCache()
    .AddSession()
    .AddDevExpressControls()
    .AddControllersWithViews();

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<CompoundService>();
builder.Services.AddTransient<UserProfileService>();
builder.Services.AddTransient<RoleService>();
builder.Services.AddTransient<UserRoleService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<RoleMenuService>();
builder.Services.AddTransient<NoticeService>();
builder.Services.AddTransient<FaqService>();
builder.Services.AddTransient<ConfigFormFieldService>();
builder.Services.AddTransient<DepartmentService>();
builder.Services.AddTransient<SectionService>();
builder.Services.AddTransient<UnitService>();
builder.Services.AddTransient<CountryService>(); 
builder.Services.AddTransient<StateService>();
builder.Services.AddTransient<DistrictService>();
builder.Services.AddTransient<TownService>();
builder.Services.AddTransient<AuditService>();
builder.Services.AddTransient<ArchiveAuditService>();
builder.Services.AddTransient<PatrolService>();
builder.Services.AddTransient<RefLawActService>();
builder.Services.AddTransient<RefLawSectionService>();
builder.Services.AddTransient<RefLawUUKService>();
builder.Services.AddTransient<RefLawOffenseService>();
builder.Services.AddTransient<BkgrTaskSMService>();
builder.Services.AddTransient<ContactUsService>();
builder.Services.AddTransient<MenuService>();
builder.Services.AddTransient<PermissionService>();
builder.Services.AddTransient<EmailerService>();


builder.Services.AddSingleton<FileUrlStorageService>();
builder.Services.AddHostedService<EmailNotificationService>();

builder.Services.AddDbContext<PBTProDbContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite()));

//builder.Services.AddDbContext<PBTProTenantDbContext>(options =>
//        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite()));

var dataDirectory = fileProvider.GetFileInfo("App_Data").PhysicalPath; //Path.Combine(hostingEnvironment.ContentRootPath, "App_Data");

AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

var cultureInfo = new CultureInfo("ms-MY");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


//Dashboard
builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) =>
{
    DashboardConfigurator configurator = new DashboardConfigurator();
    configurator.SetDashboardStorage(serviceProvider.GetService<SessionDashboardStorage>());

    //DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo("~/App_Data/Dashboards").PhysicalPath);
    // Fix: Use absolute path for Dashboards folder
    var dashboardPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Dashboards");
    DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(dashboardPath);
    configurator.SetDashboardStorage(dashboardFileStorage);

    //configurator.SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath));
    //////DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
    //////configurator.SetDataSourceStorage(dataSourceStorage);
    configurator.SetConnectionStringsProvider(new MyDataSourceWizardConnectionStringsProvider(configuration));
    return configurator;
});

//////builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
//////    return DashboardUtils.CreateDashboardConfigurator(configuration, fileProvider);
//////});

//builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<SessionDashboardStorage>();

// commented due to conflict on deploment
//builder.WebHost.UseStaticWebAssets();

//ismail - for standard login
builder.Services.AddScoped<PBTAuthUserService>();
builder.Services.AddScoped<PBTAuthStateProvider>();
builder.Services.AddScoped<PBTAuthPermissionService>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<PBTAuthStateProvider>());

//////builder.Services.AddScoped<AllocationUserService>();
//////builder.Services.AddScoped<AllocationAuthenticationStateProvider>();
//////builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AllocationAuthenticationStateProvider>());
//***
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

// 07/11/2024 - ismail adding for API call services
builder.Services.AddHttpClient<ApiConnector>(client =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//app.UseAuthentication();
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseResponseCompression();
app.UseSession();
app.UseDevExpressControls();

app.MapDashboardRoute("dashboardControl", "DefaultDashboard");
//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapControllers(); // This line maps the controller routes
    endpoints.MapFallbackToPage("/_Host");
});


if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    DevExpress.Drawing.Internal.DXDrawingEngine.ForceSkia();
}
app.Run();
