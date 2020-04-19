using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Application.Models
{
    public class KrakenComputingResult
    {
        public List<Complex> K { get; } = new List<Complex>();
        public List<double> PhaseSpeed { get; } = new List<double>();
        public List<double> GroupSpeed { get; } = new List<double>();
        public List<List<double>> Modes { get; } = new List<List<double>>();
        public List<double> ZM { get; } = new List<double>();
        public int ModesCount { get; set; }
                       
        public bool TransmissionLossCalculated { get; set; }
        public List<double> Ranges { get; } = new List<double>();
        public List<double> SourceDepths { get; } = new List<double>();
        public List<double> ReceiverDepths { get; } = new List<double>();
        public List<List<List<double>>> TransmissionLoss { get; } = new List<List<List<double>>>();

        public List<string> Warnings { get; } = new List<string>();
    }
}
