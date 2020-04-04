using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Kraken.Calculation.Field
{
    public class FieldModel
    {
        public void CalculateFieldPressure(CalculatedModesInfo modesInfo, string Opt, int MLimit, int NR, List<double> R, int NSD,
                                           List<double> SD, int NRD, List<double> RD, int Nrr, List<double> rr,
                                           ref List<double> ranges, ref List<double> sources, ref List<double> receivers,
                                           ref List<List<List<Complex>>> res, List<string> warnings)
        {
            var MaxM = Math.Min(MLimit, modesInfo.ModesCount);

            string Comp = "";
            if (Opt.Length > 2)
            {
                Comp = Opt[2].ToString();
            }

            var rangedDataManager = new RangedDataManager();
                  
            rangedDataManager.ProceedReceiverRanges(NR, R);

            var zMin = -3.40282347E+38;
            var zMax = 3.40282347E+38;

            rangedDataManager.ProceedSourceAndReceiverDepths(zMin, zMax, NSD, NRD, SD, RD);

            ranges = new List<double>(rangedDataManager.ReceiverRanges);
            sources = new List<double>(rangedDataManager.SourceDepths);
            receivers = new List<double>(rangedDataManager.ReceiverDepths);

            var C = Enumerable.Repeat(new Complex(), MaxM + 1).ToList();

            rr = Enumerable.Repeat(0d, Nrr + 1).ToList();

            if (Nrr != rangedDataManager.Nrd)
            {
                Nrr = rangedDataManager.Nrd;
                rr = Enumerable.Repeat(0d, Nrr + 1).ToList();
            }

            if (Nrr > 1)
            {
                rr[2] = -999.9;
            }
            if (Nrr > 2)
            {
                rr[3] = -999.9;
            }

            var subTabMod = new SubTabulator();

            subTabMod.SubTabulate(rr, Nrr);

            var readModesMod = new ReadModesMod();

            var phiS = readModesMod.GetModes(modesInfo, MaxM, rangedDataManager.SourceDepths, rangedDataManager.Nsd, "N", warnings);
            var phiR = readModesMod.GetModes(modesInfo, MaxM, rangedDataManager.ReceiverDepths, rangedDataManager.Nrd, Comp, warnings);
            var evaluateMod = new EvaluateMod();
            res = new List<List<List<Complex>>>();
            res.Add(new List<List<Complex>>());

            for (var IS = 1; IS <= rangedDataManager.Nsd; IS++)
            {
                for (var i = 1; i <= modesInfo.ModesCount; i++)
                {
                    C[i] = phiS[i][IS];
                }

                var P = evaluateMod.Evaluate(C, phiR, rangedDataManager.Nrd, rangedDataManager.ReceiverRanges, rangedDataManager.Nr, rr, modesInfo.K, modesInfo.ModesCount, Opt);
                res.Add(P);
            }
        }
    }
}
