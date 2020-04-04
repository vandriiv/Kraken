using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation
{
    class KrakenModule
    {    
        public int MaxMedium { get;  } = 500;
        public int NSets { get; } = 5;       
        public Complex i { get; } = new Complex(0.0, 1.0);
        public int FirstAcoustic { get; set; }
        public int LastAcoustic { get; set; }
        public int NMedia { get; set; }
        public List<int> NV { get; set; }
        public int ISet { get; set; }
        public int M { get; set; }
        
        public int ModeCount { get; set; }
        public int Mode { get; set; }
        public int IProf { get; set; }

        public List<double> HV { get; set; }       
        public double CMin { get; set; }
        public double CLow { get; set; }
        public double CHigh { get; set; }
        public double Frequency { get; set; }
        public double Omega2 { get; set; }
        public double RMax { get; set; }
        public double BumpDensity { get; set; }
        public double Eta { get; set; }
        public double Xi { get; set; }

        public List<List<double>> EVMat { get; set; }
        public List<List<double>> Extrap { get; set; }
        public List<double> VG { get; set; }

        public Complex CPTop { get; set; }
        public Complex CSTop { get; set; }
        public Complex CPBottom { get; set; }
        public Complex CSBottom { get; set; }
        public double RhoTop { get; set; }
        public double RhoBottom { get; set; }

        public List<Complex> K { get; set; }

        public List<int> Loc { get; set; }
        public List<int> NG { get; set; }
        public List<int> N { get; set; }
        public List<double> Depth { get; set; }
        public List<double> H { get; set; }
        public List<double> Sigma { get; set; }
        public List<string> Material { get; set; }
        public string BCTop { get; set; }
        public string BCBottom { get; set; }    
        public List<double> B1 { get; set; }
        public List<double> B1C { get; set; }
        public List<double> B2 { get; set; }
        public List<double> B3 { get; set; }
        public List<double> B4 { get; set; }
        public List<double> Rho { get; set; }
        public List<string> Warnings { get; set; }

        public void Init(){
            Material = Enumerable.Repeat("", MaxMedium+1).ToList();
            Loc = Enumerable.Repeat(0,MaxMedium+1).ToList();
            NG = Enumerable.Repeat(0, MaxMedium+1).ToList();
            N = Enumerable.Repeat(0, MaxMedium+1).ToList();
            Depth = Enumerable.Repeat(0d, MaxMedium+1).ToList();
            H = Enumerable.Repeat(0d, MaxMedium+1).ToList();
            Sigma = Enumerable.Repeat(0d, MaxMedium+1).ToList();
            NV = Enumerable.Repeat(0, NSets+1).ToList();         
            HV = Enumerable.Repeat(0d, NSets+1).ToList();
            Warnings = new List<string>();
        }

    }
}
