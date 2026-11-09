# Especificación: curva de dificultad (`difficulty-curve`)

## Objetivo

Hacer que la partida de Helix Jump se vuelva gradualmente más exigente a partir de los pisos superados, combinando el aumento de velocidad de caída existente con una aparición configurable de sectores rojos peligrosos.

## Suposiciones

1. `GameProgression` sigue siendo la fuente del conteo, la velocidad y el récord local en `PlayerPrefs`.
2. Los eventos de cruce de `level-stream` alimentan la progresión una sola vez por piso.
3. El aumento de dificultad se define con niveles serializados y topes editables desde el Inspector.
4. Cada piso conserva el hueco configurado y al menos un sector normal; el nivel máximo no puede convertir todos los sectores en peligros.

## Stack técnico

- Unity 6000.6.0f1, C#, `PlayerPrefs` y componentes de física existentes.
- Sin dependencias nuevas ni cambios al formato del récord guardado.

## Comandos

- Compilar Runtime: `dotnet build Assembly-CSharp.csproj --no-restore`
- Compilar Editor: `dotnet build Assembly-CSharp-Editor.csproj --no-restore`
- Revisar espacios: `git diff --check`
- Validación manual: cruzar pisos en Play Mode y observar contador, velocidad y aparición de peligros.

## Estructura del proyecto

- `Assets/Scripts/GameProgression.cs`: expone el nivel actual de dificultad y calcula los parámetros de velocidad y peligro a partir de pisos superados.
- `Assets/Scripts/ProceduralLevelGenerator.cs`: consulta la progresión al preparar o reciclar un piso y aplica la cantidad permitida de sectores rojos.
- `Assets/Editor/HelixSceneBuilder.cs` y `Assets/Scenes/Main.unity`: configuran pasos, velocidad máxima, densidad y material de peligro.

## Estilo de código

- Expresar cada nivel como datos serializables con umbral de pisos, velocidad objetivo y cantidad máxima de sectores peligrosos.
- Limitar todos los resultados al rango configurado; validar que queden sectores normales y el hueco.
- Mantener el guardado del récord fuera de `Update` y guardar solo al cambiar el récord o cerrar/pausar la partida.

```csharp
int GetHazardSectorCount(int availableSectors)
{
    int safeCapacity = Mathf.Max(0, availableSectors - 1);
    return Mathf.Clamp(currentHazardLimit, 0, safeCapacity);
}
```

## Estrategia de validación

- No se agrega una suite automatizada para esta actividad.
- Compilar Runtime y Editor; verificar manualmente niveles de dificultad antes y después de cada umbral.
- Comprobar que velocidad y peligros aumenten según Inspector, respeten sus topes, mantengan un hueco y al menos un sector seguro, y que el récord sobreviva al reinicio de la aplicación.

## Límites

- **Siempre:** conservar el contador por pisos y el récord de `PlayerPrefs`; limitar velocidad y sectores de peligro.
- **Preguntar antes:** cambiar la unidad de progreso o el sistema de persistencia.
- **Nunca:** guardar preferencias cada frame, eliminar el hueco o hacer imposible el nivel con sectores peligrosos.

## Criterios de éxito

- La dificultad sube al cruzar los umbrales definidos en el Inspector y no depende del tiempo de ejecución.
- La velocidad de caída y la cantidad de peligros alcanzan como máximo los límites configurados.
- Cada piso generado mantiene un hueco jugable y por lo menos un sector normal.
- El contador y el récord previo se mantienen correctos con la generación y el reciclaje de módulos.

## Preguntas abiertas

- Ninguna; la curva por pisos, sus topes y los sectores rojos fueron aprobados.
