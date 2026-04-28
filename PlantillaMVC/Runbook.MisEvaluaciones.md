# Runbook operativo – Mis evaluaciones (Detalle punto 4)

Sistema ASP.NET MVC (.NET Framework 4.8). Las vistas de colaborador/evaluador se entregan mediante `MisEvaluacionesController` y vistas en `Views/MisEvaluaciones/*`. Todas las acciones relevantes validan sesión con `Utilidades.ValidaSesion` y registran bitácora (`Bitacora.NuevaEntrada`).

## Alcance
- Capacitación operativa para captura y seguimiento de objetivos/evaluaciones por parte de colaboradores y evaluadores.
- Vistas principales: `MiEvaluacion`, `Evaluacion`, `DetalleObj`, `DetalleEscaleta`, `OperacionObjetivos`, `OperacionCompetencias`, `OperacionObjetivosPDP`, `OperacionObjetivosPTP`, `GuardaObjetivos`.

## Roles y permisos
- Requiere sesión activa.
- Colaborador: captura objetivos, autoevaluación.
- Evaluador (jefe): revisa/aprueba objetivos, califica evaluaciones.
- Segundo nivel (si aplica): revisión adicional según estado.

## Flujo de navegación
1) **Ingreso a Mis Evaluaciones**
   - Ruta: `MisEvaluaciones/MiEvaluacion` (`MisEvaluacionesController.MiEvaluacion`).
   - Muestra estado actual de la evaluación y accesos a secciones de objetivos/competencias.

2) **Captura/Revisión de objetivos**
   - Vistas de operación: `OperacionObjetivos`, `OperacionObjetivosPDP`, `OperacionObjetivosPTP`.
   - Detalles y resumen: `DetalleObj`, `DetalleEscaleta`.
   - Acciones típicas: agregar/editar objetivos, guardar (`GuardaObjetivos`), enviar a aprobación.

3) **Aprobación/Rechazo de objetivos (jefe)**
   - Desde `Evaluacion` o vistas de detalle, el evaluador revisa y cambia `Status` (1 cargado ? 2 rechazado ? 3 aprobado).
   - Tras aprobación, se habilita la fase de evaluación según fechas de periodo.

4) **Ejecución de evaluación**
   - Autoevaluación (colaborador) y evaluación del jefe: formulario `Evaluacion`.
   - Segundo nivel (si aplica): estados 7/13.
   - Cierre de medio año y fin de año según fechas y estados 8/14; calibración estado 15.

5) **Guardado y envío**
   - Guardar parciales (objetivos/competencias) y envío final según botones de la vista (`Evaluacion`, `GuardaObjetivos`).

## Estados de evaluación usados
- 0 Pendiente
- 1 Objetivos cargados
- 2 Objetivos rechazados
- 3 Objetivos aprobados
- 4 Inicio de evaluación
- 5 Autoevaluación
- 6 Evaluación jefe
- 7 Segundo nivel (mid-year)
- 8 Cierre medio año
- 10 Inicio evaluación fin de año
- 11 Autoevaluación fin de año
- 12 Evaluación jefe fin de año
- 13 Segundo nivel fin de año
- 14 Cierre fin de año
- 15 Calibración

## Datos y efectos
- Las operaciones escriben sobre la evaluación activa (`EEval`) del usuario y sus objetivos/competencias.
- Fechas de periodo controlan la disponibilidad de captura/revisión (ver `PeriodosController`).
- Cambios de estado se reflejan en dashboard y gestión.

## Prerrequisitos operativos
- Sesión válida y evaluación activa ligada a un periodo vigente.
- Objetivos iniciales cargados o habilitados según ventana de objetivos.
- Evaluador asignado y correo válido (para notificaciones si aplica).

## Pasos de validación (ejercicio guiado)
1. Iniciar sesión como colaborador de prueba; abrir `MisEvaluaciones/MiEvaluacion` y revisar estado.
2. Capturar objetivos en `OperacionObjetivos`; guardar y verificar que el estado pase a “cargados”.
3. Iniciar sesión como evaluador; abrir `Evaluacion` del colaborador y aprobar/rechazar objetivos (ver cambios de estado 1?2/3).
4. Con objetivos aprobados y fecha de evaluación abierta, capturar autoevaluación; guardar y verificar estado 5.
5. Evaluador captura su evaluación; verificar estado 6.
6. Si aplica segundo nivel, avanzar a estados 7/13; validar cierres 8/14 y calibración 15.
7. Confirmar reflejo de estados en `Dash/DashBoard`.

## Comprobaciones ante incidentes
- Si no se cargan objetivos o evaluación: validar sesión, periodo activo y que exista `EEval` para el usuario.
- Si no cambian estados al guardar: revisar reglas de fecha del periodo y validaciones de negocio.
- Si vistas no muestran datos: revisar que el usuario y evaluador estén activos y asignados.
- Si hay errores de guardado: revisar bitácora y logs de negocio.

## Buenas prácticas operativas
- Trabajar en ventanas de tiempo válidas (objetivos/evaluación según periodo).
- Guardar frecuentemente y revisar mensajes de respuesta en pantalla.
- Verificar que objetivos estén completos y medibles (KPI) antes de enviar a aprobación.
- Para pruebas, usar usuarios y periodos de QA; no mover estados en producción sin necesidad.

## Acciones a evitar (riesgo en MisEvaluaciones/BD)
- No editar objetivos fuera de ventana de objetivos o con evaluación en progreso (riesgo de inconsistencias).
- No forzar estados directamente en BD; usar las vistas/acciones del sistema.
- No usar cuentas compartidas para capturas/aprobaciones.
- No avanzar fases si el periodo no está configurado correctamente; valida fechas antes.
- No borrar evaluaciones activas vinculadas a usuarios en curso; afecta dashboard y gestión.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.MisEvaluaciones.md -o Runbook.MisEvaluaciones.pdf`).

