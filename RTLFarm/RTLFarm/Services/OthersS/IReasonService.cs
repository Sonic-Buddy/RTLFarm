using RTLFarm.Models.OthersModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.OthersS
{
    public interface IReasonService
    {
        Task Insert_Reason(ReasonModel _obj);
        Task Update_Reason(ReasonModel _obj);
        Task<int> Getexistcount(string _codeType);
        Task<IEnumerable<ReasonModel>> GetapiReasonmaster();
        Task<IEnumerable<ReasonModel>> GetReasonMaster();
        Task<IEnumerable<ReasonModel>> GetSpecificList(string _section);
    }
}
