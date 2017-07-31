using Ncu.Oolab.Korat.KAutomation.Configs.App;

namespace Ncu.Oolab.Korat.KAutomation.Configs
{
    internal static class ConfigLoader
    {
        public static Configuration Load(string rootPath)
        {
            Configuration config = new Configuration();
            config.App = AppConfigLoader.Load(rootPath);

            return config;
        }
    }
}
