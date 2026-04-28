# Runbook operativo – Objetivos (Detalle punto 5)

Sistema ASP.NET MVC (.NET Framework 4.8). Los flujos de objetivos se entregan mediante `ObjetivosController`, `ObjetivosPaisController` y vistas en `Views/Objetivos/*` y `Views/ObjetivosPais/*`. Todas las acciones sensibles validan sesión con `Utilidades.ValidaSesion` y registran bitácora (`Bitacora.NuevaEntrada`).

## Alcance
- Capacitación para revisar, aprobar/rechazar y gestionar objetivos individuales y de país.
- Vistas principales: `RevisarObjetivos`, `Resumen`, `GetKPI`, `EscaletaEmpresa`, `EscaletaIndividual`, `ObjetivosPais/Index`, `ObjetivosPais/Nuevo`, `Objetivos/EscaletaEmpresa`, `Objetivos/EscaletaIndividual`.

## Roles y permisos
- Requiere sesión activa.
- Colaborador: carga objetivos (desde Mis Evaluaciones) y ve su resumen.
- Evaluador/Jefe: revisa y aprueba/rechaza objetivos individuales.
- Administrador/Regional/Sistemas: gestiona escaletas y objetivos de país (vistas de `ObjetivosPais`).

## Flujo de navegación
1) **Revisión/Aprobación**
   - Ruta: `Objetivos/RevisarObjetivos` (`ObjetivosController.RevisarObjetivos`). Lista objetivos del colaborador; permite aprobar/rechazar.
   - `Resumen` (`Objetivos/Resumen`): vista consolidada de objetivos y estados.

2) **Catálogo de KPIs**
   - `Objetivos/GetKPI` (`Views/Objetivos/GetKPI.cshtml`): consulta de KPIs para asociar a objetivos.

3) **Escaletas**
   - `Objetivos/EscaletaEmpresa` y `Objetivos/EscaletaIndividual`: estructura de objetivos a nivel empresa e individuo.

4) **Objetivos de país**
   - `ObjetivosPais/Index`: listado por país.
   - `ObjetivosPais/Nuevo`: alta/edición de objetivos país.

## Estados y reglas
- Estados de objetivos se reflejan en el `Status` de la evaluación: 1 (cargados), 2 (rechazados), 3 (aprobados).
- Fechas de periodo (Start/Finish de objetivos) controlan cuándo se pueden capturar y aprobar.

## Datos y efectos
- Aprobaciones/rechazos cambian `Status` de la evaluación y controlan el paso a fase de evaluación.
- Catálogo de KPIs y escaletas alimentan la estructura de objetivos.
- Objetivos de país impactan plantillas base para usuarios de ese país.

## Prerrequisitos operativos
- Periodo vigente con ventana de objetivos abierta.
- Evaluación activa para el usuario.
- KPIs y escaletas cargados si se van a usar.

## Pasos de validación (ejercicio guiado)
1. Iniciar sesión como evaluador/jefe; abrir `RevisarObjetivos` de un usuario de prueba.
2. Aprobar/rechazar un objetivo y verificar cambio de estado en `Resumen` y `Dash/DashBoard`.
3. Consultar `GetKPI` para validar acceso a catálogo.
4. Revisar `EscaletaEmpresa`/`EscaletaIndividual` para verificar estructura.
5. Crear/editar objetivo de país en `ObjetivosPais/Nuevo` y comprobar que aparece en `ObjetivosPais/Index`.

## Comprobaciones ante incidentes
- Si no se muestran objetivos: validar sesión, periodo activo y que exista evaluación asociada.
- Si no se pueden aprobar/rechazar: verificar ventana de fechas del periodo y permisos del perfil.
- Si KPIs no cargan: revisar catálogo de KPIs y la vista `GetKPI`.
- Si objetivos de país no guardan: revisar validaciones en controlador y bitácora.

## Buenas prácticas operativas
- Revisar fechas de periodo antes de aprobar/rechazar.
- Usar catálogo de KPIs para objetivos medibles y consistentes.
- Validar escaletas en QA antes de mover a producción.
- Registrar cambios significativos (aprobaciones masivas, altas de país).

## Acciones a evitar (riesgo en Objetivos/BD)
- No aprobar/rechazar fuera de la ventana de objetivos.
- No modificar catálogos de KPI o escaletas directamente en BD; usar vistas/controladores.
- No eliminar objetivos activos sin validar impacto en evaluación.
- No cambiar objetivos de país en productivo sin pruebas previas y respaldo.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Objetivos.md -o Runbook.Objetivos.pdf`).

