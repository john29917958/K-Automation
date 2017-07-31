using System;
using System.Collections.Generic;
using Ncu.Oolab.Korat.KAutomation.Exceptions;
using Ncu.Oolab.Korat.KAutomation.Support.Languages;

namespace Ncu.Oolab.Korat.KAutomation.Behaviors
{
    public class BehaviorsPool
    {
        protected List<object> BehaviorsList;

        public BehaviorsPool()
        {
            BehaviorsList = new List<object>();
        }

        public bool HasBehaviorsType<T>() where T : class
        {
            foreach (object behaviors in BehaviorsList)
            {
                if (behaviors is T)
                {
                    return true;
                }
            }

            return false;
        }

        public void Add<T>(T behaviors) where T : class
        {
            if (behaviors == null)
            {
                throw new ArgumentNullException(Lang.Get(LanguageKeys.AddBehaviorsNullException));
            }

            if (!HasBehaviorsType<T>())
            {
                BehaviorsList.Add(behaviors);
            }
            
            throw new BehaviorsAlreadyExistsException(Lang.Get(LanguageKeys.BehaviorsAlreadyExistsException));
        }

        public T Request<T>() where T : class 
        {
            foreach (object behaviors in BehaviorsList)
            {
                T actualBehaviors = behaviors as T;

                if (actualBehaviors != null)
                {
                    return actualBehaviors;
                }
            }

            return null;
        }
    }
}
