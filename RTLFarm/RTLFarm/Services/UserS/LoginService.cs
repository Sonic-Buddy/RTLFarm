using Newtonsoft.Json;
using RTLFarm.Helpers;
using RTLFarm.Models.StatusModel;
using RTLFarm.Models.UserModel;
using RTLFarm.Services.OthersS;
using RTLFarm.Services.UserS;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoginService))]
namespace RTLFarm.Services.UserS
{
    public class LoginService : ILoginService
    {
        static SQLiteAsyncConnection db;
        static async Task DbCon()
        {
            if (db != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, ConfigurationClass.Localdb);

            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<Usermaster_Model>();
        }
        public async Task<Usermaster_Model> GetAccountspecificmodel(string _userverifycode)
        {
            await DbCon();
            var userlist = await db.Table<Usermaster_Model>().ToListAsync();
            var usermodel = userlist.Where(a => a.SalesmanCode == _userverifycode).FirstOrDefault();
            return usermodel;
        }

        public async Task<Usermaster_Model> GetActiveaccount()
        {
            await DbCon();
            var userlist = await db.Table<Usermaster_Model>().ToListAsync();
            var usermodel = userlist.Where(a => a.LoginStatus == TokenCons.IsActive).FirstOrDefault();
            return usermodel;
        }

        public async Task<Usermaster_Model> Getapidataaccount(Usermaster_Model _obj)
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync($"api/UserMasterModels/Getspecificacc/{_obj.UserName}/{_obj.Password}");
            var account = JsonConvert.DeserializeObject<Usermaster_Model>(json);
            return account;
        }

        public async Task<Usermaster_Model> Getapioveridelogin(Usermaster_Model _obj)
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync($"api/UserMasterModels/Getadminaccount/{_obj.UserName}/{_obj.Password}");
            var _userList = JsonConvert.DeserializeObject<IEnumerable<Usermaster_Model>>(json);
            var _userModel = _userList.Where(a => a.UserName == _obj.UserName && a.Password == _obj.Password).FirstOrDefault();
            return _userModel;
        }

        public async Task<IEnumerable<Usermaster_Model>> GetapiUsermastermaster()
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/UserMasters/GetFlockman/");
            var _usermaster = JsonConvert.DeserializeObject<IEnumerable<Usermaster_Model>>(json);
            return _usermaster;
        }

        public async Task<Usermaster_Model> Getapiusermodel(string _usersalescode)
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/UserMasters/GetFlockman/");
            var _usermaster = JsonConvert.DeserializeObject<IEnumerable<Usermaster_Model>>(json);
            var _returnModel = _usermaster.Where(a => a.SalesmanCode == _usersalescode).FirstOrDefault();
            return _returnModel;

        }

        public async Task<int> GetExistcount(int _serverId, string _flockmanCode)
        {
            await DbCon();
            var _masterList = await db.Table<Usermaster_Model>().ToListAsync();
            var _returnCount = _masterList.Where(a => a.UserId == _serverId && a.SalesmanCode == _flockmanCode).Count();
            return _returnCount;
        }

        public async Task<Usermaster_Model> GetSpecificAccount(string _username, string _password)
        {
            await DbCon();
            var _masterList = await db.Table<Usermaster_Model>().ToListAsync();
            var _returnModel = _masterList.Where(a => a.UserName == _username && a.Password == _password).FirstOrDefault();
            return _returnModel;
        }

        public async Task<Usermaster_Model> GetSpecificmodel(string _usersalescode)
        {
            await DbCon();
            var _masterList = await db.Table<Usermaster_Model>().ToListAsync();
            var _returnModel = _masterList.Where(a => a.SalesmanCode == _usersalescode).FirstOrDefault();
            return _returnModel;
        }

        public async Task<string> Getuserinformation(string _userverifyCode, string _type)
        {
            await DbCon();
            var _usermastermodel = await db.Table<Usermaster_Model>().Where(a => a.SalesmanCode == _userverifyCode).FirstOrDefaultAsync();
            switch (_type)
            {
                case "Userfullname":
                    return _usermastermodel.UserFullName;

                case "Useremailname":
                    return _usermastermodel.UserName;

                case "Password":
                    return _usermastermodel.Password;

            }
            return string.Empty;
        }

        public async Task<IEnumerable<Usermaster_Model>> Getuserlogin(string _username, string _password)
        {
            await DbCon();
            var user = await db.Table<Usermaster_Model>().Where(x => x.UserName == _username && x.Password == _password).ToListAsync();
            return user;
        }

        public async Task<int> GetVerifyuser(string _usersalescode)
        {
            var client = ConfigurationClass.GetClient();
            var json = await client.GetStringAsync("api/UserMasters/GetFlockman/");
            var _usermster = JsonConvert.DeserializeObject<IEnumerable<Usermaster_Model>>(json);
            var _returnCount = _usermster.Where(a => a.SalesmanCode == _usersalescode).Count();
            return _returnCount;
        }

        public async Task Insertusers_local(Usermaster_Model _obj)
        {
            await DbCon();
            await db.InsertAsync(_obj);
        }

        public async Task<Usermaster_Model> Putapiaccount(Usermaster_Model _obj, string _type = null)
        {
            var client = ConfigurationClass.GetClient();
            if (_type == "Updatesalesman")
            {
                var salescode = Preferences.Get("salesmancode", string.Empty);
                var up = await db.Table<Usermaster_Model>().Where(x => x.SalesmanCode == _obj.SalesmanCode).FirstOrDefaultAsync();
                up.Id = _obj.Id;
                up.UserId = _obj.UserId;
                up.UserFullName = _obj.UserFullName;
                up.UserName = _obj.UserName;
                up.Password = _obj.Password;
                up.UserRole = _obj.UserRole;
                up.UserDeptId = _obj.UserDeptId;
                up.WarehouseAssignedId = _obj.WarehouseAssignedId;
                up.UserStatus = _obj.UserStatus;
                up.PasswordHash = _obj.PasswordHash;
                up.PasswordSalt = _obj.PasswordSalt;
                up.LoginStatus = _obj.LoginStatus;
                var upAccount = await Updateusers_local(up);
                var json = JsonConvert.SerializeObject(upAccount);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PutAsync($"api/UserMasterModels/Putaccount/", content);

                return up;
            }
            else
            {
                var up = await db.Table<Usermaster_Model>().Where(x => x.LoginStatus == _obj.LoginStatus).FirstOrDefaultAsync();
                up.UserId = _obj.UserId;
                up.UserFullName = _obj.UserFullName;
                up.UserName = _obj.UserName;
                up.Password = _obj.Password;
                up.UserRole = _obj.UserRole;
                up.UserDeptId = _obj.UserDeptId;
                up.WarehouseAssignedId = _obj.WarehouseAssignedId;
                up.UserStatus = _obj.UserStatus;
                up.PasswordHash = _obj.PasswordHash;
                up.PasswordSalt = _obj.PasswordSalt;
                up.SalesmanCode = _obj.SalesmanCode;

                var json = JsonConvert.SerializeObject(up);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PutAsync($"api/Auth/UpdateAccount", content);

                return up;
            }
        }

        public async Task UpdateAcc_local(Usermaster_Model _obj)
        {
            await DbCon();
            var up = await db.Table<Usermaster_Model>().Where(x => x.SalesmanCode == _obj.SalesmanCode).FirstOrDefaultAsync();
            up.Id = _obj.Id;
            up.UserId = _obj.UserId;
            up.UserFullName = _obj.UserFullName;
            up.UserName = _obj.UserName;
            up.Password = _obj.Password;
            up.UserRole = _obj.UserRole;
            up.UserDeptId = _obj.UserDeptId;
            up.WarehouseAssignedId = _obj.WarehouseAssignedId;
            up.UserStatus = _obj.UserStatus;
            up.PasswordHash = _obj.PasswordHash;
            up.PasswordSalt = _obj.PasswordSalt;
            up.LoginStatus = _obj.LoginStatus;
            up.SalesmanCode = _obj.SalesmanCode;
            up.WhSpecificLocation = _obj.WhSpecificLocation;
            up.CreateStatus = _obj.CreateStatus;

            await db.UpdateAsync(up);
        }

        public async Task<Usermaster_Model> Updateusers_local(Usermaster_Model _obj)
        {
            await DbCon();
            var up = await db.Table<Usermaster_Model>().Where(x => x.LoginStatus == _obj.LoginStatus && x.SalesmanCode == _obj.SalesmanCode).FirstOrDefaultAsync();
            up.Id = _obj.Id;
            up.UserId = _obj.UserId;
            up.UserFullName = _obj.UserFullName;
            up.UserName = _obj.UserName;
            up.Password = _obj.Password;
            up.UserRole = _obj.UserRole;
            up.UserDeptId = _obj.UserDeptId;
            up.WarehouseAssignedId = _obj.WarehouseAssignedId;
            up.UserStatus = _obj.UserStatus;
            up.PasswordHash = _obj.PasswordHash;
            up.PasswordSalt = _obj.PasswordSalt;
            await db.UpdateAsync(up);

            return up;
        }
    }
}
