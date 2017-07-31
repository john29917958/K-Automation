using System;
using System.Collections.Generic;
using Ncu.Oolab.Korat.KAutomation.Exceptions;
using Ncu.Oolab.Korat.KAutomation.Support;
using Ncu.Oolab.Korat.KAutomation.Support.Languages;

namespace Ncu.Oolab.Korat.KAutomation.App
{
    public class Automator
    {
        protected Dictionary<string, object> Services;

        public Automator(IServices services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(Lang.Get(LanguageKeys.IServicesNullException));
            }

            Services = new Dictionary<string, object>();

            RegisterServices(services);
        }

        public void AddService(string name, object service)
        {
            if (Services.ContainsKey(name))
            {
                throw new ServiceAlreadyExistsException(Lang.Get(LanguageKeys.ServiceAlreadyExistsException, name));
            }

            Services.Add(name, service);
        }

        public T RequestService<T>(string serviceName) where T : class
        {
            if (!Services.ContainsKey(serviceName))
            {
                throw new ArgumentOutOfRangeException();
            }

            T service = Services[serviceName] as T;
            if (service == null)
            {
                throw new TypeLoadException(Lang.Get(LanguageKeys.ServiceTypeLoadException, typeof(T).Name));
            }

            return service;
        }

        public void Run(IScript script)
        {
            if (script == null)
            {
                throw new ArgumentNullException(Lang.Get(LanguageKeys.IScriptNullException));
            }
        }

        protected void RegisterServices(IServices services)
        {
            foreach (ServiceProvider provider in services.Providers)
            {
                provider.Register();
            }
        }
    }
}
