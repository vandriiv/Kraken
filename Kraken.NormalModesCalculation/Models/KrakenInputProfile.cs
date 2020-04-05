using System.Collections.Generic;

namespace Kraken.Calculation.Models
{
    public class KrakenInputProfile
    {
        public int NModes { get; private set; }
        public double Frequency { get; private set; }
        public int NMedia { get; private set; }
        public string Options { get; private set; }
        public string BCBottom { get; private set; }
        public List<List<double>> MediumInfo { get; private set; }
        public List<List<double>> SSP { get; private set; }
        public double BottomSigma { get; private set; }

        public double CLow { get; private set; }
        public double CHigh { get; private set; }

        public double RMax { get; private set; }

        public int Nsd { get; private set; }
        public List<double> SourceDepths { get; private set; }

        public int Nrd { get; private set; }
        public List<double> ReceiverDepths { get; private set; }
        public List<double> TopAcousticHSProperties { get; private set; }
        public List<double> TwerskyScatterParameters { get; private set; }
        public List<double> BottomAcousticHSProperties { get; private set; }

        public KrakenInputProfile(int nModes, double frequency, int nMedia, string options, string bCBottom,
                                  List<List<double>> mediumInfo, List<List<double>> sSP, double bottomSigma, double cLow,
                                  double cHigh, double rMax, int nsd, List<double> sourceDepths, int nrd,
                                  List<double> receiverDepths, List<double> topAcousticHSProperties,
                                  List<double> twerskyScatterParameters, List<double> bottomAcousticHSProperties)
        {
            NModes = nModes;
            Frequency = frequency;
            NMedia = nMedia;
            Options = options;
            BCBottom = bCBottom;
            MediumInfo = mediumInfo;
            SSP = sSP;
            BottomSigma = bottomSigma;
            CLow = cLow;
            CHigh = cHigh;
            RMax = rMax;
            Nsd = nsd;
            SourceDepths = sourceDepths;
            Nrd = nrd;
            ReceiverDepths = receiverDepths;
            TopAcousticHSProperties = topAcousticHSProperties;
            TwerskyScatterParameters = twerskyScatterParameters;
            BottomAcousticHSProperties = bottomAcousticHSProperties;
        }
    }
}
