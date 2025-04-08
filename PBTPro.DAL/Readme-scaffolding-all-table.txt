Scaffold-DbContext "server=localhost;port=5432;Username=postgres;password=Password1;database=PBTProDB;" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir "Models" -Context "PBTProDbContext" -ContextDir "." -Namespace "PBTPro.DAL.Models" -NoOnConfiguring -f -v -UseDatabaseNames


Scaffold-DbContext "server=45.64.169.215;port=54321;Username=postgres;password=ib6h~@F81;database=PBTProDB;" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir "Models" -Context "PBTProTenantDbContext" -ContextDir "." -Namespace "PBTPro.DAL.Models" -NoOnConfiguring -f -v -UseDatabaseNames -schema "tenant"