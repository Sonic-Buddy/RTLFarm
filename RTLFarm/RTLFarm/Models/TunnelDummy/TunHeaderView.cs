using RTLFarm.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTLFarm.Models.TunnelDummy
{
    public class TunHeaderView : ViewModelBase
    {
        int _id, _agtid, _userid, _loadsequence;
        string _user, _userchecked, _statuschecked, _platenumber, _loadnumber, _loadstatus, _buildinglocation, _truckdrivername, _overridestatus, _csrefno, _androidloadsheet, _reasonforreject, _cancelledloadsheet, _remarks, _usercode;
        DateTime _loaddate, _dateadded, _dateedited;
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public int AGTId { get => _agtid; set => SetProperty(ref _agtid, value); }
        public string User { get => _user; set => SetProperty(ref _user, value); }
        public string User_Checked { get => _userchecked; set => SetProperty(ref _userchecked, value); }
        public string Status_Checked { get => _statuschecked; set => SetProperty(ref _statuschecked, value); }
        public string Plate_Number { get => _platenumber; set => SetProperty(ref _platenumber, value); }
        public DateTime LoadDate { get => _loaddate; set => SetProperty(ref _loaddate, value); }
        public int LoadSequence { get => _loadsequence; set => SetProperty(ref _loadsequence, value); }
        public string LoadNumber { get => _loadnumber; set => SetProperty(ref _loadnumber, value); }
        public string Load_Status { get => _loadstatus; set => SetProperty(ref _loadstatus, value); }
        public DateTime DateAdded { get => _dateadded; set => SetProperty(ref _dateadded, value); }
        public DateTime DateEdited { get => _dateedited; set => SetProperty(ref _dateedited, value); }
        public int UserID { get => _userid; set => SetProperty(ref _userid, value); }
        public string Building_Location { get => _buildinglocation; set => SetProperty(ref _buildinglocation, value); }
        public string TruckDriverName { get => _truckdrivername; set => SetProperty(ref _truckdrivername, value); }
        public string Override_Status { get => _overridestatus; set => SetProperty(ref _overridestatus, value); }
        public string CSRefNo { get => _csrefno; set => SetProperty(ref _csrefno, value); }
        public string AndroidLoadSheet { get => _androidloadsheet; set => SetProperty(ref _androidloadsheet, value); }
        public string ReasonForReject { get => _reasonforreject; set => SetProperty(ref _reasonforreject, value); }
        public string CancelledLoadSheet { get => _cancelledloadsheet; set => SetProperty(ref _cancelledloadsheet, value); }
        public string Remarks { get => _remarks; set => SetProperty(ref _remarks, value); }
        public string User_Code { get => _usercode; set => SetProperty(ref _usercode, value); }
    }
}
