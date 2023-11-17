using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Helpers;
using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Models.OthersModel;
using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services;
using RTLFarm.ViewModels.DialogViewModel;
using RTLFarm.Views.DialogPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.TransportViewModel
{
    public class TransportInfoVM : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        string _buildinglocation;
        bool _isrefresh, _isbtnhide;
        double _grandtotalegg;

        TunHeaderView _tunheader;
        public TunHeaderView TunHeader { get => _tunheader; set => SetProperty(ref _tunheader, value); }
        public bool IsRefresh { get => _isrefresh; set => SetProperty(ref _isrefresh, value); }
        public bool Is_BtnHide { get => _isbtnhide; set => SetProperty(ref _isbtnhide, value); }
        public double GrandTotal_Egg { get => _grandtotalegg; set => SetProperty(ref _grandtotalegg, value); }
        public string Building_Location { get => _buildinglocation; set => SetProperty(ref _buildinglocation, value); }

        public AsyncCommand RefreshCommand { get; set; }
        public AsyncCommand ClosedCommand { get; set; }
        public AsyncCommand AcceptCommand { get; set; }
        public AsyncCommand RejectCommand { get; set; }

        public ObservableRangeCollection<TunnelDetails> TunnelDetails_List { get; set; }

        public TransportInfoVM()
        {
            TunnelDetails_List = new ObservableRangeCollection<TunnelDetails>();
            RefreshCommand = new AsyncCommand(OnRefresh);
            ClosedCommand = new AsyncCommand(OnClose);
            AcceptCommand = new AsyncCommand(OnAcceptLS);
            RejectCommand = new AsyncCommand(OnReject);

            MessagingCenter.Subscribe<RejectVM, string>(this, "parameterstring", async (page, e) =>
            {
                await Task.Delay(1);
                await _global.configurationService.MessageAlert(e);
                await OnRefresh();
            });
        }

        private async Task OnRefresh() 
        {
            try
            {
                IsRefresh = true;
                Is_BtnHide = false;
                TunHeader = TokenSetGet.Get_Tunnel_Header();

                Building_Location = await _global.buildinglocation.GetBuildingLocation(TunHeader.Building_Location, "Description");

                if(TunHeader.Load_Status == TokenCons.IsProcessing)
                {
                    Is_BtnHide = true;
                }
               
                await OnLoadData();
                OnGrandtotalegg(TunHeader.LoadDate, TunHeader.AndroidLoadSheet);
                IsRefresh = false;
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }


        private async Task OnLoadData()
        {
            var _tundetailsList = await _global.tunneldetails.GetTunneldetailsproduction(TunHeader.LoadDate, TunHeader.AndroidLoadSheet);
            var _statusList = _tundetailsList.Where(a => a.Load_Status == TunHeader.Load_Status).ToList();
            TunnelDetails_List.Clear();
            TunnelDetails_List.ReplaceRange(_statusList);
        }
        private void OnGrandtotalegg(DateTime _productDate, string _loadsheet)
        {            
            double _subtotal = 0;            
            foreach (var _itm in TunnelDetails_List)
            {
                _subtotal += _itm.Egg_Qty;
            }
            GrandTotal_Egg = _subtotal;
        }
        private async Task OnAcceptLS()
        {
            try
            {
                bool _response = await _global.configurationService.GetInternetConnection();
                if (!_response)
                    return;


                bool _confirmation = await App.Current.MainPage.DisplayAlert("Message Alert", $"You're sure to accept this {GrandTotal_Egg} eggs?", "Yes", "No");
                if (_confirmation == false)
                    return;

                TunnelHeader _tunnelheader = new TunnelHeader()
                {
                    AGTId = TunHeader.AGTId,
                    User = TunHeader.User,
                    Plate_Number = TunHeader.Plate_Number,
                    DateAdded = TunHeader.DateAdded,
                    DateEdited = TunHeader.DateEdited,
                    Status_Checked = TunHeader.Status_Checked,
                    User_Checked = TunHeader.User_Checked,
                    LoadDate = TunHeader.LoadDate,
                    LoadNumber = TunHeader.LoadNumber,
                    LoadSequence = TunHeader.LoadSequence,
                    Load_Status = TokenCons.IsForTrans,
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
                    await _global.configurationService.MessageAlert("Somethings is missing");
                    return;
                }

                await _global.tunnelheader.Update_TunnelHeaderStatus(_tunnelheader);
                await _global.tunnelheader.PutapiHeader(_tunnelheader);

                var _tundetailsList = await _global.tunneldetails.GetTunneldetailsproduction(_tunnelheader.LoadDate, _tunnelheader.AndroidLoadSheet);
                foreach (var _itmdetails in _tundetailsList)
                {
                    TunnelDetails _tunnedetails = new TunnelDetails()
                    {
                        AGTPDId = _itmdetails.AGTPDId,
                        LoadNumber = _tunnelheader.LoadNumber,
                        Egg_Qty = _itmdetails.Egg_Qty,
                        Egg_StatType = _itmdetails.Egg_StatType,
                        Load_Status = TokenCons.IsForTrans,
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

                await _global.configurationService.MessageAlert("Successfully accepted");
                await OnClose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }

        }

        private async Task OnReject()
        {
            ParameterModel _parammodel = new ParameterModel()
            { 
                User_Code = TunHeader.User_Code,
                PMT_1 = TunHeader.LoadNumber,
                PMT_2 = TunHeader.AndroidLoadSheet
            };
            TokenSetGet.SetParamModel(_parammodel);
            await PopupNavigation.Instance.PushAsync(new RejectDialog());
        }
        private async Task OnClose()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
