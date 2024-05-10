namespace SkySight.Data.Models;

public class WeatherInfoApi
{
    public Dictionary<string, WeatherInfoDetailsDto> Forecasts { get; set; } = new();
}
