namespace NearByCities.Models;

public class Forecast
{
    public DateTime Date { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;

    public string DayName => Date.ToString("ddd");
    public string TempDisplay => $"{TempMin:F0}° / {TempMax:F0}°";
}
