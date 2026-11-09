# Producto

<!-- impeccable:product-schema 1 -->

## Plataforma

adaptive

## Tecnologías

Unity 6.0.6f1, C#, URP y uGUI. La interacción prioriza teléfonos en vertical y adapta los controles a otras relaciones de aspecto.

## Usuarios

Los jugadores controlan un personaje 3D mediante controles táctiles en un teléfono. El estudiante y el profesor revisan el proyecto contra los requisitos de las seis actividades.

## Propósito

Entregar un prototipo jugable tridimensional para Optativa: Diseño de Juegos II, iniciando con la actividad 1 de movimiento, física, cámara y UI móvil.

## Capacidades y restricciones

- La actividad 1 establece la UI móvil, el movimiento en tres ejes, las colisiones físicas y el seguimiento de cámara en X/Z con una separación Z constante.
- Se usa Unity UI, el Input System instalado y la configuración URP existente.
- El archivo original `HelixPart.unitypackage` queda fuera de Git; solo se versionan sus recursos importados.

## Recursos disponibles

- `HelixPart.unitypackage` contiene `Assets/Models/HelixPart.FBX`, `Assets/Materials/HelixPart.mat` y `Assets/Prefabs/HelixPartPrefab.prefab`. El prefab incluye un `MeshCollider` sólido.
- El proyecto aún no tiene un modelo de personaje propio; una primitiva 3D de Unity sirve como personaje inicial.

## Principios

- Las mecánicas requeridas deben poder jugarse y revisarse dentro de Unity.
- Los controles táctiles deben ser cómodos en teléfonos verticales y respetar el área segura.
- Se entrega una actividad a la vez en ramas descriptivas y pull requests revisables.
