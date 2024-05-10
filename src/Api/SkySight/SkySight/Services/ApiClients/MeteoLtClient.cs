using SkySight.Data.Models;
using System.Text.Json;

namespace SkySight.Services.ApiClients;

public interface IForecastClient
{
    Task<IReadOnlyCollection<WeatherInfoDto>> GetWeatherInfoAsync(GetWeatherInfoRequest request);
}

public class MeteaLtForecastClient : IForecastClient
{
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly MeteoLtOptions _meteoLtOptions;
    private readonly ILogger<MeteaLtForecastClient> _logger;

    public MeteaLtForecastClient(HttpClient httpClient,
        MeteoLtOptions meteoLtOptions,
        ILogger<MeteaLtForecastClient> logger)
    {
        _httpClient = httpClient;
        _meteoLtOptions = meteoLtOptions;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<WeatherInfoDto>> GetWeatherInfoAsync(GetWeatherInfoRequest request)
    {
        if (request.Cities == null || request.Cities.Length == 0)
        {
            return new List<WeatherInfoDto>();
        }

        var places = await _httpClient.GetFromJsonAsync<ICollection<MeteoPlace>>(_meteoLtOptions.PlacesUrl);
        if (places == null)
        {
            _logger.LogWarning("Failed to fetch places from Meteo.lt API");
            return new List<WeatherInfoDto>();
        }

        var notFoundCities = new List<string>();
        foreach (var requestCity in request.Cities)
        {
            var foundPlaces = places.Any(x => x.Name == requestCity);
            if (!foundPlaces)
            {
                notFoundCities.Add(requestCity);
            }
        }

        if (notFoundCities.Any())
        {
            _logger.LogWarning("Failed to find places for cities for weather info: {Cities}", string.Join(", ", notFoundCities));
            return new List<WeatherInfoDto>();
        }

        var forecasts = new List<WeatherInfoDto>();
        foreach (var place in request.Cities)
        {
            var forecast = await GetForecastForPlace(place);
            if (forecast == null)
            {
                _logger.LogWarning("Failed to fetch forecast for place {Place}", place);
                continue;
            }

            forecasts.Add(forecast);
        }

        return forecasts;
    }

    private async Task<WeatherInfoDto?> GetForecastForPlace(string place)
    {
        var placeForecastInfoUrl = _meteoLtOptions.PlaceForecast(place);
        var placeForecastInfo = await _httpClient.GetFromJsonAsync<MeteoPlaceForecastInfo>(placeForecastInfoUrl, _serializerOptions);

        if (placeForecastInfo == null
            || placeForecastInfo.ForecastTypes == null
            || placeForecastInfo.ForecastTypes.Length == 0)
        {
            return null;
        }

        var forecastType = placeForecastInfo.ForecastTypes.First().Type;
        //https://api.meteo.lt/v1/places/vilnius/forecasts/long-term

        var placeForecastUrl = _meteoLtOptions.PlaceForecast(place).TrimEnd('/') + "/" + forecastType;


        var placeForecast = await _httpClient.GetFromJsonAsync<MeteoPlaceForecast>(placeForecastUrl, _serializerOptions);

        return MapToForecast(placeForecast);
    }

    private WeatherInfoDto? MapToForecast(MeteoPlaceForecast? placeForecast)
    {
        var latest = placeForecast
            ?.ForecastTimestamps
            .OrderBy(x => x.ForecastTimeUtc)
            .FirstOrDefault();

        return new WeatherInfoDto()
        {
            Place = placeForecast?.Place?.Name,
            Details = new WeatherInfoDetailsDto()
            {
                Temperature = (latest?.AirTemperature.ToString() ?? string.Empty) + " \u00b0C",
                Precipitation = latest?.ConditionCode ?? string.Empty,
                WindSpeed = (latest?.WindSpeed.ToString() ?? string.Empty) + " m/s"
            }
        };

    }

    private class MeteoPlaceForecast
    {
        public MeteoPlace? Place { get; set; }
        public List<ForecastTimestamp> ForecastTimestamps { get; set; } = new();
    }

    public class ForecastTimestamp
    {
        public string? ForecastTimeUtc { get; set; }
        public double AirTemperature { get; set; }
        public double FeelsLikeTemperature { get; set; }
        public int WindSpeed { get; set; }
        public int WindGust { get; set; }
        public int WindDirection { get; set; }
        public int CloudCover { get; set; }
        public int SeaLevelPressure { get; set; }
        public int RelativeHumidity { get; set; }
        public double TotalPrecipitation { get; set; }
        public string? ConditionCode { get; set; }

    }

    private class MeteoPlaceForecastInfo
    {
        public MeteoPlaceForecastType[]? ForecastTypes { get; set; }
    }

    public class MeteoPlaceForecastType
    {
        public string? Type { get; set; }
    }

    private class MeteoPlace
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}