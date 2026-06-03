using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NearByCities.Models;
using NearByCities.Services;

namespace NearByCities.ViewModels;

public partial class CityDetailsViewModel : ObservableObject, IQueryAttributable
{
    private readonly IWeatherService _weatherService;

    public CityDetailsViewModel(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [ObservableProperty]
    private City _city = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<Forecast> Forecasts { get; } = [];

    public string CoordinatesDisplay => $"{City.Latitude:F4}, {City.Longitude:F4}";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("City", out var cityObj) && cityObj is City city)
        {
            City = city;
            OnPropertyChanged(nameof(CoordinatesDisplay));
            _ = LoadForecastAsync();
        }
    }

    [RelayCommand]
    private async Task LoadForecastAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        HasError = false;

        try
        {
            // Refresh current weather
            var updatedCity = await _weatherService.GetCityWeatherAsync(City.Latitude, City.Longitude);
            updatedCity.DistanceKm = City.DistanceKm;
            City = updatedCity;
            OnPropertyChanged(nameof(CoordinatesDisplay));

            // Load forecast
            var forecasts = await _weatherService.GetForecastAsync(City.Latitude, City.Longitude);
            Forecasts.Clear();
            foreach (var f in forecasts)
                Forecasts.Add(f);
        }
        catch (TaskCanceledException)
        {
            HasError = true;
            ErrorMessage = "La solicitud tardó demasiado.";
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
}
