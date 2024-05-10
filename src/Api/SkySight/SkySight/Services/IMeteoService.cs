using SkySight.Data.Models;

namespace SkySight.Services;

public interface IMeteoService
{
    Task<WeatherInfoApi> GetWeatherInfoAsync(GetWeatherInfoRequest request);
}