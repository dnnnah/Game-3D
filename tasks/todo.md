# Implementación: Helix Jump vertical

- [x] Sustituir el runner X/Z por balón vertical, rebote y giro táctil de torre.
- [x] Seguir al balón en Y con X/Z de cámara fijos.
- [x] Construir torre segmentada, huecos, columna, colores y triggers desde `Main.unity` al abrir una escena limpia en Unity.
- [x] Configurar menú móvil Portrait y contador de pisos en el HUD.
- [x] Aumentar suavemente el límite de caída por piso, configurable en Inspector y con máximo.
- [x] Guardar y mostrar el récord con `PlayerPrefs`.
- [x] Actualizar README y PRODUCT; mantener `HelixPart.unitypackage` fuera de Git.
- [x] Abrir PR #3 para el núcleo y preparar PR #2 como cambio de progresión sobre esa base.
- [ ] Revisar visualmente la escena construida y jugar la caída, el rebote, el giro y el conteo en Play Mode.

La compilación de los ensamblados Runtime y Editor terminó con 0 errores y 0 advertencias. No fue posible abrir Unity en modo batch porque el proceso aislado no pudo inicializar el cliente de licencias. El generador Editor queda disponible al abrir el proyecto o desde **Tools > Helix Jump > Build Main Scene**.

## Actividad 3: generación continua y bucle de partida

Especificación y mapa aprobados. La implementación está en curso en la rama `actividad-3-niveles-procedurales`.

- [x] Modelar `HelixFloorModule` y actualizar el disparador para informar cada cruce una sola vez.
  - Aceptación: el módulo agrupa sectores/hueco/disparador y se puede mover y reconfigurar sin duplicar el conteo.
  - Verificar: compilar Runtime y revisar la configuración del collider y altura del trigger.
  - Archivos: `HelixFloorModule.cs`, su `.meta`, `FloorPassTrigger.cs`.
- [x] Crear `ProceduralLevelGenerator` con reserva fija y reciclaje vertical.
  - Aceptación: genera la reserva inicial configurable y reutiliza un piso atravesado debajo del más bajo; la cantidad activa no crece.
  - Verificar: compilar Runtime y revisar límites de reserva, separación y posición.
  - Archivos: `ProceduralLevelGenerator.cs`, su `.meta`.
- [x] Conectar la curva de dificultad a los eventos de piso.
  - Aceptación: velocidad y cantidad de peligros aumentan por umbrales configurables y respetan sus máximos; el récord conserva su clave de `PlayerPrefs`.
  - Verificar: compilar Runtime y revisar los valores serializados y clamps.
  - Archivos: `GameProgression.cs`, `ProceduralLevelGenerator.cs`, `HelixFloorModule.cs`.
- [x] Añadir sectores rojos letales respetando hueco y sectores normales.
  - Aceptación: los peligros se generan según el máximo recibido y los módulos siempre conservan ruta transitable.
  - Verificar: compilar Runtime y revisar los límites de generación con distintas cantidades de sectores.
  - Archivos: `HazardSector.cs`, su `.meta`, `HelixFloorModule.cs`, `ProceduralLevelGenerator.cs`.
- [x] Integrar los estados de partida, derrota y reintento.
  - Aceptación: el golpe rojo detiene la simulación una vez; iniciar/reintentar conecta la reserva y restaura balón, torre, progreso y pisos sin borrar el récord.
  - Verificar: compilar Runtime y revisar rutas de transición y métodos de reinicio existentes.
  - Archivos: `GameSceneController.cs`, `ProceduralLevelGenerator.cs`.
- [x] Conectar y guardar la configuración en la escena principal.
  - Aceptación: el constructor/editor deja referencias y defaults válidos, y el Inspector permite cambiar parámetros sin editar código.
  - Verificar: compilar Runtime/Editor, revisar referencias de escena y `git diff --check`.
  - Archivos: `Assets/Editor/HelixSceneBuilder.cs`, `Assets/Scenes/Main.unity`.
- [x] Revisar cambios de Activity 3, crear commits lógicos y preparar el PR contra `main`.
  - Aceptación: solo incluye archivos de Activity 3, cada commit se mantiene por debajo de 400 líneas cuando sea práctico y `HelixPart.unitypackage` queda fuera de Git.
  - Verificar: revisar `git status`, diffs y commits antes de abrir el PR.
  - Archivos: cambios de la actividad y documentación asociada.
