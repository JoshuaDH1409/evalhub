# Runbook operativo (recorrido por vistas)

> Sistema ASP.NET MVC (.NET Framework 4.8). Todas las acciones relevantes validan sesión con `Utilidades.ValidaSesion` y registran eventos en bitácora. Usar este documento para capacitar a personal operativo en el flujo completo de la aplicación.

## 1. Autenticación y sesión
- `Account/Login` (`Views/Account/Login.cshtml`): ingreso al sistema.
- `Account/ForgotPassword`, `ConfirmEmail` y pantallas de MFA gestionan recuperación y verificación.
- `Manage` (`Views/Manage/*.cshtml`): cambio de contraseña, manejo de inicios de sesión externos.

## 2. Dashboard
- `Dash/DashBoard` (`Views/Dash/DashBoard.cshtml`, controlador `DashController.DashBoard`): entrada al panel. Configura listas de países según perfil y registra bitácora.
- Parciales de filtros: `_DetallePeriodo` y `_DetalleArea` (cargan periodos y áreas por país/periodo).
- `Dash/Consultar` (`Views/Dash/Consultar.cshtml`, acción `DashController.Consultar`): genera conteos por estado de evaluación y produce datos para gráficos; requiere selección de país y periodo.
- Vistas específicas: `DashObjetivesView`, `DashObjetivesEmpleados`, `Dash/Index` (landing parcial), `_DetalleArea`, `_DetallePeriodo`.

## 3. Gestión de evaluaciones (Administradores/Regionales)
- `Gestion/GestionEvaluacion` (`Views/Gestion/GestionEvaluacion.cshtml`, acción `GestionController.GestionEvaluacion`): listado de sesiones/evaluaciones por país/perfil y periodo activo. Convierte fechas de periodo y muestra usuarios.
- `Gestion/NuevaGestion` (`GestionController.NuevaGestion`): operaciones sobre evaluaciones según parámetros `status` y `proceso`:
  - `status=0, proceso=0, Id>0`: inicia evaluación individual, crea `EEval`, envía correo plantilla 13.
  - `status=0, proceso=0, Id=0`: inicia evaluaciones masivas para todos los usuarios del país y notifica por correo.
  - `status=0, proceso=1, Id>0`: elimina evaluación activa.
  - `status=0, proceso=99, Id>0`: reinicia objetivos (pone `Status=0`).
  - `status=0, proceso=200, Id>0`: crea evaluación a partir de periodo del país (flujo similar al alta individual).

## 4. Mis evaluaciones (Colaboradores/Evaluadores)
- `MisEvaluaciones/MiEvaluacion` y `MisEvaluaciones/Evaluacion` (acciones en `MisEvaluacionesController`): captura y seguimiento de objetivos y evaluaciones.
- Soporta vistas de detalle y operación: `DetalleObj`, `DetalleEscaleta`, `OperacionObjetivos`, `OperacionCompetencias`, `OperacionObjetivosPDP/PTP`, `GuardaObjetivos`.
- Estados gestionados en `DashController.Consultar` (`Status` 0–15: pendiente, objetivos cargados/rechazados/aprobados, inicio evaluación, autoevaluación, evaluación jefe, segundo nivel, cierre medio año, segunda evaluación, calibración, etc.).

## 5. Objetivos
- `ObjetivosController` maneja flujos de objetivos individuales y de país.
- Vistas principales: `Objetivos/RevisarObjetivos`, `Objetivos/Resumen`, `Objetivos/GetKPI`, `Objetivos/EscaletaEmpresa`, `Objetivos/EscaletaIndividual`, `ObjetivosPais/Index`, `ObjetivosPais/Nuevo`.
- Se utilizan catálogos de KPI y escaletas; validar fechas del periodo antes de aprobar/rechazar objetivos.

## 6. Períodos
- `PeriodosController` con vistas `Periodos/Periodos`, `Periodos/NuevoPeriodo`, `Periodos/Detalle`.
- Define ventanas de objetivos, evaluación y calibración (fechas Start/Finish para cada fase). El periodo seleccionado afecta dashboard, gestión y mis evaluaciones.

## 7. Usuarios
- `UsuarioController`: administración de usuarios.
- Vistas: `Usuario/Usuarios`, `Usuario/NuevoUsuario`, `Usuario/CargaMasivaUsuarios`, `Usuario/CambiarCon`.
- Operaciones: alta/edición, asignación de evaluador, carga masiva (CSV/Excel), cambio de contraseña y activación.

## 8. Correos
- `CorreosController` y vistas `Correos/Correo`, `Correos/EdicionCorreo`.
- Plantillas parametrizadas; envío utiliza `PlantillaMVC.EnvioCorreo` y `CapaLogica.Correo`.
- Ejemplo clave: plantilla 13 para notificar inicio de carga de objetivos (usada en `GestionController.NuevaGestion`).

## 9. Historial y reportes
- `MiHistorialController` con `MiHistorial/Historial` (historial individual) y `HistorialUsrsController` con `HistorialUsrs/ConsultarUsuarios`, `HistorialUsrs/HistorialUsrs` (historial administrativo por usuario).

## 10. Seguridad y filtros
- Filtro `[Autentificado]` en acciones sensibles (`GestionEvaluacion`, etc.).
- Validación de sesión central: `Utilidades.ValidaSesion(Session, HttpContext)` devuelve parcial `Respuesta` si expira.
- Bitácora: `Utilidades.Bitacora.NuevaEntrada` registra usuario y acción.

## 11. Recorrido sugerido para capacitación
1) Ingreso: `Account/Login` y revisión de cambio de contraseña (`Manage/ChangePassword`).
2) Dashboard: abrir `Dash/DashBoard`, elegir país → periodo → (opcional) área, ejecutar `Consultar` para ver gráficas y totales por estado.
3) Gestión (perfil admin/regional/sistemas): `Gestion/GestionEvaluacion` para listar usuarios del país/region. Probar alta individual y masiva (sin ejecutar en productivo) describiendo efectos y correos.
4) Períodos: `Periodos/Periodos` para ver lista; `NuevoPeriodo`/`Detalle` para altas/ediciones (explicar fechas por fase y efecto en objetivos/evaluaciones).
5) Objetivos: `Objetivos/RevisarObjetivos` y `Objetivos/Resumen` para aprobar/rechazar; `GetKPI` para catálogo de indicadores; vistas de escaleta para estructura.
6) Mis Evaluaciones: `MisEvaluaciones/MiEvaluacion` → flujo de captura de objetivos, autoevaluación, evaluación de jefe, segundo nivel. Revisar vistas de operación y detalle.
7) Usuarios: `Usuario/Usuarios` (listado/edición), `NuevoUsuario`, `CargaMasivaUsuarios` (probar plantilla), `CambiarCon`.
8) Correos: `Correos/Correo` (listado), `EdicionCorreo` (editar plantilla). Mostrar ejemplo de envío gatillado desde gestión.
9) Historial: `MiHistorial/Historial` (usuario) y `HistorialUsrs/ConsultarUsuarios` (admin) para auditoría.

## 12. Cómo generar PDF
- Redactar/editar este `.md`, luego convertir a PDF fuera de la app (ej. `pandoc PlantillaMVC/Runbook.md -o Runbook.pdf` o con cualquier procesador de Markdown/Word). No se recomienda generar PDF desde la app productiva.

## 13. Notas operativas
- No exponer credenciales en documentos. Referir a la ubicación segura (vault) y a los roles de acceso.
- Validar siempre que la sesión esté activa antes de ejecutar acciones masivas.
- Revisión de correos: verificar que el servidor SMTP/plantilla estén configurados en `Web.config` y `Utilidades.EnvioCorreo`.
- Cuando se modifiquen periodos o estados de evaluaciones, confirmar los cambios en `Dash/DashBoard` (gráficas) y en `MisEvaluaciones`.


