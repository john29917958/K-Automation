using System.IO;
using Ncu.Oolab.Korat.KAutomation.Support.Languages;
using Newtonsoft.Json;

namespace Ncu.Oolab.Korat.KAutomation.Configs.App
{
    internal static class AppConfigLoader
    {
        public static AppConfig Load(string rootPath)
        {
            string configPath = Path.Combine(rootPath, "app.conf");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException(Lang.Get(LanguageKeys.ConfigFileNotFoundException, configPath));
            }

            string config = File.ReadAllText(configPath);

            return JsonConvert.DeserializeObject<AppConfig>(config);
        }
    }
}
