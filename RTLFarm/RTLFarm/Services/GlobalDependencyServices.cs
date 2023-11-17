using RTLFarm.Services.BuildingS;
using RTLFarm.Services.ConfigurationS;
using RTLFarm.Services.DummyS;
using RTLFarm.Services.OthersS;
using RTLFarm.Services.UserS;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RTLFarm.Services
{
    public class GlobalDependencyServices
    {

        public IToastNotificationService toastnotify = DependencyService.Get<IToastNotificationService>();
        public IConfigurationService configurationService = DependencyService.Get<IConfigurationService>();
        public IDeviceService deviceService = DependencyService.Get<IDeviceService>();
        public IMobileService mobileService = DependencyService.Get<IMobileService>();
        public ILoginService loginService = DependencyService.Get<ILoginService>();
        public ILogsService logsService = DependencyService.Get<ILogsService>();

        public IStatusTypeService statustype = DependencyService.Get<IStatusTypeService>();
        public ITruckMasterService truckmaster = DependencyService.Get<ITruckMasterService>();
        public IReasonService reasons = DependencyService.Get<IReasonService>();

        public ITunnelHeaderService tunnelheader = DependencyService.Get<ITunnelHeaderService>();
        public ITunnelDetailsService tunneldetails = DependencyService.Get<ITunnelDetailsService>();

        public IDummyTunHeaderService dummytunheader = DependencyService.Get<IDummyTunHeaderService>();
        public IDummyTunDetailsService dummytundetails = DependencyService.Get<IDummyTunDetailsService>();

        public IBuildingLocationService buildinglocation = DependencyService.Get<IBuildingLocationService>();
    }
}
