# Runbook operativo – Recorrido sugerido de capacitación (Detalle punto 11)

Secuencia propuesta para capacitar a personal operativo sobre el flujo completo de la aplicación. Basado en los runbooks parciales por módulo.

## Secuencia
1. **Autenticación**: `Account/Login`, cambio de contraseña (`Manage/ChangePassword`).
2. **Dashboard**: `Dash/DashBoard` ? seleccionar país/periodo/área ? `Consultar` (gráficas y totales).
3. **Gestión** (admin/regional/sistemas): `Gestion/GestionEvaluacion` ? alta individual/masiva, reinicio, eliminación (solo pruebas en QA).
4. **Períodos**: `Periodos/Periodos` ? `NuevoPeriodo`/`Detalle` para entender ventanas y su impacto.
5. **Objetivos**: `Objetivos/RevisarObjetivos`, `Resumen`, `GetKPI`, escaletas; objetivos de país (`ObjetivosPais/Index`, `Nuevo`).
6. **Mis Evaluaciones**: `MisEvaluaciones/MiEvaluacion`/`Evaluacion` ? captura objetivos, autoevaluación, evaluación jefe, segundo nivel, cierres.
7. **Usuarios**: `Usuario/Usuarios`, `NuevoUsuario`, `CargaMasivaUsuarios`, `CambiarCon`.
8. **Correos**: `Correos/Correo`, `EdicionCorreo`; revisar plantilla 13 usada en Gestión.
9. **Historial**: `MiHistorial/Historial` (usuario) y `HistorialUsrs/ConsultarUsuarios` (admin) para auditoría.
10. **Seguridad**: filtros `[Autentificado]`, validación de sesión, bitácora, buenas prácticas.

## Recomendaciones para la sesión
- Usar ambiente de QA y usuarios de prueba.
- Mostrar impacto cruzado: después de cada acción, validar en Dashboard y Mis Evaluaciones.
- Registrar dudas/incidentes y resolver en el momento con los logs/bitácora.

## Exportar/Convertir a PDF
Redacta/ajusta este `.md` y conviértelo fuera de la app (ejemplo: `pandoc PlantillaMVC/Runbook.Recorrido.md -o Runbook.Recorrido.pdf`).

