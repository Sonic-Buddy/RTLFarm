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

[assembly: Dependency(typeof(BuildingLocationService))]
namespace RTLFarm.Services.OthersS
{
    public class BuildingLocationService : IBuildingLocationService
    {
        static SQLiteAsyncConnection db;
        //Configation SQL lite
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<BuildingLocation>();
        }
        public async Task<IEnumerable<BuildingLocation>> GetapiBuildingLocmaster()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/AG_TunnelLocations");
            var _buildingloc = JsonConvert.DeserializeObject<IEnumerable<BuildingLocation>>(json);
            return _buildingloc;
        }

        public async Task<string> GetBuildingLocation(string _buildingCode, string _type)
        {
            await DbCon();
            var _masterList = await db.Table<BuildingLocation>().ToListAsync();
            switch (_type)
            {
                case "Initial":
                    var _returnName = _masterList.Where(a => a.AG_Code == _buildingCode).FirstOrDefault();
                    return _returnName.AG_Loc_Code;

                case "Description":
                    var _returnDesc = _masterList.Where(b => b.AG_Code == _buildingCode).FirstOrDefault();
                    return _returnDesc.AG_Location;
            }
            return "nothing follow";
        }

        public async Task<IEnumerable<BuildingLocation>> GetBuildingLocmaster()
        {
            await DbCon();
            var _buildingloclist = await db.Table<BuildingLocation>().ToListAsync();
            return _buildingloclist;
        }

        public async Task<int> Getexistcount(int _serverId)
        {
            await DbCon();
            var _buildingloc = await db.Table<BuildingLocation>().Where(a => a.AGTLId == _serverId).CountAsync();
            return _buildingloc;
        }

        public async Task Insert_BuildingLoc(BuildingLocation _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
        }

        public async Task Update_BuildingLoc(BuildingLocation _obj)
        {
            await DbCon();
            var _up = await db.Table<BuildingLocation>().Where(a => a.AGTLId == _obj.AGTLId).FirstOrDefaultAsync();
            _up.Id = _obj.Id;
            _up.AG_Location = _obj.AG_Location;
            _up.AG_Loc_Code = _obj.AG_Loc_Code;
            _up.AG_Code = _obj.AG_Loc_Code;
            _up.Sequence_No = _obj.Sequence_No;
            await db.UpdateAsync(_up);
        }
    }
}
