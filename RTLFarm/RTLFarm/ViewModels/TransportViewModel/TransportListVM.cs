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
using System.Collections.Generic;
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
                var _harvestList = await GetHarvestList(_driverviewList.ToList());
                var _tundetialssortdate = _harvestList.OrderByDescending(a => a.DateAdded).ToList();
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
                {
                    _loading.Dispose();
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsNonet, $"No internet connection", TokenCons.IsFailed);
                    return;
                }

                var _tunheaderapiList = await _global.tunnelheader.GetapiTunHeaderGenerated();
                var _tundetailapiList = await _global.tunneldetails.GetapiTunDetailsGenerated();

                foreach (var _itmHeader in _tunheaderapiList)
                {
                    var _isExistcount = await _global.tunnelheader.GetExistloadsheet(_itmHeader);
                    if (_isExistcount == 0)
                    {
                        var _tunheaderModel = await _global.tunnelheader.Insert_TunnelHeader(_itmHeader);
                        var _subdetailsList = _tundetailapiList.Where(a => a.AndroidLoadSheet == _tunheaderModel.AndroidLoadSheet).ToList();
                        foreach (var _itmDetails in _subdetailsList)
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
                await OnAddlogs(User_Model.SalesmanCode, User_Model.UserFullName, User_Model.UserRole, "Sync", $"Successfully Sync (Header count :{_tunheaderapiList.Count()})", TokenCons.IsSuccess);
                _loading.Dispose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                await OnAddlogs(User_Model.SalesmanCode, User_Model.UserFullName, User_Model.UserRole, TokenCons.IsError, ex.Message, TokenCons.IsFailed);
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

        private async Task<List<TunnelHeader>> GetHarvestList(List<TunnelHeader> _objList)
        {
            await Task.Delay(100);
            var _returnList = _objList.GroupBy(a => a.AndroidLoadSheet).Select(b => new TunnelHeader
            {
                AGTId = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().AGTId,
                User = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().User,
                User_Checked = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().User_Checked,
                Status_Checked = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().Status_Checked,
                Plate_Number = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().Plate_Number,
                LoadDate = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().LoadDate,
                LoadSequence = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().LoadSequence,
                LoadNumber = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().LoadNumber,
                Load_Status = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().Load_Status,
                DateAdded = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().DateAdded,
                DateEdited = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().DateEdited,
                UserID = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().UserID,
                Building_Location = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().Building_Location,
                TruckDriverName = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().TruckDriverName,
                Override_Status = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().Override_Status,
                CSRefNo = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().CSRefNo,
                AndroidLoadSheet = b.Key,
                ReasonForReject = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().ReasonForReject,
                CancelledLoadSheet = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().CancelledLoadSheet,
                Remarks = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().Remarks,
                User_Code = b.Where(ba => ba.AndroidLoadSheet == b.Key).FirstOrDefault().User_Code,
            }).ToList();

            return _returnList;
        }
    }
}
