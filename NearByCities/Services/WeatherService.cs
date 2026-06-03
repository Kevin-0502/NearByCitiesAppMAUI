using System.Net.Http.Json;
using NearByCities.Constants;
using NearByCities.Models;
using NearByCities.Models.Api;
using NearByCities.Utils;

namespace NearByCities.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(AppConstants.OpenWeatherMapBaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<List<City>> GetNearbyCitiesAsync(double lat, double lon)
    {
        var url = $"/data/2.5/find?lat={lat}&lon={lon}&cnt={AppConstants.NearbyCitiesCount}" +
                  $"&units={AppConstants.Units}&lang={AppConstants.Lang}&appid={AppConstants.OpenWeatherMapApiKey}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<FindCitiesApiResponse>();
        if (data?.List is null) return [];

        return data.List.Select(item => new City
        {
            Name = item.Name,
            Latitude = item.Coord.Lat,
            Longitude = item.Coord.Lon,
            Temperature = item.Main.Temp,
            TempMin = item.Main.TempMin,
            TempMax = item.Main.TempMax,
            FeelsLike = item.Main.FeelsLike,
            Humidity = item.Main.Humidity,
            WindSpeed = item.Wind.Speed,
            Condition = item.Weather.FirstOrDefault()?.Description ?? string.Empty,
            IconUrl = item.Weather.FirstOrDefault() is { } w
                ? $"https://openweathermap.org/img/wn/{w.Icon}@2x.png"
                : string.Empty,
            DistanceKm = GeoUtils.CalculateDistanceKm(lat, lon, item.Coord.Lat, item.Coord.Lon)
        }).ToList();
    }

    public async Task<City> GetCityWeatherAsync(double lat, double lon)
    {
        var url = $"/data/2.5/weather?lat={lat}&lon={lon}" +
                  $"&units={AppConstants.Units}&lang={AppConstants.Lang}&appid={AppConstants.OpenWeatherMapApiKey}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
        if (item is null) throw new Exception("Respuesta vacía de la API.");

        return new City
        {
            Name = item.Name,
            Latitude = item.Coord.Lat,
            Longitude = item.Coord.Lon,
            Temperature = item.Main.Temp,
            TempMin = item.Main.TempMin,
            TempMax = item.Main.TempMax,
            FeelsLike = item.Main.FeelsLike,
            Humidity = item.Main.Humidity,
            WindSpeed = item.Wind.Speed,
            Condition = item.Weather.FirstOrDefault()?.Description ?? string.Empty,
            IconUrl = item.Weather.FirstOrDefault() is { } w
                ? $"https://openweathermap.org/img/wn/{w.Icon}@2x.png"
                : string.Empty
        };
    }

    public async Task<List<Forecast>> GetForecastAsync(double lat, double lon)
    {
        var url = $"/data/2.5/forecast?lat={lat}&lon={lon}" +
                  $"&units={AppConstants.Units}&lang={AppConstants.Lang}&appid={AppConstants.OpenWeatherMapApiKey}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<ForecastApiResponse>();
        if (data?.List is null) return [];

        // Group by day and take one entry per day (around noon)
        return data.List
            .GroupBy(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).Date)
            .Take(AppConstants.ForecastDays)
            .Select(group =>
            {
                var midday = group.OrderBy(f =>
                    Math.Abs(DateTimeOffset.FromUnixTimeSeconds(f.Dt).Hour - 12)).First();

                return new Forecast
                {
                    Date = group.Key,
                    TempMin = group.Min(f => f.Main.TempMin),
                    TempMax = group.Max(f => f.Main.TempMax),
                    Condition = midday.Weather.FirstOrDefault()?.Description ?? string.Empty,
                    IconUrl = midday.Weather.FirstOrDefault() is { } w
                        ? $"https://openweathermap.org/img/wn/{w.Icon}@2x.png"
                        : string.Empty
                };
            }).ToList();
    }
}
