# Especificación: avance y progresión por pisos

## Objetivo

Reemplazar la puntuación horizontal por progreso vertical propio de Helix Jump. Cada piso atravesado incrementa un contador visible de forma uniforme. La mejor marca se conserva en `PlayerPrefs`, y una curva ajustable aumenta gradualmente la velocidad de descenso del balón para reducir el tiempo de reacción.

## Stack técnico

- Unity 6000.6.0f1, C# y `PlayerPrefs`.
- El módulo recibe eventos de piso desde `vertical-core` y configura la velocidad vertical del balón.
- La curva usa velocidad inicial, incremento por piso y velocidad máxima, todos ajustables en el Inspector.

## Comandos de trabajo

- Restaurar: `dotnet restore Assembly-CSharp.csproj --ignore-failed-sources`
- Compilar: `dotnet build Assembly-CSharp.csproj --no-restore`
- Validar manualmente en Play Mode: cruzar pisos, observar el contador y reiniciar la aplicación para revisar la mejor marca.

## Estructura

- `Assets/Scripts/GameProgression.cs`: avance, curva de dificultad y persistencia.
- `Assets/Scripts/GameSceneController.cs`: conecta los eventos de piso con el HUD y el menú.
- `Assets/Scenes/Main.unity`: configura límites y valores iniciales de la curva en el Inspector.

## Estilo de código

- Incrementar el puntaje una sola vez por piso completado.
- Mantener la puntuación monotónica durante una partida y limitar la velocidad de descenso con un máximo serializado.
- Guardar el récord al salir al menú, pausar o cerrar la aplicación, no cada frame.

```csharp
private void OnFloorPassed()
{
    currentScore++;
    UpdateScoreLabel();
}
```

## Estrategia de validación

- Compilar contra las referencias de Unity 6000.6.
- En Play Mode, comprobar una actualización por piso, una curva creciente con tope y recuperación del récord tras reiniciar.
- No se añadirán pruebas automatizadas salvo solicitud expresa.

## Límites

- **Siempre:** usar pisos atravesados como puntuación y conservar récord local con `PlayerPrefs`.
- **Preguntar antes:** cambiar el contador a monedas, tiempo o distancia horizontal.
- **Nunca:** puntuar el movimiento por Z ni guardar preferencias en cada frame.
- La generación procedural/infinita y el reinicio tras derrota quedan en la actividad 3.

## Criterios de éxito

- El contador del HUD sube exactamente una unidad por cada piso atravesado.
- La velocidad de descenso aumenta gradualmente con los pisos y respeta el tope del Inspector.
- El menú muestra el mejor resultado guardado en una sesión posterior.

## Preguntas abiertas

- Ninguna para la actividad 2; la referencia y el contador por pisos establecen el comportamiento.
