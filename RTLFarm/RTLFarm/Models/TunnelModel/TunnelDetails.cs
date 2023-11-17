using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.TunnelModel
{
    public class TunnelDetails
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AGTPDId { get; set; }
        public string LoadNumber { get; set; } //LoadSheet
        public int Egg_Qty { get; set; }
        public string Egg_StatType { get; set; }
        public string Load_Status { get; set; }
        public DateTime DateCreated { get; set; } //Datetoday
        public DateTime DateUpdated { get; set; }
        public string Remarks { get; set; }
        public string Egg_Location { get; set; }
        public DateTime Production_Date { get; set; }
        public int Sequence { get; set; } //Sequence
        public int UserSequence { get; set; } //UserId
        public string AndroidLoadSheet { get; set; }
    }
}
