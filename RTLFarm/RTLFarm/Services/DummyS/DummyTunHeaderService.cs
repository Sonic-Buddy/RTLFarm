using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services.DummyS;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(DummyTunHeaderService))]
namespace RTLFarm.Services.DummyS
{
    public class DummyTunHeaderService : IDummyTunHeaderService
    {
        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<DummyTunHeader>();
        }

        public async Task DeleteAllDummy()
        {
            await DbCon();
            await db.DeleteAllAsync<DummyTunHeader>();
        }

        public Task<int> GetDummyexistcount(DateTime _productionDate, string _building, string _loadsheet)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetDummyExistloadsheet(string _loadsheet)
        {
            await DbCon();
            var _masterlist = await db.Table<DummyTunHeader>().ToListAsync();
            var _returncount = _masterlist.Where(a => a.AndroidLoadSheet == _loadsheet).Count();
            return _returncount;
        }

        public async Task<int> GetDummySequencecount(DateTime _productionDate)
        {
            await DbCon();
            var _masterlist = await db.Table<DummyTunHeader>().ToListAsync();
            var _returncount = _masterlist.Where(a => a.LoadDate.Date == _productionDate.Date).Count();
            return _returncount;
        }

        public async Task<int> GetDummySpecificcount(DateTime _productionDate, string _flockmanCode)
        {
            await DbCon();
            var _masterlist = await db.Table<DummyTunHeader>().ToListAsync();
            var _returncount = _masterlist.Where(a => a.LoadDate.Date == _productionDate.Date && a.User_Code == _flockmanCode).Count();
            return _returncount;
        }

        public Task<IEnumerable<DummyTunHeader>> GetDummyTunheadermaster()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DummyTunHeader>> GetSpecificloadsheet(DateTime _productionDate, string _flockmanCode)
        {
            await DbCon();
            var _masterList = await db.Table<DummyTunHeader>().ToListAsync();
            var _returnList = _masterList.Where(a => a.LoadDate.Date == _productionDate.Date && a.User_Code == _flockmanCode).ToList();
            return _returnList;
        }

        public async Task<DummyTunHeader> Insert_DummyTunHeader(DummyTunHeader _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
            return _obj;
        }

        public Task Update_DummyTunHeader(DummyTunHeader _obj)
        {
            throw new NotImplementedException();
        }
    }
}
