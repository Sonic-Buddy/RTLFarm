using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.OthersModel
{
    public class TruckMaster_Model
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TruckPlateNo { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdate { get; set; }
        public string Actual { get; set; }
        public int Crates { get; set; }
        public int Trays { get; set; }
    }
}
