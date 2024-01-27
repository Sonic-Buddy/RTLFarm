using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Helpers;
using RTLFarm.Models.OthersModel;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.DialogViewModel
{
    public class RejectVM : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();
        string _remarkseditor, _reasontype;
        TunnelHeader _tunnelheader;
        public string Remarks_Editor { get => _remarkseditor; set => SetProperty(ref _remarkseditor, value); }
        public string ReasonType { get => _reasontype; set => SetProperty(ref _reasontype, value); }

        public TunnelHeader TunnelHeader { get => _tunnelheader; set => SetProperty(ref _tunnelheader, value); }

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
        public AsyncCommand CancelCommand { get; set; }

        public ObservableRangeCollection<ReasonModel> Reason_List { get; set; }

        public RejectVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);
            CancelCommand = new AsyncCommand(OnRejectloadsheet);
            Reason_List = new ObservableRangeCollection<ReasonModel>();
        }

        private async Task OnRefresh()
        {
            var loading = UserDialogs.Instance.Loading("Loading. . .");
            loading.Show();
            try
            {
                var _tunmodel = TokenSetGet.GetParamModel();

                TunnelHeader = await _global.tunnelheader.GetSpecificmodel(_tunmodel.User_Code, _tunmodel.PMT_2, _tunmodel.PMT_1);
                await OnLoadData();                
                loading.Dispose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                loading.Dispose();
                return;
            }
        }

        private async Task OnLoadData()
        {
            var _masterlist = await _global.reasons.GetSpecificList("DRIVER");
            Reason_List.Clear();
            Reason_List.ReplaceRange(_masterlist);
        }

        private async Task OnRejectloadsheet()
        {
            var loading = UserDialogs.Instance.Loading("Loading. . .");
            loading.Show();
            try
            {
                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                {
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsNonet, $"No internet connection", TokenCons.IsFailed);
                    loading.Dispose();
                    return;
                }

                bool _confirmation = await App.Current.MainPage.DisplayAlert("Message Alert", $"You're sure to cancel this {TunnelHeader.LoadNumber}?", "Yes", "No");
                if (_confirmation == false)
                {
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsEnable, $"You're sure to cancel this {TunnelHeader.LoadNumber}? A:{_confirmation}", TokenCons.IsFailed);
                    loading.Dispose();
                    return;
                }   

                TunnelHeader _tunnelheader = new TunnelHeader()
                {
                    AGTId = TunnelHeader.AGTId,
                    User = TunnelHeader.User,
                    Plate_Number = TunnelHeader.Plate_Number,
                    DateAdded = TunnelHeader.DateAdded,
                    DateEdited = TunnelHeader.DateEdited,
                    Status_Checked = TunnelHeader.Status_Checked,
                    User_Checked = TokenSetGet.Get_UserModel().UserFullName,
                    LoadDate = TunnelHeader.LoadDate,
                    LoadNumber = TunnelHeader.LoadNumber,
                    LoadSequence = TunnelHeader.LoadSequence,
                    Load_Status = TokenCons.IsCancel,
                    UserID = TunnelHeader.UserID,
                    Building_Location = TunnelHeader.Building_Location,
                    TruckDriverName = TokenSetGet.Get_UserModel().SalesmanCode,
                    Override_Status = TunnelHeader.Override_Status,
                    CSRefNo = TunnelHeader.CSRefNo,
                    AndroidLoadSheet = TunnelHeader.AndroidLoadSheet,
                    ReasonForReject = ReasonType,
                    CancelledLoadSheet = TunnelHeader.CancelledLoadSheet,
                    Remarks = Remarks_Editor,
                    User_Code = TunnelHeader.User_Code
                };

                var _Isexistapi = await _global.tunnelheader.GetapiExistloadsheet(_tunnelheader.User_Code, _tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                if (_Isexistapi != 0)
                {
                    await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, $"Loadsheet is already exist count", TokenCons.IsFailed);
                    await _global.configurationService.MessageAlert("Somethings is missing");
                    loading.Dispose();
                    return;
                }

                await _global.tunnelheader.PutapiHeader(_tunnelheader, "Update_Status");
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
                        Load_Status = TokenCons.IsCancel,
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

                await OnSyncuserlog();
                MessagingCenter.Send(this, "parameterstring", "Loadsheet successfully cancel");
                loading.Dispose();
                await PopupNavigation.Instance.PopAsync(true);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "parameterstring", ex.Message);
                await OnAddlogs(TokenSetGet.Get_UserModel().SalesmanCode, TokenSetGet.Get_UserModel().UserFullName, TokenSetGet.Get_UserModel().UserRole, TokenCons.IsError, ex.Message, TokenCons.IsFailed);
                loading.Dispose();
                await PopupNavigation.Instance.PopAsync(true);
            }
        }

        private async Task OnSyncuserlog()
        {
            DateTime _todayDate = DateTime.Now.AddDays(-1);
            var _logmasterlist = await _global.logsService.Getlogsmasterlist();
            var _sortdatalist = _logmasterlist.Where(a => a.Logs_Create.Date > _todayDate.Date).ToList();
            foreach (var _item in _sortdatalist)
            {
                await _global.logsService.Postlogs_API(_item);
            }
        }
    }
}
