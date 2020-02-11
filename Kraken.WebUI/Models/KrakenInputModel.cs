using System.Collections.Generic;

namespace Kraken.WebUI.Models
{
    public class KrakenInputModel
    {
        public double Frequency { get; set; }
        public int NModes { get; set; }
        public int NMedia { get; set; }

        public char TopBCType { get; set; }
        public char InterpolationType { get; set; }
        public char AttenuationUnits { get; set; }
        public char AddedVolumeAttenuation { get; set; }

        public double ZT { get; set; }
        public double CPT { get; set; }
        public double CST { get; set; }
        public double RHOT { get; set; }
        public double APT { get; set; }
        public double AST { get; set; }

        public double BumDen { get; set; }
        public double Eta { get; set; }
        public double Xi { get; set; }

        public List<double> MediumInfo { get; set; }
        public List<List<double>> SSP { get; set; }

        public char BottomBCType { get; set; }

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
        public List<double> SD { get; set; }

        public int NRD { get; set; }
        public List<double> RD { get; set; }
    }
}
