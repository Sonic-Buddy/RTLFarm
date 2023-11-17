using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.ConfigurationModel
{
    public class ParameterModel
    {
        public int Id { get; set; } = 0;
        public string User_Code { get; set; } = string.Empty;
        public string User_Name { get; set; } = string.Empty;
        public string User_Site { get; set; } = string.Empty;
        public string User_Role { get; set; } = string.Empty;
        public string PMT_1 { get; set; } = string.Empty;
        public string PMT_2 { get; set; } = string.Empty;
        public string PMT_3 { get; set; } = string.Empty;
        public string PMT_4 { get; set; } = string.Empty;
        public string PMT_5 { get; set; } = string.Empty;
        public DateTime Params_Date { get; set; } = DateTime.Now;
    }
}
