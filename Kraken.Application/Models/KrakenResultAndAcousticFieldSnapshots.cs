using Kraken.Calculation.Models;
using System.Collections.Generic;

namespace Kraken.Application.Models
{
    public class KrakenResultAndAcousticFieldSnapshots
    {
        public KrakenResult KrakenResult { get; set; }
        public AcousticFieldSnapshots AcousticFieldSnapshots { get; set; }

        public List<List<List<double>>> TransmissionLoss { get; } = new List<List<List<double>>>();
    }
}
