# Runbook operativo – Historial y reportes (Detalle punto 9)

Sistema ASP.NET MVC (.NET Framework 4.8). Los historiales se entregan mediante `MiHistorialController`, `HistorialUsrsController` y vistas en `Views/MiHistorial/*`, `Views/HistorialUsrs/*`. Acciones sensibles validan sesión y registran bitácora.

## Alcance
- Capacitación para consultar historial individual y administrativo.
- Vistas: `MiHistorial/Historial`, `HistorialUsrs/ConsultarUsuarios`, `HistorialUsrs/HistorialUsrs`.

## Roles y permisos
- Requiere sesión activa.
- Usuario: accede a su historial en `MiHistorial`.
- Admin/Regional/Sistemas: consultan historiales de usuarios en `HistorialUsrs`.

## Flujo de navegación
1) **Historial individual**
   - Ruta: `MiHistorial/Historial` (`MiHistorialController.Historial`). Muestra evaluaciones y estados del usuario.

2) **Historial administrativo**
   - `HistorialUsrs/ConsultarUsuarios`: búsqueda/listado de usuarios.
   - `HistorialUsrs/HistorialUsrs`: detalle del historial del usuario seleccionado.

## Datos y efectos
- Se consultan evaluaciones, estados y posiblemente resultados/perfiles asociados.
- Sin acciones de escritura; solo lectura/auditoría.

## Prerrequisitos operativos
- Sesión válida y permisos adecuados.
- Evaluaciones existentes para el usuario consultado.

## Pasos de validación (ejercicio guiado)
1. Iniciar sesión como usuario y abrir `MiHistorial/Historial`; verificar que se muestran evaluaciones previas.
2. Iniciar sesión como admin y usar `HistorialUsrs/ConsultarUsuarios` para buscar un usuario; abrir `HistorialUsrs` y validar datos.

## Comprobaciones ante incidentes
- Si no aparecen datos: verificar que existan evaluaciones para el usuario y que esté activo.
- Si falla la carga: revisar sesión y conectividad a BD.

## Buenas prácticas operativas
- Usar solo para consulta/auditoría; no alterar datos desde BD.
- Respetar mínimos privilegios; no compartir credenciales de admin.

## Acciones a evitar (riesgo en Historial/BD)
- No modificar historiales directamente en BD.
- No exponer historiales a perfiles sin permisos.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Historial.md -o Runbook.Historial.pdf`).

