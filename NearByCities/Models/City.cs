namespace NearByCities.Models;

public class City
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Temperature { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public double DistanceKm { get; set; }

    public string TemperatureDisplay => $"{Temperature:F0}°C";
    public string DistanceDisplay => DistanceKm < 1
        ? $"{DistanceKm * 1000:F0} m"
        : $"{DistanceKm:F1} km";
}
