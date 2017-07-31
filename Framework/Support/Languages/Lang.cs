using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Ncu.Oolab.Korat.KAutomation.Support.Languages
{
    public static class Lang
    {
        private static ResourceManager _resourceManager;

        public static string Get(string key)
        {
            if (_resourceManager == null)
            {
                _resourceManager = new ResourceManager("Framework.Properties.Resources", Assembly.GetEntryAssembly());
            }

            string content = _resourceManager.GetString(key, CultureInfo.CurrentUICulture);

            if (content == null)
            {
                throw new ArgumentNullException();
            }

            return content;
        }

        public static string Get(string key, params object[] args)
        {
            string langContent = Get(key);

            if (args == null)
            {
                throw new ArgumentNullException();
            }

            langContent = string.Format(langContent, args);

            return langContent;
        }
    }
}
