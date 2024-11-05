using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.DataProtection;

//////namespace AllocationReport {
//////{
//////    public class DefaultDashboardController : DashboardController
//////    {
//////        public DefaultDashboardController(DashboardConfigurator configurator, IDataProtectionProvider? dataProtectionProvider = null)
//////            : base(configurator, dataProtectionProvider)
//////        {
//////        }
//////    }
//////}

namespace PBT.Controllers
{
    public class DefaultDashboardController : DashboardController
    {
        public DefaultDashboardController(DashboardConfigurator configurator, IDataProtectionProvider dataProtectionProvider = null)
            : base(configurator, dataProtectionProvider)
        {
        }
    }
}