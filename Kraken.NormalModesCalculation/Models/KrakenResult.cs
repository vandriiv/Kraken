using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation.Models
{
    public class KrakenResult
    {
        public List<Complex> K { get; } = new List<Complex>();
        public List<double> PhaseSpeed { get; } = new List<double>();
        public List<double> GroupSpeed { get; } = new List<double>();
        public List<List<double>> Modes { get; } = new List<List<double>>();
        public List<double> ZM { get; } = new List<double>();
        public int ModesCount { get; set; }
        public List<string> Warnings { get; } = new List<string>();
    }
}
