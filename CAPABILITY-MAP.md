# Mapa de capacidades: Helix Jump vertical

| Módulo | Responsabilidad | Depende de |
|---|---|---|
| `vertical-core` | Columna central y pisos circulares ensamblados con piezas `HelixPart`, balón con física vertical, giro táctil de la torre y cámara que sigue Y con X/Z fijos. | — |
| `floor-progression` | Contador de pisos descendidos, dificultad gradual y récord persistente con `PlayerPrefs`. | `vertical-core` |
| `portrait-ui` | Menú y HUD para teléfono vertical, área segura y presentación del avance y récord. | `vertical-core`, `floor-progression` |

**Orden de construcción:** `vertical-core` → `floor-progression` → `portrait-ui`.

## Límites

- La torre finita de esta corrección usa las piezas importadas del paquete `HelixPart.unitypackage`.
- La actividad 3 conserva la generación procedural/infinita y el reinicio del bucle al perder.
- La referencia visual es una torre cilíndrica vertical, segmentos oscuros con huecos, acentos amarillos, balón rojo y encuadre móvil vertical.
- El paquete `.unitypackage` original permanece fuera de Git.
