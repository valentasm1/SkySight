using SkySight.Data.Models;
using SkySight.MinimApis;
using SkySight.Services;
using SkySight.Services.ApiClients;

namespace SkySight;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var meteoLtOptions = configuration
            .Read<MeteoLtOptions>();
        builder.Services.AddSingleton(meteoLtOptions);

        var serviceOptions = configuration
            .Read<ServiceOptions>();
        builder.Services.AddSingleton(serviceOptions);

        builder.Services.AddTransient<IMeteoService, MeteoService>();
        builder.Services.AddHttpClient<IForecastClient, MeteaLtForecastClient>();

        var app = builder.Build();
        app.MapWeatherApi();

        app.UseHttpsRedirection();
        app.Run();
    }
}