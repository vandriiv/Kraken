using System.Collections.Generic;

namespace Kraken.Application.Models
{
    public class NormalModes
    {
        public List<double> K { get; set; }
        public List<double> Alpha { get; set; }
        public List<double> PhaseSpeed { get; set; }
        public List<double> GroupSpeed { get; set; }
        public List<List<double>> Modes { get; set; }
        public List<double> ZM { get; set; }
    }
}
