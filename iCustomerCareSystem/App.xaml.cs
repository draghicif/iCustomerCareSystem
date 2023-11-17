using iCustomerCareSystem.Core;
using iCustomerCareSystem.Data;
using iCustomerCareSystem.ViewModels;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Windows;
using Unity;

namespace iCustomerCareSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = UnityConfig.RegisterComponents();
            Current.Resources["UnityContainer"] = container;
        }
    }
}
