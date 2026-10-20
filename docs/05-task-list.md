# Seguimiento

- [x] Lectura de 19 páginas y referencias visuales.
- [x] Diseño aprobado y especificaciones funcional/técnica/pruebas.
- [x] Crear proyecto y conectar Unity MCP.
- [x] Crear sprites y ambientes basados en referencias; adaptaciones generadas, no calcos exactos.
- [x] Estado, movimiento, olfato y Quimera. Hito: compilar, ejecutar y captura.
- [x] Campaña compacta de cinco zonas, enemigos, jefes, rescate y finales.
- [x] Interfaz, controles PC/Android, guardado. Hito: ejecución y captura.
- [x] Ambiente sonoro sintetizado y seis poses animadas de Yoko. Hito: ejecución y captura.
- [x] 8 pruebas EditMode, 4 integración PlayMode y 1 recorrido físico del tutorial aprobadas.
- [x] Revisar presentación final y correcciones de créditos/desenlace.
- [x] Compilar Windows y Android; preparar archivos e instrucciones.

No se marca una prueba como aprobada hasta disponer de evidencia.

## Incidencias resueltas

- FIX01: arranque aislado sin acceso al servicio de licencia; reinicio con permisos normales.
- FIX02: caché incompleta y Package Manager con error de ruta; dependencias reconstruidas desde paquetes locales. El aviso de Package Manager persiste, pero scripts y build Windows compilan.
- FIX03: Game View con proporción incorrecta; perfil 1600x900 para revisión.
- FIX04: hojas de personajes con celdas irregulares; rectángulos de sprites específicos.
- FIX05: créditos podían volver al estado de juego desde título; retorno explícito al título.
- FIX06: fondo fuera de cámara al cambiar directamente al desenlace; actualización de posición antes de pausar.
- FIX07: interfaz magenta en ejecutable por exclusión de shaders; inclusión explícita automática en cada build.
