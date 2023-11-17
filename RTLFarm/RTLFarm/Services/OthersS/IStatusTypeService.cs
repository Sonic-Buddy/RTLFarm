using RTLFarm.Models.StatusModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.OthersS
{
    public interface IStatusTypeService
    {
        Task Insert_Statustype(StatusType_Model _obj);
        Task Update_Statustype(StatusType_Model _obj);        
        Task<int> Getexistcount(int _serverId);
        Task<IEnumerable<StatusType_Model>> GetapiStatustypemaster();
        Task<IEnumerable<StatusType_Model>> GetStatustypemaster();
        Task<IEnumerable<StatusType_Model>> GetSubStatustypemaster(string _eggcode);
    }
}
