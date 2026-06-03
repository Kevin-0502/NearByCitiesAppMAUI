using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NearByCities.Models;
using NearByCities.Services;

namespace NearByCities.ViewModels;

public partial class MapViewModel : ObservableObject
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;

    public MapViewModel(IWeatherService weatherService, ILocationService locationService)
    {
        _weatherService = weatherService;
        _locationService = locationService;
    }

    public ObservableCollection<City> Cities { get; } = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private LocationData? _userLocation;

    /// <summary>
    /// Se dispara cuando las ciudades se recargan (manual o automáticamente).
    /// La vista se suscribe para actualizar los pins.
    /// </summary>
    public event Action? CitiesUpdated;

    [RelayCommand]
    private async Task LoadCitiesAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        HasError = false;

        try
        {
            var location = await _locationService.GetCurrentLocationAsync();
            UserLocation = location;
            await LoadCitiesForLocationAsync(location);
        }
        catch (PermissionException)
        {
            HasError = true;
            ErrorMessage = "Se requiere permiso de ubicación.";
        }
        catch (HttpRequestException)
        {
            HasError = true;
            ErrorMessage = "Error de conexión.";
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void StartLocationTracking()
    {
        _locationService.StartListening(OnLocationChanged, minDistanceMeters: 500);
    }

    public void StopLocationTracking()
    {
        _locationService.StopListening();
    }

    private async void OnLocationChanged(LocationData newLocation)
    {
        UserLocation = newLocation;

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LoadCitiesForLocationAsync(newLocation);
            });
        }
        catch
        {
            // Ignore errors during background updates
        }
    }

    private async Task LoadCitiesForLocationAsync(LocationData location)
    {
        var cities = await _weatherService.GetNearbyCitiesAsync(location.Latitude, location.Longitude);
        Cities.Clear();
        foreach (var city in cities)
            Cities.Add(city);

        CitiesUpdated?.Invoke();
    }
}
