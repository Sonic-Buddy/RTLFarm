using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.UserModel
{
    public class UserLogsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Acc_Code { get; set; }
        public string Acc_Name { get; set;}
        public string Acc_Description { get; set;}
        public string Trans_Type { get; set;}
        public string Trans_Desc { get; set;}        
        public DateTime Trans_Create { get; set;}
        public DateTime Logs_Create { get; set;}
        public string Logs_Code { get; set; }
        public string Logs_Status { get; set;}
    }
}
