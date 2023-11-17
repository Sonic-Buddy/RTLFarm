using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.StatusModel
{
    public class StatusType_Model
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int STYId { get; set; }
        public string Egg_Code { get; set; }
        public string Egg_Desc { get; set; }
        public int Egg_Qty { get; set; }
        public string Cat_Code { get; set; }
        public int Sequence_No { get; set; }

    }
}
