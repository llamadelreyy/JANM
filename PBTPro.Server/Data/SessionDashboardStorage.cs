using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace PBTPro.Data
{
    public class SessionDashboardStorage : DashboardStorageBase
    {
        const string DashboardStorageKey = "74cba564-c821-439c-a714-40ff6027b1eb";

        readonly IHttpContextAccessor contextAccessor;

        protected HttpContext HttpContext { get { return contextAccessor?.HttpContext; } }

        private Dictionary<string, string> ReadFromSession()
        {
            Dictionary<string, string> result = null;

            ISession session = HttpContext?.Session;
            if (session != null)
            {
                string serializedStorage = session.GetString(DashboardStorageKey) ?? string.Empty;
                result = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedStorage);
                if (result == null)
                {
                    result = Initialize();
                    SaveToSession(result);
                }
            }

            return result;
        }
        private void SaveToSession(Dictionary<string, string> storage)
        {
            HttpContext?.Session?.SetString(DashboardStorageKey, JsonConvert.SerializeObject(storage));
        }
        private Dictionary<string, string> Initialize()
        {
            Dictionary<string, string> storage = new Dictionary<string, string>();

            string dataDirectoryPath = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            string filePath = Path.Combine(dataDirectoryPath, "Dashboards");

            //DirectoryInfo d = new DirectoryInfo(@"D:\Test"); //Assuming Test is your Folder
            DirectoryInfo d = new DirectoryInfo(filePath); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files

            foreach (FileInfo file in Files)
            {
                InitializeCore(Path.GetFileNameWithoutExtension(file.Name), storage);
            }

            ////InitializeCore("SalesOverview", storage);
            ////InitializeCore("CustomerSupport", storage);
            ////InitializeCore("SalesPerformance", storage);
            ////InitializeCore("SalesDetails", storage);
            ////InitializeCore("Financial", storage);
            ////InitializeCore("EnergyStatistics", storage);
            ////InitializeCore("HumanResources", storage);
            ////InitializeCore("ChampionsLeagueStatistics", storage);
            ////InitializeCore("RevenueAnalysis", storage);
            ////InitializeCore("RevenueByIndustry", storage);
            ////InitializeCore("EnergyConsumption", storage);
            ////InitializeCore("WebsiteStatistics", storage);
            ////InitializeCore("EUTradeOverview", storage);
            ////InitializeCore("YTDPerformance", storage);
            ////InitializeCore("DateFilter", storage);
            ////InitializeCore("DataFederation", storage);
            ////InitializeCore("ProductDetails", storage);
            ////InitializeCore("CustomItemExtensions", storage);

            return storage;
        }

        private void InitializeCore(string dashboardID, Dictionary<string, string> storage)
        {
            string dataDirectoryPath = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            string filePath = Path.Combine(dataDirectoryPath, "Dashboards", $"{dashboardID}.xml");
            if (!storage.ContainsKey(dashboardID) && File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    storage.Add(dashboardID, reader.ReadToEnd());
                }
            }
        }

        protected override IEnumerable<string> GetAvailableDashboardsID()
        {
            return ReadFromSession().Keys;
        }
        protected override XDocument LoadDashboard(string dashboardID)
        {
            Dictionary<string, string> storage = ReadFromSession();
            XDocument document = XDocument.Parse(storage[dashboardID]);

            if (dashboardID == "ProductDetails" && HttpContext != null)
            {
                Dashboard dashboard = new Dashboard();
                dashboard.LoadFromXDocument(document);
                string applicationPath = HttpContext.Request.PathBase.ToUriComponent().TrimEnd('/') + "/";

                BoundImageDashboardItem primaryImage = dashboard.Items["primaryImage"] as BoundImageDashboardItem;
                if (primaryImage != null)
                    primaryImage.UriPattern = applicationPath + "Content/ProductDetailsImages/{0}.jpg";

                BoundImageDashboardItem secondaryImage = dashboard.Items["secondaryImage"] as BoundImageDashboardItem;
                if (secondaryImage != null)
                    secondaryImage.UriPattern = applicationPath + "Content/ProductDetailsImages/{0} Secondary.jpg";

                document = dashboard.SaveToXDocument();
            }
            return document;
        }
        protected override void SaveDashboard(string dashboardID, XDocument dashboard, bool createNew)
        {
            Dictionary<string, string> storage = ReadFromSession();
            if (createNew ^ storage.ContainsKey(dashboardID))
            {
                storage[dashboardID] = dashboard.Document.ToString();
                SaveToSession(storage);
            }
        }

        public SessionDashboardStorage(IHttpContextAccessor contextAccessor) : base()
        {
            this.contextAccessor = contextAccessor;
        }
    }
}
