using System.Collections.Generic;

namespace Kraken.Calculation.Models
{
    public class FieldInputData
    {
        public CalculatedModesInfo ModesInfo { get; }
        public string Options { get; }
        public int ModesLimit { get; }
        public int Nr { get; }
        public List<double> ReceiverRanges { get; }

        public int Nsd { get; }
        public List<double> SourceDepths { get; }

        public int Nrd { get; }
        public List<double> ReceiverDepths { get; }

        public int Nrr { get; }
        public List<double> ReceiverDisplacements { get; }

        public FieldInputData(CalculatedModesInfo modesInfo, string options, int modesLimit, int nr,
                              List<double> receiverRanges, int nsd, List<double> sourceDepths, int nrd,
                              List<double> receiverDepths, int nrr, List<double> receiverDisplacements)
        {
            ModesInfo = modesInfo;
            Options = options;
            ModesLimit = modesLimit;
            Nr = nr;
            ReceiverRanges = receiverRanges;
            Nsd = nsd;
            SourceDepths = sourceDepths;
            Nrd = nrd;
            ReceiverDepths = receiverDepths;
            Nrr = nrr;
            ReceiverDisplacements = receiverDisplacements;
        }
    }
}
