using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTLFarm.Views.BuildingPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddLoadSheetPage : ContentPage
    {
        public AddLoadSheetPage()
        {
            InitializeComponent();
                       
        }
               

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                ((Entry)sender).Text = 0.ToString();
                return;
            }

            double _;
            if (!double.TryParse(e.NewTextValue, out _))
                ((Entry)sender).Text = e.OldTextValue;
        }
    }
}