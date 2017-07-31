using System;
using Ncu.Oolab.Korat.KAutomation.Support.Languages;

namespace Ncu.Oolab.Korat.KAutomation.Behaviors
{
    public abstract class Behaviors
    {
        protected App.Korat.Korat Korat;
        protected BehaviorsPool Pool;

        protected Behaviors(App.Korat.Korat korat, BehaviorsPool pool)
        {
            if (korat == null)
            {
                throw new ArgumentNullException(Lang.Get(LanguageKeys.KoratNullException));
            }

            if (pool == null)
            {
                throw new ArgumentNullException(Lang.Get(LanguageKeys.BehaviorsPool));
            }

            Korat = korat;
            Pool = pool;
        }
    }
}
