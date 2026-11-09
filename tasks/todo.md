# Implementación: Helix Jump vertical

- [x] Sustituir el runner X/Z por balón vertical, rebote y giro táctil de torre.
- [x] Seguir al balón en Y con X/Z de cámara fijos.
- [x] Construir torre segmentada, huecos, columna, colores y triggers desde `Main.unity` al abrir una escena limpia en Unity.
- [x] Configurar menú móvil Portrait y contador de pisos en el HUD.
- [x] Aumentar suavemente el límite de caída por piso, configurable en Inspector y con máximo.
- [x] Guardar y mostrar el récord con `PlayerPrefs`.
- [x] Actualizar README y PRODUCT; mantener `HelixPart.unitypackage` fuera de Git.
- [x] Abrir PR #3 para el núcleo y preparar PR #2 como cambio de progresión sobre esa base.
- [ ] Revisar visualmente la escena construida y jugar la caída, el rebote, el giro y el conteo en Play Mode.

La compilación de los ensamblados Runtime y Editor terminó con 0 errores y 0 advertencias. No fue posible abrir Unity en modo batch porque el proceso aislado no pudo inicializar el cliente de licencias. El generador Editor queda disponible al abrir el proyecto o desde **Tools > Helix Jump > Build Main Scene**.
