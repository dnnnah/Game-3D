# 🎮 Optativa: Diseño de Juegos II — Game 3D

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.6%2B-blue?logo=unity&logoColor=white" alt="Unity 6000.6 o superior" />
  <img src="https://img.shields.io/badge/Lenguaje-C%23-green?logo=csharp&logoColor=white" alt="Lenguaje C#" />
  <img src="https://img.shields.io/badge/Render-URP-purple" alt="Universal Render Pipeline" />
</p>

Repositorio del videojuego tridimensional desarrollado por etapas para la materia **Optativa: Diseño de Juegos II**. Cada actividad se entrega en su propia rama y mediante un pull request.

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

### Actividad 1: Fundamentos 3D y controles móviles

- **Objetivo:** configurar la UI móvil, el movimiento tridimensional, las colisiones y una cámara que acompañe al personaje en X y Z conservando su separación en Z.
- **Recursos:** modelo, material y prefab `HelixPart`.
- **Estado:** 🟡 En desarrollo

### Actividad 2: Dinámicas y dificultad progresiva

- **Objetivo:** avance fluido, dificultad progresiva, persistencia con `PlayerPrefs` y contador visual.
- **Estado:** ⚪ Pendiente

### Actividad 3: Niveles procedurales y Game Loop

- **Objetivo:** generación continua de módulos configurables y reinicio al perder.
- **Estado:** ⚪ Pendiente

### Actividad 4: Iluminación y personalización visual

- **Objetivo:** iluminación ambiental y personalización del fondo y del personaje.
- **Estado:** ⚪ Pendiente

### Actividad 5: Menús y transiciones entre niveles

- **Objetivo:** navegación completa y jugabilidad en al menos dos niveles consecutivos.
- **Estado:** ⚪ Pendiente

### Actividad 6: Efectos visuales y potenciadores

- **Objetivo:** estela y potenciador temporal de velocidad, con duración máxima de tres segundos.
- **Estado:** ⚪ Pendiente

---

## 🛠️ Tecnologías

- **Motor:** Unity 6.0.6 (6000.6.0f1)
- **Lenguaje:** C#
- **Renderizado:** Universal Render Pipeline (URP)
- **UI:** Unity UI (uGUI) con controles táctiles
- **Entrada:** Input System
- **Control de versiones:** Git y GitHub
- **Commits:** Conventional Commits

---

## 💻 Ejecutar el proyecto

1. Instala Unity **6000.6.0f1** o una versión Unity 6 compatible.
2. Abre la carpeta `My project` desde Unity Hub.
3. Abre `Assets/Scenes/MainMenu.unity` cuando la actividad 1 esté integrada.
4. Pulsa **Play** en el Editor o crea una compilación para el dispositivo móvil.

El repositorio excluye los directorios generados por Unity, como `Library`, `Temp` y `Logs`. El paquete fuente `HelixPart.unitypackage` se conserva fuera del repositorio; solo se versionan los recursos importados que usa el proyecto.

---

## 🌿 Flujo de trabajo

- Una rama y un pull request por actividad.
- Las ramas usan nombres descriptivos y no incluyen `codex`.
- Los commits siguen Conventional Commits y se mantienen por debajo de 400 líneas cuando sea práctico.
- Cada actividad cambia a estado **Completado** al cerrar su pull request.
