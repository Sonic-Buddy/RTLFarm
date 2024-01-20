using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Helpers;
using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using RTLFarm.Views.AdminView.AdminDialog;
using RTLFarm.Views.BuildingPage;
using RTLFarm.Views.DialogPage;
using RTLFarm.Views.TransportPage;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.UserViewModel
{
    public class LoginVM : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        string username, password, devicecode, salesmancode, datestring, roleacc, _accsite, _verify, _fullname;
        bool cbox, _enable, _passhide, _btnhide, _btnunhide, _usernameread, _isreged, _isnotreged;
        int countS;
        public string Username
        {
            get => username;
            set
            {
                if (value == username)
                    return;
                username = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get => password;
            set
            {
                if (value == password)
                    return;
                password = value;
                OnPropertyChanged();
            }
        }

        public string Userfullname { get => _fullname; set => SetProperty(ref _fullname, value); }
        public string Verification { get => _verify; set => SetProperty(ref _verify, value); }
        public int CountAccount { get => countS; set => SetProperty(ref countS, value); }
        public string DeviceSerial { get => devicecode; set => SetProperty(ref devicecode, value); }
        public bool IsPasshide { get => _passhide; set => SetProperty(ref _passhide, value); }
        public bool IsBtnpasshide { get => _btnhide; set => SetProperty(ref _btnhide, value); }
        public bool IsBtnpassunhide { get => _btnunhide; set => SetProperty(ref _btnunhide, value); }
        public bool IsReadusername { get => _usernameread; set => SetProperty(ref _usernameread, value); }
        public bool IsRegistered { get => _isreged; set => SetProperty(ref _isreged, value); }
        public bool IsNotRegistered { get => _isnotreged; set => SetProperty(ref _isnotreged, value); }
        public string RoleAcc { get => roleacc; set => SetProperty(ref roleacc, value); }
        public string AccSite { get => _accsite; set => SetProperty(ref _accsite, value); }
        public string Stats = "login";
        public string SalesmanCode { get => salesmancode; set => SetProperty(ref salesmancode, value); }
        public string DateString { get => datestring; set => SetProperty(ref datestring, value); }
        public bool IsEnable { get => _enable; set => SetProperty(ref _enable, value); }
        public bool CheckBoxC
        {
            get => cbox;
            set
            {
                if (cbox == value)
                {
                    return;
                }
                cbox = value;
                //Total = total + value;
                OnPropertyChanged();
            }
        }

        public static bool IsRegister { get; set; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand LoginCommand { get; }
        public AsyncCommand AdminCommand { get; }
        public AsyncCommand RegisterCommand { get; }
        public AsyncCommand BtnTestCommand { get; }
        public AsyncCommand BtnDeleteCommand { get; }
        public AsyncCommand SetbtnHideCommand { get; }
        public AsyncCommand SetbtnUnhideCommand { get; }
        public AsyncCommand OtherAccCommand { get; }
        public Usermaster_Model UserData = new Usermaster_Model();
        public ObservableRangeCollection<Usermaster_Model> AccountList { get; set; }
        public LoginVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);
            AdminCommand = new AsyncCommand(OnAdminPage);
            RegisterCommand = new AsyncCommand(OnRegister);
            LoginCommand = new AsyncCommand(OnLoginCommand);
            BtnTestCommand = new AsyncCommand(OnTestDialog);
            SetbtnHideCommand = new AsyncCommand(OnSethide);
            SetbtnUnhideCommand = new AsyncCommand(OnSetunhide);
            BtnDeleteCommand = new AsyncCommand(OnDeleteData);
            OtherAccCommand = new AsyncCommand(OnOtheracc);
            AccountList = new ObservableRangeCollection<Usermaster_Model>();
        }
        private async Task OnRefresh()
        {
            try
            {
                IsBusy = true;
                IsBtnpasshide = false;
                IsEnable = true;
                IsPasshide = true;
                IsBtnpassunhide = true;
                IsReadusername = false;

                //Username = "proi113";
                //Username = "user101";
                //Password = "1234";

                IsRegister = Preferences.Get("iSregistered", false);

                if (IsRegister == true)
                {
                    IsRegistered = true;
                    IsNotRegistered = false;
                    IsReadusername = true;
                }
                else
                {
                    IsNotRegistered = true;
                    IsRegistered = false;
                }

                TokenSetGet.SetParamModel(null);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
            }

        }

        private async Task OnLoginCommand()
        {
            try
            {
                IsEnable = false;
                var _userModel = await _global.loginService.GetSpecificAccount(Username ,Password);

                TokenSetGet.Set_UserModel(_userModel);
                if (_userModel == null)
                {
                    await _global.configurationService.MessageAlert("User is not exist");
                    await OnAddlogs(string.Empty, string.Empty, $"Username: {Username}, Password: {Username}", TokenCons.IsError, "User is not exist", "null");
                    IsEnable = true;
                    return;
                }

                if (_userModel.UserRole == "TPicker")
                {
                    Preferences.Set("prmtmastercode", _userModel.SalesmanCode);
                    await OnAddlogs(_userModel.SalesmanCode, _userModel.UserFullName, _userModel.UserRole, TokenCons.IsSuccess, "Successfully login", TokenCons.IsActive);
                    await OnLoadSheet();
                }
                else if (_userModel.UserRole == "FDriver")
                {
                    Preferences.Set("prmtmastercode", _userModel.SalesmanCode);
                    await OnAddlogs(_userModel.SalesmanCode, _userModel.UserFullName, _userModel.UserRole, TokenCons.IsSuccess, "Successfully login", TokenCons.IsActive);
                    await OnDriveruser();
                }
                else
                {
                    await _global.configurationService.MessageAlert("Un-Authorize user");
                    await OnAddlogs(_userModel.SalesmanCode, _userModel.UserFullName, _userModel.UserRole, TokenCons.IsError, "Un-Authorize user", "Failed");
                    IsEnable = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                await OnAddlogs(string.Empty, string.Empty, $"Username: {Username}, Password: {Username}", TokenCons.IsError, ex.Message, "Failed");
                Username = String.Empty;
                Password = String.Empty;
                IsEnable = true;
            }
        }

        private async Task OnOtheracc()
        {
            await Task.Delay(100);
            IsRegistered = false;
            IsNotRegistered = true;
        }
        private async Task OnAdminPage()
        {            
            await PopupNavigation.Instance.PushAsync(new AdLoginDialog());
        }
        private async Task OnRegister()
        {
            var _loading = UserDialogs.Instance.Loading("Loading. . .");
            _loading.Show();

            try
            {
                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                {
                    await OnAddlogs(string.Empty, string.Empty, string.Empty, TokenCons.IsError, "Please, check your internet connection", "Failed");
                    _loading.Dispose();
                    return;
                }
                    

                if (string.IsNullOrEmpty(Verification) || string.IsNullOrWhiteSpace(Verification))
                {
                    await _global.configurationService.MessageAlert("Please enter verification code");
                    await OnAddlogs(string.Empty, string.Empty, string.Empty, TokenCons.IsError, "Please enter verification code", TokenCons.IsActive);
                    _loading.Dispose();
                    return;
                }

                var _verifyCount = await _global.loginService.GetVerifyuser(Verification);
                if (_verifyCount != 0)
                {
                    ParameterModel _paramModel = new ParameterModel()
                    {
                        User_Code = Verification
                    };

                    TokenSetGet.SetParamModel(_paramModel);
                    _loading.Dispose();
                    await OnAddlogs(_paramModel.User_Code, "Unknow", Verification, TokenCons.IsSuccess, "Verification code is Correct", TokenCons.IsActive);
                    await PopupNavigation.Instance.PushAsync(new RegisterAccountDialog());
                }
                else
                {
                    await _global.configurationService.MessageAlert("Verification code is wrong");
                    await OnAddlogs(string.Empty, string.Empty, Verification, TokenCons.IsError, "Verification code is wrong", "Failed");
                    _loading.Dispose();
                    return;
                }

                _loading.Dispose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                await OnAddlogs(string.Empty, string.Empty, Verification, TokenCons.IsError, ex.Message, "Failed");
                IsEnable = false;
                _loading.Dispose();
                return;
            }
        }

        private async Task OnDeleteData()
        {
            try
            {
                await _global.tunneldetails.DeleteAllTunDetails();
                await _global.tunnelheader.DeleteAllTunHeader();
                await _global.dummytundetails.DeleteAllDummy(); 
                await _global.dummytunheader.DeleteAllDummy();

                await OnAddlogs(string.Empty, string.Empty, string.Empty, TokenCons.IsSuccess, "Successfully Deleted", "Deleted");
                await _global.configurationService.MessageAlert("Successfully Deleted");
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                await OnAddlogs(string.Empty, string.Empty, string.Empty, TokenCons.IsError, ex.Message, "Failed");
                return;
            }
        }

        
        private async Task OnTestDialog()
        {
            await PopupNavigation.Instance.PushAsync(new UpdateDialog());
        }
        private async Task OnLoadSheet()
        {
            await Task.Delay(100);
            var route = $"/{nameof(LoadSheetListPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task OnDriveruser()
        {
            await Task.Delay(100);
            var route = $"/{nameof(TransportListPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task OnSethide()
        {
            IsBtnpasshide = false;
            IsBtnpassunhide = true;
            IsPasshide = true;
            await Task.Delay(50);
        }

        private async Task OnSetunhide()
        {
            IsBtnpassunhide = false;
            IsPasshide = false;
            IsBtnpasshide = true;
            await Task.Delay(50);
        }       

    }
}
