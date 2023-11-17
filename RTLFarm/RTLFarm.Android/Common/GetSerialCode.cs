using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RTLFarm.Droid.Common;
using RTLFarm.Services.ConfigurationS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(GetSerialCode))]
namespace RTLFarm.Droid.Common
{
    public class GetSerialCode : IDeviceService
    {
        public string GetDeviceName()
        {
            var context = Android.App.Application.Context;
            string devname = Android.OS.Build.Model;
            return devname;
        }

        public string GetDeviceSerial()
        {
            var context = Android.App.Application.Context;
            string id = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Secure.AndroidId);
            return id;
        }
    }
}