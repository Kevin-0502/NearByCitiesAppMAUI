namespace NearByCities.Models;

public class Weather
{
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Main { get; set; } = string.Empty;

    public string IconUrl => $"https://openweathermap.org/img/wn/{Icon}@2x.png";
}
