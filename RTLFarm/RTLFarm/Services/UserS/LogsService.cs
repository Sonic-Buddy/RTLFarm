using RTLFarm.Models.UserModel;
using RTLFarm.Services.UserS;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogsService))]
namespace RTLFarm.Services.UserS
{
    public class LogsService : ILogsService
    {
        static SQLiteAsyncConnection db;
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);

            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<UserLogsModel>();
        }
        public async Task<IEnumerable<UserLogsModel>> Getlogsmasterlist()
        {
            await DbCon();
            var _masterlist = await db.Table<UserLogsModel>().ToListAsync();
            return _masterlist;
        }

        public async Task<IEnumerable<UserLogsModel>> GetSpecificdate(DateTime _logsdate)
        {
            await DbCon();
            var _masterlist = await db.Table<UserLogsModel>().ToListAsync();
            var _return_Datelist = _masterlist.Where(a => a.Logs_Create.Date == _logsdate.Date).ToList();
            return _return_Datelist;
        }

        public async Task<IEnumerable<UserLogsModel>> GetSpecifictype(string _transtype)
        {
            await DbCon();
            var _masterlist = await db.Table<UserLogsModel>().ToListAsync();
            var _return_Typelist = _masterlist.Where(a => a.Trans_Type == _transtype).ToList();
            return _return_Typelist;
        }

        public async Task<IEnumerable<UserLogsModel>> GetSpecificuser(string _usercode)
        {
            await DbCon();
            var _masterlist = await db.Table<UserLogsModel>().ToListAsync();
            var _return_Userlist = _masterlist.Where(a => a.Acc_Code == _usercode).ToList();
            return _return_Userlist;
        }

        public async Task Insertlogs_local(UserLogsModel _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
        }
    }
}
