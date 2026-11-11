# Optativa: Diseño de Juegos II — Helix Jump

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.6%2B-blue?logo=unity&logoColor=white" alt="Unity 6000.6 o superior" />
  <img src="https://img.shields.io/badge/Lenguaje-C%23-green?logo=csharp&logoColor=white" alt="Lenguaje C#" />
  <img src="https://img.shields.io/badge/Render-URP-purple" alt="Universal Render Pipeline" />
</p>

Proyecto de videojuego tridimensional para la materia **Optativa: Diseño de Juegos II**. La propuesta es un Helix Jump vertical: el balón cae y rebota sobre una torre de plataformas circulares; el jugador gira la torre para alinear los huecos.

---

## Información del alumno y curso

| Campo | Detalle |
| :--- | :--- |
| **Institución** | Universidad de Guadalajara |
| **Materia** | Optativa: Diseño de Juegos II |
| **Estudiante** | Donnovan Trejo Corona |
| **Profesor** | Oscar Gregorio Silva Mares |
| **Semestre** | 2026-2 |

---

## Actividades

### Actividad 1: Base vertical, física y UI móvil

- **Descripción:** escena 3D vertical con torre segmentada y huecos, balón con Rigidbody y rebote, giro táctil de la torre, cámara que acompaña al balón únicamente en Y y menú adaptable a pantalla Portrait.
- **Recursos:** prefab `HelixPartPrefab`, modelo y material derivados de `HelixPart.unitypackage`.
- **Estado:** Completada. El PR #1 y su corrección en el PR #3 están fusionados.

### Actividad 2: Progresión y persistencia

- **Descripción:** el HUD cuenta cada piso atravesado; la velocidad de caída aumenta suavemente hasta un límite configurable y el récord se guarda con `PlayerPrefs`.
- **Estado:** Completada. El PR #2 está fusionado.

### Actividad 3: Generación continua y bucle de juego

- **Descripción:** generación procedural o modular continua de plataformas, dificultad progresiva configurable desde el Inspector y reinicio del flujo al perder.
- **Estado:** Completada. El PR #4 está fusionado.

### Actividad 4: Iluminación y personalización visual

- **Descripción:** fondo configurable en ejecución, iluminación ambiental y direccional ajustable, bola personalizable y acento seguro rojo a juego con la bola.
- **Especificación:** `docs/specs/SPEC-visual-management.md`.
- **Estado:** PR #5 abierto para revisión en la rama `actividad-4-gestion-visual`.

### Actividad 5: Menús y transiciones entre niveles

- **Descripción:** menú principal conectado a tres niveles consecutivos (metas de 6, 8 y 12 pisos), carga asíncrona validada mediante `SceneManager` y opciones de reintento/regreso al menú.
- **Especificación:** `docs/specs/SPEC-scene-flow.md`.
- **Estado:** PR #6 abierto para revisión en la rama `actividad-5-navegacion-niveles`, encadenado al PR #5 de la actividad 4.

### Actividad 6: Efectos y potenciadores

- **Descripción:** potenciador de velocidad con estela cian, temporizador en el HUD y ráfagas de partículas al recogerlo y completar un nivel.
- **Estado:** Implementado en el código; falta validar el recorrido en Play Mode y la compilación Android.

## Entrega Android privada

- **Nombre visible:** Helix 3D
- **Identificador de paquete:** `dnnnah.udg`
- **SDK mínimo:** Android API 26 (Android 8.0); el SDK objetivo se selecciona automáticamente.
- **Arquitecturas:** ARMv7 y ARM64 en un solo APK.
- **Backend:** IL2CPP
- **Ícono:** marca roja y amarilla sobre fondo oscuro, con capas adaptativas de Android.
- **Salida prevista:** `Builds/Android/Helix3D.apk` (el directorio `Builds/` está excluido de Git).

Para preparar el entorno, instala desde Unity Hub **Android Build Support**, **Android SDK & NDK Tools** y **OpenJDK** para Unity 6000.6.0f1. En el Editor, usa **Tools > Helix Jump > Configure Android Player Settings** y después **Tools > Helix Jump > Build Android APK**.

El APK se entrega directamente para revisión académica; no forma parte de una publicación en una tienda ni requiere una clave de firma de tienda. Para instalarlo en Android 8.0 o posterior, copia el APK al dispositivo, permite instalar aplicaciones desde la aplicación que lo abre y ejecútalo. La carga al buzón del curso y la retroalimentación en el foro a dos compañeros se realizan desde la cuenta académica del estudiante.

---

## Tecnologías

- **Motor:** Unity 6.0.6 (6000.6.0f1)
- **Lenguaje:** C#
- **Renderizado:** Universal Render Pipeline (URP)
- **Interfaz:** Unity UI (uGUI) con escalado móvil y área segura
- **Entrada:** Input System
- **Control de versiones:** Git y GitHub
- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/es/v1.0.0/)

---

## Ejecutar el proyecto

1. Instala Unity **6000.6.0f1** o una versión Unity 6 compatible.
2. Abre la carpeta `My project` desde Unity Hub.
3. Abre `Assets/Scenes/Main.unity` y pulsa **Play**.
4. En móvil, desliza horizontalmente para girar la torre. En el Editor usa **A/D** o las flechas.

El repositorio excluye los directorios generados por Unity, como `Library`, `Temp` y `Logs`. `HelixPart.unitypackage` se conserva fuera de Git; solo se versionan los recursos importados necesarios para el proyecto.
