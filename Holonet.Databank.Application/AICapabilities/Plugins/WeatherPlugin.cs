using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Holonet.Databank.Application.AICapabilities.Plugins;
public class WeatherPlugin(IHttpClientFactory httpClientFactory)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    [KernelFunction("get_weather_forcast")]
    [Description("Takes a latitude and longitude pair, representing a location, and returns a forecasted weather for that location for the requested number of days. The number of days is limited from one day in advance to up to sixteen..")]
    [return: Description("JSON collection containing a collection of weather forecasts for the supplied latitude and longitude values.")]
    public async Task<string> GetWeatherForcast(string latitude, string longitude, int days)
    {
        if (string.IsNullOrWhiteSpace(latitude))
        {
            throw new ArgumentNullException(nameof(latitude), "Latitude is required.");
        }
        if (string.IsNullOrWhiteSpace(longitude))
        {
            throw new ArgumentNullException(nameof(longitude), "Longitude is required.");
        }
        if (days < 1 || days > 16)
        {
            throw new ArgumentOutOfRangeException(nameof(days), "Days must be between 1 and 16.");
        }
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetStringAsync($"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation,rain,showers,snowfall,weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,snowfall,weather_code,cloud_cover,wind_speed_10m,uv_index&temperature_unit=fahrenheit&wind_speed_unit=mph&precipitation_unit=inch&forecast_days={days}");
        return response;
    }
}
