﻿using Acr.UserDialogs;
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

        bool _isrefresh, _ishidebtnsync, _isunhidebtnsync, _isclosestatus, _isselecttrack;
        double _grandtotalegg;
        int _squencecount;
        string _loadsheetcode, _platenumber, _buildinglocation;

        TunnelHeader _tunnelheadermodel;
        TunHeaderView _tunheader;
        public bool IsRefresh { get => _isrefresh; set => SetProperty(ref _isrefresh, value); }
        public double GrandTotal_Egg { get => _grandtotalegg; set => SetProperty(ref _grandtotalegg, value); }
        public bool IsHidebtnSync { get => _ishidebtnsync; set => SetProperty(ref _ishidebtnsync, value); }
        public bool IsUnHidebtnSync { get => _isunhidebtnsync; set => SetProperty(ref _isunhidebtnsync, value); }
        public bool IsClose_Status { get => _isclosestatus; set => SetProperty(ref _isclosestatus, value); }
        public bool Is_SelectTrack { get => _isselecttrack; set => SetProperty(ref _isselecttrack, value); }
        public int Squence_Count { get => _squencecount; set => SetProperty(ref _squencecount, value); }
        public string LoadSheet_Code { get => _loadsheetcode; set => SetProperty(ref _loadsheetcode, value); }
        public string Plate_Number { get => _platenumber; set => SetProperty(ref _platenumber, value); }
        public string Building_Location { get => _buildinglocation; set => SetProperty(ref _buildinglocation, value); }
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

        public AsyncCommand RefreshCommand { get; set; }
        public AsyncCommand SyncloadsheetCommand { get; set; }
        public AsyncCommand ClosedCommand { get; set; }

        public ObservableRangeCollection<TunnelDetails> TunnelDetails_List { get; set; }
        public ObservableRangeCollection<TruckMaster_Model> TruckMaster_List { get; set; }
        //public TunnelHeader TunHeader = new TunnelHeader();

        public LoadSheetInfoVM()
        {
            TunnelDetails_List = new ObservableRangeCollection<TunnelDetails>();
            TruckMaster_List = new ObservableRangeCollection<TruckMaster_Model>();

            RefreshCommand = new AsyncCommand(OnRefresh);
            ClosedCommand = new AsyncCommand(OnClose);
            SyncloadsheetCommand = new AsyncCommand(OnSpecSyncapi);
        }


        private async Task OnRefresh()
        {
            try
            {
                IsRefresh = true;
                IsHidebtnSync = false;
                IsUnHidebtnSync = false;
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

                if (TunHeader.Load_Status == TokenCons.Closed || TunHeader.Load_Status == TokenCons.IsCancel)
                {
                    IsHidebtnSync = false;
                    IsUnHidebtnSync = true;
                }
                else
                {
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
            var _loading = UserDialogs.Instance.Loading("Uploading Data. . .");
            _loading.Show();
            try
            {

                if(Plate_Number == "SELECT TRUCK" || string.IsNullOrEmpty(Plate_Number) || string.IsNullOrWhiteSpace(Plate_Number))
                {
                    await _global.configurationService.MessageAlert("Please choose a truck");
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, $"Vehicle truck, not selected", TokenCons.IsFailed);
                    _loading.Dispose();
                    return;
                }

                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                {
                    _loading.Dispose();
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsNonet, $"No internet connection", TokenCons.IsFailed);
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
                    TruckDriverName = TunHeader.TruckDriverName,
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
                _loading.Dispose();
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

                var _Isexistapi = await _global.tunnelheader.GetExistloadsheet(_tunnelheader);
                if (_Isexistapi != 0)
                {
                    await _global.configurationService.MessageAlert("Somethings is missing");
                    return;
                }

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

                await _global.configurationService.MessageAlert("Sync successfully");
                await OnClose();
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
            DateTime _todayDate = DateTime.Now.AddDays(-1);
            var _logmasterlist = await _global.logsService.Getlogsmasterlist();
            var _sortdatalist = _logmasterlist.Where(a => a.Logs_Create.Date > _todayDate.Date).ToList();
            foreach(var _item in _sortdatalist)
            {
                await _global.logsService.Postlogs_API(_item);
            }
        }
        private async Task OnClose()
        {
            await Shell.Current.GoToAsync("..");
        }
                
    }
}
