using SkySight.Data.Models;
using SkySight.Services.ApiClients;

namespace SkySight.Services;

public class MeteoService : IMeteoService
{
    private readonly IForecastClient _forecastClient;

    public MeteoService(IForecastClient forecastClient)
    {
        _forecastClient = forecastClient;
    }

    public async Task<WeatherInfoApi> GetWeatherInfoAsync(GetWeatherInfoRequest request)
    {
        var forecasts = await _forecastClient.GetWeatherInfoAsync(request);

        var response = new WeatherInfoApi();
        foreach (var forecast in forecasts)
        {
            if (string.IsNullOrEmpty(forecast.Place) || forecast.Details == null)
            {
                continue;
            }

            response.Forecasts.TryAdd(forecast.Place, forecast.Details);
        }


        return response;
    }
}