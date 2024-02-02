using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Services;
using RTLFarm.Services.ConfigurationS;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RTLFarm
{
    public class ConfigurationClass
    {
        IConfigurationService _configurationService;

        GlobalDependencyServices _global = new GlobalDependencyServices();

        public const string Localdb = "SSDIrtlfarm.db";
        public const string APIurl = "http://192.168.1.217:1334/";
        public static string BaseIPaddress;

        public ConfigurationClass()
        {
            _configurationService = DependencyService.Get<IConfigurationService>();
            _global = new GlobalDependencyServices();
        }

        public async Task SetDefaultipaddress()
        {
            var _configModel = new ConfigurationDeviceModel()
            {
                Id = 1,
                IP_Address = "http://192.168.1.120:2004/",
                Is_Use = true
            };

            var _defaultIP = await _global.configurationService.GetDefaultIP(_configModel, "Default");
            if (_defaultIP == null)
            {
                await _configurationService.AddIPaddress(_configModel);
            }
        }

        public static HttpClient GetClient()
        {
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(APIurl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return _client;
        }

        public static HttpClient GetSecondClient()
        {
            string _baseURL = "http://192.168.1.120:2004/";
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseURL);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return _client;
        }
    }
}
