namespace SkySight.Data.Models;
/// <summary>
/// Meteo.lt API options
/// https://api.meteo.lt/
/// </summary>
public class MeteoLtOptions
{
    public string PlacesUrl { get; set; } = string.Empty;
    public string PlaceForecasts { get; set; } = string.Empty;

    public string PlaceForecast(string place) => PlaceForecasts!.Replace("{place}", place);
}