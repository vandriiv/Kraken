using System.Collections.Generic;

namespace Kraken.Application.Models
{
    public class AcousticProblemData
    {
        public double Frequency { get; set; }
        public int NModes { get; set; }
        public int NMedia { get; set; }

        public string TopBCType { get; set; }
        public string InterpolationType { get; set; }
        public string AttenuationUnits { get; set; }
        public string AddedVolumeAttenuation { get; set; }

        public double ZT { get; set; }
        public double CPT { get; set; }
        public double CST { get; set; }
        public double RHOT { get; set; }
        public double APT { get; set; }
        public double AST { get; set; }

        public double BumDen { get; set; }
        public double Eta { get; set; }
        public double Xi { get; set; }

        public List<List<double>> MediumInfo { get; } = new List<List<double>>();
        public List<List<double>> SSP { get; } = new List<List<double>>();

        public string BottomBCType { get; set; }
        public double Sigma { get; set; }

        public double ZB { get; set; }
        public double CPB { get; set; }
        public double CSB { get; set; }
        public double RHOB { get; set; }
        public double APB { get; set; }
        public double ASB { get; set; }

        public double CLow { get; set; }
        public double CHigh { get; set; }

        public double RMax { get; set; }

        public int NSD { get; set; }
        public List<double> SD { get; } = new List<double>();

        public int NRD { get; set; }
        public List<double> RD { get; } = new List<double>();

        public bool CalculateTransmissionLoss { get; set; }
        public int NModesForField { get; set; }
        public string SourceType { get; set; }
        public string ModesTheory { get; set; } 
        public int NR { get; set; }
        public List<double> R { get; } = new List<double>();
        public int NRR { get; set; }
        public List<double> RR { get; } = new List<double>();

        public int NSDField { get; set; }
        public List<double> SDField { get; } = new List<double>();

        public int NRDField { get; set; }
        public List<double> RDField { get; } = new List<double>();
    }
}
