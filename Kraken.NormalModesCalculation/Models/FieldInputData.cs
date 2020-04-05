using System.Collections.Generic;

namespace Kraken.Calculation.Models
{
    public class FieldInputData
    {
        public CalculatedModesInfo ModesInfo { get; private set; }
        public string Options { get; private set; }
        public int ModesLimit { get; private set; }
        public int Nr { get; private set; }
        public List<double> ReceiverRanges { get; private set; }

        public int Nsd { get; private set; }
        public List<double> SourceDepths { get; private set; }

        public int Nrd { get; private set; }
        public List<double> ReceiverDepths { get; private set; }

        public int Nrr { get; private set; }
        public List<double> ReceiverDisplacements { get; private set; }

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
