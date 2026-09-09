# EvalHub

Sistema web de evaluaciones de desempeño: periodos, objetivos, competencias, dashboard y recordatorios por correo.

> Repo: `Sistema-de-Evalauciones`. Producto: **EvalHub**.

## Qué hace

- Evaluaciones entre evaluado y evaluador (por etapas)
- Objetivos (definición, revisión, cumplimiento, KPI)
- Competencias y catálogos
- Periodos y operación del ciclo
- Dashboard por país / área / periodo
- Usuarios (alta y carga masiva) + historial
- Plantillas de correo y servicio Windows de recordatorios
- PDF/Word (Spire) en resúmenes

## Stack

- ASP.NET MVC (.NET Framework 4.8), Razor, Bootstrap, jQuery, DataTables
- SQL Server vía ADO.NET (`CRUD` / `CapaLogica` / `Modelo`)
- Auth por formularios / sesión en controladores
- Correo con Microsoft Graph (OAuth2) — placeholders, sin secretos en el repo
- Servicio Windows de recordatorios + instalador VS

Necesitas Windows + Visual Studio (workload .NET Framework / ASP.NET) y SQL Server (LocalDB o Express). No es .NET Core multiplataforma.

## Solución

Abrir `PlantillaMVC.sln`.

| Proyecto | Rol |
|----------|-----|
| `PlantillaMVC/` | App MVC |
| `Modelo/` | Entidades |
| `CapaLogica/` | Negocio, correo, datos |
| `CRUD/` | ADO.NET / utilidades |
| `General/` | Config, cifrado, errores |
| `RecordatoriosService/` / `EvalDesServ/` / `ServiceEvalDes/` | Background / recordatorios |
| `InstaladorRecordatorios/` | Instalador (.vdproj) |
| `PruebaRecordatorios/` | Pruebas del servicio |

## Cómo ejecutar

1. Abrir `PlantillaMVC.sln` y restaurar NuGet.
2. Configurar SQL Server en `PlantillaMVC/Web.config`:
   - `connectionStrings` (vacíos a propósito)
   - `DefaultConexionBaseDatos` al nombre de tu cadena
3. Proyecto de inicio: **PlantillaMVC**
4. F5 con IIS Express

Para Graph: TenantId / ClientId / ClientSecret solo en local. No los commits.

## Notas

- UI rebrand a EvalHub; logos de cliente quitados o reemplazados
- `PlantillaMVC/Bit/` puede tener bitácoras viejas; no son parte de la UI
- Spire (`*.elic.xml`): claves de producto van aparte

## Licencia

Código propio + dependencias de terceros (Bootstrap, DataTables, Spire, etc.) con sus licencias.
