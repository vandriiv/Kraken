using System.Collections.Generic;

namespace Kraken.WebUI.Models
{
    public class TLAtReceiverDepth
    {
        public double ReceiverDepth { get; set; }
        public List<double> TransmissionLoss { get; } = new List<double>();
    }
}
