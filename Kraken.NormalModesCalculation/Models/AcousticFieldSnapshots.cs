using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation.Models
{
    public class AcousticFieldSnapshots
    {
        public List<double> Ranges { get; set; }
        public List<double> SourceDepths { get; set; }
        public List<double> ReceiverDepths { get; set; }
        public List<List<List<Complex>>> Snapshots { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
