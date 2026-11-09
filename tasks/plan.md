# Plan: Actividad 3 — generación continua y bucle de partida

## Objetivo

Implementar en la rama `actividad-3-niveles-procedurales` una torre Helix continua con dificultad ajustable y un ciclo de partida que termine al tocar un sector rojo y permita reintentar. El plan sigue el mapa y las especificaciones aprobadas:

- [Mapa de capacidades](../ACTIVITY-3-MODULES.md)
- [level-stream](../docs/specs/SPEC-level-stream.md)
- [difficulty-curve](../docs/specs/SPEC-difficulty-curve.md)
- [run-lifecycle](../docs/specs/SPEC-run-lifecycle.md)

## Orden y dependencias

| Fase | Capacidad | Resultado | Depende de |
|---|---|---|---|
| 1 | `level-stream` | Módulos de piso configurables y reserva acotada con reciclaje vertical. | — |
| 2 | `difficulty-curve` | Curva de caída y sectores peligrosos limitados por datos del Inspector. | Eventos de piso de la fase 1. |
| 3 | `run-lifecycle` | Estados de menú/partida/derrota y reintento completo sin recargar la escena. | Fases 1 y 2. |
| 4 | Integración | Valores predeterminados guardados por el constructor de escena y en `Main.unity`. | Fases 1–3. |
| 5 | Cierre | Compilación, revisión del diff, commits lógicos y PR contra `main`. | Fases 1–4. |

Las fases son secuenciales porque los peligros y el ciclo de partida consumen los módulos y eventos producidos por las fases anteriores. No hay trabajo de implementación independiente que convenga paralelizar.

## Desglose de implementación

1. **Modelar un piso modular.** Agrupar sectores y disparador en un módulo; actualizar su configuración y su altura de forma reutilizable. Mantener el hueco y los colliders de cada pieza.
2. **Crear el generador acotado.** Construir el conjunto inicial desde los valores serializados y reciclar el piso ya atravesado debajo del más bajo. Notificar una sola vez cada cruce.
3. **Conectar la progresión.** Usar el conteo existente para definir niveles de velocidad y límites de peligros. Los parámetros permanecen editables en el Inspector y no modifican la persistencia del récord.
4. **Añadir sectores peligrosos.** Aplicar color rojo y detección de contacto al número de sectores permitido por la curva, manteniendo el hueco y al menos un sector seguro.
5. **Integrar el ciclo de partida.** Añadir estados idempotentes, detener la simulación en derrota y reiniciar balón, torre, progreso y reserva al reintentar.
6. **Actualizar la escena.** Conectar componentes y valores predeterminados en el constructor/editor y guardar la escena principal sin sustituir cambios locales no relacionados.
7. **Cerrar la actividad.** Compilar ensamblados Runtime y Editor, ejecutar `git diff --check`, revisar referencias/archivos versionados, hacer commits convencionales menores a 400 líneas cuando sea práctico y preparar el PR de la actividad 3.

## Riesgos y mitigaciones

- **Huecos o colisiones desalineados al reciclar:** conservar la geometría importada y reconfigurar el módulo completo, incluido su disparador; revisar `MeshCollider` tras mover el módulo.
- **Dificultad que vuelva un piso imposible:** limitar sectores peligrosos a la capacidad disponible después de reservar el hueco y un sector normal.
- **Callbacks duplicados:** hacer que el trigger y el cambio a `GameOver` sean idempotentes.
- **Reinicio que borre el récord:** separar `BeginRun` del almacenamiento de mejor marca y conservar la clave actual de `PlayerPrefs`.
- **Cambios locales de Unity:** antes de modificar `Main.unity` o materiales, inspeccionar sus diffs actuales y mantener los cambios ajenos a esta actividad fuera del commit.
- **Licencias del Editor:** si Unity no abre por inicialización del cliente de licencias, limitar la validación reportada a compilación y revisión de archivos; no afirmar que hubo una comprobación en Play Mode.

## Criterios de cierre

- La cantidad activa de módulos queda acotada y configurable y siguen apareciendo pisos debajo del balón.
- Cada piso incrementa progreso una sola vez; la velocidad y los peligros siguen los umbrales y topes del Inspector.
- El contacto rojo detiene una sola vez la partida y muestra la interfaz de derrota.
- Reintentar restaura balón, torre, progreso y distribución mientras conserva el récord.
- La compilación y revisión del diff no muestran errores atribuibles a la actividad; la escena conserva los recursos importados y no versiona `HelixPart.unitypackage`.
