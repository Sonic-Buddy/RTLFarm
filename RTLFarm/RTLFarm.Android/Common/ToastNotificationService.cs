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

[assembly: Xamarin.Forms.Dependency(typeof(ToastNotificationService))]
namespace RTLFarm.Droid.Common
{
    public class ToastNotificationService : IToastNotificationService
    {
        public void NativeToast(string _type, string _message)
        {
            switch (_type)
            {
                case "Short":
                    Toast.MakeText(Application.Context, _message, ToastLength.Short).Show();
                    break;
                case "Long":
                    Toast.MakeText(Application.Context, _message, ToastLength.Long).Show();
                    break;
            }
        }

        public void ToastNotification(string _type, string _message = "Short")
        {
            switch (_type)
            {
                case "Short":
                    Toast.MakeText(Application.Context, _message, ToastLength.Short).Show();
                    break;
                case "Long":
                    Toast.MakeText(Application.Context, _message, ToastLength.Long).Show();
                    break;
            }
        }
    }
}