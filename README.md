# 🎮 Optativa: Diseño de Juegos II — Game 3D

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.6%2B-blue?logo=unity&logoColor=white" alt="Unity 6000.6 o superior" />
  <img src="https://img.shields.io/badge/Lenguaje-C%23-green?logo=csharp&logoColor=white" alt="Lenguaje C#" />
  <img src="https://img.shields.io/badge/Render-URP-purple" alt="Universal Render Pipeline" />
</p>

Repositorio del videojuego tridimensional para la materia **Optativa: Diseño de Juegos II**. Las actividades se entregan en ramas separadas mediante pull requests.

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
- **Recursos integrados:** modelo, material y prefab `HelixPart`.
- **Navegación:** menú inicial y pantalla de juego dentro de la escena principal.
- **Estado:** ✅ Completada mediante el PR #1

### Actividad 2: Avance y progresión

- **Objetivo:** mantener el avance fluido, aumentar la velocidad gradualmente y contabilizar la distancia recorrida.
- **Persistencia:** el récord de distancia se guarda localmente mediante `PlayerPrefs`.
- **Estado:** 🟡 Lista para revisión

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
3. Abre `Assets/Scenes/Main.unity`.
4. Pulsa **Play** en el Editor o crea una compilación para el dispositivo móvil.

El repositorio excluye los directorios generados por Unity, como `Library`, `Temp` y `Logs`. El paquete fuente `HelixPart.unitypackage` se conserva fuera del repositorio; solo se versionan los recursos importados que usa el proyecto.

---

## 🌿 Flujo de trabajo

- Una rama y un pull request por actividad.
- Las ramas usan nombres descriptivos y no incluyen `codex`.
- Los commits siguen Conventional Commits y se mantienen por debajo de 400 líneas cuando sea práctico.
- Cada actividad cambia a estado **Completado** al cerrar su pull request.
