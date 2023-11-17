using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Models.OthersModel;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.DialogViewModel
{
    public class UpdateVM : ViewModelBase
    {
        GlobalDependencyServices global = new GlobalDependencyServices();

        string _iconName, _staticloadingText, _loadingText;
        decimal _counttotal, _countforeach;
        public string IconName { get => _iconName; set => SetProperty(ref _iconName, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public decimal CountTotal { get => _counttotal; set => SetProperty(ref _counttotal, value); }
        public decimal CountForeach { get => _countforeach; set => SetProperty(ref _countforeach, value); }
        public AsyncCommand RefreshCommand { get; }

        public UpdateVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);
        }

        private async Task OnRefresh()
        {
            IsBusy = true;
            //await OnStatusType();
            //await OnTruckmaster();
            //await OnBuildingLocation();
            //await OnUseraccount();
            await OnReason();
            await Task.Delay(110);
            await PopupNavigation.Instance.PopAsync(true);
            IsBusy = false;
        }

        private async Task OnUseraccount()
        {
            try
            {
                var _usermasterList = await global.loginService.GetapiUsermastermaster();
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
                    var _isExistcount = await global.loginService.GetExistcount(_usermodel.UserId, _usermodel.SalesmanCode);
                    if (_isExistcount == 0)
                    {
                        await global.loginService.Insertusers_local(_usermodel);
                    }
                    else
                    {
                        await global.loginService.Updateusers_local(_usermodel);
                    }
                }
            }
            catch (Exception ex)
            {
                await global.configurationService.MessageAlert(ex.Message);                
                return;
            }
        }
        private async Task OnStatusType()
        {
            try
            {
                var _statustype = await global.statustype.GetapiStatustypemaster();
                CountTotal = _statustype.Count();
                CountForeach = 0;
                foreach(var _item in _statustype)
                {

                    StatusType_Model _statustypemodel = new StatusType_Model()
                    {
                        Id = _item.Id,
                        STYId = _item.STYId,
                        Egg_Code = _item.Egg_Code.ToUpperInvariant(),
                        Egg_Desc = _item.Egg_Desc.ToUpperInvariant(),
                        Egg_Qty = _item.Egg_Qty,
                    };
                    var _isExistcount = await global.statustype.Getexistcount(_statustypemodel.STYId);
                    if(_isExistcount == 0)
                    {
                        await global.statustype.Insert_Statustype(_statustypemodel);
                    }
                    else
                    {
                        await global.statustype.Update_Statustype(_statustypemodel);
                    }
                }
            }
            catch (Exception ex)
            {
                await global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnTruckmaster()
        {
            try
            {
                var _statustype = await global.truckmaster.GetapiTruckmaster();
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
                    var _isExistcount = await global.truckmaster.Getexistcount(_item.TruckPlateNo);
                    if (_isExistcount == 0)
                    {
                        await global.truckmaster.Insert_Truck(_truckmodel);
                    }
                    else
                    {
                        await global.truckmaster.Update_Truck(_truckmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                await global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnBuildingLocation()
        {
            try
            {
                var _buildinglocList = await global.buildinglocation.GetapiBuildingLocmaster();
                CountTotal = _buildinglocList.Count();
                CountForeach = 0;
                foreach (var _item in _buildinglocList)
                {
                    var _isExistcount = await global.buildinglocation.Getexistcount(_item.AGTLId);
                    if (_isExistcount == 0)
                    {
                        await global.buildinglocation.Insert_BuildingLoc(_item);
                    }
                    else
                    {
                        await global.buildinglocation.Update_BuildingLoc(_item);
                    }
                }
            }
            catch (Exception ex)
            {
                await global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }
        private async Task OnReason()
        {
            try
            {
                var _reasonlist = await global.reasons.GetapiReasonmaster();
                CountTotal = _reasonlist.Count();
                CountForeach = 0;
                foreach (var _item in _reasonlist)
                {
                    var _isExistcount = await global.reasons.Getexistcount(_item.ReasonCodeType);
                    if (_isExistcount == 0)
                    {
                        await global.reasons.Insert_Reason(_item);
                    }
                    else
                    {
                        await global.reasons.Update_Reason(_item);
                    }
                }
            }
            catch (Exception ex)
            {
                await global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }
        public async Task<string> GetPercentage(decimal[] decimalarray)
        {
            await Task.Delay(1);
            decimal e = decimalarray[0] / decimalarray[1];
            decimal f = e * 100;
            decimal g = decimal.Round(f, 2, MidpointRounding.AwayFromZero);
            g = Math.Round(g, 0);
            var msg = g + "%";
            return msg;
        }
    }
}
