using iCustomerCareSystem.Data;
using System.Configuration;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace iCustomerCareSystem.Core
{
    public static class UnityConfig
    {
        public static UnityContainer RegisterComponents()
        {

            var container = new UnityContainer();
            container.RegisterType<ClientsDbContext>();

            return container;
        }
    }
}
