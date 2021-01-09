using System;
using System.Collections.Generic;
using System.Text;

namespace EDDNCommunicator.Constants
{
    public static class EDDNConstants
    {
        public static class Connection
        {
            /// <summary>
            /// The base domain for EDDN.
            /// </summary>
            public const string EDDNDomain = "tcp://eddn.edcd.io";

            /// <summary>
            /// The EDDN base port.
            /// </summary>
            public const int EDDNPort = 9500;
        }
    }
}
