using System.Text.Json.Serialization;

namespace NearByCities.Models.Api;

// Response from /data/2.5/weather
public class WeatherApiResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("coord")]
    public CoordDto Coord { get; set; } = new();

    [JsonPropertyName("main")]
    public MainDto Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherDto> Weather { get; set; } = [];

    [JsonPropertyName("wind")]
    public WindDto Wind { get; set; } = new();
}

// Response from /data/2.5/forecast
public class ForecastApiResponse
{
    [JsonPropertyName("list")]
    public List<ForecastItemDto> List { get; set; } = [];
}

// Response from /geo/1.0/reverse
public class GeocodingApiResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

// Response from /data/2.5/find
public class FindCitiesApiResponse
{
    [JsonPropertyName("list")]
    public List<WeatherApiResponse> List { get; set; } = [];
}

public class CoordDto
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}

public class MainDto
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class WeatherDto
{
    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class WindDto
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}

public class ForecastItemDto
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public MainDto Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherDto> Weather { get; set; } = [];
}
