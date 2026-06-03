# NearByCities - Descripción del Proyecto

## Resumen

NearByCities es una aplicación móvil multiplataforma que permite al usuario visualizar ciudades cercanas a su ubicación actual junto con información meteorológica en tiempo real. La app consume la API de OpenWeatherMap y presenta los datos en dos formatos: una lista interactiva con cards y un mapa con marcadores.

## Origen

Este proyecto es una migración de una aplicación originalmente desarrollada en React Native / Expo hacia .NET MAUI, manteniendo los mismos requerimientos funcionales pero aprovechando el ecosistema .NET con C# y XAML.

## Arquitectura

La aplicación sigue el patrón **MVVM (Model-View-ViewModel)** con los siguientes principios:

- **Separation of Concerns**: Las vistas (Views) solo contienen lógica de presentación. Toda la lógica de negocio reside en los ViewModels. Los servicios encapsulan el acceso a datos externos.
- **Dependency Injection**: Todos los servicios y ViewModels se registran en el contenedor de DI nativo de MAUI (`MauiProgram.cs`) y se inyectan vía constructor.
- **Interface Segregation**: Los servicios exponen interfaces (`IWeatherService`, `ILocationService`) para facilitar el desacoplamiento y la testabilidad.
- **CommunityToolkit.Mvvm**: Se utilizan source generators (`[ObservableProperty]`, `[RelayCommand]`) para reducir el boilerplate del patrón MVVM.

### Estructura del proyecto

```
NearByCities/
├── Constants/              # Configuración centralizada
│   └── AppConstants.cs     # API keys, URLs base, unidades
├── Models/                 # Entidades del dominio
│   ├── City.cs             # Ciudad con datos meteorológicos
│   ├── Forecast.cs         # Pronóstico diario
│   ├── LocationData.cs     # Coordenadas del usuario
│   ├── Weather.cs          # Condición climática
│   └── Api/                # DTOs para deserialización JSON
│       └── WeatherApiResponse.cs
├── Services/               # Capa de acceso a datos
│   ├── Interfaces/
│   │   ├── ILocationService.cs
│   │   └── IWeatherService.cs
│   ├── LocationService.cs  # GPS + polling de ubicación
│   └── WeatherService.cs   # Consumo de OpenWeatherMap API
├── ViewModels/             # Lógica de negocio
│   ├── NearbyCitiesViewModel.cs  # Lista de ciudades
│   ├── CityDetailsViewModel.cs   # Detalle + pronóstico
│   └── MapViewModel.cs           # Mapa con marcadores
├── Views/                  # Pantallas XAML
│   ├── NearbyCitiesPage.xaml/.cs  # Tab 1: Lista
│   ├── CityDetailsPage.xaml/.cs   # Detalle de ciudad
│   └── MapPage.xaml/.cs           # Tab 2: Mapa
├── Utils/
│   └── GeoUtils.cs         # Fórmula de Haversine
└── Platforms/              # Configuración por plataforma
    ├── Android/
    ├── iOS/
    └── MacCatalyst/
```

## Tecnologías utilizadas

| Tecnología | Versión | Uso |
|---|---|---|
| .NET | 9.0 | Runtime y SDK |
| .NET MAUI | 9.x | Framework UI multiplataforma |
| CommunityToolkit.Mvvm | 8.4.0 | Source generators para MVVM |
| Microsoft.Maui.Controls.Maps | 9.x | Mapa nativo con marcadores |
| HttpClient | (built-in) | Consumo de API REST |
| System.Text.Json | (built-in) | Deserialización de respuestas JSON |
| Microsoft.Maui.Essentials | (built-in) | Geolocalización y permisos |

## APIs consumidas

Todas las llamadas se realizan a **OpenWeatherMap** (https://openweathermap.org/api):

| Endpoint | Descripción |
|---|---|
| `/data/2.5/find` | Busca ciudades cercanas por coordenadas |
| `/data/2.5/weather` | Obtiene clima actual de una ciudad |
| `/data/2.5/forecast` | Pronóstico de 5 días cada 3 horas |

## Pantallas

### Tab 1: Ciudades Cercanas
Lista de ciudades cercanas a la ubicación del usuario. Cada card muestra nombre, temperatura actual, condición climática, ícono del clima y distancia. Soporta pull-to-refresh y estados de carga, vacío y error. Al tocar una card se navega al detalle.

### Tab 2: Mapa
Mapa interactivo con marcadores (pins) en cada ciudad encontrada. Cada pin muestra nombre y temperatura al tocarlo. Al tocar la ventana de información del pin se navega al detalle de esa ciudad. Incluye botones para centrar en la ubicación del usuario y recargar ciudades.

### Detalle de Ciudad
Pantalla con información completa: nombre, coordenadas, temperatura actual/máxima/mínima, sensación térmica, humedad, velocidad del viento, condición climática, ícono oficial de OpenWeatherMap y pronóstico de 5 días en scroll horizontal.

## Actualización en tiempo real

La aplicación monitorea la ubicación del usuario mediante polling cada 30 segundos. Si detecta un desplazamiento mayor a 50 metros, recarga automáticamente las ciudades cercanas tanto en la lista como en el mapa. El tracking se activa solo cuando la pantalla está visible para optimizar el consumo de batería.

## Manejo de errores

La aplicación maneja los siguientes escenarios de error con mensajes claros al usuario:

- Permisos de ubicación denegados
- Sin conexión a internet
- Timeout en la API
- API Key inválida (HTTP 401)
- Ubicación no disponible en el dispositivo

## Decisiones técnicas

1. **.NET MAUI sobre React Native**: La migración permite aprovechar el tipado fuerte de C#, rendimiento nativo compilado, y el tooling de Visual Studio/Rider. MAUI comparte el 100% del código de negocio entre plataformas.

2. **Shell Navigation + IQueryAttributable**: Permite pasar objetos complejos (como `City`) entre páginas sin necesidad de serialización, usando el sistema de navegación nativo de MAUI.

3. **Endpoint `/data/2.5/find`**: Obtiene múltiples ciudades cercanas en una sola llamada HTTP, en lugar de hacer geocoding inverso + múltiples requests individuales.

4. **Fórmula de Haversine**: Calcula distancias geodésicas precisas entre coordenadas para mostrar la distancia del usuario a cada ciudad.

5. **Polling en vez de Geofencing**: Se usa polling cada 30 segundos con umbral de distancia mínima. Es más simple y predecible que el geofencing nativo, suficiente para el caso de uso.

6. **Interfaces para servicios**: `IWeatherService` e `ILocationService` desacoplan la implementación y permiten crear mocks para testing unitario.

7. **Mapas nativos de MAUI**: Se usa `Microsoft.Maui.Controls.Maps` que renderiza Apple Maps en iOS y Google Maps en Android, aprovechando los mapas nativos de cada plataforma.