# 2021-dotnet-tile_03

Docker DB command:
docker run -d --name SQLServer -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=p@ssw0rd" -e "MSSQL_PID=Developer" -p 1433:1433 microsoft/mssql-server-linux:2017-latest 

Tools > Package Manager Console

PM: drop-database
PM: update-database

If it doesn't work delete docker DB and run above command. lol
