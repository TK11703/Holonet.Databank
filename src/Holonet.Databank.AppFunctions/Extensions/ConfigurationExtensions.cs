using Microsoft.Extensions.Configuration;

namespace Holonet.Databank.AppFunctions.Extensions;

public static class ConfigurationExtensions
{
    public static T? TryGetSection<T>(this IConfiguration config, string sectionName)
    where T : class, new()
    {
        var section = config.GetSection(sectionName);
        return section.Exists() ? section.Get<T>() : null;
    }

}
