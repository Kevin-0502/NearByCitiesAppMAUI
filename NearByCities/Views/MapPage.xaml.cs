using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using NearByCities.Models;
using NearByCities.ViewModels;

namespace NearByCities.Views;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;
    private readonly Dictionary<Pin, City> _pinCityMap = new();

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        CityMap.MapClicked += OnMapClicked;
        _viewModel.CitiesUpdated += OnCitiesUpdated;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Cities.Count == 0)
        {
            await _viewModel.LoadCitiesCommand.ExecuteAsync(null);
            UpdateMapPins();
        }

        _viewModel.StartLocationTracking();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopLocationTracking();
    }

    private void OnCitiesUpdated()
    {
        MainThread.BeginInvokeOnMainThread(UpdateMapPins);
    }

    private void UpdateMapPins()
    {
        // Unsubscribe old pins
        foreach (var pin in CityMap.Pins)
            pin.InfoWindowClicked -= OnPinInfoWindowClicked;

        CityMap.Pins.Clear();
        _pinCityMap.Clear();

        foreach (var city in _viewModel.Cities)
        {
            var pin = new Pin
            {
                Label = city.Name,
                Address = $"{city.TemperatureDisplay} - {city.Condition}",
                Type = PinType.SearchResult,
                Location = new Location(city.Latitude, city.Longitude)
            };

            pin.InfoWindowClicked += OnPinInfoWindowClicked;
            _pinCityMap[pin] = city;
            CityMap.Pins.Add(pin);
        }

        // Center map on user location or first city
        if (_viewModel.UserLocation is not null)
        {
            var region = MapSpan.FromCenterAndRadius(
                new Location(_viewModel.UserLocation.Latitude, _viewModel.UserLocation.Longitude),
                Distance.FromKilometers(10));
            CityMap.MoveToRegion(region);
        }
        else if (_viewModel.Cities.Count > 0)
        {
            var first = _viewModel.Cities[0];
            var region = MapSpan.FromCenterAndRadius(
                new Location(first.Latitude, first.Longitude),
                Distance.FromKilometers(50));
            CityMap.MoveToRegion(region);
        }
    }

    private async void OnPinInfoWindowClicked(object? sender, PinClickedEventArgs e)
    {
        if (sender is Pin pin && _pinCityMap.TryGetValue(pin, out var city))
        {
            var parameters = new Dictionary<string, object> { { "City", city } };
            await Shell.Current.GoToAsync("CityDetails", parameters);
        }
    }

    private async void OnReloadClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoadCitiesCommand.ExecuteAsync(null);
        UpdateMapPins();
    }

    private void OnCenterLocationClicked(object? sender, EventArgs e)
    {
        if (_viewModel.UserLocation is not null)
        {
            var region = MapSpan.FromCenterAndRadius(
                new Location(_viewModel.UserLocation.Latitude, _viewModel.UserLocation.Longitude),
                Distance.FromKilometers(5));
            CityMap.MoveToRegion(region);
        }
    }

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        // Deselect any pin when tapping the map
    }
}
