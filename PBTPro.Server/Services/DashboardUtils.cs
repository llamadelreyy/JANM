using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using PBTPro.Data;
using System;

namespace PBTPro.Services
{
    public static class DashboardUtils
    {
        public static DashboardConfigurator CreateDashboardConfigurator(IConfiguration configuration, IFileProvider fileProvider)
        {
            DashboardConfigurator configurator = new DashboardConfigurator();
            //configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
            configurator.SetConnectionStringsProvider(new MyDataSourceWizardConnectionStringsProvider(configuration));

            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath);
            configurator.SetDashboardStorage(dashboardFileStorage);

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
            configurator.SetDataSourceStorage(dataSourceStorage);

            //DashboardConfigurator configurator = new DashboardConfigurator();
            //configurator.SetDashboardStorage(fileProvider.GetService<SessionDashboardStorage>());

            //DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo("~/App_Data/Dashboards").PhysicalPath);
            //configurator.SetDashboardStorage(dashboardFileStorage);

            //configurator.SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath));
            //////DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
            //////configurator.SetDataSourceStorage(dataSourceStorage);
            //configurator.SetConnectionStringsProvider(new MyDataSourceWizardConnectionStringsProvider(configuration));
            //return configurator;


            // Registers an SQL data source.
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
            sqlDataSource.DataProcessingMode = DataProcessingMode.Client;
            SelectQuery query = SelectQueryFluentBuilder
                .AddTable("Categories").SelectAllColumnsFromTable()
                .Join("Products", "CategoryID").SelectAllColumnsFromTable()
                .Build("Products_Categories");
            sqlDataSource.Queries.Add(query);
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());

            // Registers an Object data source.
            DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
            objDataSource.DataId = "Object Data Source Data Id";
            dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());

            // Registers an Excel data source.
            DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("Excel Data Source");
            excelDataSource.ConnectionName = "Excel Data Source Connection Name";
            excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Sheet1"));
            dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());

            configurator.SetDataSourceStorage(dataSourceStorage);

            configurator.DataLoading += (s, e) =>
            {
                if (e.DataId == "Object Data Source Data Id")
                {
                    //e.Data = Invoices.CreateData();
                }
            };
            configurator.ConfigureDataConnection += (s, e) =>
            {
                if (e.ConnectionName == "Excel Data Source Connection Name")
                {
                    ExcelDataSourceConnectionParameters excelParameters = (ExcelDataSourceConnectionParameters)e.ConnectionParameters;
                    excelParameters.FileName = fileProvider.GetFileInfo("App_Data/Sales.xlsx").PhysicalPath;
                }
                else if (e.ConnectionName == "Excel Data Source Connection Name Sheet1")
                {
                    ExcelDataSourceConnectionParameters excelParameters = (ExcelDataSourceConnectionParameters)e.ConnectionParameters;
                    excelParameters.FileName = fileProvider.GetFileInfo("App_Data/dbTransportation.xlsx").PhysicalPath;
                }
                else if (e.ConnectionName == "Excel Data Source Connection Name Sheet2")
                {
                    ExcelDataSourceConnectionParameters excelParameters = (ExcelDataSourceConnectionParameters)e.ConnectionParameters;
                    excelParameters.FileName = fileProvider.GetFileInfo("App_Data/db Beef Market.xlsx").PhysicalPath;
                }

            };
            return configurator;
        }
    }
}