using System.Collections.Generic;

namespace Kraken.WebUI.Models
{
    public class DepthModes
    {
        public double Depth { get; set; }
        public IEnumerable<double> Modes { get; set; }
    }
}
