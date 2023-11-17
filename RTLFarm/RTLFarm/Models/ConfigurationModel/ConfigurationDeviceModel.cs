using RTLFarm.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.ConfigurationModel
{
    public class ConfigurationDeviceModel : ViewModelBase
    {
        int _ipId;
        string _ipAddress;
        bool _ipUsed;

        [PrimaryKey, AutoIncrement]
        public int Id { get => _ipId; set => SetProperty(ref _ipId, value); }
        public string IP_Address { get => _ipAddress; set => SetProperty(ref _ipAddress, value); }
        public bool Is_Use { get => _ipUsed; set => SetProperty(ref _ipUsed, value); }
    }
}
