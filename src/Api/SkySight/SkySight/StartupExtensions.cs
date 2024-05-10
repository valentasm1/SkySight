namespace SkySight;

public static class StartupExtensions
{
    public static TOptions Read<TOptions>(this IConfiguration configuration, string? sectionName = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = typeof(TOptions).Name;
        }

        var options = configuration
            .GetSection(sectionName)
            .Get<TOptions>();

        if (options == null)
        {
            throw new NullReferenceException($"{sectionName} section is not specified in configuration");
        }

        return options;
    }
}