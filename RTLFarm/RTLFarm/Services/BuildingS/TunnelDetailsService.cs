using Newtonsoft.Json;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Services.BuildingS;
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

[assembly: Dependency(typeof(TunnelDetailsService))]
namespace RTLFarm.Services.BuildingS
{
    public class TunnelDetailsService : ITunnelDetailsService
    {
        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<TunnelDetails>();
        }

        public async Task DeleteAllTunDetails()
        {
            await DbCon();
            await db.DeleteAllAsync<TunnelDetails>();
        }

        public Task<int> GetapiExistcount(string _flockmanCode, DateTime _productDate, string _loadsheet)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TunnelDetails>> GetapiSpecificlist(string _subcode, string _egglocation)
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync($"api/AG_TunnelProdDet/GetAndroidLoadSheet/{_subcode}/{_egglocation}");
            var _tundetailsList = JsonConvert.DeserializeObject<IEnumerable<TunnelDetails>>(json);
            return _tundetailsList;
        }

        public async Task<IEnumerable<TunnelDetails>> GetapiTunDetailsGenerated()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/AG_TunnelProdDet/GetGeneratedStatus");
            var _tundetailsList = JsonConvert.DeserializeObject<IEnumerable<TunnelDetails>>(json);
            return _tundetailsList;
        }

        public Task<IEnumerable<TunnelDetails>> GetapiTunnelheadermaster()
        {
            throw new NotImplementedException();
        }

        public async Task<int> Getexistcount(DateTime _productionDate, string _building, string _loadsheet)
        {
            await DbCon();
            var _masterlist = await db.Table<TunnelDetails>().ToListAsync();
            var _returnCount = _masterlist.Where(a => a.Production_Date.Date == _productionDate.Date && a.Remarks == _building && a.AndroidLoadSheet == _loadsheet).Count();
            return _returnCount;
        }

        public Task<int> GetExistloadsheet(string _loadsheet)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetSequencecount(DateTime _productionDate)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetSpecificcount(DateTime _productionDate, string _flockmanCode)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TunnelDetails>> GetTunneldetailsproduction(DateTime _productionDate, string _loadsheet)
        {
            await DbCon();
            var _masterList = await db.Table<TunnelDetails>().ToListAsync();
            var _returnList = _masterList.Where(a => a.Production_Date.Date == _productionDate.Date && a.AndroidLoadSheet == _loadsheet).ToList();
            return _returnList;
        }
            
        public async Task Insert_TunnelDetails(TunnelDetails _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
        }

        public async Task PostapiTunneldetails(TunnelDetails _obj)
        {
            var client = ConfigurationClass.GetClient();
            var json = JsonConvert.SerializeObject(_obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/AG_TunnelProdDet/", content);
            _ = response.Content.ReadAsStringAsync().Result;
        }

        public async Task PutapiDetails(TunnelDetails _obj)
        {
            var client = ConfigurationClass.GetClient();
            var json = JsonConvert.SerializeObject(_obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/AG_TunnelProdDet/{_obj.AGTPDId}", content);
            _ = response.Content.ReadAsStringAsync().Result;
        }

        public async Task Update_TunnelDetails(TunnelDetails _obj)
        {
            await DbCon();            
            var _up = await db.Table<TunnelDetails>().Where(a => a.Egg_StatType == _obj.Egg_StatType && a.AndroidLoadSheet == _obj.AndroidLoadSheet).FirstOrDefaultAsync();
            _up.AGTPDId = _obj.AGTPDId;
            _up.LoadNumber = _obj.LoadNumber;
            _up.Egg_Qty = _obj.Egg_Qty;
            _up.Egg_StatType = _obj.Egg_StatType;
            _up.Load_Status = _obj.Load_Status;
            _up.DateCreated = _obj.DateCreated;
            _up.DateUpdated = _obj.DateUpdated;
            _up.Remarks = _obj.Remarks;
            _up.Sequence = _obj.Sequence;
            _up.UserSequence = _obj.UserSequence;

            await db.UpdateAsync(_up);
        }
    }
}
