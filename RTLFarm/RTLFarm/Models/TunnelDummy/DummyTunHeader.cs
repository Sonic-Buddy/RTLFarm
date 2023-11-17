using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.TunnelDummy
{
    public class DummyTunHeader
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 
        public int AGTId { get; set; }
        public string User { get; set; } //User Full Name
        public string User_Checked { get; set; } //Disregard
        public string Status_Checked { get; set; } //Unchecked
        public string Plate_Number { get; set; } //Plate No
        public DateTime LoadDate { get; set; } //Date Today
        public int LoadSequence { get; set; } //Depende sa Load Sheet
        public string LoadNumber { get; set; } //LS0000001
        public string Load_Status { get; set; } //Completed
        public DateTime DateAdded { get; set; } //Disregard
        public DateTime DateEdited { get; set; } //Disregard
        public int UserID { get; set; } //Users Id
        public string Building_Location { get; set; } //Users Location
        public string TruckDriverName { get; set; } //Truck
        public string Override_Status { get; set; } //Disregard
        public string CSRefNo { get; set; } //Disregard
        public string AndroidLoadSheet { get; set; } //AndroidUniqueLoadSheet
        public string ReasonForReject { get; set; }
        public string CancelledLoadSheet { get; set; }
        public string Remarks { get; set; }
        public string User_Code { get; set; }
    }
}
