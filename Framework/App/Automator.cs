using System;

namespace Ncu.Oolab.Korat.KAutomation.App
{
    public class Automator
    {
        public Automator()
        {
            
        }

        public void Run(IScript script)
        {
            if (script == null)
            {
                throw new ArgumentNullException();
            }
        }
    }
}
