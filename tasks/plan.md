# Plan: corregir el proyecto a Helix Jump vertical

## Objetivo y alcance

Reemplazar el corredor horizontal actual por un prototipo jugable que se parezca a la referencia: columna central blanca, pisos circulares oscuros segmentados con huecos y acentos amarillos, balón rojo, control táctil horizontal de la torre y encuadre Portrait. El balón rebota y desciende por Y; la cámara sigue su altura manteniendo X/Z.

La actividad 1 entrega la escena vertical base, la física, el control de torre, la cámara y la UI móvil inicial. La actividad 2 añade contador por pisos, curva de dificultad y récord con `PlayerPrefs`. La actividad 3 conserva niveles infinitos/procedurales y el ciclo de derrota/reinicio.

## Arquitectura y orden

1. **`vertical-core`**: escena y torre finita construidas con piezas del prefab; balón con `Rigidbody`/`SphereCollider` y rebote; control táctil que gira la torre alrededor de Y; cámara con X/Z fijos que acompaña Y.
2. **`floor-progression`**: recibe cada cruce de piso, incrementa contador, aumenta gradualmente la velocidad de descenso con un límite y persiste el récord.
3. **`portrait-ui`**: fuerza orientación Portrait, conserva el área segura y conecta menú/HUD al marcador.

Dependencias: `vertical-core` no depende de los otros módulos; `floor-progression` depende de `vertical-core`; `portrait-ui` depende de ambos.

## Fases de implementación

### A. Corrección del núcleo (rama `correccion-helix-vertical`)

- Crear una herramienta de Editor para componer y guardar la escena vertical con instancias del prefab, en vez de crear un corredor solo durante Play Mode.
- Reemplazar desplazamiento X/Z del personaje por rebote y caída vertical del balón.
- Incorporar giro de la torre por arrastre horizontal y cámara que sigue Y con X/Z constantes.
- Configurar orientación y menú móvil en Portrait.
- Compilar y revisar el diff; abrir el PR de corrección contra `main`.

### B. Actualización de la actividad 2

- Conectar cruces de pisos con contador y récord.
- Cambiar la progresión actual basada en distancia Z por pisos descendidos y velocidad vertical progresiva con tope.
- Actualizar README/producto y el PR #2 para que su diff contenga solo progreso vertical respecto a la corrección del núcleo.

## Verificación por fase

- `git diff --check` y revisión del diff de escena, referencias GUID y archivos de Unity.
- Compilación de scripts con `dotnet restore Assembly-CSharp.csproj --ignore-failed-sources` y `dotnet build Assembly-CSharp.csproj --no-restore`.
- Revisión manual en Unity 6000.6.0f1: abrir `Assets/Scenes/Main.unity`, confirmar la composición visible en Scene y probar rebote, giro, cámara, menú, contador y guardado en Play Mode.
- No añadir pruebas automatizadas sin solicitud expresa. Si el Editor no inicia por el cliente de licencias, reportar la limitación y no declarar validación manual.

## Riesgos y mitigaciones

- `HelixPartPrefab` es una sola pieza con `MeshCollider` no convexo. Mantener esa geometría como plataforma estática/kinemática y no convertirla en cuerpo dinámico.
- Las instancias de sector pueden dejar huecos incorrectos si la orientación del FBX no coincide con el eje radial. Verificar orientación y escala en la escena antes de ajustar el diseño.
- La orientación actual del proyecto es LandscapeRight. Cambiarla a Portrait y revisar que el CanvasScaler y el área segura sigan funcionando.
- El PR #2 aún contiene la mecánica anterior por Z. Mantenerlo sin fusionar; ajustar su base a la corrección del núcleo sin force-push. Si GitHub no permite un diff limpio, crear una rama vertical de reemplazo y cerrar el PR obsoleto.
- El paquete fuente `HelixPart.unitypackage` permanece fuera de Git; solo se versionan los recursos importados.

## Secuencia de ramas y PR

1. Completar la corrección de actividad 1 en `correccion-helix-vertical` y abrir un PR contra `main`.
2. Sobre la corrección aprobada, actualizar la rama/PR de actividad 2 con puntuación por pisos y persistencia.
3. Mantener commits lógicos con menos de 400 líneas cambiadas cada uno cuando sea práctico.
