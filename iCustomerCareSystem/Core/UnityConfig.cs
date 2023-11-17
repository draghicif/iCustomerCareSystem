using iCustomerCareSystem.Data;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace iCustomerCareSystem.Core
{
    public static class UnityConfig
    {
        public static UnityContainer RegisterComponents()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyServiceClients"].ConnectionString;
            var container = new UnityContainer();
            container.RegisterType<ClientsDbContext>(
            new PerResolveLifetimeManager(),
            new InjectionConstructor(connectionString));

            return container;
        }
    }
}
