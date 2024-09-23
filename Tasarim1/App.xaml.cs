
using ExcelToPanorama.Interface;
using System;
using System.Windows;
using WPF_LoginForm.View;


namespace Tasarim1
{
    
    /// <summary>
    /// App.xaml etkileşim mantığı
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    var serviceCollection = new ServiceCollection();
        //    ConfigureServices(serviceCollection);

            //ServiceProvider = serviceCollection.BuildServiceProvider();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            //var kolonIsterler = ServiceProvider.GetRequiredService<KolonIsterler>();
            //kolonIsterler.Show();
        }

        //private void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddTransient<ILoginView, LoginView>();
        //    services.AddTransient<KolonIsterler>();
        //}
    }
}
