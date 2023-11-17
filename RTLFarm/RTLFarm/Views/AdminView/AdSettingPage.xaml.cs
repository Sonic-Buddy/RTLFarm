using RTLFarm.ViewModels.AdminVM;
using RTLFarm.ViewModels.BuildingViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTLFarm.Views.AdminView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdSettingPage : ContentPage
	{
		public AdSettingPage ()
		{
			InitializeComponent ();

            BindingContext = new AdSettingViewModel();
        }

        private void searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var _searchkey = e.NewTextValue.ToUpperInvariant();
            var _listcontainer = BindingContext as AdSettingViewModel;

            if (string.IsNullOrWhiteSpace(_searchkey))
            {
                LoadsheetList.ItemsSource = _listcontainer.TunnelHeader_List;
            }
            else
            {
                LoadsheetList.ItemsSource = _listcontainer.TunnelHeader_List.Where(sh => sh.LoadNumber.Contains(_searchkey) || sh.AndroidLoadSheet.Contains(_searchkey));
            }
        }
    }
}