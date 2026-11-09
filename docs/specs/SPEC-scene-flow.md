# Especificación: navegación y flujo de niveles

## Objetivo

Permitir que el jugador navegue desde el menú principal a dos niveles consecutivos, complete el primero para cargar el segundo y regrese al menú después de perder o terminar el último nivel.

## Alcance

- El menú principal inicia `Level1`.
- Cada nivel inicia la partida y muestra su número en el HUD.
- Al alcanzar la meta configurable de pisos, el nivel carga la escena siguiente con `SceneManager.LoadSceneAsync`.
- El último nivel muestra la pantalla de completado, con opciones de reintento y regreso al menú.
- El botón de menú en partida carga `Main`.
- El flujo verifica que cada escena esté en Build Settings y evita iniciar varias cargas simultáneas.
- El constructor de escena genera escenas mínimas para los niveles y las registra en Build Settings al abrir/construir `Main`.

## Estructura

- `Assets/Scripts/SceneTransitionController.cs`: validación y carga asíncrona de escenas.
- `Assets/Scripts/GameSceneController.cs`: conecta menú, HUD, meta de pisos, derrota, completado y navegación.
- `Assets/Editor/HelixSceneBuilder.cs`: crea `Level1` y `Level2` si faltan y los incluye en Build Settings.
- `docs/specs/SPEC-scene-flow.md`: alcance y criterios del flujo.

## Criterios de aceptación

- JUGAR desde `Main` carga `Level1`.
- `Level1` termina al alcanzar su meta y carga `Level2`.
- `Level2` permite terminar la partida, reintentar o volver a `Main`.
- Un nombre vacío o una escena ausente se reporta de forma controlada antes de iniciar una carga.
- La carga asíncrona no duplica transiciones por pulsaciones repetidas.

## Validación

- Compilar Runtime y Editor con `dotnet build ... --no-restore`.
- Revisar los cambios con `git diff --check`.
- Abrir `Main.unity`, comprobar la generación/registro de escenas y recorrer `Main → Level1 → Level2` en Play Mode.
- No añadir pruebas automatizadas para esta actividad.

## Límites

- Se mantienen las interfaces móviles existentes y el bucle de derrota/reintento.
- `HelixPart.unitypackage` permanece fuera de Git.
