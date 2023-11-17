using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.OthersModel
{
    public class ReasonModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CodeId { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string ReasonCodeType { get; set; }
        public string BusinessUnit { get; set; }
        public string Section { get; set; }
        public int SequenceNo { get; set; }
        public DateTime Date_Created { get; set; }
        public string Status_Type { get; set; }
    }
}
