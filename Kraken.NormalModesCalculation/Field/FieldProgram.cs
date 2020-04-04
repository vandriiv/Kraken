using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation.Field
{
    public class FieldProgram
    {
        public void CalculateFieldPressure(CalculatedModesInfo modesInfo, string opt, int modesCountLimit, int Nr, List<double> receiverRanges, int Nsd,
                                           List<double> sourceDepths, int Nrd, List<double> receiverDepths, int Nrr, List<double> rr,
                                           ref List<double> ranges, ref List<double> sources, ref List<double> receivers,
                                           ref List<List<List<Complex>>> res, List<string> warnings)
        {
            var maxM = Math.Min(modesCountLimit, modesInfo.ModesCount);

            string comp = "";
            if (opt.Length > 2)
            {
                comp = opt[2].ToString();
            }

            var rangedDataManager = new RangedDataManager();
                  
            rangedDataManager.ProceedReceiverRanges(Nr, receiverRanges);

            var zMin = -3.40282347E+38;
            var zMax = 3.40282347E+38;

            rangedDataManager.ProceedSourceAndReceiverDepths(zMin, zMax, Nsd, Nrd, sourceDepths, receiverDepths);

            ranges = new List<double>(rangedDataManager.ReceiverRanges);
            sources = new List<double>(rangedDataManager.SourceDepths);
            receivers = new List<double>(rangedDataManager.ReceiverDepths);

            var C = Enumerable.Repeat(new Complex(), maxM + 1).ToList();

            var receiverDisplacements = Enumerable.Repeat(0d, Nrr + 1).ToList();            

            if (Nrr != rangedDataManager.Nrd)
            {
                Nrr = rangedDataManager.Nrd;
                receiverDisplacements = Enumerable.Repeat(0d, Nrr + 1).ToList();
            }

            for (var i = 0; i < rr.Count; i++)
            {
                receiverDisplacements[i] = rr[i];
            }

            if (Nrr > 1)
            {
                receiverDisplacements[2] = -999.9;
            }
            if (Nrr > 2)
            {
                receiverDisplacements[3] = -999.9;
            }

            var subTabMod = new SubTabulator();

            subTabMod.SubTabulate(receiverDisplacements, Nrr);

            var readModesMod = new ModesPreparationManager();

            var phiS = readModesMod.GetPreparedModes(modesInfo, maxM, rangedDataManager.SourceDepths, rangedDataManager.Nsd, "N", warnings);
            var phiR = readModesMod.GetPreparedModes(modesInfo, maxM, rangedDataManager.ReceiverDepths, rangedDataManager.Nrd, comp, warnings);
            var evaluateMod = new PressueFieldCalculator();
            res = new List<List<List<Complex>>>();
            res.Add(new List<List<Complex>>());

            for (var IS = 1; IS <= rangedDataManager.Nsd; IS++)
            {
                for (var i = 1; i <= modesInfo.ModesCount; i++)
                {
                    C[i] = phiS[i][IS];
                }

                var P = evaluateMod.Evaluate(C, phiR, rangedDataManager.Nrd, rangedDataManager.ReceiverRanges, rangedDataManager.Nr, receiverDisplacements, modesInfo.K, modesInfo.ModesCount, opt);
                res.Add(P);
            }
        }
    }
}
