# Pruebas

- UT01: asimilar restos no agrega violencia; cazar sí; clamp 0–100.
- UT02: mutación caduca y no altera la vulnerabilidad.
- UT03: frenesí solo con umbral e intervalo; termina y devuelve control.
- UT04: finales según violencia; rescate no disponible antes del núcleo.
- UT05: guardado roundtrip, límites y datos corruptos.
- IT01: título > prólogo > base; pausa detiene simulación; reintento restaura.
- IT02: daño, muerte, recuperación, checkpoint y cambio de zona.
- IT03: ruta pacífica completa y ruta violenta completa mediante comandos de prueba que invocan reglas de producción.
- RT01: sprites asignados, cámara, colliders, UI y controles táctiles.
- L1: compilación y consola sin errores. L2: ejecución Play Mode. L3: capturas de título, juego, pausa y finales.
- BUILD01: Windows ejecutable. BUILD02: APK Android. Dispositivo real solo si está conectado; no confundir APK generado con prueba en hardware.
