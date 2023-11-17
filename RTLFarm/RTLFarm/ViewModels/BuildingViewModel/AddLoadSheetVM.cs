using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using RTLFarm.Helpers;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using RTLFarm.Views.BuildingPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.BuildingViewModel
{
    public class AddLoadSheetVM : ViewModelBase
    {

        GlobalDependencyServices _global = new GlobalDependencyServices();

        int _eggquantity;
        string _flockmancode, _loadsheetcode, _eggstatuscat, _eggstatusdesc;
        bool _issubstatus, _iscolrefresh, _isenableqty;
        //TunnelHeader _tunnelheader;
        DummyTunDetails _tunneldetails;
        Usermaster_Model _usermodel;
        public int Egg_Quantity { get => _eggquantity; set => SetProperty(ref _eggquantity, value); }
        public string Flockman_Code { get => _flockmancode; set => SetProperty(ref _flockmancode, value); }
        public string EggStatus_Cat { get => _eggstatuscat; set => SetProperty(ref _eggstatuscat, value); }
        public string EggStatus_Desc { get => _eggstatusdesc; set => SetProperty(ref _eggstatusdesc, value); }
        public string Loadsheet_Code { get => _loadsheetcode; set => SetProperty(ref _loadsheetcode, value); }      
        public bool Is_Substatus { get => _issubstatus; set => SetProperty(ref _issubstatus, value); }
        public bool IsColRefresh { get => _iscolrefresh; set => SetProperty(ref _iscolrefresh, value); }
        public bool IsEnable_QTY { get => _isenableqty; set => SetProperty(ref _isenableqty, value); }
        public DummyTunDetails Tunnel_Details { get => _tunneldetails; set => SetProperty(ref _tunneldetails, value); }
        public Usermaster_Model UserModel { get => _usermodel; set => SetProperty(ref _usermodel, value); }

        private DateTime _productiondate;
        public DateTime Production_Date
        {
            get => _productiondate;
            set
            {
                if (_productiondate == value)
                    return;
                _productiondate = value;
                OnPropertyChanged();
            }
        }

        StatusType_Model _selectstatustype;
        //public StatusType_Model SelectStatustype
        //{
        //    get { return _selectstatustype; }
        //    set
        //    {                
        //        if (_selectstatustype == value) { return; }
        //        _selectstatustype = value;
        //        SetProperty(ref _selectstatustype, value);
        //        OnSubStatustype(value.Egg_Code);
        //    }
        //}
        public StatusType_Model SelectStatustype
        {
            get => _selectstatustype;
            set
            {
                if (_selectstatustype == value) { return; }
                _selectstatustype = value;
                OnPropertyChanged();
                OnSubStatustype(value.Egg_Code);
            }
        }

        StatusType_Model _selectsubstatustype;
        public StatusType_Model SelectSubStatustype
        {
            get => _selectsubstatustype;
            set
            {
                if (_selectsubstatustype == value) { return; }
                _selectsubstatustype = value;
                OnPropertyChanged();
                EggStatus_Cat = value.Cat_Code;
                EggStatus_Desc = value.Egg_Desc;
                IsEnable_QTY = true;
            }
        }
        
        
        public ObservableRangeCollection<StatusType_Model> StatusType_List { get; set; }
        public ObservableRangeCollection<StatusType_Model> SubStatusType_List { get; set; }
        public ObservableRangeCollection<DummyTunDetails> DummyTunDetails_List { get; set; }

        public DummyTunHeader DummyTun_Header  = new DummyTunHeader();

        public AsyncCommand RefreshCommand { get; set; }
        public AsyncCommand ColRefreshCommand { get; set; }
        public AsyncCommand ClosedCommand { get; set; }
        public AsyncCommand CompletedCommand { get; set; }
        public AsyncCommand SaveLSCommand { get; set; }
        public AsyncCommand<DummyTunDetails> RemoveCommand { get; }

        public AddLoadSheetVM()
        {
            DummyTun_Header = new DummyTunHeader();
            StatusType_List = new ObservableRangeCollection<StatusType_Model>();
            SubStatusType_List = new ObservableRangeCollection<StatusType_Model>();
            DummyTunDetails_List = new ObservableRangeCollection<DummyTunDetails>();
            RefreshCommand = new AsyncCommand(OnRefresh);
            ClosedCommand = new AsyncCommand(OnClose);
            ColRefreshCommand = new AsyncCommand(OnColRefresh);
            RemoveCommand = new AsyncCommand<DummyTunDetails>(OnRemoveinList);
            CompletedCommand = new AsyncCommand(OnCompletedLoadsheet);
            SaveLSCommand = new AsyncCommand(OnSaveloadsheet);
        }

        private async Task OnRefresh()
        {
            try
            {
                Is_Substatus = false;
                IsEnable_QTY = false;
                Production_Date = DateTime.Now;

                var _paramSalecode = Preferences.Get("prmtmastercode", string.Empty);
                UserModel = await _global.loginService.GetSpecificmodel(_paramSalecode);
                Flockman_Code = UserModel.SalesmanCode;

                await OnPreference();
                Loadsheet_Code = await OnGenerateLS();

                await OnColRefresh();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnColRefresh()
        {
            IsColRefresh = true;
            var _tundetailsList = await _global.dummytundetails.GetDummyTundetailsproduction(Production_Date, TokenCons.IsProcessing);
            DummyTunDetails_List.Clear();
            DummyTunDetails_List.ReplaceRange(_tundetailsList);
            
            IsColRefresh = false;
        }
        private async Task OnPreference()
        {
            Egg_Quantity = 0;
            Is_Substatus = false;
            IsEnable_QTY = false;

            var _statustypeList = await _global.statustype.GetStatustypemaster();
            var _groupList = await GetStatustypeGroup(_statustypeList.ToList());
            StatusType_List.Clear();
            StatusType_List.ReplaceRange(_groupList);
        }
        private async void OnSubStatustype(string _eggcode)
        {
            
            if (_eggcode == "GOOD")
            {
                Is_Substatus = false;
                IsEnable_QTY = true;
                EggStatus_Desc = "GOOD EGG";
                EggStatus_Cat = "201";
                
            }
            else if(_eggcode == "CRACK")
            {
                Is_Substatus = false;
                IsEnable_QTY = true;
                EggStatus_Desc = "CRACK EGG";
                EggStatus_Cat = "203";
            }
            else
            {
                Is_Substatus = true;
                IsEnable_QTY = false;
                var _substatuslist = await _global.statustype.GetSubStatustypemaster(_eggcode);
                SubStatusType_List.Clear();
                SubStatusType_List.ReplaceRange(_substatuslist);
            }     
        }

        private async Task OnSaveloadsheet()
        {
            var loading = UserDialogs.Instance.Loading("Loading. . .");
            loading.Show();
            try
            {
                if(Egg_Quantity == 0)
                {
                    await _global.configurationService.MessageAlert("No quantity added");
                    return;
                }

                await Task.Delay(10);
                DummyTunHeader _tunnelheader = new DummyTunHeader()
                {
                    User = UserModel.UserFullName,
                    Plate_Number = string.Empty,
                    DateAdded = DateTime.Now,
                    DateEdited = DateTime.MinValue,
                    Status_Checked = string.Empty,
                    User_Checked = string.Empty,
                    LoadDate = Production_Date,
                    LoadNumber = string.Empty,
                    LoadSequence = 0,
                    Load_Status = TokenCons.IsProcessing,
                    UserID = UserModel.UserId,
                    Building_Location = UserModel.WhSpecificLocation,
                    TruckDriverName = string.Empty,
                    Override_Status = string.Empty,
                    CSRefNo = string.Empty,
                    AndroidLoadSheet = Loadsheet_Code,
                    ReasonForReject = string.Empty,
                    CancelledLoadSheet = string.Empty,
                    Remarks = string.Empty,
                    User_Code = UserModel.SalesmanCode,
                };

                var _isexistloadsheet = await _global.dummytunheader.GetDummyExistloadsheet(_tunnelheader.AndroidLoadSheet);
                if (_isexistloadsheet == 0)
                {
                    var _returnObj = await _global.dummytunheader.Insert_DummyTunHeader(_tunnelheader);

                    DummyTunDetails _tunnedetails = new DummyTunDetails()
                    {
                        LoadNumber = _tunnelheader.LoadNumber,
                        Egg_Qty = Egg_Quantity,
                        Egg_StatType = EggStatus_Desc,
                        Load_Status = _tunnelheader.Load_Status,
                        DateCreated = _returnObj.DateAdded,
                        DateUpdated = DateTime.MinValue,
                        Remarks = EggStatus_Cat,
                        Egg_Location = _returnObj.Building_Location,
                        Production_Date = _returnObj.LoadDate,
                        Sequence = _returnObj.LoadSequence,
                        UserSequence = _returnObj.UserID,
                        Android_LoadSheet = _returnObj.AndroidLoadSheet
                    };

                    await _global.dummytundetails.Insert_DummyTunDetails(_tunnedetails);
                }
                else
                {
                    DummyTunDetails _tunnedetails = new DummyTunDetails()
                    {
                        LoadNumber = _tunnelheader.LoadNumber,
                        Egg_Qty = Egg_Quantity,
                        Egg_StatType = EggStatus_Desc,
                        Load_Status = _tunnelheader.Load_Status,
                        DateCreated = _tunnelheader.DateAdded,
                        DateUpdated = DateTime.MinValue,
                        Remarks = EggStatus_Cat,
                        Egg_Location = _tunnelheader.Building_Location,
                        Production_Date = _tunnelheader.LoadDate,
                        Sequence = _tunnelheader.LoadSequence,
                        UserSequence = _tunnelheader.UserID,
                        Android_LoadSheet = _tunnelheader.AndroidLoadSheet
                    };

                    await _global.dummytundetails.Insert_DummyTunDetails(_tunnedetails);
                }

                await OnColRefresh();

                var route = $"/{nameof(AddLoadSheetPage)}";
                await Shell.Current.GoToAsync(route);

                await Task.Delay(2000);
                await Shell.Current.GoToAsync($"../{route}");
                loading.Dispose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                loading.Dispose();
                return;
            }
        }
        private async void OnResetStatusType()
        {  
            StatusType_List = new ObservableRangeCollection<StatusType_Model>();
            SubStatusType_List = new ObservableRangeCollection<StatusType_Model>();

            await OnPreference();
        }
        private async Task OnCompletedLoadsheet()
        {

            bool _confirmation = await App.Current.MainPage.DisplayAlert("Message Alert", "You're sure to save it?", "Yes", "No");
            if (_confirmation == false)
                return;

            var _loading = UserDialogs.Instance.Loading("Saving Data. . .");
            _loading.Show();
            try
            {

                var _dummyheaderList = await _global.dummytunheader.GetSpecificloadsheet(Production_Date, Flockman_Code);
                foreach(var _itmHeader in _dummyheaderList)
                {
                    TunnelHeader _tunnelheader = new TunnelHeader()
                    {
                        User = _itmHeader.User,
                        Plate_Number = _itmHeader.Plate_Number,
                        DateAdded = _itmHeader.DateAdded,
                        DateEdited = _itmHeader.DateEdited,
                        Status_Checked = _itmHeader.Status_Checked,
                        User_Checked = _itmHeader.User_Checked,
                        LoadDate = _itmHeader.LoadDate,
                        LoadNumber = _itmHeader.LoadNumber,
                        LoadSequence = _itmHeader.LoadSequence,
                        Load_Status = TokenCons.IsProcessing,
                        UserID = _itmHeader.UserID,
                        Building_Location = _itmHeader.Building_Location,
                        TruckDriverName = _itmHeader.TruckDriverName,
                        Override_Status = _itmHeader.Override_Status,
                        CSRefNo = _itmHeader.CSRefNo,
                        AndroidLoadSheet = _itmHeader.AndroidLoadSheet,
                        ReasonForReject = _itmHeader.ReasonForReject,
                        CancelledLoadSheet = _itmHeader.CancelledLoadSheet,
                        Remarks = _itmHeader.Remarks,
                        User_Code = _itmHeader.User_Code
                    };

                    var _tunnelheaderModel = await _global.tunnelheader.Insert_TunnelHeader(_tunnelheader);

                    var _tundetialsList = await _global.dummytundetails.GetSpecifictundetails(_tunnelheaderModel.LoadDate, _tunnelheaderModel.AndroidLoadSheet);
                    foreach(var _itmdetails in _tundetialsList)
                    {
                        TunnelDetails _tunnedetails = new TunnelDetails()
                        {
                            LoadNumber = _itmdetails.LoadNumber,
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
                            AndroidLoadSheet = _itmdetails.Android_LoadSheet
                        };

                        var _isExistcount = await _global.tunneldetails.Getexistcount(_tunnedetails.Production_Date, _tunnedetails.Remarks, _tunnedetails.AndroidLoadSheet);
                        if(_isExistcount == 0)
                        {
                            await _global.tunneldetails.Insert_TunnelDetails(_tunnedetails);
                        }
                        else
                        {
                            await _global.tunneldetails.Update_TunnelDetails(_tunnedetails);
                        }
                    }
                }

                _loading.Dispose();
                await _global.configurationService.MessageAlert("Successfully Save");
                await OnClose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                _loading.Dispose();
                return;
            }
        }
        private async Task<string> OnGenerateLS()
        {
            await Task.Delay(10);
            string _buildingLoc = UserModel.WhSpecificLocation;
            string _dateReference = DateTime.Now.ToString("yyMMdd");
            string _loadsheet = string.Empty;
            int _returnlsCount = 0;

            _returnlsCount = await _global.tunnelheader.GetSpecificcount(Production_Date, Flockman_Code, _buildingLoc);
           
            var _countLength = _returnlsCount.ToString().Length;
            var _usercodeLength = Flockman_Code.Substring(Flockman_Code.Length - 3);
            if (_returnlsCount == 0)
            {
                var _isZero = _returnlsCount + 1;
                _loadsheet = $"{_buildingLoc}{_usercodeLength}{_dateReference}00{_isZero}";
                return _loadsheet;
            }
            else
            {
                switch (_countLength)
                {
                    default:
                        var _countDefault = _returnlsCount + 1;
                        _loadsheet = $"{_buildingLoc}{_usercodeLength}{_dateReference}0{_countDefault}";
                        return _loadsheet;

                    case 1:
                        var _countsCaseone = _returnlsCount + 1;
                        _loadsheet = $"{_buildingLoc}{_usercodeLength}{_dateReference}00{_countsCaseone}";
                        return _loadsheet;

                    case 2:
                        var _countsCasetwo = _returnlsCount + 1;
                        _loadsheet = $"{_buildingLoc}{_usercodeLength}{_dateReference}000{_countsCasetwo}";
                        return _loadsheet;
                }
            }
        }
        private async Task OnRemoveinList(DummyTunDetails _obj)
        {
            await _global.dummytundetails.RemoveinList(_obj);
            await OnColRefresh();
        }
        private async Task<List<StatusType_Model>> GetStatustypeGroup(List<StatusType_Model> _statustypelist)
        {
            await Task.Delay(100);
            var _returnList = _statustypelist.GroupBy(a => a.Egg_Code).Select(b => new StatusType_Model
            {
                STYId = b.Where(ba => ba.Egg_Code == b.Key).FirstOrDefault().STYId,
                Egg_Code = b.Key,
                Egg_Desc = b.Where(ba => ba.Egg_Code == b.Key).FirstOrDefault().Egg_Desc,
                Egg_Qty = b.Where(ba => ba.Egg_Code == b.Key).FirstOrDefault().Egg_Qty,
            }).ToList();

            return _returnList;
        }
        private async Task OnClose()
        {           
            var route = $"/{nameof(LoadSheetListPage)}";
            await Shell.Current.GoToAsync(route);
        }
    }
}
