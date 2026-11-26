# Plan: listar libros desde SQL Server

## Objetivo

Conectar la aplicación MVC a SQL Server mediante Entity Framework Core y mostrar en `/Libros` los registros de libros existentes.

## Estado de implementación

- [x] Agregar el proveedor SQL Server y las herramientas de diseño de EF Core 11.
- [x] Registrar User Secrets para la aplicación y `dotnet-ef` como herramienta local.
- [x] Crear `Data/BibliotecaDbContext.cs`.
- [x] Registrar el contexto usando `ConnectionStrings:DefaultConnection`.
- [x] Consultar los libros de forma asíncrona y sin seguimiento de cambios.
- [x] Crear la vista `Views/Libros/Index.cshtml`, incluyendo el estado sin resultados.
- [x] Configurar la conexión local en User Secrets, sin guardar la clave en el repositorio.
- [x] Confirmar que no había una base de usuario ni esquema previo en SQL Server.
- [x] Crear y aplicar la migración inicial `20261124142341_InitialCreate` a `BibliotecaDb`.
- [x] Compilar y probar `/Libros` contra SQL Server con la tabla vacía.

## Estado actual revisado

- La aplicación es `Biblioteca.Web` y apunta a `net11.0`.
- Ya existe `Models/Libro.cs`, con `Id`, `Titulo`, `Autor` y `Anio`.
- `Controllers/LibrosController.cs` consulta libros desde el contexto y devuelve la vista.
- `Views/Libros/Index.cshtml` presenta título, autor y año o indica que no hay registros.
- `Data/BibliotecaDbContext.cs` expone el conjunto `Libros`.
- La conexión local se guarda en User Secrets y utiliza el login propio `biblioteca_app`, miembro de `db_owner` únicamente en `BibliotecaDb`; la aplicación no usa `sa`.
- Se verificó que el login puede conectarse a `BibliotecaDb`, que `IS_ROLEMEMBER('db_owner')` devuelve `1` y que tiene permisos de escritura sobre `dbo.Libros`.
- SQL Server no tenía bases de usuario. Se creó `BibliotecaDb` con `dbo.Libros` y el historial de EF Core registra `20261124142341_InitialCreate` (`11.0.0`).
- La tabla coincide con el modelo (`Id`, `Titulo`, `Autor`, `Anio`) y contiene cero libros al momento de la verificación.
- `Docker/docker-compose.yml` define el servicio SQL Server `db`, publica el puerto `1433` y obtiene `MSSQL_SA_PASSWORD` del entorno. El valor del secreto no se mostró ni se guardó en el repositorio.

## Pasos propuestos

1. **Agregar dependencias compatibles con EF Core 11**
   - `Microsoft.EntityFrameworkCore.SqlServer`: proveedor de SQL Server.
   - `Microsoft.EntityFrameworkCore.Design`: soporte de diseño para migraciones.
   - Instalar `dotnet-ef` como herramienta local del repositorio y mantenerlo en una versión compatible con los paquetes EF Core.
   - Se fijaron `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design` y la herramienta local `dotnet-ef` en la versión `11.0.0`, alineada con el objetivo `net11.0`.

2. **Crear el contexto de datos**
   - Crear `Data/BibliotecaDbContext.cs` con `DbSet<Libro>`.
   - Configurar el mapeo de entidad y tabla si el esquema de la base requiere nombres o tipos específicos.
   - Antes de generar una migración inicial, comprobar si la base ya contiene la tabla de libros y comparar sus columnas con el modelo.

3. **Configurar la conexión y registrar EF Core**
   - Registrar `BibliotecaDbContext` en `Program.cs` mediante `AddDbContext` y el proveedor SQL Server.
   - Usar la clave de configuración `ConnectionStrings:DefaultConnection`.
   - En desarrollo, guardar la cadena real en User Secrets o en la variable de entorno `ConnectionStrings__DefaultConnection`; no guardar contraseñas en Git ni en `appsettings.json`.
   - Mantener `MSSQL_SA_PASSWORD` como secreto/variable de entorno para Docker Compose. Es una configuración del contenedor y no reemplaza la cadena de conexión que necesita la aplicación.
   - Ajustar el servidor según dónde corra MVC:
	 - MVC en la máquina host: normalmente `localhost,1433`.
	 - MVC dentro de la misma red de Compose: normalmente `db,1433`.
   - No incluir la clave real en este documento, en ejemplos ni en archivos versionados.

4. **Implementar la consulta del listado**
   - Inyectar `BibliotecaDbContext` en `LibrosController`.
   - Hacer que `Index` consulte los datos de forma asíncrona, de solo lectura (`AsNoTracking`) y con orden determinista.
   - Pasar los resultados a la vista. No agregar una capa Repository para este caso simple salvo que el proyecto la necesite por una decisión de arquitectura más amplia.

5. **Crear la vista MVC**
   - Crear `Views/Libros/Index.cshtml`.
   - Mostrar título, autor y año, y una indicación clara cuando no haya libros.
   - Usar el modelo fuertemente tipado y el estilo/layout existente en el proyecto.

6. **Crear o verificar el esquema**
   - Se confirmó que `BibliotecaDb` no existía y se aplicó `20261124142341_InitialCreate` mediante `dotnet ef database update`.
   - La migración creó `dbo.Libros` (`Id`, `Titulo`, `Autor`, `Anio`) y `__EFMigrationsHistory`.
   - No aplicar migraciones automáticamente al iniciar la aplicación en producción. El login local de desarrollo puede administrar `BibliotecaDb`; en producción, usar un login de ejecución con permisos mínimos y un proceso administrativo separado para migraciones.

7. **Verificar el resultado**
   - La compilación terminó correctamente.
   - Se comprobó que el login de la aplicación se conecta a `BibliotecaDb` y puede consultar `dbo.Libros`.
   - Se probó `GET /Libros`: devolvió HTTP 200 y el mensaje de lista vacía, porque la tabla tiene cero registros.
   - La prueba HTTP se ejecutó localmente y el servidor temporal quedó detenido.

## Archivos previstos

### Crear

- `Biblioteca.Web/Data/BibliotecaDbContext.cs`
- `Biblioteca.Web/Views/Libros/Index.cshtml`
- `Biblioteca.Web/.config/dotnet-tools.json` (herramienta local `dotnet-ef`)
- `Biblioteca.Web/Data/Migrations/` y sus archivos generados, únicamente si se requieren migraciones de EF Core

### Modificar

- `Biblioteca.Web/Program.cs`: registro del contexto y proveedor.
- `Biblioteca.Web/Biblioteca.Web.csproj`: referencias a los paquetes necesarios.
- `Biblioteca.Web/Controllers/LibrosController.cs`: consulta de libros y envío a la vista.
- `Biblioteca.Web/Views/Shared/_Layout.cshtml`: enlace de navegación al listado.
- `Biblioteca.Web/appsettings.json`: solo la estructura de configuración no secreta, si hace falta. Los valores sensibles van fuera del repositorio.

### Reutilizar

- `Biblioteca.Web/Models/Libro.cs`
- `Biblioteca.Web/Docker/docker-compose.yml` y su configuración existente para el servicio SQL Server.

## Configuración local aplicada

La aplicación lee `ConnectionStrings:DefaultConnection`. La cadena se configuró en User Secrets para `BibliotecaDb`, usando `localhost,1433` y el login propio `biblioteca_app`, que tiene el rol `db_owner` solo dentro de esta base local para permitir operaciones de desarrollo. No se usa la cuenta `sa` en la aplicación. La contraseña no se muestra en este documento ni se almacena en el repositorio. `TrustServerCertificate=True` se usa para el SQL Server local del contenedor; en producción debe emplearse un certificado confiable.

Para otros entornos, configurar `ConnectionStrings__DefaultConnection` con la cadena correspondiente y guardar la credencial mediante el gestor de secretos del entorno. La contraseña del contenedor y las credenciales de aplicación deben permanecer fuera del repositorio.

Para el entorno local, `biblioteca_app` puede aplicar migraciones en `BibliotecaDb` por su rol `db_owner`. No usar `sa` como cuenta de ejecución de la aplicación. En producción, aplicar migraciones mediante un proceso administrativo separado y mantener permisos mínimos para el usuario de la aplicación.

## Comandos ejecutados y reproducibles

Ejecutar los comandos desde PowerShell. En los ejemplos, los valores entre `<...>` son marcadores: reemplazarlos solo localmente. **No pegues la contraseña real en este Markdown, en Git, ni en una captura o salida compartida.** Los archivos de código (`Program.cs`, controlador, contexto, vista y navegación) se crearon/editaron en el IDE, no mediante comandos de consola.

### 1. Paquetes y herramienta EF Core

Desde la carpeta `Biblioteca.Web`:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 11.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 11.0.0
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 11.0.0
dotnet user-secrets init
```

El manifiesto local de herramientas se ubicó en `.config/dotnet-tools.json`.

### 2. Iniciar/verificar SQL Server en Docker

El contenedor ya estaba en ejecución. Para iniciarlo desde el Compose del proyecto, si hace falta:

```powershell
docker compose --env-file .\Docker\archivoEnvirotment.env -f .\Docker\docker-compose.yml up -d
docker ps
```

El archivo de entorno debe existir localmente y contener `MSSQL_SA_PASSWORD`; no mostrar su contenido ni añadirlo al repositorio.

### 3. Guardar la cadena como secreto local

En este proyecto se usó User Secrets, no `appsettings.json`. El comando tiene la forma siguiente; sustituye los marcadores por los valores locales. La contraseña del login de aplicación debe ser la misma que se estableció al crear ese login en SQL Server:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=BibliotecaDb;User Id=biblioteca_app;Password=<CLAVE_LOCAL_DE_BIBLIOTECA_APP>;Encrypt=True;TrustServerCertificate=True"
```

`<CLAVE_LOCAL_DE_BIBLIOTECA_APP>` es un marcador, no una clave real. No uses aquí la contraseña `sa` para la aplicación. Desde la raíz del repositorio, especifica el proyecto al listar secretos:

```powershell
dotnet user-secrets list --project .\Biblioteca.Web\Biblioteca.Web.csproj
```

El comando muestra los valores configurados; no compartas su salida. Como alternativa, cambia a la carpeta `Biblioteca.Web` y ejecuta `dotnet user-secrets list` allí.

### 4. Crear/aplicar migración

La base `BibliotecaDb` no existía, así que se generó la migración inicial y se aplicó:

```powershell
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
dotnet ef database update
```

El `database update` requiere una conexión con permisos para crear/modificar el esquema. En el entorno local, `biblioteca_app` tiene `db_owner` sobre `BibliotecaDb`.

### 5. Conceder `db_owner` al login de desarrollo

`biblioteca_app` se creó localmente con una contraseña aleatoria que no se guardó en el repositorio. La creación inicial del login requiere una contraseña nueva y fuerte; usa el mismo valor en el secreto de conexión. Ejemplo de SQL para una base local nueva (requiere una sesión administrativa; `sa` se usa solo para esta operación. Si ya existe el login/usuario, no repitas los comandos `CREATE`):

```sql
CREATE LOGIN [biblioteca_app] WITH PASSWORD = '<CLAVE_LOCAL_GENERADA>', CHECK_POLICY = ON;
GO
USE [BibliotecaDb];
CREATE USER [biblioteca_app] FOR LOGIN [biblioteca_app];
ALTER ROLE [db_owner] ADD MEMBER [biblioteca_app];
GO
```

`<CLAVE_LOCAL_GENERADA>` es solo un marcador, no la contraseña real. No escribas la clave de `sa` en el archivo ni en el comando. Si se usa `sqlcmd` desde PowerShell, pásala de forma local por `SQLCMDPASSWORD`, ejecuta `sqlcmd -S tcp:localhost,1433 -U sa -C -b` y elimina la variable al terminar:

```powershell
$env:SQLCMDPASSWORD = '<CLAVE_SA_LOCAL>'
# Ejecutar sqlcmd y enviar las sentencias SQL anteriores por la entrada estándar.
Remove-Item Env:\SQLCMDPASSWORD
```

En producción, no conceder `db_owner` al usuario de ejecución ni usar `sa` desde la aplicación.

### 6. Validar

```powershell
dotnet build
dotnet ef migrations list
```

Para probar la página, desde `Biblioteca.Web` iniciar temporalmente la aplicación y abrir `/Libros`:

```powershell
dotnet run --urls http://localhost:5187
```

En otra ventana de PowerShell:

```powershell
Invoke-WebRequest http://localhost:5187/Libros -UseBasicParsing
```

La verificación realizada devolvió HTTP 200 y el mensaje de lista vacía. Detener la aplicación con `Ctrl+C` al terminar.
