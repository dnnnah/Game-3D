# Mapa de capacidades: Actividad 3

| Módulo | Responsabilidad | Dependencias |
|---|---|---|
| `level-stream` | Mantener una reserva acotada de pisos Helix configurables y reciclar los ya atravesados debajo del balón. | — |
| `difficulty-curve` | Calcular velocidad de caída y densidad de peligros según los pisos superados, con límites ajustables en el Inspector. | Eventos de piso de `level-stream` |
| `run-lifecycle` | Coordinar menú, partida, derrota por peligro, interfaz de fin y reinicio completo. | `level-stream`, `difficulty-curve` |

Orden de desarrollo: `level-stream` → `difficulty-curve` → `run-lifecycle`.

Especificaciones: [level-stream](docs/specs/SPEC-level-stream.md), [difficulty-curve](docs/specs/SPEC-difficulty-curve.md) y [run-lifecycle](docs/specs/SPEC-run-lifecycle.md).
