using RTLFarm.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.UserS
{
    public interface ILogsService
    {
        Task Insertlogs_local(UserLogsModel _obj);
        Task<IEnumerable<UserLogsModel>> GetSpecificuser(string _usercode);
        Task<IEnumerable<UserLogsModel>> GetSpecifictype(string _transtype);
        Task<IEnumerable<UserLogsModel>> GetSpecificdate(DateTime _logsdate);
        Task<IEnumerable<UserLogsModel>> Getlogsmasterlist();
        Task Postlogs_API(UserLogsModel _obj);
        Task DeletePerWeeks();
    }
}
