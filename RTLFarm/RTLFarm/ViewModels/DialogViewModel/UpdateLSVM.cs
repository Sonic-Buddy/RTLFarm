using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using RTLFarm.Helpers;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.DialogViewModel
{
    public class UpdateLSVM : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        string _iconName, _staticloadingText, _loadingText;
        decimal _counttotal, _countforeach;
        public string IconName { get => _iconName; set => SetProperty(ref _iconName, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public decimal CountTotal { get => _counttotal; set => SetProperty(ref _counttotal, value); }
        public decimal CountForeach { get => _countforeach; set => SetProperty(ref _countforeach, value); }
        public AsyncCommand RefreshCommand { get; }


        public UpdateLSVM()
        {
            RefreshCommand = new AsyncCommand(OnRefresh);
        }

        private async Task OnRefresh()
        {
            await OnUpdateProduction();
        }

        private async Task OnUpdateProduction()
        {
            try
            {
                var _parammodel = TokenSetGet.GetParamModel();
                TunnelHeader _tunheaderparam = new TunnelHeader()
                {
                    User_Checked = _parammodel.User_Code,
                    LoadDate = _parammodel.Params_Date
                };
                var _tunheaderList = await _global.tunnelheader.GetSpecifictunheader(_tunheaderparam, "OnSyncNUpdate");
                foreach (var _itmheader in _tunheaderList)
                {
                    TunnelHeader _tunnelheader = new TunnelHeader()
                    {
                        User = _itmheader.User,
                        Plate_Number = _itmheader.Plate_Number,
                        DateAdded = _itmheader.DateAdded,
                        DateEdited = _itmheader.DateEdited,
                        Status_Checked = _itmheader.Status_Checked,
                        User_Checked = _itmheader.User_Checked,
                        LoadDate = _itmheader.LoadDate,
                        LoadNumber = _parammodel.PMT_1,
                        LoadSequence = Convert.ToInt32(_parammodel.PMT_2),
                        Load_Status = TokenCons.Closed,
                        UserID = _itmheader.UserID,
                        Building_Location = _itmheader.Building_Location,
                        TruckDriverName = _itmheader.TruckDriverName,
                        Override_Status = _itmheader.Override_Status,
                        CSRefNo = _itmheader.CSRefNo,
                        AndroidLoadSheet = _itmheader.AndroidLoadSheet
                    };

                    var _Isexistapi = await _global.tunnelheader.GetExistloadsheet(_tunnelheader);
                    if (_Isexistapi != 0)
                    {
                        MessagingCenter.Send(this, "parameterstring", "somethings is missing");
                        await PopupNavigation.Instance.PopAsync(true);
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
                            Load_Status = TokenCons.Closed,
                            DateCreated = _itmdetails.DateCreated,
                            DateUpdated = _itmdetails.DateUpdated,
                            Remarks = _itmdetails.Remarks,
                            Egg_Location = _itmdetails.Egg_Location,
                            Production_Date = _itmdetails.Production_Date,
                            Sequence = _tunnelheader.LoadSequence,
                            UserSequence = _itmdetails.UserSequence,
                            AndroidLoadSheet = _itmdetails.AndroidLoadSheet
                        };

                        await _global.tunneldetails.Update_TunnelDetails(_tunnedetails);
                    }
                }


                MessagingCenter.Send(this, "parameterstring", "Sync successfully");
                await PopupNavigation.Instance.PopAsync(true);
            }
            catch (Exception ex)
            {                
                MessagingCenter.Send(this, "parameterstring", ex.Message);
                await PopupNavigation.Instance.PopAsync(true);
            }

            
        }
    }
}
