using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NearByCities.Models;
using NearByCities.Services;

namespace NearByCities.ViewModels;

public partial class NearbyCitiesViewModel : ObservableObject
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;

    public NearbyCitiesViewModel(IWeatherService weatherService, ILocationService locationService)
    {
        _weatherService = weatherService;
        _locationService = locationService;
    }

    public ObservableCollection<City> Cities { get; } = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoadCitiesAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        HasError = false;
        ErrorMessage = string.Empty;
        IsEmpty = false;

        try
        {
            var location = await _locationService.GetCurrentLocationAsync();
            await LoadCitiesForLocationAsync(location);
        }
        catch (PermissionException)
        {
            HasError = true;
            ErrorMessage = "Se requiere permiso de ubicación para buscar ciudades cercanas.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            HasError = true;
            ErrorMessage = "API Key inválida. Verifica tu configuración.";
        }
        catch (TaskCanceledException)
        {
            HasError = true;
            ErrorMessage = "La solicitud tardó demasiado. Verifica tu conexión a internet.";
        }
        catch (HttpRequestException)
        {
            HasError = true;
            ErrorMessage = "Error de conexión. Verifica tu acceso a internet.";
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadCitiesAsync();
    }

    [RelayCommand]
    private async Task NavigateToCityAsync(City city)
    {
        var parameters = new Dictionary<string, object>
        {
            { "City", city }
        };
        await Shell.Current.GoToAsync("CityDetails", parameters);
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

        IsEmpty = Cities.Count == 0;
    }
}
