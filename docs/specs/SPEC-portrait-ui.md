# Especificación: interfaz para Helix Jump vertical

## Objetivo

Presentar un menú inicial y HUD legibles en teléfono vertical, con navegación entre menú y partida, puntuación por pisos y récord. La interacción de juego debe dejar el área táctil libre para girar la torre horizontalmente.

## Stack técnico

- Unity 6000.6.0f1, Unity UI (uGUI), Input System y área segura.
- `ProjectSettings` fuerza orientación Portrait; `CanvasScaler` adapta el diseño a otras proporciones.

## Comandos de trabajo

- Restaurar: `dotnet restore Assembly-CSharp.csproj --ignore-failed-sources`
- Compilar: `dotnet build Assembly-CSharp.csproj --no-restore`
- Validar en una vista vertical y en una proporción alternativa; comprobar menú, partida y retorno al menú.

## Estructura

- `Assets/Scripts/GameSceneController.cs`: crea y conecta menú y HUD.
- `Assets/Scripts/SafeAreaFitter.cs`: respeta recortes y áreas seguras del teléfono.
- `ProjectSettings/ProjectSettings.asset`: orientación Portrait.

## Estilo de código

- Una sola fuente visual para puntaje y récord, suministrada por `floor-progression`.
- Mantener controles fuera del área principal de arrastre; orientar elementos desde anchors, no coordenadas absolutas de pantalla.
- Usar texto de interfaz claro y valores actualizados únicamente cuando cambia el piso o el récord.

## Estrategia de validación

- Compilar scripts C#.
- En Play Mode, revisar menú inicial, HUD, retorno al menú, área segura y adaptación a pantalla vertical.
- No se añadirán pruebas automatizadas salvo solicitud expresa.

## Límites

- **Siempre:** priorizar teléfono vertical y respetar el área segura.
- **Preguntar antes:** agregar pantallas, servicios externos o un nuevo sistema de UI.
- **Nunca:** orientar el juego en horizontal o cubrir la interacción de arrastre con controles de movimiento X/Z.

## Criterios de éxito

- Al iniciar, el menú aparece en formato Portrait y permite iniciar la partida.
- Durante el juego se muestra el piso actual y el mejor resultado.
- El gesto horizontal para rotar la torre permanece utilizable sin botones de movimiento direccional.
- El retorno al menú funciona y conserva el récord.

## Preguntas abiertas

- Ninguna: la referencia compartida y la preferencia por teléfono vertical determinan el formato.
