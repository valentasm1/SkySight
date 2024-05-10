using SkySight.Data.Models;
using SkySight.Services;

namespace SkySight.MinimApis;

public static class WeatherApiExtension
{
    public static IEndpointRouteBuilder MapWeatherApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/lithuania/cities/top", async (x) =>
        {
            var weatherService = x.RequestServices.GetRequiredService<IMeteoService>();
            var serviceOptions = x.RequestServices.GetRequiredService<ServiceOptions>();
            var forecasts = await weatherService.GetWeatherInfoAsync(new GetWeatherInfoRequest()
            {
                Cities = serviceOptions.TopCities
            });

            x.Response.ContentType = "application/json";
            await x.Response.WriteAsJsonAsync(forecasts);
        });

        return endpoints;
    }
}