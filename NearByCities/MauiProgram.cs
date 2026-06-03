using Microsoft.Extensions.Logging;
using NearByCities.Services;
using NearByCities.ViewModels;
using NearByCities.Views;

namespace NearByCities;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IWeatherService, WeatherService>();

        // ViewModels
        builder.Services.AddTransient<NearbyCitiesViewModel>();
        builder.Services.AddTransient<CityDetailsViewModel>();
        builder.Services.AddTransient<MapViewModel>();

        // Pages
        builder.Services.AddTransient<NearbyCitiesPage>();
        builder.Services.AddTransient<CityDetailsPage>();
        builder.Services.AddTransient<MapPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
