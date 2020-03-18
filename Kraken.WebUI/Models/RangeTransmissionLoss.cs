using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kraken.WebUI.Models
{
    public class RangeTransmissionLoss
    {
        public double Range { get; set; }
        public IEnumerable<Tuple<double, IEnumerable<Tuple<double, double>>>> TransmissionLoss { get; set; }
    }
}
