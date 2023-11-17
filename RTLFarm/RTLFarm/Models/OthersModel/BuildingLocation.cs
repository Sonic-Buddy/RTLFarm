using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.OthersModel
{
    public class BuildingLocation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AGTLId { get; set; }
        public string AG_Location { get; set; }
        public string AG_Loc_Code { get; set; }
        public string AG_Code { get; set; }
        public int Sequence_No { get; set; }
    }
}
