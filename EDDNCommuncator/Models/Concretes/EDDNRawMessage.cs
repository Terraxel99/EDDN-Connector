namespace EDDNCommunicator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EDDNRawMessage
    {
        /// <summary>
        /// Gets or sets the raw message.
        /// </summary>
        /// <value>
        /// The raw message.
        /// </value>
        public string RawMessage { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public EDDNMessageType Type { get; set; }
    }
}
