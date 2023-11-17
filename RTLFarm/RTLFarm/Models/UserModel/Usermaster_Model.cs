using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.UserModel
{
    public class Usermaster_Model
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public int UserDeptId { get; set; }
        public int WarehouseAssignedId { get; set; }
        public string UserStatus { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string LoginStatus { get; set; }
        public string SalesmanCode { get; set; }
        public string WhSpecificLocation { get; set; }
        public string CreateStatus { get; set; }
    }
}
