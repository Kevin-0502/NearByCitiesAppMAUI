using NearByCities.Models;

namespace NearByCities.Services;

public interface IWeatherService
{
    Task<List<City>> GetNearbyCitiesAsync(double lat, double lon);
    Task<City> GetCityWeatherAsync(double lat, double lon);
    Task<List<Forecast>> GetForecastAsync(double lat, double lon);
}
