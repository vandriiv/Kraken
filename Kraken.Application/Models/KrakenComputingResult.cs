using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Application.Models
{
    public class KrakenComputingResult
    {
        public List<Complex> K { get; set; }     
        public List<double> PhaseSpeed { get; set; }
        public List<double> GroupSpeed { get; set; }
        public List<List<double>> Modes { get; set; }
        public List<double> ZM { get; set; }
        public int ModesCount { get; set; }

        
        public bool TransmissionLossCalculated { get; set; }
        public List<double> Ranges { get; set; }
        public List<double> SourceDepths { get; set; }
        public List<List<List<double>>> TransmissionLoss { get; set; }
    }
}
