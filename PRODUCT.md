# Producto

<!-- impeccable:product-schema 1 -->

## Plataforma

adaptive

## Tecnologías

Unity 6.0.6f1, C#, URP y uGUI. La interacción prioriza teléfonos en vertical y adapta los controles a otras relaciones de aspecto.

## Usuarios

Los jugadores controlan un personaje 3D mediante controles táctiles en un teléfono. El estudiante y el profesor revisan el proyecto contra los requisitos de las seis actividades.

## Propósito

Entregar un prototipo jugable de Helix Jump vertical para Optativa: Diseño de Juegos II. El balón cae y rebota junto a una torre de plataformas segmentadas; el jugador gira la torre para alinear sus huecos y avanzar.

## Capacidades y restricciones

- La actividad 1 configura UI móvil en Portrait, una pelota con Rigidbody y rebote, una torre que gira con el arrastre táctil y una cámara que acompaña al balón solo en Y manteniendo X/Z fijos.
- La actividad 2 cuenta cada piso atravesado, aumenta suavemente el límite de caída hasta un máximo y conserva el récord local con `PlayerPrefs`.
- La progresión se configura en el Inspector mediante límite de caída inicial, incremento por piso, límite máximo y ritmo de cambio.
- Se usa Unity UI, el Input System instalado y la configuración URP existente.
- El archivo original `HelixPart.unitypackage` queda fuera de Git; solo se versionan sus recursos importados.

## Recursos disponibles

- `HelixPart.unitypackage` contiene `Assets/Models/HelixPart.FBX`, `Assets/Materials/HelixPart.mat` y `Assets/Prefabs/HelixPartPrefab.prefab`. El prefab es una sección de plataforma con `MeshRenderer`, `MeshFilter` y `MeshCollider`, no una torre completa.
- El balón rojo se crea con una esfera de Unity y usa `Rigidbody`, `SphereCollider` y material físico de rebote.

## Principios

- Las mecánicas requeridas deben poder jugarse y revisarse dentro de Unity.
- Los controles táctiles deben ser cómodos en teléfonos verticales y respetar el área segura.
- Se entrega una actividad a la vez en ramas descriptivas y pull requests revisables.
