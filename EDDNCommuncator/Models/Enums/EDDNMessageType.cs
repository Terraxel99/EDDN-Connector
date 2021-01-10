
namespace EDDNCommunicator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum EDDNMessageType
    {
        UNKNOWN = 0,
        JOURNAL = 1,
        COMMODITY = 2,
        BLACK_MARKET = 3,
        OUTFITTING = 4,
        SHIPYARD = 5,
    }
}
