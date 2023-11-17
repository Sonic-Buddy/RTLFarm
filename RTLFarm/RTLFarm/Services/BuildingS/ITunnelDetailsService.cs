using RTLFarm.Models.TunnelModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.BuildingS
{
    public interface ITunnelDetailsService
    {
        Task Insert_TunnelDetails(TunnelDetails _obj);
        Task Update_TunnelDetails(TunnelDetails _obj);
        Task<int> Getexistcount(DateTime _productionDate, string _building, string _loadsheet);
        Task<int> GetSequencecount(DateTime _productionDate);
        Task<int> GetSpecificcount(DateTime _productionDate, string _flockmanCode);
        Task<int> GetExistloadsheet(string _loadsheet);
        Task<int> GetapiExistcount(string _flockmanCode, DateTime _productDate ,string _loadsheet);
        Task<IEnumerable<TunnelDetails>> GetapiTunnelheadermaster();        
        Task<IEnumerable<TunnelDetails>> GetapiTunDetailsGenerated();
        Task<IEnumerable<TunnelDetails>> GetapiSpecificlist(string _subcode, string _egglocation);
        Task<IEnumerable<TunnelDetails>> GetTunneldetailsproduction(DateTime _productionDate, string _loadsheet);
        Task PostapiTunneldetails(TunnelDetails _obj);
        Task PutapiDetails(TunnelDetails _obj);
        Task DeleteAllTunDetails();
    }
}
