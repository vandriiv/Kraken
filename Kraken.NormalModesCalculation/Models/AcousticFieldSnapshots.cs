using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation.Models
{
    public class AcousticFieldSnapshots
    {
        public List<double> Ranges { get; } = new List<double>();
        public List<double> SourceDepths { get; } = new List<double>();
        public List<double> ReceiverDepths { get; } = new List<double>();
        public List<List<List<Complex>>> Snapshots { get; } = new List<List<List<Complex>>>();
        public List<string> Warnings { get;} = new List<string>();
    }
}
