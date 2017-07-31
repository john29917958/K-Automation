using System;
using Ncu.Oolab.Korat.KAutomation.App;
using Ncu.Oolab.Korat.KAutomation.Support.Languages;

namespace Ncu.Oolab.Korat.KAutomation.Support
{
    public abstract class ServiceProvider
    {
        public abstract string Name { get; set; }

        protected Automator Automator;

        protected ServiceProvider(Automator automator)
        {
            if (automator == null)
            {
                throw new ArgumentNullException(Lang.Get(LanguageKeys.AutomatorNullException));
            }

            Automator = automator;
        }

        public abstract object Register();
    }
}
