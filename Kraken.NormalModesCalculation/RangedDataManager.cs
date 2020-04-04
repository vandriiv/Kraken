using Kraken.Calculation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.Calculation
{
    public class RangedDataManager
    {
        public int Nsd { get; set; }
        public int Nrd { get; set; }
        public int Nr { get; set; }   
      
        public List<double> SourceDepths { get; set; }
        public List<double> ReceiverDepths { get; set; }      
        public List<double> ReceiverRanges { get; set; }      

        public void ProceedSourceAndReceiverDepths(double zMin, double zMax, int Nsd, int Nrd, List<double> zsr, List<double> zrc)
        {
            if (Nsd <= 0)
            {
                throw new KrakenException("Number of sources must be positive");
            }

            if (Nrd <= 0)
            {
                throw new KrakenException("Number of receivers must be positive");
            }

            var sourceDepths = Enumerable.Repeat(0d, Math.Max(3 + 1, Nsd + 1)).ToList();

            sourceDepths[3] = -999.9;
            var IQ = zsr.Count;
            for (var i = 1; i < IQ; i++)
            {
                sourceDepths[i] = zsr[i];
            }

            var subTabMod = new SubTabulator();
            subTabMod.SubTabulate(sourceDepths, Nsd);

            var receiverDepths = Enumerable.Repeat(0d, Math.Max(3 + 1, Nrd + 1)).ToList();

            receiverDepths[3] = -999.9;
            IQ = zrc.Count;
            for (var i = 1; i < IQ; i++)
            {
                receiverDepths[i] = zrc[i];
            }

            subTabMod.SubTabulate(receiverDepths, Nrd);

            for (var IS = 1; IS <= Nsd; IS++)
            {
                if (sourceDepths[IS] < zMin)
                {
                    sourceDepths[IS] = zMin;                    
                }
                else if (sourceDepths[IS] > zMax)
                {
                    sourceDepths[IS] = zMax;
                }
            }

            for (var IR = 1; IR <= Nrd; IR++)
            {
                if (receiverDepths[IR] < zMin)
                {
                    receiverDepths[IR] = zMin;
                }
                else if (receiverDepths[IR] > zMax)
                {
                    receiverDepths[IR] = zMax;
                }
            }

            SourceDepths = sourceDepths;
            ReceiverDepths = receiverDepths;
            this.Nrd = Nrd;
            this.Nsd = Nsd;
        }

        public void ProceedReceiverRanges(int Nr, List<double> ranges)
        {
            var receiverRanges = Enumerable.Repeat(0d, Math.Max(3, Nr) + 1).ToList();
            receiverRanges[3] = -999.9;
            var IQ = ranges.Count;
            for (var i = 1; i < IQ; i++)
            {
                receiverRanges[i] = ranges[i];
            }

            var subtabMod = new SubTabulator();
            subtabMod.SubTabulate(receiverRanges, Nr);
            receiverRanges.Sort();

            for (var i = 1; i < receiverRanges.Count; i++)
            {
                receiverRanges[i] *= 1000;
            }

            var isIncreasing = receiverRanges.OrderBy(x => x).SequenceEqual(receiverRanges);
            if (!isIncreasing)
            {
                throw new KrakenException("Receiver ranges are not monotonically increasing");
            }

            ReceiverRanges = receiverRanges;
            this.Nr = Nr;
        }
    }
}
