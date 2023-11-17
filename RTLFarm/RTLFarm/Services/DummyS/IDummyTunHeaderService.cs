using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.DummyS
{
    public interface IDummyTunHeaderService
    {
        Task<DummyTunHeader> Insert_DummyTunHeader(DummyTunHeader _obj);
        Task Update_DummyTunHeader(DummyTunHeader _obj);
        Task<int> GetDummyexistcount(DateTime _productionDate, string _building, string _loadsheet);
        Task<int> GetDummySequencecount(DateTime _productionDate);
        Task<int> GetDummySpecificcount(DateTime _productionDate, string _flockmanCode);
        Task<int> GetDummyExistloadsheet(string _loadsheet);        
        Task<IEnumerable<DummyTunHeader>> GetDummyTunheadermaster();
        Task DeleteAllDummy();
        Task<IEnumerable<DummyTunHeader>> GetSpecificloadsheet(DateTime _productionDate, string _flockmanCode);
    }
}
