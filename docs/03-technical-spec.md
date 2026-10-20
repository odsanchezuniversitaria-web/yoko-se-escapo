# Arquitectura

Unity 6000.4.10f1, renderizador integrado 2D, sprites transparentes, cámara ortográfica. Proyecto independiente en Unity/. Runtime ensamblado en Yoko.Runtime. Sistemas: GameSession y SaveData (reglas/guardado), YokoGame (estado y campaña), YokoMotor (Rigidbody2D/input), WorldFactory (geometría y ambientación), WorldActor (interactuables/IA), GameUI (Canvas camera-space), Soundscape (audio), DebugManager (solo editor/desarrollo). Herramientas Editor para crear escena, importar sprites, pruebas y builds.

Assets de referencia separados de sprites finales. Colliders rectangulares para suelo; cápsula para Yoko. Cámara sigue con amortiguación, límites de zona y escalado horizontal mínimo. UI usa CanvasScaler 1600x900 y safe area. Entradas táctiles por IPointerDown/Up y teclado por Input clásico, configurado expresamente.

Windows x64; Android ARM64 IL2CPP, orientación horizontal. APK local con firma de desarrollo, sin publicación. Guardado en Application.persistentDataPath; ajustes en PlayerPrefs. Ningún servicio externo en el juego.
