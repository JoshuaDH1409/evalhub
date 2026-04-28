# Runbook operativo – Correos (Detalle punto 8)

Sistema ASP.NET MVC (.NET Framework 4.8). El manejo de plantillas y envíos se entrega mediante `CorreosController`, utilidades `PlantillaMVC.EnvioCorreo`, `CapaLogica.Correo` y vistas en `Views/Correos/*`. Acciones sensibles validan sesión y registran bitácora.

## Alcance
- Capacitación para consultar y editar plantillas de correo y validar envíos usados en los flujos de evaluación.
- Vistas: `Correos/Correo`, `Correos/EdicionCorreo`.

## Roles y permisos
- Requiere sesión activa.
- Perfiles administrativos (Sistemas/Admin) para editar plantillas.

## Flujo de navegación
1) **Listado de plantillas**
   - Ruta: `Correos/Correo` (`CorreosController.Correo`). Muestra plantillas disponibles.

2) **Edición de plantilla**
   - `Correos/EdicionCorreo`: permite editar asunto y cuerpo; placeholders se procesan en `EnvioCorreo.ProcesarMsg`.

3) **Uso en flujos**
   - Ejemplo clave: plantilla 13 se usa en `GestionController.NuevaGestion` para notificar inicio de carga de objetivos.

## Datos y efectos
- Plantillas almacenan asunto y mensaje HTML con placeholders que se sustituyen con datos del usuario/evaluación.
- Cambios afectan inmediatamente los correos enviados por los flujos de negocio.

## Prerrequisitos operativos
- Configuración SMTP correcta en `Web.config` (`PlantillaMVC.Utilidades.EnvioCorreo`).
- Plantillas con placeholders válidos y probados en QA.

## Pasos de validación (ejercicio guiado)
1. Abrir `Correos/Correo` y ubicar la plantilla a revisar (ej. 13).
2. Editar en `EdicionCorreo`, guardar y revisar vista previa (si aplica).
3. Ejecutar un envío de prueba en ambiente QA desde el flujo que consume la plantilla (ej. alta individual en `Gestion`).

## Comprobaciones ante incidentes
- Si correos no se envían: revisar SMTP en `Web.config`, credenciales y conectividad.
- Si los placeholders no se sustituyen: revisar `ProcesarMsg` en `EnvioCorreo` y el formato de la plantilla.
- Si el cuerpo llega vacío o con HTML roto: validar la plantilla guardada y caracteres especiales.

## Buenas prácticas operativas
- Versionar plantillas y probar cambios en QA antes de producción.
- Usar placeholders documentados; evitar insertar credenciales o datos sensibles.
- Registrar cambios de plantilla (fecha/hora/responsable).

## Acciones a evitar (riesgo en Correos/BD)
- No editar plantillas en productivo sin prueba previa.
- No cambiar SMTP en `Web.config` en caliente; reinicia la app.
- No eliminar plantillas usadas por flujos activos (ej. 13) sin reemplazo.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Correos.md -o Runbook.Correos.pdf`).

