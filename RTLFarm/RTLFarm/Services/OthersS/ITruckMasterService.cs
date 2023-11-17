using RTLFarm.Models.OthersModel;
using RTLFarm.Models.StatusModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.OthersS
{
    public interface ITruckMasterService
    {
        Task Insert_Truck(TruckMaster_Model _obj);
        Task Update_Truck(TruckMaster_Model _obj);
        Task<int> Getexistcount(string _plateno);
        Task<IEnumerable<TruckMaster_Model>> GetapiTruckmaster();
        Task<IEnumerable<TruckMaster_Model>> GetTruckmaster();
    }
}
