# Runbook operativo – Usuarios (Detalle punto 7)

Sistema ASP.NET MVC (.NET Framework 4.8). La administración de usuarios se entrega mediante `UsuarioController` y vistas en `Views/Usuario/*`. Acciones sensibles validan sesión con `Utilidades.ValidaSesion` y registran bitácora.

## Alcance
- Capacitación para alta, edición, carga masiva y cambio de contraseña de usuarios.
- Vistas: `Usuarios`, `NuevoUsuario`, `CargaMasivaUsuarios`, `CambiarCon`.

## Roles y permisos
- Requiere sesión activa.
- Perfiles administrativos (Sistemas/Regional/Admin) para altas, edición y masivos.

## Flujo de navegación
1) **Listado**
   - Ruta: `Usuario/Usuarios` (`UsuarioController.Usuarios`). Muestra usuarios y permite edición.

2) **Alta/Edición individual**
   - `Usuario/NuevoUsuario`: crear o editar usuario, asignar evaluador (`EvaluadorIdSap`), país, división, correo.

3) **Carga masiva**
   - `Usuario/CargaMasivaUsuarios`: subir CSV/Excel con plantilla definida; procesa altas/actualizaciones.

4) **Cambio de contraseña**
   - `Usuario/CambiarCon`: cambio de contraseña para el usuario seleccionado.

## Datos y efectos
- Usuarios se asocian a país, división y evaluador. Correos se usan para notificaciones.
- Carga masiva crea o actualiza registros según plantilla; puede disparar evaluaciones si se ligan a periodos.

## Prerrequisitos operativos
- Plantilla correcta para carga masiva (formato CSV/Excel esperado).
- Evaluadores existentes y válidos para asignación.
- Correos válidos para notificaciones.

## Pasos de validación (ejercicio guiado)
1. Abrir `Usuarios` y verificar listado por país.
2. Crear usuario de prueba en `NuevoUsuario`; guardar y confirmar que aparece en el listado.
3. Ejecutar carga masiva de prueba con pocos registros; verificar resultados y mensajes.
4. Cambiar contraseña en `CambiarCon` para el usuario de prueba; validar acceso.

## Comprobaciones ante incidentes
- Si carga masiva falla: revisar formato de archivo y validaciones mostradas.
- Si no se asigna evaluador: confirmar que existe y que el campo `EvaluadorIdSap` es correcto.
- Si correos no se envían: revisar configuración SMTP y que el usuario tenga email.

## Buenas prácticas operativas
- Probar cargas masivas en QA antes de producción.
- Mantener plantilla de carga actualizada y versionada.
- Registrar cambios masivos (fecha/hora/responsable y archivo usado).

## Acciones a evitar (riesgo en Usuarios/BD)
- No ejecutar cargas masivas sin respaldo y sin validar plantilla.
- No asignar evaluadores inexistentes; produce datos huérfanos.
- No modificar usuarios directamente en BD; usar las vistas/controladores.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Usuarios.md -o Runbook.Usuarios.pdf`).

