# Especificación: flujo procedural de pisos (`level-stream`)

## Objetivo

Sustituir la torre finita de Helix Jump por un flujo vertical continuo de pisos modulares sin aumentar indefinidamente la cantidad de objetos activos. El jugador debe encontrar pisos por debajo de su posición durante toda la partida.

## Suposiciones

1. Cada módulo contiene un anillo de sectores, un hueco transitable y un disparador de piso.
2. Se conserva una cantidad configurable de módulos activos, con 24 como valor inicial.
3. Al cruzar el disparador inferior de un piso, ese módulo ya quedó atrás y puede moverse debajo del piso más bajo.
4. La geometría de sector importada y la rotación de la torre actuales se conservan.

## Stack técnico

- Unity 6000.6.0f1, C#, `Transform`, `MeshCollider`, `BoxCollider` y `Rigidbody` cinemático de la torre.
- No se agrega ningún paquete.
- `HelixPart.unitypackage` permanece fuera de Git; el proyecto usa los recursos ya importados.

## Comandos

- Restaurar referencias: `dotnet restore Assembly-CSharp.csproj --ignore-failed-sources`
- Compilar Runtime: `dotnet build Assembly-CSharp.csproj --no-restore`
- Compilar Editor: `dotnet build Assembly-CSharp-Editor.csproj --no-restore`
- Revisar espacios: `git diff --check`
- Validación manual: abrir `Assets/Scenes/Main.unity` y comprobar el flujo en Play Mode.

## Estructura del proyecto

- `Assets/Scripts/ProceduralLevelGenerator.cs`: crea la reserva inicial, detecta el módulo atravesado y lo recicla al fondo.
- `Assets/Scripts/HelixFloorModule.cs`: agrupa sectores y disparador, configura el hueco y actualiza la altura al reciclar.
- `Assets/Scripts/FloorPassTrigger.cs`: detecta una sola vez el paso descendente y notifica al generador y a la progresión.
- `Assets/Scripts/GameSceneController.cs`: inicializa el generador y restablece su distribución al comenzar o reiniciar.
- `Assets/Editor/HelixSceneBuilder.cs` y `Assets/Scenes/Main.unity`: guardan los valores iniciales y referencias editables.

## Estilo de código

- Mantener los parámetros de generación en campos privados serializados con validaciones `Min`/`Range`.
- Separar el módulo de piso de la lógica que mantiene y recicla la reserva.
- Conservar un único disparo de progreso por cruce, incluso si Unity envía más de una notificación de trigger.
- Mantener fija la cantidad de módulos durante el reciclaje; no crear un objeto por cada piso recorrido.

```csharp
private void RecyclePassedModule(HelixFloorModule module)
{
    float nextY = lowestModuleY - floorSpacing;
    module.ConfigureAt(nextY, NextFloorIndex());
    lowestModuleY = nextY;
}
```

## Estrategia de validación

- No se agrega una suite automatizada para esta actividad.
- Compilar los ensamblados Runtime y Editor y hacer la comprobación manual en Unity Play Mode.
- Revisar que el número de módulos no cambie tras 100 pisos, que cada módulo reciclado conserve sus colliders y que el contador reciba una única notificación por cruce.

## Límites

- **Siempre:** conservar el movimiento vertical, la geometría de sectores, el hueco mínimo jugable y la reserva acotada.
- **Preguntar antes:** cambiar los ejes de movimiento/cámara, sustituir la geometría importada o añadir dependencias.
- **Nunca:** incluir el paquete original en Git o crear objetos de piso sin límite.

## Criterios de éxito

- El jugador siempre tiene pisos disponibles por debajo mientras progresa.
- El tamaño de la reserva y la separación vertical se pueden cambiar desde el Inspector.
- El hueco y el disparador se actualizan correctamente al reciclar cada módulo.
- Tras 100 cruces, el número de módulos y colliders permanece acotado y estable.

## Preguntas abiertas

- Ninguna; el mapa de módulos y el orden de desarrollo fueron aprobados.
