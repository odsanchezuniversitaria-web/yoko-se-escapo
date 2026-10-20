# Yoko se escapó

Primera versión jugable 2D para Windows y Android, creada con Unity **6000.4.10f1** a partir de la Biblia Narrativa suministrada. Campaña compacta de cinco zonas: base, ciudad, encuentro con Yimmy, epicentro y núcleo.

Actualización **0.2**: joystick analógico, botones multitáctiles, esquiva, salto variable, efectos de combate, música ambiental, peligros de esporas y objetivos narrativos durante la partida. Detalles en `docs/08-mejoras-v02.md`.

## Jugar

Windows: abrir `Builds/Windows/Yoko.exe`. Mantener el ejecutable junto a Yoko_Data, UnityPlayer.dll y las demás carpetas del build. El ZIP de Windows contiene el conjunto completo.

Android: el archivo de entrega es `Builds/Android/Yoko.apk`, destinado a Android 8 o superior y procesadores ARM64. Orientación horizontal; controles táctiles aparecen automáticamente. La generación del APK no equivale a una prueba en teléfono físico.

## Controles

| Acción | Teclado | Táctil |
|---|---|---|
| Mover | A/D o flechas | < / > |
| Saltar | Espacio | Salto |
| Planear con mutación de ave | Mantener Espacio | Mantener Salto |
| Olfato | Mantener Shift | Mantener Olfato |
| Interactuar / asimilar | E | Explorar |
| Atacar / romper micelio con fuerza | J | Acción |
| Esquivar | K o Ctrl izquierdo | Esquiva |
| Pausa | Escape | II |

El suero de la base habilita Quimera. Busca el ave muerta y usa los saltos con planeo para salir. Las mutaciones duran 35 segundos. Los restos reaparecen; las presas vivas aumentan la violencia. El olfato revela el rastro y reduce la velocidad. Los enemigos anuncian el ataque en rojo: puedes evadirlos. En el núcleo, rompe los tres nodos con Quimera de fuerza y acércate a Yimmy.

El valor de violencia se consulta solo en Pausa > Configuración. El daño se comunica con la postura de Yoko; no hay barra de vida. El juego guarda al comenzar cada zona y al encontrar a Yimmy. Continuar carga ese punto; Reintentar restaura el último refugio. Se puede silenciar el sonido y activar controles táctiles en PC desde Configuración.

El joystick permite caminar despacio o correr. Mantén Salto para ganar altura; suéltalo antes para un salto corto. La esquiva evita el daño durante su impulso y necesita 1,1 segundos para recuperarse. Los círculos de esporas avisan antes de activarse: espera, salta por encima o usa la ruta elevada.

## Editar en Unity

Abrir la carpeta `Unity` con Unity Hub usando **6000.4.10f1**. Escena principal: `Assets/Scenes/Yoko.unity`. Pulsar Play. Los escenarios y la UI se construyen al entrar en Play Mode; en modo de edición se ve el objeto raíz.

- `Assets/Scripts`: reglas, campaña, movimiento, UI, actores y audio.
- `Assets/Resources/Art`: sprites y panorama originales generados.
- `Assets/Editor`: configuración del proyecto, builds y herramientas de prueba.
- `Assets/Tests`: pruebas EditMode y PlayMode.
- Menú **Yoko > Build Windows / Build Android** para recompilar.
- Menú **Yoko > View 1600x900** para preparar la vista de revisión.

En esta instalación el gestor de paquetes de Unity presenta un error de ruta `Received undefined`. Se reconstruyó la caché desde paquetes locales y se continuó la importación; no es un error del código del juego. Si reaparece al abrir esta carpeta, **Continue** permite usar la caché conservada. No borres Library sin tener los paquetes disponibles. La distribución fuente incluye las dependencias externas como paquetes integrados para facilitar la importación.

## Fidelidad y alcance

Yoko es la única jugable, tal como indica la página 8. Yimmy permanece no jugable, según la página 11. Se conservan el collar amarillo, el pelaje negro/fuego, las gafas y ropa SKATE de Yimmy, Slays, olfato, mutaciones temporales, búsqueda/rescate y desenlaces por violencia.

Esta versión no es una producción final: reutiliza un fondo con variaciones, usa seis poses de Yoko y animación corporal sencilla, audio sintetizado y viñetas para prólogo/finales. No incluye cinematográficas completas, una banda sonora profesional ni animaciones detalladas de transformación. Las imágenes son adaptaciones generadas a partir de las láminas, no copias exactas de ellas. Las decisiones de diseño no especificadas en el PDF están documentadas en `docs`.

## Créditos

Biblia narrativa: Laura Daniela Rodríguez Bernal, Angie Jasbleidy Fajardo Valbuena y Leidy Yised Estupiñán Salazar. Fundación Universitaria Compensar, 2026. Arte de esta adaptación generado con image_gen; referencias y prompts en `docs/06-arte-y-adaptacion.md`.
