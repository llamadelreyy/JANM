using DevExpress.DashboardCommon;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Web;

namespace PBTPro.Data
{
    // ...

    public class MyDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        public IConfiguration Configuration { get; }
        public MyDataSourceWizardConnectionStringsProvider(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public Dictionary<string, string> GetConnectionDescriptions()
        {
            Dictionary<string, string> connections = new Dictionary<string, string>();

            // Customize the loaded connections list.  
            //////connections.Add("jsonUrlConnection", "JSON URL Connection");
            connections.Add("mySqlConnection", "PBT Connection");
            return connections;
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name)
        {
            if (name == "mySqlConnection")
            {
                //string? strConnectionString = Configuration.GetConnectionString("DashboardDataConnectionString");
                string? strServer = Configuration["Server"];
                string? strDatabase = Configuration["Database"];
                string? strUserID = Configuration["UserID"];
                string? strPassword = Configuration["Password"];
                string? strPort = Configuration["Port"];

                return new MySqlConnectionParameters(strServer, strDatabase, strUserID, strPassword, strPort); // MsSqlConnectionParameters("localhost", "Northwind", "", "", MsSqlAuthorizationType.Windows);
            }

            //////// Return custom connection parameters for the custom connection.
            //////if (name == "jsonUrlConnection")
            //////{
            //////    return new JsonSourceConnectionParameters()
            //////    {
            //////        JsonSource = new UriJsonSource(
            //////            new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"))
            //////    };
            //////}
            //////else if (name == "msSqlConnection")
            //////{
            //////    return new MsSqlConnectionParameters("localhost", "Northwind", "", "", MsSqlAuthorizationType.Windows);
            //////}
            throw new Exception("The connection string is undefined.");
        }
    }
}
