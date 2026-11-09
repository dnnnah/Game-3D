# Producto

<!-- impeccable:product-schema 1 -->

## Plataforma

adaptive

## Tecnologías

Unity 6.0.6f1, C#, URP y uGUI. La interacción prioriza teléfonos en vertical y adapta los controles a otras relaciones de aspecto.

## Usuarios

Los jugadores controlan un personaje 3D mediante controles táctiles en un teléfono. El estudiante y el profesor revisan el proyecto contra los requisitos de las seis actividades.

## Propósito

Entregar un prototipo jugable tridimensional para Optativa: Diseño de Juegos II, comenzando con movimiento, física, cámara y UI móvil y avanzando con una puntuación persistente.

## Capacidades y restricciones

- La actividad 1 establece la UI móvil, el movimiento en tres ejes, las colisiones físicas y el seguimiento de cámara en X/Z con una separación Z constante.
- La actividad 2 añade avance automático durante la partida, contador de distancia, aumento gradual de velocidad y récord local con `PlayerPrefs`.
- La progresión se configura en el Inspector mediante velocidad inicial, incremento por metro, velocidad máxima, ritmo de cambio y puntos por metro.
- Se usa Unity UI, el Input System instalado y la configuración URP existente.
- El archivo original `HelixPart.unitypackage` queda fuera de Git; solo se versionan sus recursos importados.

## Recursos disponibles

- `HelixPart.unitypackage` contiene `Assets/Models/HelixPart.FBX`, `Assets/Materials/HelixPart.mat` y `Assets/Prefabs/HelixPartPrefab.prefab`. El prefab incluye un `MeshCollider` sólido.
- El proyecto aún no tiene un modelo de personaje propio; una primitiva 3D de Unity sirve como personaje inicial.

## Principios

- Las mecánicas requeridas deben poder jugarse y revisarse dentro de Unity.
- Los controles táctiles deben ser cómodos en teléfonos verticales y respetar el área segura.
- Se entrega una actividad a la vez en ramas descriptivas y pull requests revisables.
