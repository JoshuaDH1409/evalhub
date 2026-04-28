# Runbook operativo – Gestión de evaluaciones (Detalle punto 3)

Sistema ASP.NET MVC (.NET Framework 4.8). La gestión se entrega mediante `GestionController` y vistas en `Views/Gestion/*`. Todas las acciones relevantes validan sesión con `Utilidades.ValidaSesion` y registran bitácora (`Bitacora.NuevaEntrada`).

## Alcance
- Capacitación operativa para administrar evaluaciones (alta, masiva, reinicio, eliminación) por país/periodo.
- Vistas: `GestionEvaluacion` (principal) y parciales `Respuesta` para resultados.

## Roles y permisos
- Requiere sesión activa.
- Perfiles:
  - **Sistemas**: ve todos los países.
  - **Regional**: ve países de su región (`paisesRegion`).
  - **Otros perfiles**: ven únicamente su país.

## Flujo de navegación
1) **Ingresar a Gestión**
   - Ruta: `Gestion/GestionEvaluacion` (`GestionController.GestionEvaluacion`).
   - Acción: carga usuarios y periodo según perfil/país; convierte fechas del periodo a temporales.
   - Resultado: vista `Views/Gestion/GestionEvaluacion.cshtml` con lista de usuarios/estado de evaluación y datos del periodo.

2) **Seleccionar usuario(s)**
   - Para acciones individuales se usa `Id` del usuario.
   - Para acciones masivas, `Id=0` aplica a todos los usuarios activos del país.

3) **Ejecutar acción (`Gestion/NuevaGestion`)**
   - Se llama con parámetros `status`, `proceso`, `Id`:
     - `status=0, proceso=0, Id>0`: **Iniciar evaluación individual**. Crea `EEval`, asigna evaluador del usuario, marca activo y envía correo plantilla 13 al colaborador.
     - `status=0, proceso=0, Id=0`: **Iniciar evaluaciones masivas** para todos los usuarios del país (solo los que no tienen evaluación). Envía correo plantilla 13 a cada uno.
     - `status=0, proceso=1, Id>0`: **Eliminar evaluación activa** del usuario (marca inactiva).
     - `status=0, proceso=99, Id>0`: **Reiniciar objetivos** (pone `Status=0`, mantiene evaluación activa).
     - `status=0, proceso=200, Id>0`: **Crear evaluación** tomando el periodo del país (similar al alta individual; usa `RecuperaPeriodopais`).
   - Resultado: parcial `Respuesta` con mensaje de éxito o error.

## Datos y efectos
- Alta individual/masiva: crea registros `EEval` ligados a usuario, periodo y evaluador; marca `Activo=true` y setea `Modificadopor` con el usuario en sesión.
- Correos: usa plantilla 13 (`CorreosController`/catálogo de correos) con `EnvioCorreo.SendMail`.
- Reinicio de objetivos: resetea `Status` a 0 en la evaluación activa.
- Eliminación: marca evaluación activa como inactiva.

## Prerrequisitos operativos
- Sesión válida y perfil con permisos sobre el país.
- Periodo activo configurado para el país (`RecuperaPeriodopais`).
- Usuarios con evaluador asignado (`EvaluadorIdSap`) y correo válido para notificaciones.

## Pasos de validación (ejercicio guiado)
1. Iniciar sesión con perfil autorizado (admin/regional/sistemas).
2. Abrir `Gestion/GestionEvaluacion` y confirmar que se muestran usuarios y fechas del periodo.
3. Probar **alta individual** con un usuario de prueba: ejecutar acción `proceso=0, Id=<usuario>` y validar mensaje `Respuesta` y recepción de correo (en entorno de pruebas).
4. Probar **alta masiva** (`Id=0`) en entorno de pruebas: confirmar que solo crea evaluaciones faltantes y envía correos.
5. Probar **reinicio de objetivos** (`proceso=99`) sobre un usuario con evaluación existente: validar que `Status` vuelva a 0 y que aparezca como pendiente.
6. Probar **eliminación** (`proceso=1`) sobre un usuario de prueba: validar que la evaluación quede inactiva.
7. Verificar reflejo de cambios en `Dash/DashBoard` y `MisEvaluaciones` tras cada acción.

## Comprobaciones ante incidentes
- Si no carga usuarios o periodo: validar sesión y que `RecuperaLiUsuariosPais`/`RecuperaPeriodopais` devuelvan datos para el país/perfil.
- Si correos no se envían: revisar plantilla 13, configuración SMTP en `Web.config` y `Utilidades.EnvioCorreo`.
- Si una acción devuelve error genérico: revisar bitácora y logs de negocio (`Utilidades.negocio.GuardaEvaluacion`).
- Si se crean evaluaciones duplicadas: confirmar filtros en masivo (solo crea cuando `Evaluacion == null`).

## Buenas prácticas operativas
- Realizar pruebas en ambiente QA antes de ejecuciones masivas.
- Usar usuarios de prueba para validar correos y estado de evaluaciones.
- Registrar fecha/hora y responsable de acciones masivas.
- Confirmar en dashboard los conteos después de operaciones críticas.
- Mantener actualizadas las plantillas de correo en QA antes de promover a producción.

## Acciones a evitar (riesgo en gestión/BD)
- No ejecutar altas masivas en producción sin respaldo y ventana autorizada.
- No eliminar evaluaciones con `Status>0` sin validar impacto en `MisEvaluaciones`/dashboard.
- No reiniciar objetivos en periodos cerrados o fuera de ventana; puede desalinear fechas y estados.
- No modificar el periodo activo del país durante operaciones en curso.
- No editar `Web.config` ni plantillas de correo en caliente sin pruebas previas.
- No correr acciones con sesión expirada; siempre revalidar sesión antes de masivos.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Gestion.md -o Runbook.Gestion.pdf`).

