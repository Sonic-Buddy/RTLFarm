using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Helpers;
using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using RTLFarm.ViewModels.DialogViewModel;
using RTLFarm.Views;
using RTLFarm.Views.BuildingPage;
using RTLFarm.Views.DialogPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.BuildingViewModel
{
    public class LoadSheetListVM : ViewModelBase
    {

        GlobalDependencyServices _global = new GlobalDependencyServices();

        TunnelHeader _selecttunheader;
        int _squencecount;
        string _loadsheetcode, _flockmanname, _newregister, _flockmancode, _statustitle;
        bool _isrefresh;
        public bool IsRefresh { get => _isrefresh; set => SetProperty(ref _isrefresh, value); }
        public int Squence_Count { get => _squencecount; set => SetProperty(ref _squencecount, value); }
        public string LoadSheet_Code { get => _loadsheetcode; set => SetProperty(ref _loadsheetcode, value); }
        public string FLockman_Code { get => _flockmancode; set => SetProperty(ref _flockmancode, value); }
        public string FLockman_Name { get => _flockmanname; set => SetProperty(ref _flockmanname, value); }
        public string New_Register { get => _newregister; set => SetProperty(ref _newregister, value); }
        public string Status_Title { get => _statustitle; set => SetProperty(ref _statustitle, value); }

        public TunnelHeader Select_TunHeader { get => _selecttunheader; set => SetProperty(ref _selecttunheader, value); }
        public AsyncCommand RefreshCommand { get; set; }
        public AsyncCommand CreateLoadsheetCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }
        public AsyncCommand SyncCommand { get; set; }
        public AsyncCommand<TunnelHeader> SpecSyncCommand { get; set; }
        public AsyncCommand SelectCommand { get; set; }
        public ObservableRangeCollection<TunnelHeader> TunnelHeader_List { get; set; }
        public ObservableRangeCollection<StatusType_Model> Status_List { get; set; }
        public Usermaster_Model User_Model = new Usermaster_Model();

        StatusType_Model _selectstatuslist;
        public StatusType_Model SelectStatusList
        {
            get => _selectstatuslist;
            set
            {
                if (_selectstatuslist == value) { return; }
                _selectstatuslist = value;
                OnPropertyChanged();
                OnSubStatusList(value.Egg_Desc);
            }
        }
        public LoadSheetListVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);
            CreateLoadsheetCommand = new AsyncCommand(OnCreateloadsheet);
            SelectCommand = new AsyncCommand(OnSelectrow);
            SyncCommand = new AsyncCommand(OnSyncapi);
            SpecSyncCommand = new AsyncCommand<TunnelHeader>(OnSpecSyncapi);
            LogoutCommand = new AsyncCommand(OnLogout);
            TunnelHeader_List = new ObservableRangeCollection<TunnelHeader>();
            Status_List = new ObservableRangeCollection<StatusType_Model>();

            MessagingCenter.Subscribe<UpdateLSVM, string>(this, "parameterstring", async (page, e) =>
            {
                await Task.Delay(1);
                await _global.configurationService.MessageAlert(e);
            });
        }

        private async Task OnRefresh()
        {
            try
            {
                New_Register = Preferences.Get("prmtmastercode", string.Empty);
                
                User_Model = await _global.loginService.GetSpecificmodel(New_Register);
                FLockman_Code = User_Model.SalesmanCode;
                FLockman_Name = User_Model.UserFullName;
                await OnLoadDataBuilding(User_Model.SalesmanCode);                
                await OnLoadStatusList();
                IsRefresh = false;
            }
            catch (Exception ex) 
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnLoadDataBuilding(string _usercode)
        {
            Status_Title = "STATUS";
            var _tunnelheaderList = await _global.tunnelheader.GetTunheaderbyflockman(_usercode);
            var _tunheadersortdate = _tunnelheaderList.OrderByDescending(a => a.DateAdded).ToList();
            TunnelHeader_List.Clear();
            TunnelHeader_List.ReplaceRange(_tunheadersortdate);
        }
        private async Task OnSpecSyncapi(TunnelHeader _obj)
        {
            try
            {
                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                    return;

                await OnGenerateLS();

                TunnelHeader _tunnelheader = new TunnelHeader()
                {
                    User = _obj.User,
                    Plate_Number = _obj.Plate_Number,
                    DateAdded = _obj.DateAdded,
                    DateEdited = _obj.DateEdited,
                    Status_Checked = _obj.Status_Checked,
                    User_Checked = _obj.User_Checked,
                    LoadDate = _obj.LoadDate,
                    LoadNumber = LoadSheet_Code,
                    LoadSequence = Squence_Count,
                    Load_Status = TokenCons.IsTrans,
                    UserID = _obj.UserID,
                    Building_Location = _obj.Building_Location,
                    TruckDriverName = _obj.TruckDriverName,
                    Override_Status = _obj.Override_Status,
                    CSRefNo = _obj.CSRefNo,
                    AndroidLoadSheet = _obj.AndroidLoadSheet
                };

                var _Isexistapi = await _global.tunnelheader.GetapiExistloadsheet(_tunnelheader.User_Checked, _tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                if (_Isexistapi != 0)
                    return;

                var _tunheaderModel = await _global.tunnelheader.PostapiTunnelheader(_tunnelheader);
                var _tundetailsList = await _global.tunneldetails.GetTunneldetailsproduction(_tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                foreach (var _itmdetails in _tundetailsList)
                {
                    TunnelDetails _tunnedetails = new TunnelDetails()
                    {
                        LoadNumber = _tunnelheader.LoadNumber,
                        Egg_Qty = _itmdetails.Egg_Qty,
                        Egg_StatType = _itmdetails.Egg_StatType,
                        Load_Status = TokenCons.IsTrans,
                        DateCreated = _itmdetails.DateCreated,
                        DateUpdated = _itmdetails.DateUpdated,
                        Remarks = _itmdetails.Remarks,
                        Egg_Location = _itmdetails.Egg_Location,
                        Production_Date = _itmdetails.Production_Date,
                        Sequence = _itmdetails.Sequence,
                        UserSequence = _itmdetails.UserSequence,
                        AndroidLoadSheet = _tunnelheader.AndroidLoadSheet
                    };

                    await _global.tunneldetails.PostapiTunneldetails(_tunnedetails);
                }

                ParameterModel _paramModel = new ParameterModel()
                {
                    User_Code = _obj.User_Checked,
                    PMT_1 = LoadSheet_Code,
                    PMT_2 = Squence_Count.ToString(),
                    Params_Date = _obj.LoadDate
                };
                TokenSetGet.SetParamModel(_paramModel);
                await PopupNavigation.Instance.PushAsync(new UpdateLSDialog());

                //await OnUpdateproduction(_obj);
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
                DateTime _todayDate = DateTime.Now.AddDays(-2);

                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                {
                    _loading.Dispose();
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsNonet, $"No internet connection", TokenCons.IsFailed);
                    return;
                }
               
                TunnelHeader _upHeader = new TunnelHeader();

                var _tunheaderList = await _global.tunnelheader.GetapiSpecificlist(User_Model.SalesmanCode, User_Model.WhSpecificLocation);
                var _tunheadsortdate = _tunheaderList.Where(a => a.DateAdded.Date > _todayDate.Date).ToList();

                foreach (var _itmHead in _tunheadsortdate)
                {
                    if(_itmHead.Load_Status == TokenCons.IsCancel)
                    {
                        _itmHead.Load_Status = TokenCons.IsCancel;
                    }
                    else
                    {
                        _itmHead.Load_Status = TokenCons.Closed;
                    }

                    int Is_existCount = await _global.tunnelheader.GetExistls(_itmHead);
                    if(Is_existCount == 0)
                    {
                        _upHeader = await _global.tunnelheader.Insert_TunnelHeader(_itmHead);
                        await OnAddlogs(FLockman_Code, FLockman_Name, User_Model.LoginStatus, "Sync", $"Insert to android storage LS: {_itmHead.LoadNumber}", TokenCons.IsSuccess);
                    }
                    else
                    {
                        _upHeader = await _global.tunnelheader.Update_TunHeader(_itmHead);
                        await OnAddlogs(FLockman_Code, FLockman_Name, User_Model.LoginStatus, "Sync", $"Update android storage LS: {_itmHead.LoadNumber}", TokenCons.IsSuccess);
                    }

                    var _tundetailslist = await _global.tunneldetails.GetapiSpecificlist(_upHeader.AndroidLoadSheet, _upHeader.Building_Location);
                    foreach (var _itmDetails in _tundetailslist)
                    {
                        int Is_CountExist = await _global.tunneldetails.Getexistcount(_itmDetails.Production_Date, _itmDetails.Remarks, _itmDetails.AndroidLoadSheet);
                        if (Is_CountExist == 0)
                        {
                            await _global.tunneldetails.Insert_TunnelDetails(_itmDetails);
                        }
                        else
                        {
                            await _global.tunneldetails.Update_TunnelDetails(_itmDetails);
                        }
                    }

                }

                await OnAddlogs(FLockman_Code, FLockman_Name, User_Model.LoginStatus, "Sync", $"Successfully Sync", TokenCons.IsSuccess);
                _loading.Dispose();
                await _global.configurationService.MessageAlert("Successfully update");
                await OnRefresh();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                await OnAddlogs(FLockman_Code, FLockman_Name, User_Model.LoginStatus, "Sync", TokenCons.IsError + ex.Message, "Failed");
                _loading.Dispose();
                return;
            }
        }
        private async Task OnUpdateproduction(TunnelHeader _obj)
        {
            try
            {
                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                    return;

                TunnelHeader _tunnelheader = new TunnelHeader()
                {
                    User = _obj.User,
                    Plate_Number = _obj.Plate_Number,
                    DateAdded = _obj.DateAdded,
                    DateEdited = _obj.DateEdited,
                    Status_Checked = _obj.Status_Checked,
                    User_Checked = _obj.User_Checked,
                    LoadDate = _obj.LoadDate,
                    LoadNumber = LoadSheet_Code,
                    LoadSequence = Squence_Count,
                    Load_Status = TokenCons.IsTrans,
                    UserID = _obj.UserID,
                    Building_Location = _obj.Building_Location,
                    TruckDriverName = _obj.TruckDriverName,
                    Override_Status = _obj.Override_Status,
                    CSRefNo = _obj.CSRefNo,
                    AndroidLoadSheet = _obj.AndroidLoadSheet
                };

                var _Isexistapi = await _global.tunnelheader.GetExistloadsheet(_tunnelheader);
                if (_Isexistapi != 0)
                {
                    await _global.configurationService.MessageAlert("Somethings is missing");
                    return;
                }

                await _global.tunnelheader.Update_TunnelHeader(_tunnelheader);

                var _tundetailsList = await _global.tunneldetails.GetTunneldetailsproduction(_tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                foreach (var _itmdetails in _tundetailsList)
                {
                    TunnelDetails _tunnedetails = new TunnelDetails()
                    {
                        LoadNumber = _tunnelheader.LoadNumber,
                        Egg_Qty = _itmdetails.Egg_Qty,
                        Egg_StatType = _itmdetails.Egg_StatType,
                        Load_Status = TokenCons.IsTrans,
                        DateCreated = _itmdetails.DateCreated,
                        DateUpdated = _itmdetails.DateUpdated,
                        Remarks = _itmdetails.Remarks,
                        Egg_Location = _itmdetails.Egg_Location,
                        Production_Date = _itmdetails.Production_Date,
                        Sequence = _itmdetails.Sequence,
                        UserSequence = _itmdetails.UserSequence,
                        AndroidLoadSheet = _tunnelheader.AndroidLoadSheet
                    };

                    await _global.tunneldetails.Update_TunnelDetails(_tunnedetails);
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }

        }

        private async Task OnGenerateLS()
        {
            try
            {
                //Code sapag buhat ug loadsheet
                string[] returnValue = new string[2];

                var startingValue = "LS";

                var checkToday = await _global.tunnelheader.GetapiSequenceno();

                if (checkToday == 0)
                {
                    var reValue = startingValue + "0000001";
                    returnValue[0] = reValue;
                    returnValue[1] = "1";
                }
                else
                {
                    int seqNo = checkToday + 1;
                    if (seqNo >= 1 && seqNo <= 9)
                    {
                        string value = "000000";
                        var reValue = startingValue + value + seqNo;
                        returnValue[0] = reValue;
                        returnValue[1] = seqNo.ToString();
                    }
                    else if (seqNo >= 10 && seqNo <= 99)
                    {
                        string value = "00000";
                        var reValue = startingValue + value + seqNo;
                        returnValue[0] = reValue;
                        returnValue[1] = seqNo.ToString();
                    }
                    else if (seqNo >= 100 && seqNo <= 999)
                    {
                        string value = "0000";
                        var reValue = startingValue + value + seqNo;
                        returnValue[0] = reValue;
                        returnValue[1] = seqNo.ToString();
                    }
                    else if (seqNo >= 1000 && seqNo <= 9999)
                    {

                        string value = "000";
                        var reValue = startingValue + value + seqNo;
                        returnValue[0] = reValue;
                        returnValue[1] = seqNo.ToString();
                    }
                    else if (seqNo >= 10000 && seqNo <= 99999)
                    {
                        string value = "00";
                        var reValue = startingValue + value + seqNo;
                        returnValue[0] = reValue;
                        returnValue[1] = seqNo.ToString();
                    }
                    else if (seqNo >= 100000 && seqNo <= 999999)
                    {
                        string value = "0";
                        var reValue = startingValue + value + seqNo;
                        returnValue[0] = reValue;
                        returnValue[1] = seqNo.ToString();
                    }

                }

                LoadSheet_Code = returnValue[0];
                Squence_Count = Convert.ToInt32(returnValue[1]);

            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
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
                TruckDriverName = Select_TunHeader.TruckDriverName,
                Override_Status = Select_TunHeader.Override_Status,
                CSRefNo = Select_TunHeader.CSRefNo,
                AndroidLoadSheet = Select_TunHeader.AndroidLoadSheet,
                ReasonForReject = Select_TunHeader.ReasonForReject,
                CancelledLoadSheet = Select_TunHeader.CancelledLoadSheet,
                Remarks = Select_TunHeader.Remarks,
                User_Code = Select_TunHeader.User_Code
            };

            TokenSetGet.Set_Tunnel_Header(_tunheaderview);
            var route = $"/{nameof(LoadSheetInfoPage)}";
            await Shell.Current.GoToAsync(route);
        }
        private async Task OnCreateloadsheet()
        {
            await Task.Delay(100);

            //DateTime _todayDate = DateTime.Now.AddDays(-1);

            //var _testlist = await _global.logsService.Getlogsmasterlist();
            //var _sortlist = _testlist.Where(a => a.Logs_Create.Date > _todayDate.Date).ToList();

            //await _global.configurationService.MessageAlert($"{_sortlist.Count}");

            await _global.dummytundetails.DeleteAllDummy();
            await _global.dummytunheader.DeleteAllDummy();
            await OnAddlogs(FLockman_Code, FLockman_Name, User_Model.LoginStatus, TokenCons.IsSuccess, "Navigation is success", TokenCons.IsSuccess);
            var route = $"/{nameof(AddLoadSheetPage)}";
            await Shell.Current.GoToAsync(route);
        }
        private async void OnSubStatusList(string _statusDesc)
        {
            if(_statusDesc.Equals(TokenCons.IsClear))
            {
                await OnLoadDataBuilding(User_Model.SalesmanCode);
                return;
            }

            var _queryList = TunnelHeader_List.Where(a => a.Load_Status == _statusDesc).ToList();
            if(_queryList.Count.Equals(0))
            {
                await OnLoadDataBuilding(User_Model.SalesmanCode);
            }
            else
            {
                TunnelHeader_List.Clear();
                TunnelHeader_List.ReplaceRange(_queryList);
            }
        }
        private async Task OnLoadStatusList()
        {
            Status_List.Clear();
            await Task.Delay(100);
            Status_List.Add(new StatusType_Model { Id = 1, STYId = 101, Egg_Code = "", Egg_Desc = TokenCons.IsProcessing, Egg_Qty = 10, Cat_Code = "", Sequence_No = 1 });
            Status_List.Add(new StatusType_Model { Id = 2, STYId = 102, Egg_Code = "", Egg_Desc = TokenCons.Closed, Egg_Qty = 11, Cat_Code = "", Sequence_No = 2 });
            Status_List.Add(new StatusType_Model { Id = 3, STYId = 103, Egg_Code = "", Egg_Desc = TokenCons.IsCancel, Egg_Qty = 12, Cat_Code = "", Sequence_No = 3 });
            Status_List.Add(new StatusType_Model { Id = 4, STYId = 104, Egg_Code = "", Egg_Desc = "STATUS", Egg_Qty = 13, Cat_Code = "", Sequence_No = 4 });
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

            await OnAddlogs(FLockman_Code, FLockman_Name, _usermodel.LoginStatus, TokenCons.IsSuccess, "Successfully log out", "Logout");
            Preferences.Remove("prmtmastercode");

            var route = $"/{nameof(LoginPage)}";
            await Shell.Current.GoToAsync(route);
        }
    }
}
