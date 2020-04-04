using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation
{
    public class CalculatedModesInfo
    {       
        public int NMedia { get; set; }
        public int NTot { get; set; }
        public int NMat { get; set; }
        public List<int> N { get; set; }
        public List<string> Material { get; set; }
        public List<double> Depth { get; set; }
        public List<double> Rho { get; set; }
        public double Frequency { get; set; }
        public List<double> Z { get; set; }

        public int ModesCount { get; set; }
        public List<Complex> K { get; set; }

        public string BCTop { get; set; }
        public Complex CPTop { get; set; }
        public Complex CSTop { get; set; }
        public double RhoTop { get; set; }
        public double DepthTop { get; set; }

        public string BCBottom { get; set; }
        public Complex CPBottom { get; set; }
        public Complex CSBottom { get; set; }
        public double RhoBottom { get; set; }
        public double DepthBottom { get; set; }

        public List<List<Complex>> Phi { get; set; } = new List<List<Complex>>();
    }
}
