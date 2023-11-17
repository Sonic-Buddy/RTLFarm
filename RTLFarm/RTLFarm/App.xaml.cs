using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm
{
    public partial class App : Application
    {
        ConfigurationClass _configureClass = new ConfigurationClass();

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            await Task.Delay(500);
            //await _configureClass.SetDefaultipaddress();
            //await _configureClass.SetIpaddress();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
