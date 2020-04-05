using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation.Models
{
    public class KrakenResult
    {
        public List<Complex> K { get; set; }
        public List<double> PhaseSpeed { get; set; }
        public List<double> GroupSpeed { get; set; }
        public List<List<double>> Modes { get; set; }
        public List<double> ZM { get; set; }
        public int ModesCount { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
