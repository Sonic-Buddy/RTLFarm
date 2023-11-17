using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Helpers;
using RTLFarm.Models.OthersModel;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using RTLFarm.Views;
using RTLFarm.Views.BuildingPage;
using RTLFarm.Views.DialogPage;
using RTLFarm.Views.TransportPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.DialogViewModel
{
    public class RegisterAccountVM : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        bool _showregister, _passhide, _btnhide, _btnunhide, _showrecover, _enable, _iseditacc, _iseditcancel;
        string _username, _password, _copass;
        decimal _counttotal, _countforeach;
        Usermaster_Model _usermodel;
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string CoPassword { get => _copass; set => SetProperty(ref _copass, value); }
        public bool Show_Register { get => _showregister; set => SetProperty(ref _showregister, value); }
        public bool Show_Recover { get => _showrecover; set => SetProperty(ref _showrecover, value); }
        public bool IsPasshide { get => _passhide; set => SetProperty(ref _passhide, value); }
        public bool IsBtnpasshide { get => _btnhide; set => SetProperty(ref _btnhide, value); }
        public bool IsBtnpassunhide { get => _btnunhide; set => SetProperty(ref _btnunhide, value); }
        public bool IsEnable { get => _enable; set => SetProperty(ref _enable, value); }
        public bool IsEdit_Acc { get => _iseditacc; set => SetProperty(ref _iseditacc, value); }
        public bool IsEdit_Cancel { get => _iseditcancel; set => SetProperty(ref _iseditcancel, value); }
        public decimal CountTotal { get => _counttotal; set => SetProperty(ref _counttotal, value); }
        public decimal CountForeach { get => _countforeach; set => SetProperty(ref _countforeach, value); }

        public Usermaster_Model User_Model { get => _usermodel; set => SetProperty(ref _usermodel, value); }

        public AsyncCommand RegisterCommand { get; }
        public AsyncCommand SaveAccountCommand { get; }
        public AsyncCommand EditAccountCommand { get; }
        public AsyncCommand CancelAccountCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand SetbtnHide { get; }
        public AsyncCommand SetbtnUnhide { get; }

        public RegisterAccountVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);

            SetbtnHide = new AsyncCommand(OnSethide);
            SetbtnUnhide = new AsyncCommand(OnSetunhide);
            EditAccountCommand = new AsyncCommand(OnEditaccount);
            CancelAccountCommand = new AsyncCommand(OnCancel);
            SaveAccountCommand = new AsyncCommand(OnSaveaccount);
        }

        private async Task OnRefresh()
        {
            try
            {
                IsBtnpasshide = false;
                IsEnable = true;
                IsPasshide = true;
                IsBtnpassunhide = true;
                IsEdit_Acc = true;
                IsEdit_Cancel = false;

                var _userModel = await _global.loginService.Getapiusermodel(TokenSetGet.GetParamModel().User_Code);

                // ==> parameter sa user account

                await OnQueryData(_userModel);
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
           
        }

        private async Task OnQueryData(Usermaster_Model _obj)
        {
            await Task.Delay(10);
            if (string.IsNullOrEmpty(_obj.UserName) || string.IsNullOrWhiteSpace(_obj.UserName) && string.IsNullOrEmpty(_obj.Password) || string.IsNullOrWhiteSpace(_obj.Password))
            {

                Show_Register = true;
                Show_Recover = false;
                await OnSethide();
            }
            else
            {
                User_Model = _obj;
                Show_Register = false;
                Show_Recover = true;
            }
        }


        private async Task OnRegisteracc()
        {
            await Task.Delay(10);
            
        }

        private async Task OnSaveaccount()
        {
            await Task.Delay(10);
            try
            {
                if(IsEdit_Acc == true)
                {
                    Usermaster_Model _usermodel = new Usermaster_Model()
                    {
                        UserId = User_Model.UserId,
                        UserFullName = User_Model.UserFullName.ToUpperInvariant(),
                        UserName = User_Model.UserName,
                        Password = User_Model.Password,
                        UserRole = User_Model.UserRole,
                        UserDeptId = User_Model.UserDeptId,
                        WarehouseAssignedId = User_Model.WarehouseAssignedId,
                        UserStatus = User_Model.UserStatus,
                        PasswordHash = User_Model.PasswordHash,
                        PasswordSalt = User_Model.PasswordSalt,
                        LoginStatus = TokenCons.IsActive,
                        SalesmanCode = User_Model.SalesmanCode,
                        WhSpecificLocation = User_Model.WhSpecificLocation,
                        CreateStatus = User_Model.CreateStatus
                    };

                    Preferences.Set("prmtmastercode", _usermodel.SalesmanCode);
                    await _global.loginService.Insertusers_local(_usermodel);
                }

                if (IsEdit_Cancel == true)
                {
                    if (User_Model.Password == CoPassword)
                    {
                        Usermaster_Model _usermodel = new Usermaster_Model()
                        {
                            UserId = User_Model.UserId,
                            UserFullName = User_Model.UserFullName.ToUpperInvariant(),
                            UserName = User_Model.UserName,
                            Password = CoPassword,
                            UserRole = User_Model.UserRole,
                            UserDeptId = User_Model.UserDeptId,
                            WarehouseAssignedId = User_Model.WarehouseAssignedId,
                            UserStatus = User_Model.UserStatus,
                            PasswordHash = User_Model.PasswordHash,
                            PasswordSalt = User_Model.PasswordSalt,
                            LoginStatus = TokenCons.IsActive,
                            SalesmanCode = User_Model.SalesmanCode,
                            WhSpecificLocation = User_Model.WhSpecificLocation,
                            CreateStatus = User_Model.CreateStatus
                        };
                        Preferences.Set("prmtmastercode", _usermodel.SalesmanCode);
                        await _global.loginService.Insertusers_local(_usermodel);
                    }
                    else
                    {
                        await _global.configurationService.MessageAlert("Your Password is not match");
                        return;
                    }
                }

                await OnDownloadmasterdata();
                await _global.configurationService.MessageAlert("Successfully Save");
                await OnClosed(User_Model.UserRole);
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnEditaccount()
        {
            await Task.Delay(10);
            IsEdit_Acc = false;
            IsEdit_Cancel = true;
        }
        private async Task OnCancel()
        {
            await Task.Delay(10);
            IsEdit_Acc = true;
            IsEdit_Cancel = false;
        }
        private async Task OnSethide()
        {
            await Task.Delay(10);
            IsBtnpasshide = false;
            IsBtnpassunhide = true;
            IsPasshide = true;
        }

        private async Task OnSetunhide()
        {
            await Task.Delay(10);
            IsBtnpassunhide = false;
            IsPasshide = false;
            IsBtnpasshide = true;
        }

        private async Task OnDownloadmasterdata()
        {
            await OnStatusType();
            await OnTruckmaster();
            await OnBuildingLocation();
            await OnUseraccount();
            await OnReason();
        }
        private async Task OnStatusType()
        {
            try
            {
                var _statustype = await _global.statustype.GetapiStatustypemaster();
                CountTotal = _statustype.Count();
                CountForeach = 0;
                foreach (var _item in _statustype)
                {
                    StatusType_Model _statustypemodel = new StatusType_Model()
                    {
                        Id = _item.Id,
                        STYId = _item.STYId,
                        Egg_Code = _item.Egg_Code.ToUpperInvariant(),
                        Egg_Desc = _item.Egg_Desc.ToUpperInvariant(),
                        Egg_Qty = _item.Egg_Qty,
                        Cat_Code = _item.Cat_Code,
                        Sequence_No = _item.Sequence_No
                    };
                    var _isExistcount = await _global.statustype.Getexistcount(_statustypemodel.STYId);
                    if (_isExistcount == 0)
                    {
                        await _global.statustype.Insert_Statustype(_statustypemodel);
                    }
                    else
                    {
                        await _global.statustype.Update_Statustype(_statustypemodel);
                    }
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }
        private async Task OnTruckmaster()
        {
            try
            {
                var _statustype = await _global.truckmaster.GetapiTruckmaster();
                CountTotal = _statustype.Count();
                CountForeach = 0;
                foreach (var _item in _statustype)
                {

                    TruckMaster_Model _truckmodel = new TruckMaster_Model()
                    {
                        TruckPlateNo = _item.TruckPlateNo.ToUpperInvariant(),
                        DateCreated = _item.DateCreated,
                        DateUpdate = _item.DateUpdate,
                        Actual = _item.Actual,
                        Crates = _item.Crates,
                        Trays = _item.Trays
                    };
                    var _isExistcount = await _global.truckmaster.Getexistcount(_item.TruckPlateNo);
                    if (_isExistcount == 0)
                    {
                        await _global.truckmaster.Insert_Truck(_truckmodel);
                    }
                    else
                    {
                        await _global.truckmaster.Update_Truck(_truckmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }
        private async Task OnBuildingLocation()
        {
            try
            {
                var _buildinglocList = await _global.buildinglocation.GetapiBuildingLocmaster();
                CountTotal = _buildinglocList.Count();
                CountForeach = 0;
                foreach (var _item in _buildinglocList)
                {
                    var _isExistcount = await _global.buildinglocation.Getexistcount(_item.AGTLId);
                    if (_isExistcount == 0)
                    {
                        await _global.buildinglocation.Insert_BuildingLoc(_item);
                    }
                    else
                    {
                        await _global.buildinglocation.Update_BuildingLoc(_item);
                    }
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnReason()
        {
            try
            {
                var _reasonlist = await _global.reasons.GetapiReasonmaster();
                CountTotal = _reasonlist.Count();
                CountForeach = 0;
                foreach (var _item in _reasonlist)
                {
                    var _isExistcount = await _global.reasons.Getexistcount(_item.ReasonCodeType);
                    if (_isExistcount == 0)
                    {
                        await _global.reasons.Insert_Reason(_item);
                    }
                    else
                    {
                        await _global.reasons.Update_Reason(_item);
                    }
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }
        private async Task OnUseraccount()
        {
            try
            {
                var _usermasterList = await _global.loginService.GetapiUsermastermaster();
                CountTotal = _usermasterList.Count();
                CountForeach = 0;
                foreach (var _item in _usermasterList)
                {
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
                        LoginStatus = _item.LoginStatus,
                        SalesmanCode = _item.SalesmanCode,
                        WhSpecificLocation = _item.WhSpecificLocation,
                        CreateStatus = _item.CreateStatus
                    };
                    var _isExistcount = await _global.loginService.GetExistcount(_usermodel.UserId, _usermodel.SalesmanCode);
                    if (_isExistcount == 0)
                    {
                        await _global.loginService.Insertusers_local(_usermodel);
                    }
                    else
                    {                        
                        await _global.loginService.UpdateAcc_local(_usermodel);
                    }
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }
        private async Task OnClosed(string _roles)
        {
            Preferences.Set("iSregistered", true);
            if (_roles == "TPicker")
            {
                var route = $"/{nameof(LoadSheetListPage)}";
                await Shell.Current.GoToAsync(route);
                await PopupNavigation.Instance.PopAsync(true);
            }
            else if (_roles == "FDriver")
            {
                var route = $"/{nameof(TransportListPage)}";
                await Shell.Current.GoToAsync(route);
                await PopupNavigation.Instance.PopAsync(true);
            }
            
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
