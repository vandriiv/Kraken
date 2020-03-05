using System.Collections.Generic;
using System.Numerics;

namespace Kraken.NormalModesCalculation
{
    public class ModesOut
    {
        public int LRecL { get; set; }
        public string Title { get; set; }
        public int NFreq { get; set; }
        public int NMedia { get; set; }
        public int NTot { get; set; }
        public int NMat { get; set; }
        public List<int> N { get; set; }
        public List<string> Material { get; set; }
        public List<double> Depth { get; set; }
        public List<double> rho { get; set; }
        public List<double> freqVec { get; set; }
        public List<double> Z { get; set; }

        public int M { get; set; }
        public List<Complex> k { get; set; }

        public string BCTop { get; set; }
        public Complex cPT { get; set; }
        public Complex cST { get; set; }
        public double rhoT { get; set; }
        public double DepthT { get; set; }

        public string BCBot { get; set; }
        public Complex cPB { get; set; }
        public Complex cSB { get; set; }
        public double rhoB { get; set; }
        public double DepthB { get; set; }

        public List<List<Complex>> Phi { get; set; } = new List<List<Complex>>();
    }
}
