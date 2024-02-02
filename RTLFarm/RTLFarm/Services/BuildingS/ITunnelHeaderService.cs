using RTLFarm.Models.StatusModel;
using RTLFarm.Models.TunnelModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.BuildingS
{
    public interface ITunnelHeaderService
    {
        Task<TunnelHeader> Insert_TunnelHeader(TunnelHeader _obj);
        Task Update_TunnelHeader(TunnelHeader _obj);
        Task<TunnelHeader> Update_TunHeader(TunnelHeader _obj);
        Task Update_TunnelHeaderStatus(TunnelHeader _obj);
        Task<int> Getexistcount(string _building, string _loadsheet);
        Task<int> GetSequencecount(DateTime _productionDate);
        Task<int> GetSpecificcount(DateTime _productionDate, string _flockmanCode, string _building);
        Task<int> GetExistloadsheet(TunnelHeader _obj, string _usercode);
        Task<int> GetExistls(TunnelHeader _obj);
        Task<int> GetapiExistloadsheet(string _flockmanCode, DateTime _productionDate, string _loadsheet);
        Task<int> GetapiSequenceno();
        Task<TunnelHeader> PostapiTunnelheader(TunnelHeader _obj);
        Task PutapiHeader(TunnelHeader _obj, string _foredit);
        Task<IEnumerable<TunnelHeader>> GetapiTunHeaderGenerated();
        Task<TunnelHeader> GetserverModel(string _androidLS, string _usercode);
        Task<IEnumerable<TunnelHeader>> GetapiSpecificlist(string _flockmanCode, string _buildingCode);
        Task<IEnumerable<TunnelHeader>> GetTunnelheadermaster();
        Task<List<TunnelHeader>> GetTunnelheaderList();
        Task<TunnelHeader> GetSpecificmodel(string _usercode, string _subcode, string _loadsheet);
        Task<IEnumerable<TunnelHeader>> GetTunheaderbyflockman(string _flockmanCode);
        Task<IEnumerable<TunnelHeader>> GetSpecifictunheader(TunnelHeader _obj, string _type);
        Task DeleteAllTunHeader();
        Task DeleteHarvestbyUser(string _userId);
    }
}
