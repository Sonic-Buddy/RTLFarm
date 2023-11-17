using RTLFarm.Views;
using RTLFarm.Views.AdminView;
using RTLFarm.Views.BuildingPage;
using RTLFarm.Views.TransportPage;
using System;
using Xamarin.Forms;

namespace RTLFarm
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(AddLoadSheetPage), typeof(AddLoadSheetPage));
            Routing.RegisterRoute(nameof(LoadSheetListPage), typeof(LoadSheetListPage));
            Routing.RegisterRoute(nameof(LoadSheetInfoPage), typeof(LoadSheetInfoPage));
            Routing.RegisterRoute(nameof(TransportListPage), typeof(TransportListPage));
            Routing.RegisterRoute(nameof(TransportInfoPage), typeof(TransportInfoPage));
            Routing.RegisterRoute(nameof(AdLoadsheetInfoPage), typeof(AdLoadsheetInfoPage));
            Routing.RegisterRoute(nameof(AdSettingPage), typeof(AdSettingPage));
        }
        //
        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
