using RTLFarm.Models.StatusModel;
using RTLFarm.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RTLFarm.Services.UserS
{
    public interface ILoginService
    {
        Task Insertusers_local(Usermaster_Model _obj);
        Task UpdateAcc_local(Usermaster_Model _obj);
        Task<Usermaster_Model> Updateusers_local(Usermaster_Model _obj);
        Task<IEnumerable<Usermaster_Model>> Getuserlogin(string _username, string _password);
        Task<Usermaster_Model> GetSpecificAccount(string _username, string _password);
        Task<Usermaster_Model> GetAccountspecificmodel(string _userverifycode);
        Task<string> Getuserinformation(string _userverifyCode, string _type);
        Task<int> GetExistcount(int _serverId, string _flockmanCode);
        Task<Usermaster_Model> GetActiveaccount();
        Task<Usermaster_Model> Getapioveridelogin(Usermaster_Model _obj);
        Task<Usermaster_Model> Putapiaccount(Usermaster_Model _obj, string _type = null);
        Task<Usermaster_Model> Getapiusermodel(string _usersalescode);
        Task<Usermaster_Model> GetSpecificmodel(string _usersalescode);

        Task<int> GetVerifyuser(string _usersalescode);
        Task<IEnumerable<Usermaster_Model>> GetapiUsermastermaster();
    }
}
