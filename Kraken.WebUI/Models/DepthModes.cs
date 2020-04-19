using System.Collections.Generic;

namespace Kraken.WebUI.Models
{
    public class DepthModes
    {
        public double Depth { get; }
        public IEnumerable<double> Modes { get; }

        public DepthModes(double depth, IEnumerable<double> modes)
        {
            Depth = depth;
            Modes = modes;
        }
    }
}
