using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Kraken.NormalModesCalculation.Field
{
    public class FieldModel
    {
        public void CalculateFieldPressure(ModesOut modesOut, string Opt, int MLimit, int NProf, List<double> rProf,
                                           int NR, List<double> R, int NSD, List<double> SD, int NRD, List<double> RD,
                                           int Nrr, List<double> rr, ref List<double> ranges, ref List<double> sources, ref List<double> receivers,
                                           ref List<List<List<Complex>>> res)
        {
            var MaxM = Math.Min(MLimit, modesOut.M);

            string Comp = "";
            if (Opt.Length > 2)
            {
                Comp = Opt[2].ToString();
            }

            if (NProf > 2)
            {
                rProf[3] = -999.9;
            }

            var subTabMod = new SubTabMod();
            subTabMod.SUBTAB(rProf, NProf);

            if (rProf[1] != 0)
            {
                throw new ArgumentException("The first profile must be at 0 km");
            }

            var sdrdMod = new SDRDRMod();
            sdrdMod.Nr = NR;
            sdrdMod.Nsd = NSD;
            sdrdMod.Nrd = NRD;
          
            sdrdMod.ReadRcvrRanges(sdrdMod.Nr, R);

            var zMin = -3.40282347E+38;
            var zMax = 3.40282347E+38;

            sdrdMod.SDRD(zMin, zMax, sdrdMod.Nsd, sdrdMod.sd, sdrdMod.Nrd, sdrdMod.rd,
                        SD, RD);

            ranges = new List<double>(sdrdMod.r);
            sources = new List<double>(sdrdMod.sd);
            receivers = new List<double>(sdrdMod.rd);

            var C = Enumerable.Repeat(new Complex(), MaxM + 1).ToList();

            if (Nrr != sdrdMod.Nrd)
            {
                Nrr = sdrdMod.Nrd;
            }

            if (Nrr > 1)
            {
                rr[2] = -999.9;
            }
            if (Nrr > 2)
            {
                rr[3] = -999.9;
            }

            subTabMod.SUBTAB(rr, Nrr);

            var readModesMod = new ReadModesMod();

            var phiS = readModesMod.GetModes(modesOut, sdrdMod, MaxM, sdrdMod.sd, sdrdMod.Nsd, "N");
            var phiR = readModesMod.GetModes(modesOut, sdrdMod, MaxM, sdrdMod.rd, sdrdMod.Nrd, Comp);
            var evaluateMod = new EvaluateMod();
            res = new List<List<List<Complex>>>();
            res.Add(new List<List<Complex>>());

            for (var IS = 1; IS <= sdrdMod.Nsd; IS++)
            {
                for (var i = 1; i <= modesOut.M; i++)
                {
                    C[i] = phiS[i][IS];
                }

                var P = evaluateMod.Evaluate(C, phiR, sdrdMod.Nrd, sdrdMod.r, sdrdMod.Nr, rr, modesOut.k, modesOut.M, Opt);
                res.Add(P);
            }
        }
    }
}
