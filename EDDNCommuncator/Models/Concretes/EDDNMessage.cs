namespace EDDNCommunicator.Models.Concretes
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    public class EDDNMessage
    {
        [DataMember(Name = "$schemaRef")]
        public string SchemaRef { get; set; }

        [DataMember(Name = "header")]
        public EDDNMessageHeader Header { get; set; }

        [DataMember(Name = "message")]
        public JObject Message { get; set; }
    }
}
