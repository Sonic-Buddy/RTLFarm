using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.ConfigurationModel
{
    public class DeviceInformationModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Salesman_Code { get; set; }
        public string Salesman_Name { get; set; }
        public string Device_Code { get; set; }
        public string Device_Name { get; set; }
        public string Asset_Code { get; set; }
        public string Site { get; set; }
        public DateTime Register { get; set; }
        public string Device_Status { get; set; }
    }
}
