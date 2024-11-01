using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Models
{
    public class LostItemRequests : AuditData
    {
        public string Description { get; set; }
        public string?  Color { get; set; }
        public string?  Size { get; set; }
        public string?  Brand { get; set; }
        public string?  Model { get; set; }
        public string?  DistinguishingFeatures { get; set; }
        public string?  ItemCategory { get; set; }
        public string?  SerialNumber { get; set; }
        public DateTime? DateTimeWhenLost { get; set; }
        public string?  Location { get; set; }
        public decimal? ItemValue { get; set; }
        public string?  ItemPhoto { get; set; }
        public string?  ProofofOwnership { get; set; }
        public string?  HowtheItemLost { get; set; }
        public string?  ReferenceNumber { get; set; }
        public string?  AdditionalInformation { get; set; }
        public string?  OtherRelevantDetails { get; set; }
        
    }
}
