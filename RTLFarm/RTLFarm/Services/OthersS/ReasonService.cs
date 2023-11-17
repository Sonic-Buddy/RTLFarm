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

[assembly: Dependency(typeof(ReasonService))]
namespace RTLFarm.Services.OthersS
{
    public class ReasonService : IReasonService
    {

        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<ReasonModel>();
        }
        public async Task<IEnumerable<ReasonModel>> GetapiReasonmaster()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/ReasonLists");
            var _reasonlist = JsonConvert.DeserializeObject<IEnumerable<ReasonModel>>(json);
            return _reasonlist;
        }

        public async Task<int> Getexistcount(string _codeType)
        {
            await DbCon();
            var _existCount = await db.Table<ReasonModel>().Where(a => a.ReasonCodeType == _codeType).CountAsync();
            return _existCount;
        }

        public async Task<IEnumerable<ReasonModel>> GetReasonMaster()
        {
            await DbCon();
            var _masterList = await db.Table<ReasonModel>().ToListAsync();
            return _masterList;
        }

        public async Task<IEnumerable<ReasonModel>> GetSpecificList(string _section)
        {
            await DbCon();
            var _masterList = await db.Table<ReasonModel>().ToListAsync();
            var _returnList = _masterList.Where(a => a.Section == _section).ToList();
            return _returnList;
        }

        public async Task Insert_Reason(ReasonModel _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
        }

        public async Task Update_Reason(ReasonModel _obj)
        {
            await DbCon();
            var _up = await db.Table<ReasonModel>().Where(a => a.CodeId == _obj.CodeId && a.ReasonCodeType == _obj.ReasonCodeType).FirstOrDefaultAsync();
            _up.Id = _obj.Id;
            _up.ReasonCode = _obj.ReasonCode;
            _up.ReasonName = _obj.ReasonName;
            _up.BusinessUnit = _obj.BusinessUnit;
            _up.Date_Created = _obj.Date_Created;
            _up.Section = _obj.Section;
            _up.SequenceNo = _obj.SequenceNo;
            _up.Status_Type = _obj.Status_Type;

            _ = await db.UpdateAsync(_up);
        }
    }
}
