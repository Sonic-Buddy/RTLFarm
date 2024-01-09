using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using RTLFarm.Helpers;
using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using RTLFarm.Views;
using RTLFarm.Views.BuildingPage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm.ViewModels.AdminVM
{
    public class AdSettingViewModel : ViewModelBase
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        int _enterport, _enterp1, _enterp2, _enterp3, _enterp4;
        string _enteraddress, _ipaddress, _usercodes;
        bool _isnotenteraddress, _isenteraddress, _isrefreshing, _isrefreshtrans, _isrefreshlogs;
        ConfigurationDeviceModel _selectip;
        TunnelHeader _selecttunheader;
        public bool IsNotEnteraddress { get => _isnotenteraddress; set => SetProperty(ref _isnotenteraddress, value); }
        public bool IsEnteraddress { get => _isenteraddress; set => SetProperty(ref _isenteraddress, value); }
        public bool IsRefreshing { get => _isrefreshing; set => SetProperty(ref _isrefreshing, value); }
        public bool IsRefreshTrans { get => _isrefreshtrans; set => SetProperty(ref _isrefreshtrans, value); }
        public bool IsRefreshLogs { get => _isrefreshlogs; set => SetProperty(ref _isrefreshlogs, value); }
        public string User_Codes { get => _usercodes; set => SetProperty(ref _usercodes, value); }
        public ConfigurationDeviceModel SelectIP { get => _selectip; set => SetProperty(ref _selectip, value); }
        public TunnelHeader Select_TunHeader { get => _selecttunheader; set => SetProperty(ref _selecttunheader, value); }
        //
        public string EnterAddress
        {
            get => _enteraddress;
            set
            {
                if (value == _enteraddress)
                    return;
                _enteraddress = value;
                OnPropertyChanged();
            }
        }
        public string IPAddress
        {
            get => _ipaddress;
            set
            {
                if (value == _ipaddress)
                    return;
                _ipaddress = value;
                OnPropertyChanged();
            }
        }
        public int EnterP1
        {
            get => _enterp1;
            set
            {
                if (value == _enterp1)
                    return;
                _enterp1 = value;
                OnPropertyChanged();
            }
        }
        public int EnterP2
        {
            get => _enterp2;
            set
            {
                if (value == _enterp2)
                    return;
                _enterp2 = value;
                OnPropertyChanged();
            }
        }
        public int EnterP3
        {
            get => _enterp3;
            set
            {
                if (value == _enterp3)
                    return;
                _enterp3 = value;
                OnPropertyChanged();
            }
        }
        public int EnterP4
        {
            get => _enterp4;
            set
            {
                if (value == _enterp4)
                    return;
                _enterp4 = value;
                OnPropertyChanged();
            }
        }
        public int EnterPORT
        {
            get => _enterport;
            set
            {
                if (value == _enterport)
                    return;
                _enterport = value;
                OnPropertyChanged();
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
            }
        }

        TunnelHeader _selectuserlist;
        public TunnelHeader SelectUser_List
        {
            get => _selectuserlist;
            set
            {
                if (_selectuserlist == value) { return; }
                _selectuserlist = value;
                OnPropertyChanged();
                User_Codes = value.User_Code;
                OnSortbyuser(value.User_Code);
                
            }
        }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand IsChangeIPCommand { get; }
        public AsyncCommand IsNotChangeIPCommand { get; }
        public AsyncCommand AddIPCommand { get; }
        public AsyncCommand RefreshIPCommand { get; }
        public AsyncCommand RefreshTransCommand { get; }
        public AsyncCommand RefreshLogsCommand { get; }
        public AsyncCommand SetupCommand { get; }
        public AsyncCommand LogoutCommand { get; }
        public AsyncCommand BtnTestCommand { get; }
        public AsyncCommand DelHarvestCommand { get; }
        public AsyncCommand SelectCommand { get; set; }

        public ObservableRangeCollection<ConfigurationDeviceModel> IP_List { get; set; }
        public ObservableRangeCollection<TunnelHeader> TunnelHeader_List { get; set; }
        public ObservableRangeCollection<TunnelHeader> TunUser_List { get; set; }
        public ObservableRangeCollection<StatusType_Model> SubStatusType_List { get; set; }
        public ObservableRangeCollection<UserLogsModel> Userlogs_List { get; set; }
        //

        public AdSettingViewModel()
        {
            IP_List = new ObservableRangeCollection<ConfigurationDeviceModel>();
            TunnelHeader_List = new ObservableRangeCollection<TunnelHeader>();
            SubStatusType_List = new ObservableRangeCollection<StatusType_Model>();
            TunUser_List = new ObservableRangeCollection<TunnelHeader>();
            Userlogs_List =new ObservableRangeCollection<UserLogsModel>();
            RefreshCommand = new AsyncCommand(OnRefresh);
            LogoutCommand = new AsyncCommand(OnLogout);
            IsChangeIPCommand = new AsyncCommand(OnChangeEnter);
            IsNotChangeIPCommand = new AsyncCommand(OnNotChangeEnter);
            RefreshIPCommand = new AsyncCommand(OnIPaddress);
            AddIPCommand = new AsyncCommand(OnInsertIP);
            RefreshTransCommand = new AsyncCommand(OnRefreshTrans);
            RefreshLogsCommand = new AsyncCommand(OnRefreshLogs);
            DelHarvestCommand = new AsyncCommand(OnDeletebydata);
            BtnTestCommand = new AsyncCommand(OnSyncuserlog);
        }
        private async Task OnRefresh()
        {            
            IsEnteraddress = false;
            IsNotEnteraddress = true;

            await OnIPaddress();
            
        }

        private async Task OnInsertIP()
        {
            try
            {
                string _slash = "/";
                if (IsNotEnteraddress == true)
                {
                    if (string.IsNullOrWhiteSpace(EnterAddress) || string.IsNullOrEmpty(EnterAddress))
                    {
                        await _global.configurationService.MessageAlert("Missing entry, Please fill in the IP Address");
                        return;
                    }

                    if (EnterPORT == 0)
                    {
                        await _global.configurationService.MessageAlert("Missing entry, Please insert PORT");
                        return;
                    }

                    IPAddress = $"http://{EnterAddress}:{EnterPORT}{_slash}";
                    var _database = await _global.configurationService.GetIPaddressList();
                    if (_database.Where(d => d.IP_Address == IPAddress).FirstOrDefault() == null)
                    {
                        var _newIP = new ConfigurationDeviceModel()
                        {
                            IP_Address = IPAddress,
                            Is_Use = false
                        };
                                                
                        await _global.configurationService.AddIPaddress(_newIP);
                        await _global.configurationService.MessageAlert("Database added succesfully.");
                        await OnIPaddress();
                    }
                    else
                    {
                        await _global.configurationService.MessageAlert("IP address is already exist.");
                        return;
                    }
                }
                else
                {
                    if (EnterP1 == 0)
                    {
                        await _global.configurationService.MessageAlert("Missing entry, Please fill in the IP Address");
                        return;
                    }

                    if (EnterP2 == 0)
                    {
                        await _global.configurationService.MessageAlert("Missing entry, Please fill in the IP Address");
                        return;
                    }

                    if (EnterP3 == 0)
                    {
                        await _global.configurationService.MessageAlert("Missing entry, Please fill in the IP Address");
                        return;
                    }

                    if (EnterP4 == 0)
                    {
                        await _global.configurationService.MessageAlert("Missing entry, Please fill in the IP Address");
                        return;
                    }

                    if (EnterPORT == 0)
                    {
                        await _global.configurationService.MessageAlert("Missing entry, Please insert PORT");
                        return;
                    }

                    IPAddress = $"http://{EnterP1}.{EnterP2}.{EnterP3}.{EnterP4}:{EnterPORT}{_slash}";                    
                    var _database = await _global.configurationService.GetIPaddressList();
                    if (_database.Where(d => d.IP_Address == IPAddress).FirstOrDefault() == null)
                    {
                        var _newIP = new ConfigurationDeviceModel()
                        {
                            IP_Address = IPAddress,
                            Is_Use = false
                        };
                                                
                        await _global.configurationService.AddIPaddress(_newIP);                        
                        await _global.configurationService.MessageAlert("Database added succesfully.");
                        await OnIPaddress();
                    }
                    else
                    {                        
                        await _global.configurationService.MessageAlert("IP address is already exist."); 
                        return;
                    }
                }
            }
            catch (Exception ex)
            {                
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }

        }

        private async Task OnIPaddress()
        {
            try
            {
                IsEnteraddress = false;
                IsNotEnteraddress = true;
                var _ipList = await _global.configurationService.GetIPaddressList();
                if (_ipList != null)
                {
                    IP_List.Clear();
                    IP_List.ReplaceRange(_ipList);
                }

                IsRefreshing = false;
            }
            catch (Exception ex)
            {                
                _global.toastnotify.NativeToast("Short", ex.Message);
            }
        }
        private async void OnSortbyuser(string _usercode)
        {
            var _tunnelheaderList = await _global.tunnelheader.GetTunheaderbyflockman(_usercode);
            var _tunheadersortdate = _tunnelheaderList.OrderByDescending(a => a.LoadDate).ToList();
            TunnelHeader_List.Clear();
            TunnelHeader_List.ReplaceRange(_tunheadersortdate);
        }
        private async Task OnRefreshTrans()
        {
            var _tunnelheaderList = await _global.tunnelheader.GetTunnelheadermaster();
            if(_tunnelheaderList.Count().Equals(0))
            {
                await _global.configurationService.MessageAlert("No record found.");
                await OnGenerateUser();
                IsRefreshTrans = false;
                return;
            }

            var _tunheadersortdate = _tunnelheaderList.OrderByDescending(a => a.LoadDate).ToList();
            TunnelHeader_List.Clear();
            TunnelHeader_List.ReplaceRange(_tunheadersortdate);

            await OnGenerateUser();
            IsRefreshTrans = false;
        }

        private async Task OnRefreshLogs()
        {
            var _userlogsList = await _global.logsService.Getlogsmasterlist();
            var _userlogssortdate = _userlogsList.OrderByDescending(a => a.Logs_Create).ToList();

            Userlogs_List.Clear();
            Userlogs_List.ReplaceRange(_userlogssortdate);
            IsRefreshLogs = false;
        }

        private async Task OnGenerateUser()
        {
            var _tunuserList = await _global.tunnelheader.GetTunnelheadermaster();
            var _groupUser = await GroupAG_Users(_tunuserList.ToList());

            TunUser_List.Clear();
            TunUser_List.ReplaceRange(_groupUser);
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
        private async Task OnChangeEnter()
        {
            await Task.Delay(10);
            IsEnteraddress = false;
            IsNotEnteraddress = true;
        }
        private async Task OnNotChangeEnter()
        {
            await Task.Delay(10);
            IsEnteraddress = true;
            IsNotEnteraddress = false;
        }

        private async Task OnLogout()
        {
            var route = $"/{nameof(LoginPage)}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task OnDeletebydata()
        {
            try
            {
                if(string.IsNullOrEmpty(User_Codes) || string.IsNullOrWhiteSpace(User_Codes))
                {
                    await _global.configurationService.MessageAlert("Please select user to delete data");
                    return;
                }

                bool _confirmation = await App.Current.MainPage.DisplayAlert("Message Alert", $"You're sure to DELETE this data of {User_Codes}?", "Yes", "No");
                if (_confirmation == false)
                {                    
                    return;
                }

                await _global.tunnelheader.DeleteHarvestbyUser(User_Codes);

            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                return;
            }
        }

        private async Task OnSyncuserlog()
        {
            var loading = UserDialogs.Instance.Loading("Loading. . .");
            loading.Show();
            try
            {
                
                DateTime _todayDate = DateTime.Now.AddDays(-2);
                var _logmasterlist = await _global.logsService.Getlogsmasterlist();
                var _sortdatalist = _logmasterlist.Where(a => a.Logs_Create.Date > _todayDate.Date).ToList();
                foreach (var _item in _sortdatalist)
                {
                    //string _datestring = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
                    //await Task.Delay(500);
                    //string _logscode = $"{_item.Acc_Code}{_datestring}";
                    //_item.Logs_Code = _logscode;
                    await _global.logsService.Postlogs_API(_item);
                }

                await _global.configurationService.MessageAlert("Successfully sync");
                loading.Dispose();
            }
            catch (Exception ex)
            {
                await _global.configurationService.MessageAlert(ex.Message);
                loading.Dispose();
            }
            
        }
        private async Task<List<TunnelHeader>> GroupAG_Users(List<TunnelHeader> _tunheaderList)
        {
            await Task.Delay(10);
            var _Detailslist = _tunheaderList.GroupBy(l => l.User_Code).Select(cl => new TunnelHeader
            {
                AGTId = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().AGTId,
                User = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().User,
                User_Checked = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().User_Checked,
                Status_Checked = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().Status_Checked,
                Plate_Number = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().Plate_Number,
                LoadDate = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().LoadDate,
                LoadSequence = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().LoadSequence,
                LoadNumber = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().LoadNumber,
                Load_Status = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().Load_Status,
                DateAdded = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().DateAdded,
                DateEdited = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().DateEdited,
                UserID = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().UserID,
                Building_Location = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().Building_Location,
                TruckDriverName = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().TruckDriverName,
                Override_Status = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().Override_Status,
                CSRefNo = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().CSRefNo,
                AndroidLoadSheet = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().AndroidLoadSheet,
                ReasonForReject = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().ReasonForReject,
                CancelledLoadSheet = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().CancelledLoadSheet,
                Remarks = cl.Where(a => a.User_Code == cl.Key).FirstOrDefault().Remarks,
                User_Code = cl.Key
            }).ToList();

            return _Detailslist;
        }
    }
}
