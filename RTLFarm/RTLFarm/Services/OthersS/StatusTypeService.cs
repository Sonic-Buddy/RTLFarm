using Newtonsoft.Json;
using RTLFarm.Models.StatusModel;
using RTLFarm.Services.ConfigurationS;
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

[assembly: Dependency(typeof(StatusTypeService))]
namespace RTLFarm.Services.OthersS
{
    public class StatusTypeService : IStatusTypeService
    {
        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<StatusType_Model>();
        }
        public async Task<IEnumerable<StatusType_Model>> GetapiStatustypemaster()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/AG_StatusType");
            var _statustype = JsonConvert.DeserializeObject<IEnumerable<StatusType_Model>>(json);
            return _statustype;
        }

        public async Task<int> Getexistcount(int _serverId)
        {
            await DbCon();
            var _countstatus = await db.Table<StatusType_Model>().Where(a => a.STYId == _serverId).CountAsync();
            return _countstatus;
        }

        public async Task<IEnumerable<StatusType_Model>> GetStatustypemaster()
        {
            await DbCon();
            var _statustypelist = await db.Table<StatusType_Model>().ToListAsync();
            return _statustypelist;
        }

        public async Task<List<StatusType_Model>> GetSubStatustypeList(string _eggcode)
        {
            await DbCon();
            var _masterlist = await db.Table<StatusType_Model>().ToListAsync();
            var _queryList = _masterlist.Where(a => a.Egg_Code == _eggcode).ToList();
            return _queryList;
        }

        public async Task<IEnumerable<StatusType_Model>> GetSubStatustypemaster(string _eggcode)
        {
            await DbCon();
            var _masterlist = await db.Table<StatusType_Model>().ToListAsync();
            var _queryList = _masterlist.Where(a => a.Egg_Code == _eggcode).ToList();
            return _queryList;
        }

        public async Task Insert_Statustype(StatusType_Model _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
        }


        public async Task Update_Statustype(StatusType_Model _obj)
        {
            await DbCon();
            var _up = await db.Table<StatusType_Model>().Where(a => a.STYId == _obj.STYId).FirstOrDefaultAsync();
            _up.Id = _obj.Id;
            _up.Egg_Code = _obj.Egg_Code;
            _up.Egg_Desc = _obj.Egg_Desc;
            _up.Egg_Qty = _obj.Egg_Qty;
            _up.Cat_Code = _obj.Cat_Code;
            _up.Sequence_No = _obj.Sequence_No;
            await db.UpdateAsync(_up);
        }
      
    }
}
