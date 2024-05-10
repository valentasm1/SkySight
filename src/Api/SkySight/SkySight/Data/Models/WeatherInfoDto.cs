namespace SkySight.Data.Models;

public class WeatherInfoDto
{
    public string? Place { get; set; }
    public WeatherInfoDetailsDto? Details { get; set; }
}

public class WeatherInfoDetailsDto
{
    public string? Temperature { get; set; }
    public string? Precipitation { get; set; }
    public string? WindSpeed { get; set; }
}