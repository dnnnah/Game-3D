# Tareas: Helix Jump vertical

- [ ] **Escena vertical editable**
  - Aceptación: `Main.unity` contiene una columna central, pisos segmentados con huecos, sectores amarillos y balón rojo visibles en Scene; usa instancias de `HelixPartPrefab`.
  - Verificación: abrir escena en Unity y revisar jerarquía, orientación radial, escala y colisiones.
  - Archivos: `Assets/Editor/HelixSceneBuilder.cs`, `Assets/Editor/HelixSceneBuilder.cs.meta`, `Assets/Scenes/Main.unity`, `Assets/Materials/HelixBall.physicMaterial`, `Assets/Materials/HelixBall.physicMaterial.meta`.

- [ ] **Física vertical y giro táctil**
  - Aceptación: el balón cae/rebota sobre Y; el arrastre horizontal gira la torre sobre Y; no hay avance del balón por X/Z.
  - Verificación: compilar y probar una caída, rebote y giro desde mouse/touch en Play Mode.
  - Archivos: `Assets/Scripts/PlayerMovement.cs`, `Assets/Scripts/MobileInputController.cs`, `Assets/Scripts/TowerRotationController.cs`, `Assets/Scripts/TowerRotationController.cs.meta`.

- [ ] **Cámara vertical y conexión de escena**
  - Aceptación: la cámara acompaña Y con X/Z constantes y la sesión conecta el balón, la torre y la cámara sin referencias ausentes.
  - Verificación: comparar posición antes/después de un descenso y revisar la escena serializada.
  - Archivos: `Assets/Scripts/CameraFollowXZ.cs`, `Assets/Scripts/CameraFollowXZ.cs.meta`, `Assets/Scripts/GameSceneController.cs`, `Assets/Scenes/Main.unity`.

- [ ] **UI y orientación Portrait**
  - Aceptación: build target inicia en Portrait; menú, botones y área segura se adaptan a teléfono vertical.
  - Verificación: iniciar/volver al menú y revisar una proporción móvil alternativa.
  - Archivos: `ProjectSettings/ProjectSettings.asset`, `Assets/Scripts/GameSceneController.cs`, `Assets/Scripts/SafeAreaFitter.cs`.

- [ ] **Progresión por pisos y persistencia**
  - Aceptación: cada piso cruzado suma uno, la velocidad vertical aumenta con tope y el récord sobrevive al reinicio.
  - Verificación: cruzar varios pisos, volver al menú y reiniciar la aplicación en Play Mode.
  - Archivos: `Assets/Scripts/FloorPassTrigger.cs`, `Assets/Scripts/FloorPassTrigger.cs.meta`, `Assets/Scripts/GameProgression.cs`, `Assets/Scripts/PlayerMovement.cs`, `Assets/Scripts/GameSceneController.cs`.

- [ ] **Documentación y entrega**
  - Aceptación: README y PRODUCT describen el Helix Jump vertical y el alcance correcto de actividad 1/2; el paquete fuente sigue fuera de Git.
  - Verificación: `git diff --check`, revisar archivos versionados y compilar scripts.
  - Archivos: `README.md`, `PRODUCT.md`.

- [ ] **Pull requests**
  - Aceptación: PR de corrección del núcleo contra `main`; PR #2 actualizado para mostrar solo la progresión vertical; ninguna rama contiene `codex`.
  - Verificación: revisar base/head/diff en GitHub y confirmar que ambos PR estén limpios.
  - Archivos: ramas y metadatos de PR.
