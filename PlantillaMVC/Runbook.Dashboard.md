# Runbook operativo – Dashboard (Detalle punto 2)

Sistema ASP.NET MVC (.NET Framework 4.8). El dashboard se entrega mediante `DashController` y vistas en `Views/Dash/*`. Todas las acciones validan sesión con `Utilidades.ValidaSesion` y registran bitácora (`Bitacora.NuevaEntrada`).

## Alcance
- Capacitación operativa para navegar y usar el dashboard.
- Vistas: `DashBoard`, `Consultar`, parciales `_DetallePeriodo`, `_DetalleArea`, `DashObjetivesView`, `DashObjetivesEmpleados`, `Index` (landing parcial).

## Roles y permisos
- Requiere sesión activa (usuarios autenticados). Los países listados se filtran por perfil: Sistemas ve todos; Regional ve países de su región; otros, solo su país.

## Flujo de navegación
1) **Ingresar a Dashboard**
   - Ruta: `Dash/DashBoard` (`DashController.DashBoard`).
   - Acción: carga listas de países según perfil, asigna `ViewData["ListaPaises"]`.
   - Resultado: vista `Views/Dash/DashBoard.cshtml` (parcial) con selector de país.

2) **Elegir país**
   - Selecciona uno o varios países (si el perfil lo permite).
   - Dispara carga del parcial `_DetallePeriodo` (`DashController._DetallePeriodo`) pasando `IdPais`.
   - Parcial `Views/Dash/_DetallePeriodo.cshtml` muestra los periodos disponibles para el país.

3) **Elegir periodo**
   - Selecciona un periodo de la lista.
   - Dispara carga del parcial `_DetalleArea` (`DashController._DetalleArea`) pasando `IdPeriodo`.
   - Parcial `Views/Dash/_DetalleArea.cshtml` muestra áreas/divisiones asociadas al país del periodo.

4) **(Opcional) Elegir área**
   - Filtra resultados por división/área. Si no se selecciona, se usan todas.

5) **Consultar**
   - Acción: `DashController.Consultar(EFlitrosDash filtros)`.
   - Requiere: `Pais` (lista) y `Periodo` seleccionados. Puede incluir `Division` y la bandera `PeriodoActivo`.
   - Lógica: recupera evaluaciones del periodo (o periodos homólogos por llave), filtra por activos y división, genera conteos por estado `Evaluacion.Status`.
   - Resultado: vista `Views/Dash/Consultar.cshtml` consume la cadena `resultado` con datos para gráficos (Google Charts/JS) y tablas.

## Estados de evaluación (Status) mostrados en dashboard
- 0 Pendiente
- 1 Objetivos cargados
- 2 Objetivos rechazados
- 3 Objetivos aprobados
- 4 Inicio de evaluación
- 5 Comentario evaluado (autoevaluación)
- 6 Comentario jefe
- 7 Evaluación de segundo nivel
- 8 Calificación medio año cerrada
- 10 Inicio evaluación fin de año
- 11 Comentario evaluado fin de año
- 12 Comentario jefe fin de año
- 13 Evaluación de segundo nivel fin de año
- 14 Calificación fin de año cerrada
- 15 Calibración de la evaluación

## Datos generados (salida de `Consultar`)
- Cadena `resultado` con filas: `['Etapa','Cantidad',{role:"style"}]` y pares (etapa, conteo, color). Colores difieren cuando hay filtro por división.
- Totales calculados sobre `ListaEvalSession` (solo usuarios activos).
- Si no hay datos suficientes, devuelve parcial `Respuesta` con mensaje.

## Archivos clave
- Controlador: `PlantillaMVC/Controllers/DashController.cs`
- Vistas: `Views/Dash/DashBoard.cshtml`, `Views/Dash/Consultar.cshtml`, `Views/Dash/_DetallePeriodo.cshtml`, `Views/Dash/_DetalleArea.cshtml`, `Views/Dash/DashObjetivesView.cshtml`, `Views/Dash/DashObjetivesEmpleados.cshtml`, `Views/Dash/Index.cshtml`

## Prerrequisitos operativos
- Sesión válida (si expira, se devuelve parcial `Respuesta`).
- Periodos configurados para los países (ver `PeriodosController`).
- Usuarios y evaluaciones activos ligados a esos periodos.
- Permisos de perfil adecuados para ver países.

## Pasos de validación (ejercicio guiado)
1. Iniciar sesión con usuario con permisos adecuados.
2. Navegar a `Dash/DashBoard` y verificar carga de lista de países según perfil.
3. Seleccionar país → verificar carga de periodos.
4. Seleccionar periodo → verificar carga de áreas (y bandera de periodo activo en `ViewBag.PerActivo`).
5. (Opcional) Seleccionar área.
6. Ejecutar consulta → validar que se renderizan gráficos y totales.
7. Cambiar filtros (otro país/periodo) y repetir para comprobar consistencia.

## Comprobaciones ante incidentes
- Si no aparecen países: revisar perfil del usuario y la sesión.
- Si no aparecen periodos: validar que el país tiene periodos configurados y que `Utilidades.negocio.RecuperaTodosPeriodosPais` devuelve datos.
- Si la consulta devuelve “No se cuenta con suficiente información”: confirmar que hay evaluaciones activas en el periodo y usuarios activos.
- Si gráficos no renderizan: revisar la cadena `resultado` en la vista y la carga de scripts en `Consultar.cshtml` (dependencias JS/Google Charts).

## Buenas prácticas operativas
- Evitar consultas en ventanas de mantenimiento (cuando se estén modificando periodos/evaluaciones masivamente).
- Tras cambios de periodo o altas masivas, refrescar dashboard y verificar totales.
- No exponer credenciales; si se requiere validar conexión a BD/servicios, usar entornos de prueba.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Dashboard.md -o Runbook.Dashboard.pdf`).

## Acciones a evitar (riesgo de errores en dashboard/BD)
- No modificar periodos activos en productivo durante horario operativo; detiene carga de datos y rompe llaves de periodo.
- No ejecutar altas/eliminaciones masivas de evaluaciones/usuarios sin ambiente de prueba y respaldo.
- No borrar evaluaciones activas (Status>0) desde `Gestion` sin confirmar impacto en dashboard y mis evaluaciones.
- No cambiar `Web.config` (cadenas de conexión, SMTP) en caliente; reinicia la app y puede romper envíos o consultas.
- No alterar ni quitar scripts de gráficos en `Views/Dash/Consultar.cshtml`; provoca que no se rendericen los gráficos.
- No editar plantillas de correo en productivo sin validarlas en QA; los placeholders erróneos rompen notificaciones.
- No lanzar consultas sin filtros de país/periodo; puede devolver datos incompletos o tiempos de espera.
- No actualizar catálogos de KPI/áreas directamente en BD sin scripts controlados y respaldo.
- No detener tareas programadas/servicios relacionados con evaluaciones/calibraciones sin ventana aprobada.
- No compartir credenciales ni roles de admin; usar mínimos privilegios y cuentas nominativas.
- No aplicar cambios en horarios de corte de periodo sin comunicar y validar en dashboard post cambio.


