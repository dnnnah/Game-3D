# 🎮 Optativa: Diseño de Juegos II — Helix Jump

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.6%2B-blue?logo=unity&logoColor=white" alt="Unity 6000.6 o superior" />
  <img src="https://img.shields.io/badge/Lenguaje-C%23-green?logo=csharp&logoColor=white" alt="Lenguaje C#" />
  <img src="https://img.shields.io/badge/Render-URP-purple" alt="Universal Render Pipeline" />
</p>

Proyecto de videojuego tridimensional para la materia **Optativa: Diseño de Juegos II**. La propuesta es un Helix Jump vertical: el balón cae y rebota sobre una torre de plataformas circulares; el jugador gira la torre para alinear los huecos.

---

## 👨‍🎓 Información del alumno y curso

| Campo | Detalle |
| :--- | :--- |
| **Institución** | Universidad de Guadalajara |
| **Materia** | Optativa: Diseño de Juegos II |
| **Estudiante** | Donnovan Trejo Corona |
| **Profesor** | Oscar Gregorio Silva Mares |
| **Semestre** | 2026-2 |

---

## 🚀 Actividades

### 📍 Actividad 1: Base vertical, física y UI móvil

- **Descripción:** escena 3D vertical con torre segmentada y huecos, balón con Rigidbody y rebote, giro táctil de la torre, cámara que acompaña al balón únicamente en Y y menú adaptable a pantalla Portrait.
- **Recursos:** prefab `HelixPartPrefab`, modelo y material derivados de `HelixPart.unitypackage`.
- **Estado:** 🟡 Corrección vertical en revisión mediante el PR #3.

### 📍 Actividad 2: Progresión y persistencia

- **Descripción:** el HUD cuenta cada piso atravesado; la velocidad de caída aumenta suavemente hasta un límite configurable y el récord se guarda con `PlayerPrefs`.
- **Estado:** 🟡 En revisión mediante el PR #2, actualizado para la mecánica vertical.

---

## 🛠️ Tecnologías

- **Motor:** Unity 6.0.6 (6000.6.0f1)
- **Lenguaje:** C#
- **Renderizado:** Universal Render Pipeline (URP)
- **Interfaz:** Unity UI (uGUI) con escalado móvil y área segura
- **Entrada:** Input System
- **Control de versiones:** Git y GitHub
- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/es/v1.0.0/)

---

## 💻 Ejecutar el proyecto

1. Instala Unity **6000.6.0f1** o una versión Unity 6 compatible.
2. Abre la carpeta `My project` desde Unity Hub.
3. Abre `Assets/Scenes/Main.unity` y pulsa **Play**.
4. En móvil, desliza horizontalmente para girar la torre. En el Editor usa **A/D** o las flechas.

El repositorio excluye los directorios generados por Unity, como `Library`, `Temp` y `Logs`. `HelixPart.unitypackage` se conserva fuera de Git; solo se versionan los recursos importados necesarios para el proyecto.

---

## 🌿 Flujo de trabajo

- Una rama y un pull request por actividad.
- Las ramas usan nombres descriptivos y no incluyen `codex`.
- Los commits siguen Conventional Commits y se mantienen por debajo de 400 líneas cuando sea práctico.
- Una actividad pasa a **Completada** cuando su pull request se cierra.
