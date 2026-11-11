# Diseño: Helix 3D para Android

**Fecha:** 2026-11-10
**Estado:** diseño aprobado en conversación; pendiente de revisión del documento.

## Objetivo

Completar el proyecto existente Helix Jump para entregar una aplicación Android instalable que cubra los criterios de la actividad: menú y navegación por Canvas, juego tridimensional, al menos tres niveles con dificultad creciente, puntuación y récord persistente, efectos de juego y configuración técnica de Android.

La salida prevista es un APK local y el enlace al repositorio existente. No se publicará en una tienda.

## Contexto y límites

- El proyecto usa Unity 6.0.6f1, C#, URP, uGUI, Input System y escenas en orientación vertical.
- `Main` ya contiene el menú; `GameSceneController` construye el Canvas, HUD, estados de partida y feedback de botones. `SceneTransitionController` valida y carga escenas de forma asíncrona.
- `Level1` tiene una meta de 6 pisos y pasa a `Level2`, que tiene una meta de 8. `Level2` está configurado como final. `HelixSceneBuilder` crea escenas ausentes y las registra en Build Settings.
- `GameProgression` ya cuenta pisos y guarda el récord local con `PlayerPrefs`. `ProceduralLevelGenerator` suministra la torre y sectores peligrosos. El trabajo local actual incluye el potenciador de velocidad; se conservará y se completará.
- Hay modificaciones locales sin commit en la rama `actividad-6-potenciador-estela`, incluidas escenas, scripts y recursos visuales. Todo ese trabajo se debe preservar; esta especificación es el único archivo nuevo que se añade en esta etapa.
- La instalación actual de Unity tiene soporte para macOS y WebGL, pero no Android Build Support ni sus herramientas. La compilación Android requiere instalar desde Unity Hub el módulo Android Build Support, Android SDK & NDK Tools y OpenJDK.

## Diseño del juego

### Flujo de escenas

`Main → Level1 → Level2 → Level3 → completado`

- El botón **JUGAR** del menú carga `Level1`.
- `Level1` se completa al pasar 6 pisos y carga `Level2`.
- `Level2` se completa al pasar 8 pisos y carga `Level3`.
- `Level3` se completa al pasar 12 pisos y muestra la pantalla de juego completado, con opciones para reintentar o volver al menú.
- El botón **MENÚ** durante una partida vuelve a `Main`; la derrota conserva su opción de reintento.
- Las escenas mantienen sus metas y perfiles de dificultad serializados y visibles en el Inspector. El flujo reutiliza los componentes de partida existentes.

### Curva de dificultad

Cada escena tendrá un perfil propio de velocidad y sectores peligrosos, editable desde Unity. Las metas y la dificultad crecerán de forma monotónica entre niveles; el nivel 3 será el más exigente. Los topes existentes de `GameProgression` y `ProceduralLevelGenerator` seguirán impidiendo que todos los sectores de un piso sean peligrosos o que la velocidad supere su límite.

`HelixSceneBuilder` añadirá `Level3` a Build Settings y configurará el paso de `Level2` a `Level3`. La actualización de escenas existentes debe limitarse al flujo y la configuración necesaria, preservando sus demás datos.

### Interfaz, progreso y persistencia

- Se conserva el menú, Canvas, HUD, área segura y controles táctiles actuales.
- El HUD muestra nivel y pisos superados en tiempo real; el récord local continúa en `PlayerPrefs`.
- Los botones de derrota/completado permiten reintentar o regresar al menú.
- No se añaden cuentas, servicios en línea ni guardado en la nube.

### Efectos de juego

Se completa el potenciador de velocidad ya presente en los cambios locales. La recogida mostrará su temporizador y respuesta visual; partículas breves y feedback de UI señalarán la recogida y la finalización de nivel. Se usarán componentes nativos de Unity y recursos existentes, sin dependencias nuevas. Los textos de la interfaz se mantendrán en español y se revisarán ortográficamente.

## Configuración Android

- Nombre visible: `Helix 3D`.
- Identificador de paquete solicitado: `dnnnah.udg`.
- `dnnnah.udg` es un identificador válido: Android requiere al menos dos segmentos, iniciados con letras y formados por caracteres alfanuméricos o guion bajo.
- Orientación: vertical, conservando los valores actuales.
- API mínima: 26 (Android 8.0), conservando la configuración actual.
- API objetivo: automática, usando la versión más reciente instalada y soportada por el Editor.
- Backend: IL2CPP.
- Arquitecturas: ARMv7 y ARM64 en un único APK; no separar el APK por ABI.
- Ícono: crear un ícono original y legible con la paleta existente — torre oscura, acento amarillo y balón rojo — y asignarlo a las densidades Android.
- Instalar Android Build Support, SDK, NDK y OpenJDK compatibles con Unity 6.0.6f1 desde Unity Hub. Unity recomienda utilizar las dependencias que instala el Hub, en lugar de versiones externas.
- Salida: `Builds/Android/Helix3D.apk`. El directorio `Builds/` está excluido de Git, así que el APK se entrega como archivo aparte del repositorio.
- No se configura publicación en Google Play ni firma con keystore de tienda. Una APK instalable de evaluación no requiere crear una cuenta de desarrollador de Google Play.
- La entrega por buzón académico y la instalación directa quedan fuera de una publicación en tienda. La verificación de desarrollador Android anunciada para 2026 aplica inicialmente a tiendas participantes en cuatro países; la instalación directa por sideload no está incluida en esa primera fase. Revisar la política si la distribución cambia o antes de 2027.

## Manejo de errores

- La carga conserva la validación de nombres de escena y evita transiciones simultáneas de `SceneTransitionController`.
- Si falta Android Build Support o alguna dependencia del SDK, la compilación se detiene con el error de Unity correspondiente; se informa el módulo que falta y no se declara un APK generado.
- Antes de exportar, las tres escenas deben estar activas en Build Settings y el identificador de paquete debe ser válido para Android.
- La entrega local no depende de credenciales de tienda ni de servicios externos.

## Verificación y entrega

- Compilar el proyecto y resolver errores de compilación atribuibles a los cambios.
- Revisar el flujo `Main → Level1 → Level2 → Level3 → completado`, derrota, reintento y retorno al menú en Play Mode.
- Confirmar conteo de pisos, curva de dificultad, límites de peligros, feedback del potenciador y persistencia del récord al reiniciar.
- Generar el APK y comprobar que el artefacto exista y contenga las escenas configuradas.
- Instalar y abrir el APK en un dispositivo Android o emulador si hay uno disponible. Si no lo hay, informar que la ejecución física en un teléfono no se pudo comprobar.
- Revisar `git diff --check` y preservar los cambios locales ya existentes.
- Proporcionar el enlace del repositorio `https://github.com/dnnnah/Game-3D` y el archivo APK.
- No hay un conector al buzón académico ni al foro de compañeros en este entorno. La carga al buzón y la retroalimentación personal a dos compañeros siguen siendo acciones del estudiante desde su cuenta.

No se agrega una suite automatizada para este cambio; la aceptación funcional requiere recorrer el flujo en Unity y, si es posible, instalar el APK en Android.

## Fuera de alcance

- Publicar la aplicación en Google Play u otra tienda.
- Crear cuentas de tienda, un keystore de producción o un servicio de guardado en la nube.
- Automatizar publicaciones en el foro o el envío a un buzón académico sin una integración disponible.
- Añadir paquetes externos para efectos o construir un sistema de niveles nuevo desde cero.

## Criterios de aceptación

1. El menú y la navegación Canvas funcionan en orientación vertical y el jugador puede completar los tres niveles consecutivos.
2. Las metas de nivel son 6, 8 y 12 pisos; la velocidad y los peligros aumentan progresivamente con topes que mantengan una ruta posible.
3. El HUD registra pisos y nivel en tiempo real; el récord se conserva entre ejecuciones.
4. El potenciador y los efectos breves aportan feedback visual/UI claro, y la derrota/completado permiten reintentar o volver al menú.
5. Player Settings muestran `Helix 3D`, el identificador `dnnnah.udg`, API mínima 26, API objetivo automática, IL2CPP y ARMv7+ARM64.
6. Se produce `Builds/Android/Helix3D.apk` con las tres escenas y se confirma su instalación/arranque cuando haya un dispositivo o emulador disponible.
7. Las dependencias de Android proceden de Unity Hub y son compatibles con Unity 6.0.6f1.

## Referencias

- Tutorial de exportación indicado: [Exportar el juego APK (13-Cap)](https://www.youtube.com/watch?v=0bytkPu10m0).
- [Unity 6: configuración del entorno Android](https://docs.unity3d.com/6000.1/Documentation/Manual/android-sdksetup.html).
- [Unity Manual: Player Settings de Android](https://docs.unity3d.com/es/current/Manual/class-PlayerSettingsAndroid.html).
- [Unity 6: niveles de API de Android](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AndroidSdkVersions.html).
- [Android: configuración del módulo y reglas para el identificador de aplicación](https://developer.android.com/build/configure-app-module).
- [Android: verificación de desarrolladores y calendario de distribución](https://developer.android.com/developer-verification).
