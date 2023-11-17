using Newtonsoft.Json;
using RTLFarm.Models.OthersModel;
using RTLFarm.Models.StatusModel;
using RTLFarm.Services.OthersS;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(TruckMasterService))]
namespace RTLFarm.Services.OthersS
{
    public class TruckMasterService : ITruckMasterService
    {
        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<TruckMaster_Model>();
        }
        public async Task<IEnumerable<TruckMaster_Model>> GetapiTruckmaster()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/AG_Truck");
            var _truckmaster = JsonConvert.DeserializeObject<IEnumerable<TruckMaster_Model>>(json);
            return _truckmaster;
        }

        public async Task<int> Getexistcount(string _plateno)
        {
            await DbCon();
            var _masterList = await db.Table<TruckMaster_Model>().ToListAsync();
            var _returnCount = _masterList.Where(a => a.TruckPlateNo == _plateno).Count();
            return _returnCount;
        }

        public async Task<IEnumerable<TruckMaster_Model>> GetTruckmaster()
        {
            await DbCon();
            var _masterList = await db.Table<TruckMaster_Model>().ToListAsync();
            return _masterList;
        }

        public async Task Insert_Truck(TruckMaster_Model _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
        }

        public async Task Update_Truck(TruckMaster_Model _obj)
        {
            await DbCon();
            var _up = await db.Table<TruckMaster_Model>().Where(a => a.TruckPlateNo == _obj.TruckPlateNo).FirstOrDefaultAsync();

            _up.Id = _obj.Id;
            _up.DateCreated = _obj.DateCreated;
            _up.DateUpdate = _obj.DateUpdate;
            _up.Actual = _obj.Actual;
            _up.Crates = _obj.Crates;
            _up.Trays = _obj.Trays;

            await db.UpdateAsync(_up);
        }
    }
}
