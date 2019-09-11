# SQL DB Clustering example

This example contains SQL DB clusting example client & silo host,

Besure to create a [SQL Server Express LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) database instance called `Orleans`, and run the [SQLServer-Clustering.sql](./SqlSiloHost/OrleansAdoNetContent/SQLServer/SQLServer-Clustering.sql) to create Silo Membership table, remeber to install [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient) nuget on both client & silo host projects.
