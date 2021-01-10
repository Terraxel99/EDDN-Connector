using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EDDNCommunicator.Models.Concretes
{
    public class EDDNMessageHeader
    {
        [DataMember(Name = "uploaderID")]
        public string UploaderId { get; set; }

        [DataMember(Name = "softwareName")]
        public string SoftwareName { get; set; }

        [DataMember(Name = "softwareVersion")]
        public string SoftwareVersion { get; set; }

        [DataMember(Name = "gatewayTimestamp")]
        public DateTime GatewayTimestamp { get; set; }
    }
}
