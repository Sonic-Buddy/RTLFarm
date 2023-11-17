using Newtonsoft.Json;
using RTLFarm.Helpers;
using RTLFarm.Models.ConfigurationModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RTLFarm.Services.ConfigurationS
{
    public class MobileService : IMobileService
    {
        static SQLiteAsyncConnection db;
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);

            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<DeviceInformationModel>();
        }
        public async Task<string> GetapiSalesmancode(string _serialCode, string _busite)
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync($"api/DeviceMasterModels/Getdataforlist");
            var device = JsonConvert.DeserializeObject<IEnumerable<DeviceInformationModel>>(json);
            var deviceUser = device.Where(x => x.Device_Code == _serialCode && x.Site == _busite).FirstOrDefault();
            return deviceUser.Salesman_Code;
        }

        public async Task<int> GetDeviceisexist(string _usercode)
        {
            await DbCon();
            var _isExistcount = await db.Table<DeviceInformationModel>().Where(x => x.Salesman_Code == _usercode).CountAsync();
            return _isExistcount;
        }

        public async Task<DeviceInformationModel> GetDevicespecificModel(DeviceInformationModel _obj, string _type)
        {
            await DbCon();
            List<DeviceInformationModel> _deviceMasterlist = await db.Table<DeviceInformationModel>().ToListAsync();
            if (_type == "Devicecodeonly")
            {
                var _response = _deviceMasterlist.Where(a => a.Device_Code == _obj.Device_Code && a.Device_Status == TokenCons.IsActive).FirstOrDefault();
                return _response;
            }
            else
            {
                var _response = _deviceMasterlist.Where(a => a.Device_Code == _obj.Device_Code && a.Salesman_Code == _obj.Salesman_Code).FirstOrDefault();
                return _response;
            }
        }

        public async Task<string> GetDeviceuser(string _serialCode)
        {
            await DbCon();
            List<DeviceInformationModel> _devicemasterList = await db.Table<DeviceInformationModel>().ToListAsync();
            var devicemodel = _devicemasterList.Where(x => x.Device_Code == _serialCode && x.Device_Status == TokenCons.IsActive).FirstOrDefault();
            return devicemodel.Salesman_Code;
        }

        public Task<bool> GetSendingImage(int _id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetSpecificInteger(int _id = 0, string _type = null)
        {
            throw new NotImplementedException();
        }

        public async Task Postapi_Deviceinfo(DeviceInformationModel _obj)
        {
            var client = ConfigurationClass.GetClient();

            var device = new DeviceInformationModel()
            {
                Salesman_Code = _obj.Salesman_Code,
                Salesman_Name = _obj.Salesman_Name,
                Device_Code = _obj.Device_Code,
                Device_Name = _obj.Device_Name,
                Asset_Code = _obj.Asset_Code,
                Site = _obj.Site,
                Register = DateTime.Today.Date
            };

            var json = JsonConvert.SerializeObject(device);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"api/DeviceMasterModels/Newdevice", content);
            _ = response.Content.ReadAsStringAsync().Result;
        }

        public async Task<DeviceInformationModel> RegisterDevice(DeviceInformationModel _obj)
        {
            await DbCon();
            _ = await db.InsertAsync(_obj);
            return _obj;
        }

        public async Task<DeviceInformationModel> Update_Devicelocal(DeviceInformationModel _obj)
        {
            await DbCon();
            var up = await db.Table<DeviceInformationModel>().Where(x => x.Device_Code == _obj.Device_Code && x.Salesman_Code == _obj.Salesman_Code).FirstOrDefaultAsync();
            up.Id = _obj.Id;
            up.Salesman_Name = _obj.Salesman_Name;
            up.Device_Name = _obj.Device_Name;
            up.Site = _obj.Site;
            up.Register = _obj.Register;
            up.Device_Status = _obj.Device_Status;
            await db.UpdateAsync(up);
            return up;
        }
    }
}
