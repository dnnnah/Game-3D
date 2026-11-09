# Especificación: núcleo vertical de Helix Jump

## Objetivo

Corregir la escena principal para que represente el juego de referencia. El jugador controla una torre vertical de plataformas; un balón rebota y desciende por el eje Y. El gesto horizontal del teléfono gira la torre alrededor de su eje vertical. La cámara acompaña la altura del balón y conserva X y Z.

La escena debe mostrar la columna central, los pisos de plataforma, el balón y la cámara al abrir `Assets/Scenes/Main.unity` en el Editor, además de funcionar en Play Mode.

## Stack técnico

- Unity 6000.6.0f1, C#, URP, Input System y Unity UI.
- El paquete aporta una pieza en forma de sector `HelixPart` con mesh, material y `MeshCollider`; la escena compone pisos circulares finitos alrededor de una columna blanca.
- El balón rojo usa `SphereCollider`, `Rigidbody` y material físico con rebote; los sectores conservan colisión para que el balón rebote en la superficie.
- La orientación de ejecución es Portrait.

## Comandos de trabajo

- Restaurar el proyecto C# generado por Unity: `dotnet restore Assembly-CSharp.csproj --ignore-failed-sources`
- Compilar scripts: `dotnet build Assembly-CSharp.csproj --no-restore`
- Verificar manualmente: abrir `Assets/Scenes/Main.unity` en Unity 6000.6.0f1 y entrar en Play Mode.

## Estructura

- `Assets/Scenes/Main.unity`: columna, pisos circulares, balón, cámara y sesión visibles en el Editor.
- `Assets/Scripts/`: control de torre, física del balón, seguimiento vertical e integración de escena.
- `Assets/Prefabs/HelixPartPrefab.prefab`, `Assets/Models/HelixPart.FBX` y `Assets/Materials/HelixPart.mat`: pieza de plataforma importada.
- `ProjectSettings/ProjectSettings.asset`: orientación vertical para el dispositivo.

## Estilo de código

- Componentes Unity pequeños con campos serializados para ajustes visibles en el Inspector.
- Los nombres describen movimiento vertical y rotación de torre; no usar semántica de corredor o avance por Z.
- La cámara modifica solo su coordenada Y al seguir al jugador:

```csharp
Vector3 position = new Vector3(fixedX, target.position.y + heightOffset, fixedZ);
transform.position = Vector3.Lerp(transform.position, position, followSpeed * Time.deltaTime);
```

## Estrategia de validación

- Compilar los scripts con las referencias de Unity 6000.6.
- En Play Mode, comprobar que el balón cae/rebota en Y, el gesto horizontal gira la torre y la cámara mantiene X/Z constantes mientras acompaña Y.
- No se añadirán pruebas automatizadas salvo que se soliciten; la validación funcional requiere observar Unity en ejecución.

## Límites

- **Siempre:** mantener la torre vertical, usar el prefab importado y dejar sus piezas visibles en la escena de edición.
- **Preguntar antes:** cambiar la paleta de la referencia, agregar paquetes o alterar el gesto de rotación.
- **Nunca:** incluir el archivo original `.unitypackage` en Git o volver al corredor con movimiento principal en Z.
- La generación infinita y el reinicio por derrota pertenecen a la actividad 3.

## Criterios de éxito

- `Main.unity` muestra una columna blanca, los pisos circulares y el balón rojo al abrirse en el Editor.
- En ejecución vertical, el balón se mueve sobre Y y no es conducido por los ejes X/Z.
- Arrastrar horizontalmente gira la torre alrededor de Y.
- La cámara cambia su Y siguiendo al balón y conserva los valores configurados de X/Z.
- La disposición usa plataformas oscuras con huecos y sectores amarillos, de acuerdo con la imagen de referencia.

## Preguntas abiertas

- Ninguna para esta base: la imagen compartida define el encuadre, la orientación y el lenguaje visual.
