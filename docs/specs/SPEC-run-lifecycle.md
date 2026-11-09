# Especificación: ciclo de partida (`run-lifecycle`)

## Objetivo

Completar el ciclo de Helix Jump con estados claros de menú, partida y derrota. Tocar un sector rojo detiene el juego y muestra una opción de reintento que restaura el estado de la partida sin recargar la escena.

## Suposiciones

1. Los sectores rojos son peligros sólidos: el balón rebota en plataformas normales y pierde al hacer contacto con uno rojo.
2. La derrota se procesa una sola vez aunque se reciban varias colisiones en el mismo paso de física.
3. Reintentar conserva el mejor récord de `PlayerPrefs`, pero reinicia conteo, balón, rotación y distribución de pisos.
4. El menú y la interfaz de juego actuales se amplían con un panel de fin de partida, manteniendo la adaptación vertical móvil.

## Stack técnico

- Unity 6000.6.0f1, C#, `Rigidbody`, `Collider`, Unity UI y `PlayerPrefs` existentes.
- `SceneManager` no es necesario para reintentar; el flujo se reinicia en la escena actual.

## Comandos

- Compilar Runtime: `dotnet build Assembly-CSharp.csproj --no-restore`
- Compilar Editor: `dotnet build Assembly-CSharp-Editor.csproj --no-restore`
- Revisar espacios: `git diff --check`
- Validación manual: iniciar, perder al tocar un sector rojo, reintentar y comprobar el restablecimiento en Play Mode.

## Estructura del proyecto

- `Assets/Scripts/HazardSector.cs`: identifica un sector peligroso y notifica el contacto del balón.
- `Assets/Scripts/GameSceneController.cs`: administra los estados `Menu`, `Playing` y `GameOver`, y controla las acciones de menú/reintento.
- `Assets/Scripts/PlayerMovement.cs`: permite detener el Rigidbody y devolver el balón a su punto inicial.
- `Assets/Scripts/TowerRotationController.cs`: detiene y restablece la rotación de la torre.
- `Assets/Scripts/GameProgression.cs` y `Assets/Scripts/ProceduralLevelGenerator.cs`: reinician progreso y distribución sin borrar el récord.
- `Assets/Editor/HelixSceneBuilder.cs` y `Assets/Scenes/Main.unity`: configuran el panel de derrota y las referencias.

## Estilo de código

- Representar el ciclo con un `enum` serializable/privado y permitir transiciones solo desde el estado válido.
- Hacer idempotente la derrota para evitar duplicar guardados, cambios de panel o reinicios.
- Reiniciar cada subsistema mediante métodos explícitos; no destruir y reconstruir toda la escena.

```csharp
private void EndRun()
{
    if (runState != RunState.Playing)
        return;

    runState = RunState.GameOver;
    progression.EndRun();
    playerMovement.SetMovementEnabled(false);
    towerRotation.SetRotationEnabled(false);
    gameOverPanel.SetActive(true);
}
```

## Estrategia de validación

- No se agrega una suite automatizada para esta actividad.
- Compilar Runtime y Editor y recorrer manualmente menú, partida, derrota y reintento.
- Confirmar que el contacto rojo muestra la derrota una vez; que el balón, torre, conteo y pisos vuelven a sus valores iniciales; que el récord persiste; y que no aparecen errores en la consola.

## Límites

- **Siempre:** mantener el menú móvil, conservar el récord y restaurar el estado de todos los subsistemas al reintentar.
- **Preguntar antes:** cambiar la condición de derrota aprobada o añadir carga de escenas/paquetes.
- **Nunca:** continuar la simulación en `GameOver`, borrar el récord al reintentar o procesar varias derrotas por una colisión.

## Criterios de éxito

- El ciclo admite `Menu → Playing → GameOver → Playing` sin recargar la escena.
- El contacto con un sector rojo detiene el balón y la torre y muestra una interfaz de derrota con reintento.
- Reintentar restablece balón, torre, contador, dificultad y pisos; el récord guardado permanece.
- Los tres estados funcionan en la escena principal sin errores de consola durante el flujo manual.

## Preguntas abiertas

- Ninguna; la derrota por sector rojo y el reinicio local fueron aprobados.
