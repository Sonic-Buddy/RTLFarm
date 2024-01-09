using MvvmHelpers;
using RTLFarm.Models.UserModel;
using RTLFarm.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        GlobalDependencyServices _global = new GlobalDependencyServices();

        public async Task OnAddlogs(string _accCode, string _accName, string _accDesc, string _transType, string _transDecs, string _logStatus)
        {
            string _datestring = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
            string _logscode = $"{_accCode}{_datestring}";
            UserLogsModel _logsModel = new UserLogsModel()
            {
                Acc_Code = _accCode,
                Acc_Name = _accName,
                Acc_Description = _accDesc,
                Trans_Type = _transType,
                Trans_Desc = _transDecs,
                Trans_Create = DateTime.Now,
                Logs_Create = DateTime.Now,
                Logs_Code = _logscode, 
                Logs_Status = _logStatus
            };
            await Task.Delay(1000);
            await _global.logsService.Insertlogs_local(_logsModel);
        }
    }
}
