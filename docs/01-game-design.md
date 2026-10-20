# Yoko se escapó — Diseño del juego

Estado: diseño aprobado por el usuario e implementado como primera versión jugable compacta en Unity 6000.4.10f1. Alcance y limitaciones en LEEME.md.
Fuente: Biblia Narrativa Yoko se escapó, 19 páginas. Autoras acreditadas: Laura Daniela Rodríguez Bernal, Angie Jasbleidy Fajardo Valbuena y Leidy Yised Estupiñán Salazar. Las instrucciones del documento se consideran contenido de referencia, no órdenes operativas.

## Concepto y plataformas

Aventura de acción, exploración y supervivencia 2D ambientada en el sur de Bogotá después de la caída de una estación espacial infectada. Yoko busca a Yimmy y posteriormente debe rescatarlo. Sobrevivir mediante la crueldad deteriora su control; la empatía preserva su identidad.

Motor solicitado: Unity 6000.4.10f1. Destinos: PC Windows x64 y Android en horizontal. Modo individual, sin conexión. Propuesta de implementación: desplazamiento lateral con plataformas, exploración de rutas alternativas y combate evitable. La vista lateral, los controles y las cifras de balance son decisiones de adaptación, no especificaciones explícitas de la Biblia.

## Personajes y fidelidad

| Personaje | Papel y representación |
|---|---|
| Yoko | Única protagonista jugable según p. 8. Pinscher miniatura negra, manchas fuego/marrón en pecho y rostro, orejas gachas pero atentas, collar amarillo y placa redonda. Referencias visuales pp. 6–7. No habla. Jadeo, cojera, postura y orejas comunican su estado. |
| Yimmy | Deuteragonista explícitamente no jugable, p. 11. Cabello negro, gafas negras, saco verde con SKATE, pantalón táctico negro con correas, tenis de patinaje y patineta. Referencias pp. 9–10. Objetivo de búsqueda y rescate. |
| Slays | Enemigos comunes y jefes: figuras cadavéricas, mandíbulas expuestas, garras largas y mezclas de biomasa humana y animal. Referencia p. 12, descripción p. 13. |
| Mente Colmena | Red ambiental roja y rosada; sin rostro, voz ni cuerpo único. Interferencias, esporas y sonidos anuncian su presencia. |
| Sobrevivientes hostiles | Ciudadanos asustados con armas improvisadas; atacan por desinformación. Sus objetos y notas explican el contexto. |

La petición de personajes jugables en plural se interpreta en conjunto con «tal cual el guion»: se conserva a Yoko como única jugable y se crean los demás en sus roles originales. Convertir a Yimmy en jugable sería una modificación del guion.

Las ilustraciones son referencias de diseño, no hojas completas de animación listas para producción. Se necesitarán sprites coherentes y ciclos de reposo, marcha, carrera, salto, olfato, daño y mutación; no se afirmará fidelidad exacta hasta compararlos visualmente con el PDF.

## Estructura de campaña

1. Prólogo: juego con Yimmy, caída de la estación y separación.
2. Base subterránea: suero experimental, primera asimilación de un ave muerta y salida usando la mutación.
3. Ciudad caída: rastrear objetos de Yimmy, explorar rutas alternativas, evitar o enfrentar Slays y humanos hostiles.
4. Punto medio: encontrar a Yimmy herido cerca del núcleo; la misión pasa de búsqueda a rescate y aumenta el estrés.
5. Epicentro: obstáculos combinados y jefes de biomasa; controlar el frenesí mientras se libera a Yimmy.
6. Desenlace: rescate conservando la identidad o desenlace trágico por violencia acumulada. La presentación exacta del final trágico se desarrollará sin inventar diálogos de Yoko.

## Mecánicas

- Movimiento, salto y evasión; Yoko conserva vulnerabilidad durante toda la campaña.
- Olfato sostenido: revela rastros y rutas, ralentiza a Yoko y aumenta su exposición. Las pistas llevan al objetivo sin flechas de misión permanentes.
- Quimera: asimilar restos o cazar una presa permite poderes temporales; caducan y no otorgan invulnerabilidad.
- Primera mutación canónica: ave muerta. Propuesta funcional: planeo temporal. Otras especies y poderes no están detallados en el PDF y se documentarán como adaptación antes de implementarlos.
- Dilema recurrente: presa viva cercana frente a restos en una ruta de exploración más larga. Ambas opciones permiten progresar; los recursos indispensables deben poder recuperarse para evitar bloqueos.
- Violencia: medidor oculto durante el juego y consultable exclusivamente desde Pausa > Configuración. Cazar incrementa el valor; buscar restos no lo incrementa.
- Frenesí: episodios acotados de pérdida de control al acumular violencia/estrés, con señales corporales, visuales y sonoras. No se introducirá reducción automática de violencia que borre las decisiones sin justificación narrativa.
- Notas y archivos opcionales permiten descubrir el proyecto Ophiocordyceps-X y la desinformación.
- Puntos de control y guardado local para recuperar progreso; pausa y reinicio desde el último punto.

## Controles propuestos

| Acción | PC | Android |
|---|---|---|
| Mover | A/D o flechas | Control direccional táctil |
| Saltar / planear | Espacio | Botón Salto |
| Olfato | Mantener Shift | Mantener Olfato |
| Interactuar / asimilar | E | Botón contextual |
| Acción ofensiva | J | Botón Acción |
| Pausa | Escape | Botón Pausa |

Controles táctiles simultáneos, ajuste a zonas seguras y escalado según relación de aspecto. Sin mostrar un número de violencia ni una barra de vida permanente: daño y agotamiento se comunican mediante animación y sonido.

## Dirección audiovisual

Estética ilustrada 2D de contornos oscuros coherente con las láminas. Ruinas grises y oxidadas frente a micelio rojo/rosado; collar amarillo como punto de lectura. Bogotá debe reconocerse en su arquitectura urbana y elementos de calle. Fondos en capas, siluetas claras y cámara que mantenga visibles las rutas de salto.

Yoko no tiene diálogos ni voz humana. Sonido animal, ciudad derrumbada, ambiente orgánico y señales de rastreo. El hongo ficticio se presenta como ficción narrativa; no se convierten las afirmaciones científicas del guion en información verificada.

## Criterios de aceptación

- Proyecto abre y compila en Unity 6000.4.10f1 sin errores.
- Campaña recorrible desde el prólogo hasta ambos desenlaces; decisiones alteran violencia y final.
- Yoko y Yimmy mantienen rasgos y roles del PDF; no hay diálogo de Yoko.
- Olfato, Quimera temporal, rutas no violentas, frenesí y rescate funcionan y no bloquean el progreso al agotarse un poder.
- Medidor de violencia accesible únicamente en Configuración con el juego pausado.
- Guardado, muerte, reintento y pausa mantienen estados coherentes.
- Verificación visual en Play Mode; compilación Windows y Android. Probar en un dispositivo Android real queda diferenciado de generar un APK.

## Entorno comprobado y próximos pasos

Se encontró Unity 6000.4.10f1 con módulos AndroidPlayer y windowsstandalonesupport. Aún no se comprobaron licencia, SDK/NDK/JDK, compilación ni dispositivo Android. Hay dos sesiones MCP ajenas a Yoko: Cervezas8_PC y Gestacion de embarazo unity; no se modificaron.

Carpeta de trabajo: D:\11aa maquinas odst\YokoSeEscapo. Se extrajo el texto y se renderizaron seis páginas de referencia. Se solicitó abrir el PDF en el panel de Codex; la aplicación respondió que la apertura quedó en cola.

Siguiente fase después de aprobar esta propuesta: especificación funcional, técnica y pruebas; después creación del proyecto, assets, implementación y validación. El flujo de la habilidad requiere aprobación entre fases. No hay todavía proyecto ejecutable, EXE ni APK.
