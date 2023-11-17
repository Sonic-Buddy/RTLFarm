using RTLFarm.Models.ConfigurationModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.ConfigurationS
{
    public interface IMobileService
    {
        Task<DeviceInformationModel> RegisterDevice(DeviceInformationModel _obj);
        Task<DeviceInformationModel> Update_Devicelocal(DeviceInformationModel _obj);
        Task<DeviceInformationModel> GetDevicespecificModel(DeviceInformationModel _obj, string _type);
        Task<int> GetDeviceisexist(string _usercode);
        Task<string> GetDeviceuser(string _serialCode);

        Task Postapi_Deviceinfo(DeviceInformationModel _obj);
        Task<string> GetapiSalesmancode(string _serialCode, string _busite);
        Task<int> GetSpecificInteger(int _id = 0, string _type = null);
        Task<bool> GetSendingImage(int _id);
    }
}
