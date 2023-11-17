using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using RTLFarm.Helpers;
using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using RTLFarm.Views;
using RTLFarm.Views.TransportPage;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.TransportViewModel
{
    public class TransportListVM : ViewModelBase
    {

        GlobalDependencyServices _global = new GlobalDependencyServices();

        string _drivercode, _drivername, _newregister;
        bool _isrefresh;
        TunnelHeader _selecttunheader;
        public string Driver_Code { get => _drivercode; set => SetProperty(ref _drivercode, value); }
        public string Driver_Name { get => _drivername; set => SetProperty(ref _drivername, value); }
        public string New_Register { get => _newregister; set => SetProperty(ref _newregister, value); }
        public bool IsRefresh { get => _isrefresh; set => SetProperty(ref _isrefresh, value); }
        public TunnelHeader Select_TunHeader { get => _selecttunheader; set => SetProperty(ref _selecttunheader, value); }
        public AsyncCommand RefreshCommand { get; set; }
        public AsyncCommand SelectCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        public AsyncCommand SyncCommand { get; set; }

        public ObservableRangeCollection<TunnelHeader> TunnelHeader_List { get; set; }
        public Usermaster_Model User_Model = new Usermaster_Model();

        public TransportListVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);
            SelectCommand = new AsyncCommand(OnSelectrow);
            SyncCommand = new AsyncCommand(OnSyncapi);
            LogoutCommand = new AsyncCommand(OnLogout);

            TunnelHeader_List = new ObservableRangeCollection<TunnelHeader>();
        }

        private async Task OnRefresh()
        {
            try
            {
                New_Register = Preferences.Get("prmtmastercode", string.Empty);

                User_Model = await _global.loginService.GetSpecificmodel(New_Register);
                Driver_Code = User_Model.SalesmanCode;
                Driver_Name = User_Model.UserFullName;
                await OnLoadData(User_Model.SalesmanCode);

                IsRefresh = false;
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnLoadData(string _drivercode)
        {
            try
            {
                TunnelHeader _tunheaderparam = new TunnelHeader()
                {
                    User_Checked = _drivercode
                };
                var _driverviewList = await _global.tunnelheader.GetSpecifictunheader(_tunheaderparam, "OnDriverView");
                var _tundetialssortdate = _driverviewList.OrderByDescending(a => a.LoadDate).ToList();
                TunnelHeader_List.Clear();
                TunnelHeader_List.ReplaceRange(_tundetialssortdate);
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnSyncapi()
        {
            var _loading = UserDialogs.Instance.Loading("Updating Data. . .");
            _loading.Show();
            try
            {
                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                    return;

                var _tunheaderapiList = await _global.tunnelheader.GetapiTunHeaderGenerated();
                var _tundetailapiList = await _global.tunneldetails.GetapiTunDetailsGenerated();
                foreach(var _itmHeader in _tunheaderapiList)
                {
                    var _isExistcount = await _global.tunnelheader.GetExistloadsheet(_itmHeader);
                    if(_isExistcount == 0)
                    {
                        var _tunheaderModel = await _global.tunnelheader.Insert_TunnelHeader(_itmHeader);
                        var _subdetailsList = _tundetailapiList.Where(a => a.AndroidLoadSheet == _tunheaderModel.AndroidLoadSheet).ToList();
                        foreach(var _itmDetails in _subdetailsList)
                        {
                            await _global.tunneldetails.Insert_TunnelDetails(_itmDetails);
                        }
                    }
                    else
                    {
                        var _tunheaderModel = await _global.tunnelheader.Update_TunHeader(_itmHeader);
                        var _subdetailsList = _tundetailapiList.Where(a => a.AndroidLoadSheet == _tunheaderModel.AndroidLoadSheet).ToList();
                        foreach (var _itmDetails in _subdetailsList)
                        {
                            await _global.tunneldetails.Update_TunnelDetails(_itmDetails);
                        }
                    }
                }

                await Task.Delay(2000);

                await OnLoadData(User_Model.SalesmanCode);
                _loading.Dispose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                _loading.Dispose();
                return;
            }
        }

        private async Task OnSelectrow()
        {

            if (Select_TunHeader == null)
                return;

            TunHeaderView _tunheaderview = new TunHeaderView()
            {
                AGTId = Select_TunHeader.AGTId,
                User = Select_TunHeader.User,
                Plate_Number = Select_TunHeader.Plate_Number,
                DateAdded = Select_TunHeader.DateAdded,
                DateEdited = Select_TunHeader.DateEdited,
                Status_Checked = Select_TunHeader.Status_Checked,
                User_Checked = Select_TunHeader.User_Checked,
                LoadDate = Select_TunHeader.LoadDate,
                LoadNumber = Select_TunHeader.LoadNumber,
                LoadSequence = Select_TunHeader.LoadSequence,
                Load_Status = Select_TunHeader.Load_Status,
                UserID = Select_TunHeader.UserID,
                Building_Location = Select_TunHeader.Building_Location,
                TruckDriverName = User_Model.SalesmanCode,
                Override_Status = Select_TunHeader.Override_Status,
                CSRefNo = Select_TunHeader.CSRefNo,
                AndroidLoadSheet = Select_TunHeader.AndroidLoadSheet,
                ReasonForReject = Select_TunHeader.ReasonForReject,
                CancelledLoadSheet = Select_TunHeader.CancelledLoadSheet,
                Remarks = Select_TunHeader.Remarks,
                User_Code = Select_TunHeader.User_Code
            };

            TokenSetGet.Set_Tunnel_Header(_tunheaderview);
            var route = $"/{nameof(TransportInfoPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task OnLogout()
        {
            var _item = await _global.loginService.GetSpecificmodel(New_Register);
            Usermaster_Model _usermodel = new Usermaster_Model()
            {
                UserId = _item.UserId,
                UserFullName = _item.UserFullName.ToUpperInvariant(),
                UserName = _item.UserName,
                Password = _item.Password,
                UserRole = _item.UserRole,
                UserDeptId = _item.UserDeptId,
                WarehouseAssignedId = _item.WarehouseAssignedId,
                UserStatus = _item.UserStatus,
                PasswordHash = _item.PasswordHash,
                PasswordSalt = _item.PasswordSalt,
                LoginStatus = TokenCons.IsInactive,
                SalesmanCode = _item.SalesmanCode,
                WhSpecificLocation = _item.WhSpecificLocation,
                CreateStatus = _item.CreateStatus
            };
            await _global.loginService.UpdateAcc_local(_usermodel);

            var route = $"/{nameof(LoginPage)}";
            await Shell.Current.GoToAsync(route);
        }
    }
}
