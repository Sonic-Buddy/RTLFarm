using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services.BuildingS;
using RTLFarm.Services.DummyS;
using SQLite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(DummyTunDetailsService))]
namespace RTLFarm.Services.DummyS
{
    public class DummyTunDetailsService : IDummyTunDetailsService
    {
        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<DummyTunDetails>();
        }

        public async Task DeleteAllDummy()
        {
            await DbCon();
            await db.DeleteAllAsync<DummyTunDetails>();
        }

        public Task<int> GetDummyexistcount(DateTime _productionDate, string _building, string _loadsheet)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetDummySequencecount(DateTime _productionDate)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetDummySpecificcount(DateTime _productionDate, string _flockmanCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DummyTunDetails>> GetDummyTundetailsmaster()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DummyTunDetails>> GetDummyTundetailsproduction(DateTime _productionDate, string _loadstatus)
        {
            await DbCon();
            var _masterList = await db.Table<DummyTunDetails>().ToListAsync();
            var _returnList = _masterList.Where(a => a.Production_Date.Date == _productionDate.Date && a.Load_Status == _loadstatus).ToList();
            return _returnList;
        }

        public async Task<IEnumerable<DummyTunDetails>> GetSpecifictundetails(DateTime _productionDate, string _loadsheet)
        {
            await DbCon();
            var _masterlist = await db.Table<DummyTunDetails>().ToArrayAsync();
            var _returnlist = _masterlist.Where(a => a.Production_Date.Date == _productionDate.Date && a.Android_LoadSheet == _loadsheet).ToList();
            return _returnlist;
        }

        public async Task Insert_DummyTunDetails(DummyTunDetails _obj)
        {
            await DbCon();

            var _dummModel = await db.Table<DummyTunDetails>().Where(a => a.Android_LoadSheet == _obj.Android_LoadSheet && a.Egg_StatType == _obj.Egg_StatType).FirstOrDefaultAsync();
            if(_dummModel == null)
            {
                _  = await db.InsertAsync(_obj);
            }
            else
            {
                _dummModel.Egg_Qty += _obj.Egg_Qty;
                _dummModel.DateCreated = _obj.DateCreated;
                
                _ = await db.UpdateAsync(_dummModel);
            }
        }

        public async Task RemoveinList(DummyTunDetails _obj)
        {
            await DbCon();
            await db.DeleteAsync(_obj);
        }

        public Task Update_DummyTunDetails(DummyTunDetails _obj)
        {
            throw new NotImplementedException();
        }
    }
}
