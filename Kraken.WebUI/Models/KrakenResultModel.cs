using System.Collections.Generic;

namespace Kraken.WebUI.Models
{
    public class KrakenResultModel
    {
        public IEnumerable<double> K { get; set; }
        public IEnumerable<double> Alpha { get; set; }
        public IEnumerable<double> PhaseSpeed { get; set; }
        public IEnumerable<double> GroupSpeed { get; set; }
        public IEnumerable<DepthModes> Modes { get; set; }
        public int ModesCount { get; set; }

        public bool TransmissionLossCalculated { get; set; }
        public IEnumerable<double> Ranges { get; set; }
        public IEnumerable<double> SourceDepths { get; set; }
        public IEnumerable<IEnumerable<IEnumerable<double>>> TransmissionLoss { get; set; }
    }
}
