using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation.Models
{
    public class CalculatedModesInfo
    {       
        public int NMedia { get; set; }
        public int NTot { get; set; }
        public int NMat { get; set; }
        public List<int> N { get; } = new List<int>();
        public List<string> Material { get; } = new List<string>();
        public List<double> Depth { get; } = new List<double>();
        public List<double> Rho { get; } = new List<double>();
        public double Frequency { get; set;  }
        public List<double> Z { get; } = new List<double>();

        public int ModesCount { get; set; }
        public List<Complex> K { get; } = new List<Complex>();

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
