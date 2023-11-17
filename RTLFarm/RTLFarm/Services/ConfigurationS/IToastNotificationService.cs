using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Services.ConfigurationS
{
    public interface IToastNotificationService
    {
        void NativeToast(string _type, string _message);
        void ToastNotification(string _type, string _message);
    }
}
