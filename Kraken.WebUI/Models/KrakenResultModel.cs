using System.Collections.Generic;

namespace Kraken.WebUI.Models
{
    public class KrakenResultModel
    {
        public List<double> K { get; } = new List<double>();
        public List<double> Alpha { get; } = new List<double>();
        public List<double> PhaseSpeed { get; } = new List<double>();
        public List<double> GroupSpeed { get; } = new List<double>();
        public List<DepthModes> Modes { get; } = new List<DepthModes>();
        public int ModesCount { get; set; }

        public bool TransmissionLossCalculated { get; set; }
        public List<double> Ranges { get; } = new List<double>();
        public List<double> SourceDepths { get; } = new List<double>();
        public List<double> ReceiverDepths { get; } = new List<double>();
        public List<TLAtSourceDepth> TransmissionLoss { get; } = new List<TLAtSourceDepth>();

        public List<string> Warnings { get; } = new List<string>();
    }
}
