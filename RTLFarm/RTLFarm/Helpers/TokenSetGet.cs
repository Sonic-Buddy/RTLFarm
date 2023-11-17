using RTLFarm.Models.ConfigurationModel;
using RTLFarm.Models.TunnelDummy;
using RTLFarm.Models.TunnelModel;
using RTLFarm.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Helpers
{
    public class TokenSetGet
    {

        private static Usermaster_Model UserModel { get; set; }
        public static void Set_UserModel(Usermaster_Model _obj)
        {
            UserModel = _obj;
        }
        public static Usermaster_Model Get_UserModel()
        {
            return UserModel;
        }       

        private static ParameterModel ParamModel;
        public static void SetParamModel(ParameterModel _parammodel)
        {
            ParamModel = _parammodel;
        }
        public static ParameterModel GetParamModel()
        {
            return ParamModel;
        }

        private static TunHeaderView Tunnel_Header { get; set; }
        public static void Set_Tunnel_Header(TunHeaderView _obj)
        {
            Tunnel_Header = _obj;
        }
        public static TunHeaderView Get_Tunnel_Header()
        {
            return Tunnel_Header;
        }

        private static TunnelHeader Tun_Header { get; set; }
        public static void Set_Tun_Header(TunnelHeader _obj)
        {
            Tun_Header = _obj;
        }
        public static TunnelHeader Get_Tun_Header()
        {
            return Tun_Header;
        }

        private static string LoginStatus { get; set; }
        public static void Set_LoginStatus(string _logstat)
        {
            LoginStatus = _logstat;
        }
        public static string Get_LoginStatus()
        {
            return LoginStatus;
        }


    }
}
