# Runbook operativo – Períodos (Detalle punto 6)

Sistema ASP.NET MVC (.NET Framework 4.8). La administración de periodos se entrega mediante `PeriodosController` y vistas en `Views/Periodos/*`. Acciones sensibles validan sesión con `Utilidades.ValidaSesion` y registran bitácora.

## Alcance
- Capacitación para alta, edición y consulta de periodos y sus ventanas (objetivos, evaluación, calibración).
- Vistas: `Periodos/Periodos`, `Periodos/NuevoPeriodo`, `Periodos/Detalle`.

## Roles y permisos
- Requiere sesión activa.
- Perfiles administrativos (Sistemas/Regional/Admin) para crear/editar periodos.

## Flujo de navegación
1) **Listado de periodos**
   - Ruta: `Periodos/Periodos` (`PeriodosController.Periodos`). Muestra periodos por país.

2) **Crear nuevo periodo**
   - `Periodos/NuevoPeriodo` (`PeriodosController.NuevoPeriodo`): captura de llaves, país, fechas de objetivos, evaluación (mid y fin de año) y calibración.

3) **Detalle/Edición de periodo**
   - `Periodos/Detalle`: consulta y posible edición de un periodo existente.

## Fechas y ventanas
- Objetivos: `StartDateObj`, `FinishDateObj` controlan captura/aprobación de objetivos.
- Evaluación mid-year: `StartDateEva`, `FinishDateEva`.
- Evaluación fin-year: `StartDateEva2`, `FinishDateEva2`.
- Calibración: `StartDateCali`, `FinishDateCali` y sus equivalentes de fin de año.

## Datos y efectos
- Periodo define las ventanas que controlan disponibilidad de acciones en Dashboard, Gestión y Mis Evaluaciones.
- La llave de periodo (`Llave`) permite homologar periodos entre países para reporteo.

## Prerrequisitos operativos
- Definir país y llaves únicas por ciclo.
- Validar que no existan traslapes de fechas con periodos activos del mismo país.

## Pasos de validación (ejercicio guiado)
1. Abrir `Periodos/Periodos` y verificar la lista por país.
2. Crear un periodo de prueba en `NuevoPeriodo` con fechas coherentes; guardar.
3. Abrir `Detalle` del periodo creado y confirmar fechas y país.
4. Validar en Dashboard que el periodo aparece en la lista tras guardar.

## Comprobaciones ante incidentes
- Si no se guardan periodos: revisar validaciones de fechas y llaves duplicadas.
- Si no aparecen en Dashboard: verificar `RecuperaTodosPeriodosPais` y estado activo.
- Si ventanas no habilitan acciones: comprobar las fechas configuradas y el país correcto.

## Buenas prácticas operativas
- Planificar fechas sin traslapes; documentar ventanas.
- Probar creación/edición en QA antes de producción.
- Registrar cambios de periodos (fecha/hora/responsable).

## Acciones a evitar (riesgo en Periodos/BD)
- No editar periodos activos en horario operativo sin ventana autorizada.
- No duplicar llaves de periodo entre países sin necesidad; afecta reporteo.
- No dejar ventanas de evaluación/calibración abiertas indefinidamente.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Periodos.md -o Runbook.Periodos.pdf`).

