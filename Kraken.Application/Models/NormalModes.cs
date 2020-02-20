using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Application.Models
{
    public class NormalModes
    {
        public List<Complex> K { get; set; }     
        public List<double> PhaseSpeed { get; set; }
        public List<double> GroupSpeed { get; set; }
        public List<List<double>> Modes { get; set; }
        public List<double> ZM { get; set; }
    }
}
