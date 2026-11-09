# Especificación: iluminación y apariencia visual

## Objetivo

Permitir configurar desde el Inspector y cambiar durante la partida el fondo, la iluminación y la apariencia de la bola en el prototipo Helix Jump. El sector seguro de acento cambia de amarillo a rojo para coincidir con el color inicial de la bola; los peligros mantienen un rojo más brillante para distinguirlos.

## Alcance

- Aplicar un color sólido configurable al fondo de la cámara.
- Configurar color ambiental, color/intensidad de la luz direccional y su rotación.
- Personalizar color, metalicidad y suavidad de la bola.
- Exponer métodos para cambiar el color de fondo, luz ambiental y apariencia del jugador en ejecución.
- Mantener los colores del balón por renderer con `MaterialPropertyBlock`, sin editar los materiales compartidos.
- Guardar el rojo como color por defecto del sector de acento seguro y de la bola.

## Estructura

- `Assets/Scripts/GameSceneController.cs`: valores serializados, configuración de luz/fondo y métodos para aplicar cambios visuales.
- `Assets/Scripts/ProceduralLevelGenerator.cs` y `Assets/Scripts/HelixFloorModule.cs`: identificar el material de acento como sector seguro y aplicarlo en los módulos reciclados.
- `Assets/Editor/HelixSceneBuilder.cs`: crear el material rojo de acento al construir la escena.
- `README.md`: documentar la entrega y el estado de la actividad.

## Criterios de aceptación

- Al iniciar, la cámara usa el color de fondo configurado y la luz ambiental/direccional respeta sus valores del Inspector.
- `SetBackgroundColor` cambia el fondo de la cámara activa durante la ejecución.
- La bola presenta el color, metalicidad y suavidad configurados, y `SetPlayerAppearance` permite cambiarlos en ejecución.
- El sector seguro de acento aparece rojo por defecto, conservando los sectores peligrosos distinguibles en rojo brillante.
- Los módulos procedurales conservan ese acento después de reciclarse.

## Validación

- Compilar los ensamblados Runtime y Editor con `dotnet build ... --no-restore`.
- Revisar los cambios con `git diff --check`.
- Comprobar en Play Mode los valores de fondo, luz y apariencia, y cambiar el fondo y color de la bola desde sus métodos públicos.
- No añadir pruebas automatizadas para esta actividad.

## Límites

- `HelixPart.unitypackage` permanece fuera de Git.
- Los cambios locales existentes en `Main.unity`, URP y materiales generados no se incluyen en el commit.
- La personalización no agrega una pantalla de opciones ni nuevos paquetes.
