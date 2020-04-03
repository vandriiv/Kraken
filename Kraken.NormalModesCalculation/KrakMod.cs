using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation
{
    public class KrakMod
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
        public int LRECL { get; set; }
        public int ModeCount { get; set; }
        public int Mode { get; set; }
        public int IProf { get; set; }

        public List<double> ET { get; set; }
        public List<double> HV { get; set; }
        public double rhoT { get; set; }
        public double rhoB { get; set; }
        public double CMin { get; set; }
        public double CLow { get; set; }
        public double CHigh { get; set; }
        public double Freq { get; set; }
        public double Omega2 { get; set; }
        public double RMax { get; set; }
        public double BumDen { get; set; }
        public double eta { get; set; }
        public double xi { get; set; }

        public List<List<double>> EVMat { get; set; }
        public List<List<double>> Extrap { get; set; }
        public List<double> VG { get; set; }

        public Complex CPT { get; set; }
        public Complex CST { get; set; }
        public Complex CPB { get; set; }
        public Complex CSB { get; set; }

        public List<Complex> k { get; set; }

        public List<int> LOC { get; set; }
        public List<int> NG { get; set; }
        public List<int> N { get; set; }
        public List<double> Depth { get; set; }
        public List<double> H { get; set; }
        public List<double> SIGMA { get; set; }
        public List<string> Mater { get; set; }
        public string TopOpt { get; set; }
        public string BotOpt { get; set; }
        public string Title { get; set; }
        public List<double> B1 { get; set; }
        public List<double> B1C { get; set; }
        public List<double> B2 { get; set; }
        public List<double> B3 { get; set; }
        public List<double> B4 { get; set; }
        public List<double> RHO { get; set; }
        public List<string> Warnings { get; set; }

        public void Init(){
            Mater = Enumerable.Repeat("", MaxMedium+1).ToList();
            LOC = Enumerable.Repeat(0,MaxMedium+1).ToList();
            NG = Enumerable.Repeat(0, MaxMedium+1).ToList();
            N = Enumerable.Repeat(0, MaxMedium+1).ToList();
            Depth = Enumerable.Repeat(0d, MaxMedium+1).ToList();
            H = Enumerable.Repeat(0d, MaxMedium+1).ToList();
            SIGMA = Enumerable.Repeat(0d, MaxMedium+1).ToList();
            NV = Enumerable.Repeat(0, NSets+1).ToList();
            ET = Enumerable.Repeat(0d, NSets+1).ToList();
            HV = Enumerable.Repeat(0d, NSets+1).ToList();
            Warnings = new List<string>();
        }

    }
}
