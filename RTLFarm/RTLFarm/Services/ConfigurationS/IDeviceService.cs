using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Services.ConfigurationS
{
    public interface IDeviceService
    {
        string GetDeviceSerial();
        string GetDeviceName();
    }
}
