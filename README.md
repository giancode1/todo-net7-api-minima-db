Api minina .net 7:
```
dotnet new web -o TodosApi -f net7.0
```
```
dotnet build
```
Paquetes, instación individual:
```cmd
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 7.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design --version 7.0.2
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 7.0.2
```

Si no lo tienes instalado:
```
dotnet tool install --global dotnet-ef
```
migraciones:
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```