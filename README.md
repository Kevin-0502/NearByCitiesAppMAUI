# NearByCities - .NET MAUI

Aplicación móvil multiplataforma que muestra ciudades cercanas al usuario con información climática en tiempo real, utilizando la API de OpenWeatherMap.

## Arquitectura

**MVVM (Model-View-ViewModel)** con inyección de dependencias nativa de MAUI.

```
NearByCities/
├── Constants/          # Configuración y constantes (API keys, URLs)
├── Models/             # Entidades del dominio (City, Forecast, Weather, LocationData)
│   └── Api/            # DTOs para deserialización de respuestas HTTP
├── Services/           # Capa de datos (WeatherService, LocationService)
├── ViewModels/         # Lógica de negocio (NearbyCitiesViewModel, CityDetailsViewModel)
├── Views/              # Pantallas XAML (solo renderizado)
└── Utils/              # Utilidades (cálculo de distancias Haversine)
```

### Principios aplicados

- **Separation of Concerns**: Views sin lógica de negocio, ViewModels sin conocimiento de UI
- **Dependency Injection**: Servicios registrados en `MauiProgram.cs`, inyectados vía constructor
- **Interface Segregation**: `IWeatherService`, `ILocationService` para desacoplamiento y testabilidad
- **CommunityToolkit.Mvvm**: Generación de código con `[ObservableProperty]` y `[RelayCommand]`

## Tecnologías

| Tecnología | Uso |
|---|---|
| .NET 9 + MAUI | Framework multiplataforma |
| CommunityToolkit.Mvvm | MVVM source generators |
| Shell Navigation | Navegación con paso de parámetros |
| HttpClient | Consumo de API REST |
| Microsoft.Maui.Essentials | Geolocalización y permisos |
| OpenWeatherMap API | Datos meteorológicos |

## Configuración del entorno

### Requisitos

- .NET 9 SDK
- Visual Studio 2022 / Rider con workload MAUI
- Xcode (para iOS/macOS) o Android SDK

### Variables de entorno

Editar `Constants/AppConstants.cs` y agregar tu API Key:

```csharp
public const string OpenWeatherMapApiKey = "TU_API_KEY_AQUI";
```

Obtener key gratuita en: https://openweathermap.org/api

### APIs utilizadas

- **Current Weather** (`/data/2.5/weather`): clima actual por coordenadas
- **Find Nearby** (`/data/2.5/find`): ciudades cercanas por coordenadas
- **5 Day Forecast** (`/data/2.5/forecast`): pronóstico cada 3 horas

## Instalación y ejecución

```bash
# Clonar
git clone https://github.com/Kevin-0502/NearByCitiesApp.git
cd NearByCities

# Restaurar paquetes
dotnet restore

# Ejecutar en Android
dotnet build -t:Run -f net9.0-android

# Ejecutar en iOS
dotnet build -t:Run -f net9.0-ios

# Ejecutar en macOS
dotnet build -t:Run -f net9.0-maccatalyst
```

## Funcionalidades

### Pantalla 1: Ciudades Cercanas
- Ubicación actual del usuario con permisos
- 5 ciudades cercanas con temperatura, condición, icono y distancia
- Pull to refresh
- Estados: loading, empty, error

### Pantalla 2: Detalle de Ciudad
- Temperatura actual, máxima, mínima
- Sensación térmica, humedad, velocidad del viento
- Coordenadas geográficas
- Icono oficial OpenWeatherMap
- Pronóstico de 5 días

### Manejo de errores
- Permisos de ubicación denegados
- Sin conexión a internet
- API timeout
- API Key inválida
- Ubicación no disponible

## Decisiones técnicas

1. **MAUI en lugar de React Native**: Migración a ecosistema .NET para aprovechar tipado fuerte de C#, rendimiento nativo y tooling de Visual Studio/Rider.
2. **CommunityToolkit.Mvvm**: Reduce boilerplate con source generators, manteniendo el patrón MVVM limpio.
3. **Shell Navigation con IQueryAttributable**: Permite pasar objetos complejos entre páginas sin serialización.
4. **Haversine para distancias**: Cálculo preciso de distancia geodésica entre coordenadas.
5. **Endpoint `/data/2.5/find`**: Obtiene ciudades cercanas en una sola llamada HTTP en lugar de múltiples requests.
6. **Interfaces para servicios**: Facilita testing unitario y desacoplamiento.
