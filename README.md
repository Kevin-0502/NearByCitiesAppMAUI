# NearByCities

Aplicación móvil multiplataforma desarrollada con .NET MAUI que muestra ciudades cercanas al usuario con información climática en tiempo real y un mapa interactivo con marcadores.

## Requisitos previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) con workload "Desarrollo de aplicaciones .NET Multi-platform App UI" **o** [JetBrains Rider](https://www.jetbrains.com/rider/) con soporte MAUI
- Para Android: Android SDK (API 21+)
- Para iOS/macOS: Xcode 15+ (solo en macOS)
- Una API Key gratuita de [OpenWeatherMap](https://openweathermap.org/api)
- Una API Key de [Google Maps](https://console.cloud.google.com/) (solo necesaria para el mapa en Android)

## Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/Kevin-0502/NearByCitiesAppMAUI.git
cd NearByCities
```

### 2. Configurar API Keys

Abrir `NearByCities/Constants/AppConstants.cs` y colocar tu API Key de OpenWeatherMap:

```csharp
public const string OpenWeatherMapApiKey = "TU_API_KEY_AQUI";
```

Para el mapa en Android, abrir `NearByCities/Platforms/Android/AndroidManifest.xml` y reemplazar:

```xml
<meta-data android:name="com.google.android.geo.API_KEY" android:value="TU_GOOGLE_MAPS_API_KEY" />
```

### 3. Restaurar paquetes

```bash
dotnet restore
```

### 4. Ejecutar la aplicación

```bash
# Android (requiere emulador o dispositivo conectado)
dotnet build -t:Run -f net9.0-android

# iOS (requiere macOS con Xcode)
dotnet build -t:Run -f net9.0-ios

# macOS
dotnet build -t:Run -f net9.0-maccatalyst
```

O simplemente presionar Run/Debug desde Visual Studio o Rider seleccionando el target deseado.

## Permisos requeridos

La aplicación solicita los siguientes permisos al usuario:

- **Ubicación (GPS)**: Para detectar la posición actual y buscar ciudades cercanas
- **Internet**: Para consumir la API de OpenWeatherMap y cargar íconos del clima

Estos permisos ya están configurados en:
- Android: `Platforms/Android/AndroidManifestPermissions.cs` y `AndroidManifest.xml`
- iOS: `Platforms/iOS/Info.plist` (`NSLocationWhenInUseUsageDescription`)

## Solución de problemas

- **"Failed to upload APK"**: Verificar que hay un emulador Android corriendo o un dispositivo conectado con depuración USB activada
- **Mapa en blanco (Android)**: Falta la API Key de Google Maps en `AndroidManifest.xml`
- **Error 401 de la API**: La API Key de OpenWeatherMap es inválida o no está activada (puede tardar unas horas después de crearla)
- **"Permiso de ubicación denegado"**: Ir a ajustes del dispositivo y otorgar el permiso manualmente

![alt text](<WhatsApp Image 2026-06-03 at 14.55.21.jpeg>)![alt text](<WhatsApp Image 2026-06-03 at 14.54.05.jpeg>) ![alt text](<WhatsApp Image 2026-06-03 at 14.54.05 (1).jpeg>)