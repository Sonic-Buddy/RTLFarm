using Newtonsoft.Json;
using RTLFarm.Helpers;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services.BuildingS;
using RTLFarm.Services.OthersS;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(TunnelHeaderService))]
namespace RTLFarm.Services.BuildingS
{
    public class TunnelHeaderService : ITunnelHeaderService
    {
        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<TunnelHeader>();
        }

        public async Task DeleteAllTunHeader()
        {
            await DbCon();
            await db.DeleteAllAsync<TunnelHeader>();
        }

        public async Task<int> GetapiExistloadsheet(string _flockmanCode, DateTime _productionDate, string _loadsheet)
        {
            string _dateString = _productionDate.ToString("yyyy-MM-dd");
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync($"api/AG_TunnelProd/GetExistloadsheet/{_flockmanCode}/{_dateString}/{_loadsheet}");
            var _returnCount = JsonConvert.DeserializeObject<int>(json);
            return _returnCount;
        }

        public async Task<int> GetapiSequenceno()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/AG_TunnelProd/GetMaxSequence/");
            var _returnCount = JsonConvert.DeserializeObject<int>(json);
            return _returnCount;
        }

        public async Task<IEnumerable<TunnelHeader>> GetapiSpecificlist(string _flockmanCode, string _buildingCode)
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync($"api/AG_TunnelProd/GetFlockBuilding/{_flockmanCode}/{_buildingCode}");
            var _tunheaderList = JsonConvert.DeserializeObject<IEnumerable<TunnelHeader>>(json);
            return _tunheaderList;
        }

        public async Task<IEnumerable<TunnelHeader>> GetapiTunHeaderGenerated()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/AG_TunnelProd/GetGeneratedStatus");
            var _tunheaderList = JsonConvert.DeserializeObject<IEnumerable<TunnelHeader>>(json);
            return _tunheaderList;
        }

        public Task<int> Getexistcount(DateTime _productionDate, string _building, string _loadsheet)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetExistloadsheet(string _loadsheet)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelHeader>().ToListAsync();
            var _returncount = _masterlist.Where(a => a.AndroidLoadSheet == _loadsheet).Count();
            return _returncount;
        }

        public async Task<int> GetExistloadsheet(TunnelHeader _obj)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelHeader>().ToListAsync();
            var _userList = _masterlist.Where(a => a.User_Code == _obj.User_Code && a.LoadDate.Date == _obj.LoadDate.Date).ToList();
            var _returnCount = _userList.Where(b => b.AndroidLoadSheet == _obj.AndroidLoadSheet && b.Load_Status == _obj.Load_Status).Count();
            return _returnCount;
        }

        public async Task<int> GetExistls(TunnelHeader _obj)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelHeader>().ToListAsync();
            var _returnCount = _masterlist.Where(a => a.User_Code == _obj.User_Code && a.LoadDate.Date == _obj.LoadDate.Date && a.AndroidLoadSheet == _obj.AndroidLoadSheet).Count();
            return _returnCount;
        }

        public async Task<int> GetSequencecount(DateTime _productionDate)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelHeader>().ToListAsync();
            var _returncount = _masterlist.Where(a => a.LoadDate.Date == _productionDate.Date).Count();
            return _returncount;
        }

        public async Task<int> GetSpecificcount(DateTime _productionDate, string _flockmanCode, string _building)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelHeader>().ToListAsync();
            var _returncount = _masterlist.Where(a => a.LoadDate.Date == _productionDate.Date && a.User_Code == _flockmanCode && a.Building_Location == _building).Count();
            return _returncount;
        }

        public async Task<TunnelHeader> GetSpecificmodel(string _usercode, string _subcode, string _loadsheet)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelHeader>().ToListAsync();
            var _returnModel = _masterlist.Where(a => a.User_Code == _usercode && a.AndroidLoadSheet == _subcode && a.LoadNumber == _loadsheet).FirstOrDefault();
            return _returnModel;
        }

        public async Task<IEnumerable<TunnelHeader>> GetSpecifictunheader(TunnelHeader _obj, string _type)
        {
            await DbCon();
            var _masterList = await db.Table<TunnelHeader>().ToListAsync();
            switch (_type)
            {
                default:
                    return _masterList;

                case "OnSyncNUpdate":
                    var _onsyncnupdate = _masterList.Where(a => a.User_Code == _obj.User_Code && a.LoadDate.Date == _obj.LoadDate.Date && a.Load_Status == TokenCons.IsProcessing).ToList();
                    return _onsyncnupdate;

                case "OnDriverView":
                    var _ondriverview = _masterList.Where(b => b.Load_Status == TokenCons.IsProcessing || b.Load_Status == TokenCons.IsForTrans || b.Load_Status == TokenCons.IsCancel).ToList();
                    return _ondriverview;
            }
        }

        public async Task<IEnumerable<TunnelHeader>> GetTunheaderbyflockman(string _flockmanCode)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelHeader>().ToListAsync();
            var _userList = _masterlist.Where(a => a.User_Code == _flockmanCode).ToList();
            var _returnList = _userList.Where(b => b.Load_Status == TokenCons.Closed || b.Load_Status == TokenCons.IsProcessing || b.Load_Status == TokenCons.IsCancel).ToList();
            return _returnList;
        }

        public async Task<IEnumerable<TunnelHeader>> GetTunnelheadermaster()
        {
            await DbCon();
            var _masterList = await db.Table<TunnelHeader>().ToListAsync();
            return _masterList;
        }

        public async Task<TunnelHeader> Insert_TunnelHeader(TunnelHeader _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
            return _obj;
        }

        public async Task<TunnelHeader> PostapiTunnelheader(TunnelHeader _obj)
        {
            var client = ConfigurationClass.GetClient();
            var json = JsonConvert.SerializeObject(_obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/AG_TunnelProd/", content);
            _ = response.Content.ReadAsStringAsync().Result;
            return _obj;
        }

        public async Task PutapiHeader(TunnelHeader _obj)
        {
            var client = ConfigurationClass.GetClient();
            var json = JsonConvert.SerializeObject(_obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/AG_TunnelProd/AndroidUpdate/{_obj.AGTId}", content);
            _ = response.Content.ReadAsStringAsync().Result;
        }

        public async Task<TunnelHeader> Update_TunHeader(TunnelHeader _obj)
        {
            await DbCon();
            var _up = await db.Table<TunnelHeader>().Where(a => a.User_Code == _obj.User_Code && a.AndroidLoadSheet == _obj.AndroidLoadSheet).FirstOrDefaultAsync();
            _up.AGTId = _obj.AGTId;
            _up.User = _obj.User;
            _up.Status_Checked = _obj.Status_Checked;
            _up.User_Checked = _obj.User_Checked;
            _up.Plate_Number = _obj.Plate_Number;
            _up.LoadDate = _obj.LoadDate;
            _up.LoadSequence = _obj.LoadSequence;
            _up.LoadNumber = _obj.LoadNumber;
            _up.Load_Status = _obj.Load_Status;
            _up.DateAdded = _obj.DateAdded;
            _up.DateEdited = _obj.DateEdited;
            _up.UserID = _obj.UserID;
            _up.Building_Location = _up.Building_Location;
            _up.TruckDriverName = _obj.TruckDriverName;
            _up.Override_Status = _obj.Override_Status;
            _up.CSRefNo = _obj.CSRefNo;
            _up.ReasonForReject = _obj.ReasonForReject;
            _up.CancelledLoadSheet = _obj.CancelledLoadSheet;
            _up.Remarks = _obj.Remarks;
            

            _ = await db.UpdateAsync(_up);
            return _obj;
        }

        public async Task Update_TunnelHeader(TunnelHeader _obj)
        {
            await DbCon();
            var _up = await db.Table<TunnelHeader>().Where(a => a.User_Code == _obj.User_Code && a.AndroidLoadSheet == _obj.AndroidLoadSheet).FirstOrDefaultAsync();
            _up.AGTId = _obj.AGTId;
            _up.User = _obj.User;
            _up.Status_Checked = _obj.Status_Checked;
            _up.Plate_Number = _obj.Plate_Number;
            _up.LoadDate = _obj.LoadDate;
            _up.LoadSequence = _obj.LoadSequence;
            _up.LoadNumber = _obj.LoadNumber;
            _up.Load_Status = _obj.Load_Status;
            _up.DateAdded = _obj.DateAdded;
            _up.DateEdited = _obj.DateEdited;
            _up.UserID = _obj.UserID;
            _up.Building_Location = _up.Building_Location;
            _up.TruckDriverName = _obj.TruckDriverName;
            _up.Override_Status = _obj.Override_Status;
            _up.CSRefNo = _obj.CSRefNo;
            _up.ReasonForReject = _obj.ReasonForReject;
            _up.CancelledLoadSheet = _obj.CancelledLoadSheet;
            _up.Remarks = _obj.Remarks;            

            _ = await db.UpdateAsync(_up);
        }

        public async Task Update_TunnelHeaderStatus(TunnelHeader _obj)
        {
            await DbCon();
            var _up = await db.Table<TunnelHeader>().Where(a => a.AGTId == _obj.AGTId && a.User_Code == _obj.User_Code && a.AndroidLoadSheet == _obj.AndroidLoadSheet).FirstOrDefaultAsync();
               
            _up.Plate_Number = _obj.Plate_Number;
            _up.LoadDate = _obj.LoadDate;
            _up.LoadNumber = _obj.LoadNumber;
            _up.LoadSequence = _obj.LoadSequence;
            _up.Load_Status = _obj.Load_Status;
            _up.ReasonForReject = _obj.ReasonForReject;
            _up.CancelledLoadSheet = _obj.CancelledLoadSheet;
            _up.Remarks = _obj.Remarks;

            _ = await db.UpdateAsync(_up);
        }
    }
}
