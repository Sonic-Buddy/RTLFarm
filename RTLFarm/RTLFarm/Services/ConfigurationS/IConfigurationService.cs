using RTLFarm.Models.ConfigurationModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.ConfigurationS
{
    public interface IConfigurationService
    {
        Task<ConfigurationDeviceModel> GetDefaultIP(ConfigurationDeviceModel _obj, string _type);
        Task<IEnumerable<ConfigurationDeviceModel>> GetIPaddressList(ConfigurationDeviceModel _obj = null, string _type = null);
        Task<ConfigurationDeviceModel> AddIPaddress(ConfigurationDeviceModel _obj, string _type = null);
        Task<ConfigurationDeviceModel> UpdateIPaddress(ConfigurationDeviceModel _obj, string _type = null);
        Task DeleteIPaddress(ConfigurationDeviceModel _obj = null, string _type = null);

        Task<bool> MessageAlert(string _caption);
        Task<bool> GetInternetConnection();
    }
}
