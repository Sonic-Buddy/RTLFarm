using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Services.ConfigurationS;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConfigurationService))]
namespace RTLFarm.Services.ConfigurationS
{
    public class ConfigurationService : IConfigurationService
    {
        static SQLiteAsyncConnection db;
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);

            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<ConfigurationDeviceModel>();
        }
        public async Task<ConfigurationDeviceModel> AddIPaddress(ConfigurationDeviceModel _obj, string _type = null)
        {
            await DbCon();
            await db.InsertAsync(_obj);
            return _obj;
        }

        public async Task DeleteIPaddress(ConfigurationDeviceModel _obj = null, string _type = null)
        {
            await DbCon();
            switch (_type)
            {
                //default:
                //    await db.DeleteAsync<ConfigurationDeviceModel>();
                //    break;

                case "SINGLE":
                    await db.DeleteAsync(_obj);
                    break;
            }
        }

        public async Task<ConfigurationDeviceModel> GetDefaultIP(ConfigurationDeviceModel _obj, string _type)
        {
            await DbCon();
            switch (_type)
            {
                default:
                    var _default = await db.Table<ConfigurationDeviceModel>().Where(a => a.Id == _obj.Id).FirstOrDefaultAsync();
                    return _default;

                case "IsUsed":
                    var _usedIP = await db.Table<ConfigurationDeviceModel>().Where(b => b.Is_Use == true).FirstOrDefaultAsync();
                    return _usedIP;
            }
        }

        public async Task<bool> GetInternetConnection()
        {
            var _currentConnect = Connectivity.NetworkAccess;
            if (_currentConnect.Equals(NetworkAccess.Internet))
            {
                return true;
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Message Alert", "Please, check your internet connection", "OK");
                return false;
            }
        }

        public async Task<IEnumerable<ConfigurationDeviceModel>> GetIPaddressList(ConfigurationDeviceModel _obj = null, string _type = null)
        {
            await DbCon();
            var _ipList = await db.Table<ConfigurationDeviceModel>().ToListAsync();
            return _ipList;
        }

        public async Task<bool> MessageAlert(string _caption)
        {
            await App.Current.MainPage.DisplayAlert("Message Alert", $"{_caption}", "OK");
            return false;
        }

        public async Task<ConfigurationDeviceModel> UpdateIPaddress(ConfigurationDeviceModel _obj, string _type = null)
        {
            await DbCon();
            var _updateIP = await db.Table<ConfigurationDeviceModel>().Where(c => c.Id == _obj.Id).FirstOrDefaultAsync();
            _updateIP = _obj;
            await db.UpdateAsync(_updateIP);

            return null;
        }
    }
}
