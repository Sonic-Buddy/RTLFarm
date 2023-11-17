using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Services;
using RTLFarm.Views.AdminView;
using RTLFarm.Views.BuildingPage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.AdminVM.AdminDialogVM
{
    public class AdLoginVM : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        string _username, _password;
        bool _ispassencr, _isbtnpasshide, _isbtnpassunhide;        

        public bool IsPass_Encr { get => _ispassencr; set => SetProperty(ref _ispassencr, value); }
        public bool IsBtnpasshide { get => _isbtnpasshide; set => SetProperty(ref _isbtnpasshide, value); }
        public bool IsBtnpassunhide { get => _isbtnpassunhide; set => SetProperty(ref _isbtnpassunhide, value); }

        public string AdUsername
        {
            get => _username;
            set
            {
                if (value == _username)
                    return;
                _username = value;
                OnPropertyChanged();
            }
        }
        public string AdPassword
        {
            get => _password;
            set
            {
                if (value == _password)
                    return;
                _password = value;
                OnPropertyChanged();
            }
        }
        //
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand SigninCommand { get; }
        public AsyncCommand SetbtnHideCommand { get; }
        public AsyncCommand SetbtnUnhideCommand { get; }

        public AdLoginVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);
            SigninCommand = new AsyncCommand(OnSignin);
        }

        private async Task OnRefresh()
        {
            await Task.Delay(1000);
            IsBtnpasshide = false;            
            IsPass_Encr = true;
            IsBtnpassunhide = true;

        }

        private async Task OnSignin()
        {
            try
            {
                if(AdUsername != "it" && AdPassword != "11")
                {
                    await _global.configurationService.MessageAlert("Sorry... Username and Password is incorrect.");
                    return;
                }

                var route = $"/{nameof(AdSettingPage)}";
                await Shell.Current.GoToAsync(route);
                await PopupNavigation.Instance.PopAsync(true);
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
            }
        }
    }
}
