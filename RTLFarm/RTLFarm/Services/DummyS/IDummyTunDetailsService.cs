using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.DummyS
{
    public interface IDummyTunDetailsService
    {
        Task Insert_DummyTunDetails(DummyTunDetails _obj);
        Task Update_DummyTunDetails(DummyTunDetails _obj);
        Task<int> GetDummyexistcount(DateTime _productionDate, string _building, string _loadsheet);
        Task<int> GetDummySequencecount(DateTime _productionDate);
        Task<int> GetDummySpecificcount(DateTime _productionDate, string _flockmanCode);
        Task RemoveinList(DummyTunDetails _obj);
        Task DeleteAllDummy();
        Task<IEnumerable<DummyTunDetails>> GetDummyTundetailsmaster();
        Task<IEnumerable<DummyTunDetails>> GetDummyTundetailsproduction(DateTime _productionDate, string _loadstatus);
        Task<IEnumerable<DummyTunDetails>> GetSpecifictundetails(DateTime _productionDate, string _loadsheet);
    }
}
