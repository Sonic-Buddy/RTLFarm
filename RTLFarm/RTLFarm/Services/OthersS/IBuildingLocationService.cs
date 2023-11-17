using RTLFarm.Models.OthersModel;
using RTLFarm.Models.StatusModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.OthersS
{
    public interface IBuildingLocationService
    {
        Task Insert_BuildingLoc(BuildingLocation _obj);
        Task Update_BuildingLoc(BuildingLocation _obj);
        Task<int> Getexistcount(int _serverId);
        Task<string> GetBuildingLocation(string _buildingCode, string _type);
        Task<IEnumerable<BuildingLocation>> GetapiBuildingLocmaster();
        Task<IEnumerable<BuildingLocation>> GetBuildingLocmaster();
    }
}
