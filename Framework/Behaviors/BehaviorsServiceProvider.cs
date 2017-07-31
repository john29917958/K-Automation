using Ncu.Oolab.Korat.KAutomation.App;
using Ncu.Oolab.Korat.KAutomation.Support;

namespace Ncu.Oolab.Korat.KAutomation.Behaviors
{
    public class BehaviorsServiceProvider : ServiceProvider
    {
        public override string Name { get; set; } = "Behaviors";

        public BehaviorsServiceProvider(Automator automator) : base(automator)
        {

        }
        
        public override object Register()
        {
            BehaviorsPool pool = new BehaviorsPool();

            Automator.AddService(Name, pool);
            // TODO: Loads config file and adds Behaviors into pool.

            return pool;
        }
    }
}
