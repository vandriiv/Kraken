using System.Collections.Generic;

namespace Kraken.Calculation.Models
{
    public class KrakenInputProfile
    {
        public int NModes { get; }
        public double Frequency { get; }
        public int NMedia { get; }
        public string Options { get; }
        public string BCBottom { get; }
        public List<List<double>> MediumInfo { get; }
        public List<List<double>> SSP { get; }
        public double BottomSigma { get; }

        public double CLow { get; }
        public double CHigh { get; }

        public double RMax { get; }

        public int Nsd { get; }
        public List<double> SourceDepths { get; }

        public int Nrd { get; }
        public List<double> ReceiverDepths { get; }
        public List<double> TopAcousticHSProperties { get; }
        public List<double> TwerskyScatterParameters { get; }
        public List<double> BottomAcousticHSProperties { get; }

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
