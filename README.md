# GeoJSON Polygon Viewer

Aplicación de escritorio Unity que visualiza datos geográficos GeoJSON en un entorno 3D interactivo.

<img width="1026" height="807" alt="image" src="https://github.com/user-attachments/assets/cc1b4ff6-5d36-4ff7-b0f3-07f6deb1fa7a" />


## Descripción

GeoJSON Polygon Viewer carga archivos GeoJSON y renderiza sus geometrías como objetos 3D navegables. Está orientado especialmente a la visualización de polígonos geográficos complejos con soporte de agujeros, colecciones de features y múltiples tipos de geometría.

## Características

- Carga archivos `.geojson` y `.json` mediante un diálogo nativo de Windows
- Soporta todos los tipos de geometría GeoJSON:
  - `Point` / `MultiPoint` → esferas 3D
  - `LineString` / `MultiLineString` → líneas renderizadas
  - `Polygon` / `MultiPolygon` → mallas teseladas con bordes
  - `GeometryCollection` y `FeatureCollection`
- Teselación de polígonos con soporte de anillos internos (agujeros) usando LibTessDotNet
- Extracción de color desde propiedades del feature (`color` en formato HTML)
- Cámara orbital interactiva con órbita, paneo y zoom
- Encuadre automático de la geometría al cargar (`FitToView`)
- Manejo robusto de JSON: sanitiza valores `NaN` e `Infinity` antes de parsear

## Tecnologías

| Componente | Tecnología |
|---|---|
| Motor | Unity 6000.3.14f1 |
| Lenguaje | C# (.NET) |
| Serialización JSON | Newtonsoft.Json 3.2.2 |
| Teselación | LibTessDotNet |
| Input | Unity InputSystem 1.11.2 |
| UI | uGUI 2.0.0 + TextMesh Pro |
| Gráficos | Universal Render Pipeline (URP) |

## Estructura del proyecto

```
Assets/
├── GeoJSON/
│   ├── Core/           # Modelos de datos (Point, Polygon, Feature, etc.)
│   │   ├── GeoJSONObject.cs
│   │   ├── Geometry.cs
│   │   ├── Feature.cs
│   │   └── GeoJSONSettings.cs
│   ├── Converters/     # Deserialización polimórfica personalizada
│   │   └── GeometryConverter.cs
│   └── Helpers/        # Lógica de rendering geometría → GameObject
│       └── GeoJSONHelper.cs
├── Viewer/
│   ├── GeoJSONViewer.cs   # Controlador principal: UI, carga de archivos
│   └── OrbitCamera.cs     # Cámara 3D interactiva
├── Plugins/
│   └── LibTessDotNet/  # Librería de teselación de polígonos
├── Scenes/
│   └── Main.unity      # Escena principal
└── Settings/           # Assets de URP
```

## Controles de cámara

| Acción | Control |
|---|---|
| Órbita | Arrastrar con botón izquierdo del mouse |
| Paneo | Arrastrar con botón derecho o medio |
| Zoom | Rueda del mouse |
| Encuadrar geometría | Tecla `F` |

## Sistema de coordenadas

Las coordenadas GeoJSON `[longitud, latitud, altitud?]` se transforman a:
- `X` → longitud
- `Y` → altitud (elevación)
- `Z` → latitud

## Uso

1. Abrir el proyecto en Unity 6000.3.14f1
2. Cargar la escena `Assets/Scenes/Main.unity`
3. Ejecutar la aplicación
4. Hacer clic en **Browse** para seleccionar un archivo GeoJSON
5. La geometría se renderiza automáticamente y la cámara se encuadra sobre ella

## Licencia

MIT License — Timining, 2026
