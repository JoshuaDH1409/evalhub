# EvalHub

**EvalHub** es un sistema web de **evaluaciones de desempeño** pensado como proyecto de portafolio. Permite gestionar periodos de evaluación, objetivos, competencias, reportes/dashboard y recordatorios por correo.

> Repositorio GitHub: `Sistema-de-Evalauciones` (nombre histórico). Producto visible: **EvalHub**.

## ¿Qué hace?

- **Evaluaciones de desempeño** entre evaluado y evaluador (flujos por etapas).
- **Objetivos** (definición, revisión, cumplimiento, resumen/KPI).
- **Competencias** y catálogos asociados.
- **Periodos** de evaluación y gestión operativa.
- **Dashboard** con conteos y vistas por país/área/periodo.
- **Usuarios** (alta, carga masiva) e historial.
- **Plantillas de correo** y servicio de recordatorios (Windows Service).
- Generación de **PDF/Word** (Spire) en flujos de resumen.

## Stack

| Capa | Tecnología |
|------|------------|
| Web | ASP.NET MVC (.NET Framework 4.8), Razor, Bootstrap, jQuery, DataTables |
| Datos | SQL Server vía **ADO.NET** (capas `CRUD` / `CapaLogica` / `Modelo`) |
| Auth | Formularios / Identity helpers (sesión validada en controladores) |
| Correo | Microsoft Graph (OAuth2) — credenciales **no incluidas** (placeholders) |
| Extra | Servicio Windows de recordatorios, instalador Visual Studio |

> **Requisito realista:** Windows + Visual Studio (o Build Tools) con workload .NET Framework / ASP.NET, y SQL Server (LocalDB o Express). No es una app multiplataforma .NET Core.

## Estructura de la solución

Abrir `PlantillaMVC.sln` en Visual Studio.

| Carpeta / proyecto | Rol |
|--------------------|-----|
| `PlantillaMVC/` | App web MVC (Views, Controllers, Content, Imgs) |
| `Modelo/` | Entidades y modelos de dominio |
| `CapaLogica/` | Lógica de negocio, correo, acceso a datos |
| `CRUD/` | Acceso ADO.NET a SQL Server / utilidades |
| `General/` | Utilidades compartidas (config, cifrado, errores) |
| `RecordatoriosService/` / `EvalDesServ/` / `ServiceEvalDes/` | Servicios de recordatorios / background |
| `InstaladorRecordatorios/` | Proyecto de instalador (.vdproj) |
| `PruebaRecordatorios/` | Proyecto de prueba del servicio |

## Cómo abrir y ejecutar (Visual Studio)

1. Clonar el repo y abrir **`PlantillaMVC.sln`**.
2. Restaurar paquetes NuGet.
3. Configurar SQL Server y editar `PlantillaMVC/Web.config`:
   - Completar `connectionStrings` (valores vacíos a propósito en este portafolio).
   - Ajustar `DefaultConexionBaseDatos` al nombre de la cadena que uses.
4. Establecer **PlantillaMVC** como proyecto de inicio.
5. Ejecutar con IIS Express (F5).

### Configuración sensible

Los archivos `Web.config` / `App.config` y las clases de correo usan **placeholders vacíos**. No hay credenciales de producción en la rama de portafolio. Para correo Graph, rellena TenantId / ClientId / ClientSecret solo en local (no commits).

## Notas de portafolio

- UI rebrand a **EvalHub** (layout, login, CSS, logos neutros).
- Logos/empresas de cliente originales sustituidos o eliminados.
- Bitácoras históricas en `PlantillaMVC/Bit/` pueden contener texto operativo antiguo; no forman parte de la UI.
- Licencias Spire (`*.elic.xml`): metadatos de organización neutralizados; las claves de producto son del stack comercial y deben gestionarse aparte.

## Licencia

Proyecto personal de portafolio. Todo el código y marcas de terceros (Bootstrap, DataTables, Spire, etc.) conservan sus respectivas licencias.
