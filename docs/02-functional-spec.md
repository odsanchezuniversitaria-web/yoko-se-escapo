# Especificación funcional aprobada para implementación

La autorización «sí hazlo» cubre las fases restantes del diseño aprobado. Adaptaciones concretas: campaña compacta de cinco zonas y prólogo, cámara lateral, planeo de ave y fuerza de biomasa como segunda mutación. La fuerza y los valores siguientes son diseño de jugabilidad, no texto literal del PDF.

Estados: Título, Prólogo, Jugando, Pausa, Configuración, Archivo, Derrota, Final. Cinco zonas: base, barrio, encuentro, epicentro y núcleo. Yoko es la única controlable. Las señales del mundo comunican objetivos; los menús pueden contener instrucciones.

Movimiento 5 unidades/s; olfato 2; salto 8; planeo limita caída a 1.6. Vida interna 3, sin barra. Mutaciones duran 35 segundos; restos renovables tras 12 segundos evitan bloqueos. Cazar agrega 25 de violencia, atacar hostiles 8, recibir daño agrega estrés separado. Violencia persistente de 0 a 100; frenesí si violencia >= 75 o estrés >= 85, dura 3 segundos con intervalo mínimo 14. El final trágico requiere violencia >= 75. Nunca se revela violencia fuera de pausa/configuración.

Cada zona guarda un checkpoint. Guardado JSON versionado con zona, violencia, notas y desbloqueos. Reintentar restaura el checkpoint con salud; no duplica notas. Nueva partida solicita confirmación antes de reemplazar progreso. Continuar valida datos y límites; archivo inválido se conserva y se inicia de forma segura.

Jefes con anticipación, ataque y recuperación. El último requiere golpear tres nodos miceliales con fuerza temporal; rutas de restos hacen posible finalizar sin cazar. Enemigos pueden evadirse. El rescate se activa solo tras completar el núcleo.

PC: A/D, flechas, Espacio, Shift, E, J y Escape. Android: botones simultáneos izquierda/derecha, salto, olfato, interacción y ataque. UI adaptable a safe area. Audio y controles táctiles configurables.
