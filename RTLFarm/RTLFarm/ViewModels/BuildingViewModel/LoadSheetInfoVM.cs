using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Helpers;
using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Models.OthersModel;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services;
using RTLFarm.Views.DialogPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.BuildingViewModel
{
    public class LoadSheetInfoVM : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        bool _isrefresh, _ishidebtnsync, _isunhidebtnsync, _isclosestatus, _isselecttrack, _ishidereason, _ishideeditbtn;
        double _grandtotalegg;
        int _squencecount;
        string _loadsheetcode, _platenumber, _buildinglocation, _remarkseditor, _reasontype, _parameterref, _btnsaveedit;

        TunnelHeader _tunnelheadermodel;
        TunHeaderView _tunheader;
        public bool IsRefresh { get => _isrefresh; set => SetProperty(ref _isrefresh, value); }
        public double GrandTotal_Egg { get => _grandtotalegg; set => SetProperty(ref _grandtotalegg, value); }
        public bool IsHidebtnSync { get => _ishidebtnsync; set => SetProperty(ref _ishidebtnsync, value); }
        public bool IsUnHidebtnSync { get => _isunhidebtnsync; set => SetProperty(ref _isunhidebtnsync, value); }
        public bool IsClose_Status { get => _isclosestatus; set => SetProperty(ref _isclosestatus, value); }
        public bool Is_SelectTrack { get => _isselecttrack; set => SetProperty(ref _isselecttrack, value); }
        public bool Is_Hidereason { get => _ishidereason; set => SetProperty(ref _ishidereason, value); }
        public bool Is_Hideeditbtn { get => _ishideeditbtn; set => SetProperty(ref _ishideeditbtn, value); }
        public int Squence_Count { get => _squencecount; set => SetProperty(ref _squencecount, value); }
        public string LoadSheet_Code { get => _loadsheetcode; set => SetProperty(ref _loadsheetcode, value); }
        public string Plate_Number { get => _platenumber; set => SetProperty(ref _platenumber, value); }
        public string Building_Location { get => _buildinglocation; set => SetProperty(ref _buildinglocation, value); }
        public string Parameter_Ref { get => _parameterref; set => SetProperty(ref _parameterref, value); }
        public string Remarks_Editor { get => _remarkseditor; set => SetProperty(ref _remarkseditor, value); }
        public string ReasonType { get => _reasontype; set => SetProperty(ref _reasontype, value); }
        public string Btn_SaveEdit { get => _btnsaveedit; set => SetProperty(ref _btnsaveedit, value); }
        public TunnelHeader TunnelHeader_Model { get => _tunnelheadermodel; set => SetProperty(ref _tunnelheadermodel, value); }
        public TunHeaderView TunHeader { get => _tunheader; set => SetProperty(ref _tunheader, value); }

        TruckMaster_Model _truckmaster;
        public TruckMaster_Model SelectTruckMaster
        {
            get => _truckmaster;
            set
            {
                if (_truckmaster == value) { return; }
                _truckmaster = value;
                OnPropertyChanged();
                Plate_Number = value.TruckPlateNo;

                if(string.IsNullOrEmpty(Plate_Number) || string.IsNullOrWhiteSpace(Plate_Number))
                {
                    Is_SelectTrack = false;
                }
                else
                {
                    Is_SelectTrack = true;
                }
            }
        }

        ReasonModel _selectreason;
        public ReasonModel SelectReason
        {
            get => _selectreason;
            set
            {
                if (_selectreason == value) { return; }
                _selectreason = value;
                OnPropertyChanged();
                ReasonType = value.ReasonCodeType;
                //OnSubStatustype(value.Egg_Code);
            }
        }

        public AsyncCommand RefreshCommand { get; set; }
        public AsyncCommand SyncloadsheetCommand { get; set; }
        public AsyncCommand ClosedCommand { get; set; }
        public AsyncCommand EditCommand { get; set; }

        public ObservableRangeCollection<ReasonModel> Reason_List { get; set; }
        public ObservableRangeCollection<TunnelDetails> TunnelDetails_List { get; set; }
        public ObservableRangeCollection<TruckMaster_Model> TruckMaster_List { get; set; }
        //public TunnelHeader TunHeader = new TunnelHeader();

        public LoadSheetInfoVM()
        {
            TunnelDetails_List = new ObservableRangeCollection<TunnelDetails>();
            TruckMaster_List = new ObservableRangeCollection<TruckMaster_Model>();
            Reason_List = new ObservableRangeCollection<ReasonModel>();

            RefreshCommand = new AsyncCommand(OnRefresh);
            ClosedCommand = new AsyncCommand(OnClose);
            EditCommand = new AsyncCommand(OnEditLoadsheet);
            SyncloadsheetCommand = new AsyncCommand(OnSpecSyncapi);
        }


        private async Task OnRefresh()
        {
            try
            {
                IsRefresh = true;
                Btn_SaveEdit = "SYNC LOADSHEET";
                IsHidebtnSync = false;
                IsUnHidebtnSync = false;                
                Is_Hidereason = false;
                TunHeader = TokenSetGet.Get_Tunnel_Header();
                Building_Location = await _global.buildinglocation.GetBuildingLocation(TunHeader.Building_Location, "Description");
                Plate_Number = "SELECT TRUCK";
                IsClose_Status = true;
                if (TunHeader.Load_Status == TokenCons.Closed || TunHeader.Load_Status == TokenCons.IsCancel)
                {
                    Plate_Number = TunHeader.Plate_Number;
                    IsClose_Status = false;
                }

                await OnLoadTunneldetails(TunHeader);
                await OnLoadTruckMaster();
                await OnGrandtotalegg(TunHeader.LoadDate, TunHeader.AndroidLoadSheet);

                if (TunHeader.Load_Status == TokenCons.Closed)
                {
                    IsHidebtnSync = false;
                    IsUnHidebtnSync = true;                    
                    Is_Hideeditbtn = true;
                }
                else
                {
                    Is_Hideeditbtn = false;
                    IsHidebtnSync = true;
                    IsUnHidebtnSync = false;
                }
                IsRefresh = false;
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }
        private async Task OnEditLoadsheet()
        {
            await Task.Delay(100);

            Btn_SaveEdit = "EDIT LOADSHEET";

            var _masterlist = await _global.reasons.GetSpecificList("DRIVER");
            Reason_List.Clear();

            foreach(var _item in _masterlist)
            {
                if(_item.ReasonCodeType == "17003" || _item.ReasonCodeType == "17004")
                {
                    Reason_List.Add(_item);
                }
            }

            IsClose_Status = true;
            IsHidebtnSync = true;
            Is_Hideeditbtn = false;
            Is_Hidereason = true;


            Parameter_Ref = "For_Edit";
        }
        private async Task OnLoadTunneldetails(TunHeaderView _obj)
        {
            var _tundetailList = await _global.tunneldetails.GetTunneldetailsproduction(_obj.LoadDate, _obj.AndroidLoadSheet);
            var _grouplist = await GetEggsizeGroup(_tundetailList.ToList());
            TunnelDetails_List.Clear();
            TunnelDetails_List.ReplaceRange(_grouplist);
        }

        private async Task<List<TunnelDetails>> GetEggsizeGroup(List<TunnelDetails> _tundetailsList)
        {
            await Task.Delay(100);
            var _returnList = _tundetailsList.GroupBy(a => a.Egg_StatType).Select(b => new TunnelDetails
            {
                AGTPDId = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().AGTPDId,
                LoadNumber = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().LoadNumber,
                Egg_Qty = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().Egg_Qty,
                Egg_StatType = b.Key,
                Load_Status = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().Load_Status,
                DateCreated = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().DateCreated,
                DateUpdated = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().DateUpdated,
                Remarks = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().Remarks,
                Egg_Location = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().Egg_Location,
                Production_Date = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().Production_Date,
                Sequence = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().Sequence,
                UserSequence = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().UserSequence,
                AndroidLoadSheet = b.Where(ba => ba.Egg_StatType == b.Key).FirstOrDefault().AndroidLoadSheet
            }).ToList();

            return _returnList;
        }
        private async Task OnLoadTruckMaster()
        {
            var _truckmasterlist = await _global.truckmaster.GetTruckmaster();
            TruckMaster_List.Clear();
            TruckMaster_List.ReplaceRange(_truckmasterlist);
        }

        private async Task OnGrandtotalegg(DateTime _productDate, string _loadsheet)
        {
            double _subtotal = 0;
            var _tundetailList = await _global.tunneldetails.GetTunneldetailsproduction(_productDate, _loadsheet);
            foreach (var _itm in TunnelDetails_List)
            {
                _subtotal += _itm.Egg_Qty;
            }
            GrandTotal_Egg = _subtotal;
        }

        private async Task OnSpecSyncapi()
        {

            DateTime _todayDate = DateTime.Now;
            var _loading = UserDialogs.Instance.Loading("Uploading Data. . .");
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

                if (Parameter_Ref == "For_Edit")
                {
                    bool _confirmation = await App.Current.MainPage.DisplayAlert("Message Alert", "Are you sure to edit this loadsheet?", "Yes", "No");
                    if (_confirmation == false)
                    {
                        _loading.Dispose();
                        await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, $"Are you sure to edit this loadsheet?. A:{_confirmation}", TokenCons.IsFailed);
                        return;
                    }

                    if (string.IsNullOrEmpty(ReasonType) || string.IsNullOrWhiteSpace(ReasonType))
                    {
                        await _global.configurationService.MessageAlert("Please enter reason for edit");
                        await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, $"No reason type enter.", TokenCons.IsFailed);
                        _loading.Dispose();
                        return;
                    }

                    var _apiModel = await _global.tunnelheader.GetserverModel(TunHeader.AndroidLoadSheet, TunHeader.User_Code);

                    string _conbineError = $"{ReasonType}--{Remarks_Editor}";

                    TunnelHeader _tunnelheader = new TunnelHeader()
                    {
                        AGTId = _apiModel.AGTId,
                        User = _apiModel.User,
                        Plate_Number = Plate_Number,
                        DateAdded = _apiModel.DateAdded,
                        Status_Checked = _apiModel.Status_Checked,
                        DateEdited = _apiModel.DateEdited,
                        User_Checked = _apiModel.User_Checked,
                        LoadDate = _apiModel.LoadDate,
                        LoadNumber = _apiModel.LoadNumber,
                        LoadSequence = _apiModel.LoadSequence,
                        Load_Status = TokenCons.IsProcessing,
                        UserID = _apiModel.UserID,
                        Building_Location = _apiModel.Building_Location,
                        TruckDriverName = "NDT",
                        Override_Status = _apiModel.Override_Status,
                        CSRefNo = _apiModel.CSRefNo,
                        AndroidLoadSheet = _apiModel.AndroidLoadSheet,
                        ReasonForReject = _apiModel.ReasonForReject,
                        CancelledLoadSheet = "Edited",
                        Remarks = _conbineError,
                        User_Code = _apiModel.User_Code
                    };

                    await _global.tunnelheader.PutapiHeader(_tunnelheader, "For_Edit");
                    var _tundetailsList = await _global.tunneldetails.GetTunneldetailsproduction(_tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                    foreach (var _itmdetails in _tundetailsList)
                    {
                        TunnelDetails _tunnedetails = new TunnelDetails()
                        {
                            LoadNumber = _tunnelheader.LoadNumber,
                            Egg_Qty = _itmdetails.Egg_Qty,
                            Egg_StatType = _itmdetails.Egg_StatType,
                            Load_Status = TokenCons.IsProcessing,
                            DateCreated = _itmdetails.DateCreated,
                            DateUpdated = _itmdetails.DateUpdated,
                            Remarks = _itmdetails.Remarks,
                            Egg_Location = _itmdetails.Egg_Location,
                            Production_Date = _itmdetails.Production_Date,
                            Sequence = _itmdetails.Sequence,
                            UserSequence = _itmdetails.UserSequence,
                            AndroidLoadSheet = _tunnelheader.AndroidLoadSheet
                        };

                        await _global.tunneldetails.PutapiDetails(_tunnedetails);
                    }

                    await OnUpdateproduction(_tunnelheader);
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, "Edit", $"Successfully edit loadsheet {_tunnelheader.AndroidLoadSheet}/{_tunnelheader.LoadNumber}", _tunnelheader.Load_Status);
                    await OnSyncuserlog();
                    await _global.configurationService.MessageAlert("Successfully edit");
                    await OnClose();
                    _loading.Dispose();
                }
                else
                {
                    //saving in the live server

                    if (Plate_Number == "SELECT TRUCK" || string.IsNullOrEmpty(Plate_Number) || string.IsNullOrWhiteSpace(Plate_Number))
                    {
                        await _global.configurationService.MessageAlert("Please choose a truck");
                        await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, $"Vehicle truck, not selected", TokenCons.IsFailed);
                        _loading.Dispose();
                        return;
                    }                   

                    bool _confirmation = await App.Current.MainPage.DisplayAlert("Message Alert", "Trays can't fit the Tray Capacity of the Truck.", "Yes", "No");
                    if (_confirmation == false)
                    {
                        _loading.Dispose();
                        await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, $"Trays can't fit the Tray Capacity of the Truck. A:{_confirmation}", TokenCons.IsFailed);
                        return;
                    }

                    await OnGenerateLS();

                    TunnelHeader _tunnelheader = new TunnelHeader()
                    {
                        AGTId = TunHeader.AGTId,
                        User = TunHeader.User,
                        Plate_Number = Plate_Number,
                        DateAdded = TunHeader.DateAdded,
                        Status_Checked = TunHeader.Status_Checked,
                        DateEdited = TunHeader.DateEdited,
                        User_Checked = TunHeader.User_Checked,
                        LoadDate = TunHeader.LoadDate,
                        LoadNumber = LoadSheet_Code,
                        LoadSequence = Squence_Count,
                        Load_Status = TokenCons.IsProcessing,
                        UserID = TunHeader.UserID,
                        Building_Location = TunHeader.Building_Location,
                        TruckDriverName = "NDT",
                        Override_Status = TunHeader.Override_Status,
                        CSRefNo = TunHeader.CSRefNo,
                        AndroidLoadSheet = TunHeader.AndroidLoadSheet,
                        ReasonForReject = TunHeader.ReasonForReject,
                        CancelledLoadSheet = TunHeader.CancelledLoadSheet,
                        Remarks = TunHeader.Remarks,
                        User_Code = TunHeader.User_Code
                    };

                    var _Isexistapi = await _global.tunnelheader.GetapiExistloadsheet(_tunnelheader.User_Code, _tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                    if (_Isexistapi != 0)
                    {
                        await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, $"Create loadsheet number {_tunnelheader.AndroidLoadSheet} is already exist", TokenCons.IsFailed);
                        return;
                    }

                    var _tunheaderModel = await _global.tunnelheader.PostapiTunnelheader(_tunnelheader);

                    var _tundetailsList = await _global.tunneldetails.GetTunneldetailsproduction(_tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                    foreach (var _itmdetails in _tundetailsList)
                    {
                        TunnelDetails _tunnedetails = new TunnelDetails()
                        {
                            LoadNumber = _tunnelheader.LoadNumber,
                            Egg_Qty = _itmdetails.Egg_Qty,
                            Egg_StatType = _itmdetails.Egg_StatType,
                            Load_Status = _tunnelheader.Load_Status,
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

                    await OnUpdateproduction(_tunnelheader);

                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, "Sync", $"Successfully sync loadsheet {_tunnelheader.AndroidLoadSheet}/{_tunnelheader.LoadNumber}", _tunnelheader.Load_Status);
                    await OnSyncuserlog();
                    await _global.configurationService.MessageAlert("Sync successfully");
                    await OnClose();
                    _loading.Dispose();
                }
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, ex.Message, TokenCons.IsFailed);
                _loading.Dispose();
                return;
            }
        }
        private async Task OnUpdateproduction(TunnelHeader _obj)
        {
            try
            {
                TunnelHeader _tunnelheader = new TunnelHeader()
                {
                    AGTId = _obj.AGTId,
                    User = _obj.User,
                    Plate_Number = _obj.Plate_Number,
                    DateAdded = _obj.DateAdded,
                    DateEdited = _obj.DateEdited,
                    Status_Checked = _obj.Status_Checked,
                    User_Checked = _obj.User_Checked,
                    LoadDate = _obj.LoadDate,
                    LoadNumber = _obj.LoadNumber,
                    LoadSequence = _obj.LoadSequence,
                    Load_Status = TokenCons.Closed,
                    UserID = _obj.UserID,
                    Building_Location = _obj.Building_Location,
                    TruckDriverName = _obj.TruckDriverName,
                    Override_Status = _obj.Override_Status,
                    CSRefNo = _obj.CSRefNo,
                    AndroidLoadSheet = _obj.AndroidLoadSheet,
                    ReasonForReject = _obj.ReasonForReject,
                    CancelledLoadSheet = _obj.CancelledLoadSheet,
                    Remarks = _obj.Remarks,
                    User_Code = _obj.User_Code
                };             

                await _global.tunnelheader.Update_TunnelHeaderStatus(_tunnelheader);

                var _tundetailsList = await _global.tunneldetails.GetTunneldetailsproduction(_tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                foreach (var _itmdetails in _tundetailsList)
                {
                    TunnelDetails _tunnedetails = new TunnelDetails()
                    {
                        AGTPDId = _itmdetails.AGTPDId,
                        LoadNumber = _tunnelheader.LoadNumber,
                        Egg_Qty = _itmdetails.Egg_Qty,
                        Egg_StatType = _itmdetails.Egg_StatType,
                        Load_Status = _tunnelheader.Load_Status,
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
        //Code sapag buhat ug loadsheet
        private async Task OnGenerateLS()
        {
            try
            {
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
        private async Task OnSyncuserlog()
        {
            try
            {
                DateTime _todayDate = DateTime.Now.AddDays(-1);
                var _logmasterlist = await _global.logsService.Getlogsmasterlist();
                var _sortdatalist = _logmasterlist.Where(a => a.Logs_Create.Date > _todayDate.Date).ToList();

                if(_sortdatalist.Count() == 0)
                {
                    await OnClose();
                    return;
                }

                foreach (var _item in _sortdatalist)
                {
                    await _global.logsService.Postlogs_API(_item);
                }
            }
            catch
            {
                await OnClose();
                throw new Exception();
            }
            
        }
        private async Task OnClose()
        {
            await Shell.Current.GoToAsync("..");
        }
                
    }
}
