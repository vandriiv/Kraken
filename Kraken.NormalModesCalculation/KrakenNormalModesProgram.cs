using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Kraken.NormalModesCalculation
{
    //need to take a good look at loops, arguments changing in methods, list init

    /*
    Input:
  frq = frequency
  nm = # modes
  nl= no. layers, e.g., 2
  note1 = text flags for modes, e.g., 'SVW'
  b= NG,sigma,depth of each layer - stacked
  ssp=depths, alphaR, betaR, rhoR, alphaI, betaI for layers 1,2 
                   [  C       0    1.03     0      0  ]
         matrix has each layer - stacked
  note2 = text flag for bottom option
  bsig=bottom sigma
  clh= CLow, CHigh - spectral limits
  rng = range (m); used for error estimate
  nsr,zsr = # and source depths (m)
  nrc,zrc = # and receiver depths (m)    
         If nrc or nsr is greater than the number of depths given, kraken
         will do an autointerpolation.  Thus if nrc is 500 with 
         zrc's = [0 5000], 500 depths between 0 and 5000 will be used for 
         the mode eigenvectors.

Output:
  cg = nm group speeds
  cp = nm phase spees
  zm = depths of mode functions
  modes = nm mode functions at depths zm*/
    public class KrakenNormalModesProgram
    {
        public ModesOut OceanAcousticNormalModes(int nm, double frq, int nl, string note1, List<List<double>> bb, int nc,
                                             List<List<double>> ssp, string note2, double bsig, List<double> clh, double rng, int nsr, List<double> zsr,
                                             int nrc, List<double> zrc, int nz, List<double> tahsp, List<double> tsp, List<double> bahsp, ref List<double> cg, ref List<double> cp,
                                             ref List<double> zm, ref List<List<double>> modes, ref List<Complex> k)
        {
            var krakMod = new KrakMod();
            krakMod.Init();

            krakMod.NV = new List<int> { 0, 1, 2, 4, 8, 16 };

            krakMod.Freq = frq;
            krakMod.NMedia = nl;
            krakMod.TopOpt = note1.Substring(0, 3);
            krakMod.BotOpt = note2[0].ToString();

            var sDRDRMod = new SDRDRMod();
            var readinMod = new Readin(krakMod);

            krakMod.Depth[1] = 0;
            for (var il = 1; il <= nl; il++)
            {
                krakMod.NG[il] = (int)bb[il][1];
                krakMod.SIGMA[il] = bb[il][2];
                krakMod.Depth[il + 1] = bb[il][3];
            }

            krakMod.SIGMA[nl + 1] = bsig;
            readinMod.READIN(krakMod, nc, ssp, tahsp, tsp, bahsp);

            krakMod.CLow = clh[1];
            krakMod.CHigh = clh[2];

            krakMod.RMax = rng;

            var ZMin = krakMod.Depth[1];
            var ZMax = krakMod.Depth[krakMod.NMedia + 1];
            sDRDRMod.Nsd = nsr;
            sDRDRMod.Nrd = nrc;

            sDRDRMod.SDRD(ZMin, ZMax, sDRDRMod.Nsd, sDRDRMod.sd, sDRDRMod.Nrd, sDRDRMod.rd,
                        zsr, zrc);

            krakMod.Omega2 = Math.Pow((2.0 * 3.1415926535898 * krakMod.Freq), 2);

            double error = 0;
            var isSuccess = false;
            var modesOut = new ModesOut();
            for (krakMod.ISet = 1; krakMod.ISet <= krakMod.NSets; krakMod.ISet++)
            {
                krakMod.N = krakMod.NG.Select(x => x * krakMod.NV[krakMod.ISet]).ToList();
                for (var j = 1; j <= krakMod.NMedia; j++)
                {
                    krakMod.H[j] = (krakMod.Depth[j + 1] - krakMod.Depth[j]) / krakMod.N[j];
                }
                krakMod.HV[krakMod.ISet] = krakMod.H[1];
                SOLVE(modesOut, krakMod, sDRDRMod, readinMod, ref error, nm, nz, ref zm, ref modes);

                if (error * 1000.0 * krakMod.RMax < 1.0)
                {
                    isSuccess = true;
                    break;
                }
            }

            if (isSuccess)
            {
                var OMEGA = Math.Sqrt(krakMod.Omega2);
                var minVal = krakMod.Extrap[1].GetRange(1, krakMod.M).Where(x => x > krakMod.Omega2 / Math.Pow(krakMod.CHigh, 2)).Min();
                var Min_Loc = krakMod.Extrap[1].FindIndex(x => x == minVal);
                krakMod.M = Min_Loc;

                for (var i = 1; i <= krakMod.M; i++)
                {
                    krakMod.k[i] = Complex.Sqrt(krakMod.Extrap[1][i] + krakMod.k[i]);
                }

                var MMM = Math.Min(krakMod.M, nm);
                modesOut.M = MMM;
                modesOut.k = new List<Complex>(krakMod.k);

                cp = Enumerable.Repeat(0d, MMM + 1).ToList();
                cg = Enumerable.Repeat(0d, MMM + 1).ToList();
                k = Enumerable.Repeat(new Complex(), MMM + 1).ToList();

                for (krakMod.Mode = 1; krakMod.Mode <= MMM; krakMod.Mode++)
                {
                    cp[krakMod.Mode] = (OMEGA / krakMod.k[krakMod.Mode]).Real;
                    cg[krakMod.Mode] = krakMod.VG[krakMod.Mode];
                    k[krakMod.Mode] = krakMod.k[krakMod.Mode];
                }
            }

            return modesOut;
        }

        private void INIT(KrakMod krakMod, Readin readinMod)
        {
            var ELFLAG = false;
            double CP2 = 0.0, CS2 = 0.0;
            var dummy = new List<List<double>>(7);

            krakMod.CMin = double.MaxValue;
            krakMod.FirstAcoustic = 0;
            krakMod.LOC[1] = 0;

            var NPTS = krakMod.N.GetRange(1, krakMod.NMedia).Sum() + krakMod.NMedia;

            krakMod.B1 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakMod.B1C = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakMod.B2 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakMod.B3 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakMod.B4 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakMod.RHO = Enumerable.Repeat(0d, NPTS + 1).ToList();

            var CP = Enumerable.Repeat(new Complex(), NPTS + 1).ToList();
            var CS = Enumerable.Repeat(new Complex(), NPTS + 1).ToList();

            for (var Medium = 1; Medium <= krakMod.NMedia; Medium++)
            {
                if (Medium != 1)
                {
                    krakMod.LOC[Medium] = krakMod.LOC[Medium - 1] + krakMod.N[Medium - 1] + 1;
                }
                var N1 = krakMod.N[Medium] + 1;
                var II = krakMod.LOC[Medium] + 1;

                var TASK = "TAB";
                readinMod.PROFIL(krakMod, krakMod.Depth, CP, CS, krakMod.RHO, Medium, ref N1, II, krakMod.Freq, krakMod.TopOpt[0].ToString(), krakMod.TopOpt.Substring(2), TASK, 1, dummy);

                if (CS[II].Real == 0)
                {
                    krakMod.Mater[Medium] = "ACOUSTIC";
                    if (krakMod.FirstAcoustic == 0)
                    {
                        krakMod.FirstAcoustic = Medium;
                    }
                    krakMod.LastAcoustic = Medium;

                    var CPMin = CP.GetRange(II, krakMod.N[Medium] + 1).Select(x => x.Real).Min();
                    krakMod.CMin = Math.Min(krakMod.CMin, CPMin);

                    for (var i = II; i <= II + krakMod.N[Medium]; i++)
                    {
                        krakMod.B1[i] = -2.0 + Math.Pow(krakMod.H[Medium], 2) * (krakMod.Omega2 / Complex.Pow(CP[i], 2)).Real;
                    }

                    for (var i = II; i <= II + krakMod.N[Medium]; i++)
                    {
                        krakMod.B1C[i] = (krakMod.Omega2 / Complex.Pow(CP[i], 2)).Imaginary;
                    }
                }
                else
                {
                    if (krakMod.SIGMA[Medium] != 0)
                    {
                        throw new Exception("Rough elastic interfaces are not allowed");
                    }

                    krakMod.Mater[Medium] = "ELASTIC";
                    ELFLAG = true;
                    var TWOH = 2.0 * krakMod.H[Medium];

                    for (var J = II; J <= II + krakMod.N[Medium]; J++)
                    {
                        krakMod.CMin = Math.Min(CS[J].Real, krakMod.CMin);

                        CP2 = Complex.Pow(CP[J], 2).Real;
                        CS2 = Complex.Pow(CS[J], 2).Real;

                        krakMod.B1[J] = TWOH / (krakMod.RHO[J] * CS2);
                        krakMod.B2[J] = TWOH / (krakMod.RHO[J] * CP2);
                        krakMod.B3[J] = 4.0 * TWOH * krakMod.RHO[J] * CS2 * (CP2 - CS2) / CP2;
                        krakMod.B4[J] = TWOH * (CP2 - 2.0 * CS2) / CP2;
                        krakMod.RHO[J] = TWOH * krakMod.Omega2 * krakMod.RHO[J];
                    }
                }
            }

            if (krakMod.BotOpt[0] == 'A')
            {
                if (krakMod.CSB.Real > 0.0)
                {
                    ELFLAG = true;
                    krakMod.CMin = Math.Min(krakMod.CMin, krakMod.CSB.Real);
                    krakMod.CHigh = Math.Min(krakMod.CHigh, krakMod.CSB.Real);
                }
                else
                {
                    krakMod.CMin = Math.Min(krakMod.CMin, krakMod.CPB.Real);
                    krakMod.CHigh = Math.Min(krakMod.CHigh, krakMod.CPB.Real);
                }
            }

            if (krakMod.TopOpt.Length > 1)
            {
                if (krakMod.TopOpt[1] == 'A')
                {
                    if (krakMod.CST.Real > 0.0)
                    {
                        ELFLAG = true;
                        krakMod.CMin = Math.Min(krakMod.CMin, krakMod.CST.Real);
                        krakMod.CHigh = Math.Min(krakMod.CHigh, krakMod.CST.Real);
                    }
                    else
                    {
                        krakMod.CMin = Math.Min(krakMod.CMin, krakMod.CPT.Real);
                        krakMod.CHigh = Math.Min(krakMod.CHigh, krakMod.CPT.Real);
                    }
                }
            }

            if (ELFLAG)
            {
                krakMod.CMin = 0.85 * krakMod.CMin;
            }

            krakMod.CLow = Math.Max(krakMod.CLow, krakMod.CMin);
        }

        private void SOLVE(ModesOut modesOut, KrakMod krakMod, SDRDRMod sDRDRMod, Readin readinMod, ref double error, int nm, int nz, ref List<double> zm, ref List<List<double>> modes)
        {
            var NOMODES = 0;

            INIT(krakMod, readinMod);

            if (krakMod.IProf > 1 && krakMod.ISet <= 2 && krakMod.TopOpt[3] == 'C')
            {
                SOLVE3(krakMod);
            }
            else if (krakMod.ISet <= 2 && krakMod.NMedia <= (krakMod.LastAcoustic - krakMod.FirstAcoustic + 1))
            {
                SOLVE1(krakMod, nm);
            }
            else
            {
                SOLVE2(krakMod);
            }

            for (var i = 1; i <= krakMod.M; i++)
            {
                krakMod.Extrap[krakMod.ISet][i] = krakMod.EVMat[krakMod.ISet][i];
            }

            var minVal = krakMod.Extrap[1].GetRange(1, krakMod.M).Where(x => x > krakMod.Omega2 / Math.Pow(krakMod.CHigh, 2)).Min();
            var Min_Loc = krakMod.Extrap[1].FindIndex(x => x == minVal);
            krakMod.M = Min_Loc;

            if (krakMod.ISet == 1 && NOMODES == 0)
            {
                VECTOR(modesOut, krakMod, sDRDRMod, nm, nz, ref zm, ref modes);
            }

            error = 10;
            var KEY = 2 * krakMod.M / 3 + 1;
            if (krakMod.ISet > 1)
            {
                var T1 = krakMod.Extrap[1][KEY];

                for (var J = krakMod.ISet - 1; J >= 1; J--)
                {
                    for (krakMod.Mode = 1; krakMod.Mode <= krakMod.M; krakMod.Mode++)
                    {
                        var X1 = Math.Pow(krakMod.NV[J], 2);
                        var X2 = Math.Pow(krakMod.NV[krakMod.ISet], 2);
                        var F1 = krakMod.Extrap[J][krakMod.Mode];
                        var F2 = krakMod.Extrap[J + 1][krakMod.Mode];
                        krakMod.Extrap[J][krakMod.Mode] = F2 - (F1 - F2) / (X2 / X1 - 1.0);
                    }
                }

                var T2 = krakMod.Extrap[1][KEY];
                error = Math.Abs(T2 - T1);
            }

        }

        private void VECTOR(ModesOut modesOut, KrakMod krakMod, SDRDRMod sDRDRMod, int nm, int nz, ref List<double> zm, ref List<List<double>> modes)
        {
            var BCTop = krakMod.TopOpt[1].ToString();
            var BCBot = krakMod.BotOpt[0].ToString();

            var NTot = krakMod.N.GetRange(krakMod.FirstAcoustic, krakMod.LastAcoustic - krakMod.FirstAcoustic + 1).Sum();
            var NTot1 = NTot + 1;

            //allocate
            var Z = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var E = Enumerable.Repeat(0d, NTot1 + 1 + 1).ToList();
            var D = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var PHI = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var J = 1;
            Z[1] = krakMod.Depth[krakMod.FirstAcoustic];

            var Hrho = 0.0;
            for (var Medium = krakMod.FirstAcoustic; Medium <= krakMod.LastAcoustic; Medium++)
            {
                Hrho = krakMod.H[Medium] * krakMod.RHO[krakMod.LOC[Medium] + 1];

                var temp = 1;
                for (var i = J + 1; i <= J + krakMod.N[Medium]; i++)
                {
                    E[i] = 1.0 / Hrho;
                    Z[i] = Z[J] + krakMod.H[Medium] * temp;
                    temp++;
                }

                J += krakMod.N[Medium];
            }

            E[NTot1 + 1] = 1.0 / Hrho;
            var mergevMod = new MergeVMod();
            var ZTAB = Enumerable.Repeat(0d, sDRDRMod.Nrd + sDRDRMod.Nsd + 1).ToList();
            var NZTAB = 0;
            mergevMod.MERGEV(sDRDRMod.sd, sDRDRMod.Nsd, sDRDRMod.rd, sDRDRMod.Nrd, ZTAB, ref NZTAB);

            var WTS = Enumerable.Repeat(0d, NZTAB + 1).ToList();
            var IZTAB = Enumerable.Repeat(0, NZTAB + 1).ToList();
            var PHITAB = Enumerable.Repeat(new Complex(), NZTAB + 1).ToList();

            var weightMod = new WeightMod();
            weightMod.WEIGHT(Z, NTot1, ZTAB, NZTAB, WTS, IZTAB);

            var modesave = new List<List<double>>(NZTAB + 1);
            for (var i = 0; i <= NZTAB + 1; i++)
            {
                modesave.Add(Enumerable.Repeat(0d, krakMod.M + 1).ToList());
            }

            modesOut.LRecL = 32;
            modesOut.Title = "Dummy title";
            modesOut.NFreq = 1;
            modesOut.NMedia = krakMod.LastAcoustic - krakMod.FirstAcoustic + 1;
            modesOut.NTot = NZTAB;
            modesOut.NMat = NZTAB;

            modesOut.N = new List<int>(krakMod.N);
            modesOut.Material = new List<string>(krakMod.Mater);
            modesOut.Depth = new List<double>(krakMod.Depth);
            modesOut.rho = new List<double>(krakMod.RHO);
            modesOut.freqVec = new List<double> { 0, krakMod.Freq };
            modesOut.Z = new List<double>(ZTAB);

            modesOut.BCTop = krakMod.TopOpt[1].ToString();//changed
            modesOut.cPT = krakMod.CPT;
            modesOut.cST = krakMod.CST;
            modesOut.rhoT = krakMod.rhoT;
            modesOut.DepthT = krakMod.Depth[1];

            modesOut.BCBot = krakMod.BotOpt[0].ToString();
            modesOut.cPB = krakMod.CPB;
            modesOut.cSB = krakMod.CSB;
            modesOut.rhoB = krakMod.rhoB;
            modesOut.DepthB = krakMod.Depth[krakMod.NMedia + 1];

            var F = 0.0;
            var G = 0.0;
            var IPower = 0;
            modesOut.Phi.Add(new List<Complex>());
            for (krakMod.Mode = 1; krakMod.Mode <= krakMod.M; krakMod.Mode++)
            {
                var X = krakMod.EVMat[1][krakMod.Mode];
                var bcimpMod = new BCIMPMod();
                bcimpMod.BCIMP(krakMod, X, BCTop, "TOP", krakMod.CPT, krakMod.CST, krakMod.rhoT, ref F, ref G, ref IPower);

                int L;
                double XH2;
                if (G == 0.0)
                {
                    D[1] = 1.0;
                    E[2] = 2.220446049250313 / Math.Pow(10, 16);
                }
                else
                {
                    L = krakMod.LOC[krakMod.FirstAcoustic] + 1;
                    XH2 = X * krakMod.H[krakMod.FirstAcoustic] * krakMod.H[krakMod.FirstAcoustic];
                    Hrho = krakMod.H[krakMod.FirstAcoustic] * krakMod.RHO[L];
                    D[1] = (krakMod.B1[L] - XH2) / Hrho / 2.0 + F / G;
                }

                var ITP = NTot;
                J = 1;
                L = krakMod.LOC[krakMod.FirstAcoustic] + 1;

                for (var Medium = krakMod.FirstAcoustic; Medium <= krakMod.LastAcoustic; Medium++)
                {
                    XH2 = X * Math.Pow(krakMod.H[Medium], 2);
                    Hrho = krakMod.H[Medium] * krakMod.RHO[krakMod.LOC[Medium] + 1];

                    if (Medium >= krakMod.FirstAcoustic + 1)
                    {
                        L += 1;
                        D[J] = (D[J] + (krakMod.B1[L] - XH2) / Hrho) / 2.0;
                    }

                    for (var ii = 1; ii <= krakMod.N[Medium]; ii++)
                    {                        
                        J += 1;
                        L += 1;
                        D[J] = (krakMod.B1[L] - XH2) / Hrho;

                        if (krakMod.B1[L] - XH2 + 2.0 > 0.0)
                        {
                            ITP = Math.Min(J, ITP);
                        }
                    }
                }
                
                bcimpMod.BCIMP(krakMod, X, BCBot, "BOT", krakMod.CPB, krakMod.CSB, krakMod.rhoB, ref F, ref G, ref IPower);
                if (G == 0.0)
                {
                    D[NTot1] = 1.0;
                    E[NTot1] = 2.220446049250313 / Math.Pow(10, 16);
                }
                else
                {
                    D[NTot1] = D[NTot1] / 2.0 - F / G;
                }

                var sinvitMod = new SinvitdMod();
                sinvitMod.SINVIT(NTot1, D, E, 0, PHI);

                /* D.RemoveAt(0);
                 E.RemoveAt(0);
                 PHI.RemoveAt(0);
                 int IERR = 0;

                 var DArr = D.ToArray();
                 var EArr = E.ToArray();
                 var PHIArr = PHI.ToArray();
                 FortranDllWrap.INVITER( ref NTot1, DArr, EArr, ref IERR, PHIArr);

                 PHI = new List<double>(PHIArr);
                 PHI.Insert(0, 0);
                 D.Insert(0, 0);
                 E.Insert(0, 0);*/

                NORMIZ(krakMod, PHI, ITP, NTot1, X);

                for (var i = 1; i <= NZTAB; i++)
                {
                    PHITAB[i] = PHI[IZTAB[i]] + WTS[i] * (PHI[IZTAB[i] + 1] - PHI[IZTAB[i]]);
                }

                modesOut.Phi.Add(new List<Complex>(PHITAB));

                for (var i = 1; i <= NZTAB; i++)
                {
                    modesave[i][krakMod.Mode] = PHITAB[i].Real;
                }
            }


            modes = new List<List<double>>(NZTAB + 1);
            for (var i = 0; i <= NZTAB; i++)
            {
                modes.Add(Enumerable.Repeat(0d, nm + 1).ToList());
            }
            zm = Enumerable.Repeat(0d, NZTAB + 1).ToList();

            var MMM = Math.Min(krakMod.M, nm);
            for (var MZ = 1; MZ <= NZTAB; MZ++)
            {
                for (krakMod.Mode = 1; krakMod.Mode <= MMM; krakMod.Mode++)
                {
                    modes[MZ][krakMod.Mode] = modesave[MZ][krakMod.Mode];
                }
                zm[MZ] = ZTAB[MZ];
            }
        }

        private void SOLVE1(KrakMod krakMod, int nm)
        {
            var XMin = 1.00001 * krakMod.Omega2 / Math.Pow(krakMod.CHigh, 2);
            var DELTA = 0.0;
            var IPower = 0;
            FUNCT(krakMod, XMin, ref DELTA, ref IPower);
            krakMod.M = krakMod.ModeCount;

            var XL = Enumerable.Repeat(0d, krakMod.M + 1).ToList();
            var XR = Enumerable.Repeat(0d, krakMod.M + 1).ToList();

            if (krakMod.ISet == 1)
            {//check
                krakMod.EVMat = new List<List<double>>(krakMod.NSets + 1);
                for (var i = 0; i <= krakMod.NSets; i++)
                {
                    krakMod.EVMat.Add(Enumerable.Repeat(0d, krakMod.M + 1).ToList());
                }

                krakMod.Extrap = new List<List<double>>(krakMod.NSets + 1);
                for (var i = 0; i <= krakMod.NSets; i++)
                {
                    krakMod.Extrap.Add(Enumerable.Repeat(0d, krakMod.M + 1).ToList());
                }

                krakMod.k = Enumerable.Repeat(new Complex(), krakMod.M + 1).ToList();
                krakMod.VG = Enumerable.Repeat(0d, krakMod.M + 1).ToList();
            }

            var XMax = krakMod.Omega2 / Math.Pow(krakMod.CLow, 2);
            FUNCT(krakMod, XMax, ref DELTA, ref IPower);
            krakMod.M -= krakMod.ModeCount;
            krakMod.M = Math.Min(krakMod.M, nm + 1);

            var NTot = krakMod.N.GetRange(krakMod.FirstAcoustic, krakMod.LastAcoustic - krakMod.FirstAcoustic + 1).Sum();
            //0.00098697030971334134
            //0.00098697030971334134

            //0.0017545963379714496
            //0.0020142049798141637
            BISECT(krakMod, XMin, XMax, ref XL, ref XR);

            krakMod.M = Math.Min(krakMod.M, nm);
            var X = 0.0;
            for (krakMod.Mode = 1; krakMod.Mode <= krakMod.M; krakMod.Mode++)
            {
                var X1 = XL[krakMod.Mode];
                var X2 = XR[krakMod.Mode];
                var EPS = Math.Abs(X2) * 10 * Math.Pow(0.1, 14);
                ZBRENTX(krakMod, ref X, ref X1, ref X2, EPS);

                //error

                krakMod.EVMat[krakMod.ISet][krakMod.Mode] = X;
            }

        }

        private void SOLVE2(KrakMod krakMod)
        {
            var X = krakMod.Omega2 / Math.Pow(krakMod.CLow, 2);
            var MaxIT = 500;
            var P = Enumerable.Repeat(0d, 11).ToList();

            if (krakMod.k == null)
            {
                krakMod.M = 3000;
                krakMod.EVMat = new List<List<double>>(krakMod.NSets + 1);
                for (var i = 0; i <= krakMod.NSets; i++)
                {
                    krakMod.EVMat.Add(Enumerable.Repeat(0d, krakMod.M + 1).ToList());
                }

                krakMod.Extrap = new List<List<double>>(krakMod.NSets + 1);
                for (var i = 0; i <= krakMod.NSets; i++)
                {
                    krakMod.Extrap.Add(Enumerable.Repeat(0d, krakMod.M + 1).ToList());
                }

                krakMod.k = Enumerable.Repeat(new Complex(), krakMod.M + 1).ToList();
                krakMod.VG = Enumerable.Repeat(0d, krakMod.M + 1).ToList();
            }

            for (krakMod.Mode = 1; krakMod.Mode <= krakMod.M; krakMod.Mode++)
            {
                X = 1.00001 * X;
                if (krakMod.ISet >= 2)
                {
                    for (var i = 1; i <= krakMod.ISet - 1; i++)
                    {
                        P[i] = krakMod.EVMat[i][krakMod.Mode];
                    }

                    if (krakMod.ISet >= 3)
                    {
                        for (var II = 1; II <= krakMod.ISet - 2; II++)
                        {
                            for (var J = 1; J <= krakMod.ISet - II - 1; J++)
                            {
                                var X1 = Math.Pow(krakMod.HV[J], 2);
                                var X2 = Math.Pow(krakMod.HV[J + II], 2);

                                P[J] = ((Math.Pow(krakMod.HV[krakMod.ISet], 2) - X2) * P[J] -
                                        (Math.Pow(krakMod.HV[krakMod.ISet], 2) - X1) * P[J + 1]) / (X1 - X2);
                            }
                        }
                        X = P[1];
                    }
                }
                var IT = 1;
                var TOL = Math.Abs(X) * (krakMod.B1.Count - 1) * Math.Pow(0.1, 15);

                ZSCEX(krakMod, ref X, TOL, IT, MaxIT);

                krakMod.EVMat[krakMod.ISet][krakMod.Mode] = X;

                if (krakMod.Omega2 / X > Math.Pow(krakMod.CHigh, 2))
                {
                    krakMod.M = krakMod.Mode - 1;
                    return;
                }
            }


        }

        private void SOLVE3(KrakMod krakMod)
        {
            var MaxIT = 500;
            var XMin = 1.00001 * krakMod.Omega2 / Math.Pow(krakMod.CHigh, 2);
            var DELTA = 0.0;
            var IPower = 0;
            var IT = 0;

            FUNCT(krakMod, XMin, ref DELTA, ref IPower);
            krakMod.M = krakMod.ModeCount;

            for (krakMod.Mode = 1; krakMod.Mode <= krakMod.M; krakMod.Mode++)
            {
                var X = krakMod.EVMat[krakMod.ISet][krakMod.Mode];
                var TOL = Math.Abs(X) * Math.Pow(10, (2.0 - 15));
                ZSCEX(krakMod, ref X, TOL, IT, MaxIT);

                // X = 2.2 * Math.Pow(0.1, 307);

                krakMod.EVMat[krakMod.ISet][krakMod.Mode] = X;
                if (krakMod.Omega2 / X > Math.Pow(krakMod.CHigh, 2))
                {
                    krakMod.M = krakMod.Mode - 1;
                    return;
                }
            }
        }

        private void FUNCT(KrakMod krakMod, double X, ref double DELTA, ref int IPower)
        {
            double Roof = Math.Pow(10, 50);
            double Floor = Math.Pow(0.1, 50);
            int IPowerR = 50;
            int IPowerF = -50;

            double F = 0.0, G = 0.0, F1 = 0.0, G1 = 0.0;
            int IPower1 = 0;

            var bcimpMod = new BCIMPMod();

            if (X <= krakMod.Omega2 / Math.Pow(krakMod.CHigh, 2))
            {
                DELTA = 0.0;
                IPower = 0;
                return;
            }

            krakMod.ModeCount = 0;
            var BCType = krakMod.BotOpt[0].ToString();
            bcimpMod.BCIMP(krakMod, X, BCType, "BOT", krakMod.CPB, krakMod.CSB, krakMod.rhoB, ref F, ref G, ref IPower);

            ACOUST(krakMod, X, ref F, ref G, ref IPower);
            BCType = krakMod.TopOpt[1].ToString();
            bcimpMod.BCIMP(krakMod, X, BCType, "TOP", krakMod.CPT, krakMod.CST, krakMod.rhoT, ref F1, ref G1, ref IPower1);

            DELTA = F * G1 - G * F1;
            if (G * DELTA > 0.0)
            {
                krakMod.ModeCount++;
            }

            if (krakMod.Mode > 1 && krakMod.NMedia > (krakMod.LastAcoustic - krakMod.FirstAcoustic + 1))
            {
                for (var J = 1; J <= krakMod.Mode - 1; J++)
                {
                    DELTA /= (X - krakMod.EVMat[krakMod.ISet][J]);

                    while (Math.Abs(DELTA) < Floor && Math.Abs(DELTA) > 0.0)
                    {
                        DELTA = Roof * DELTA;
                        IPower -= IPowerR;
                    }

                    while (Math.Abs(DELTA) > Roof)
                    {
                        DELTA = Floor * DELTA;
                        IPower -= IPowerF;
                    }
                }
            }
        }

        private void ACOUST(KrakMod krakMod, double X, ref double F, ref double G, ref int IPower)
        {// todo
            var Roof = Math.Pow(10, 50);
            var Floor = Math.Pow(0.1, 50);
            var IPowerF = -50;

            if (krakMod.FirstAcoustic == 0)
            {
                return;
            }

            for (var Medium = krakMod.LastAcoustic; Medium >= krakMod.FirstAcoustic; Medium--)
            {
                var H2K2 = Math.Pow(krakMod.H[Medium], 2) * X;
                var ii = krakMod.LOC[Medium] + krakMod.N[Medium] + 1;
                var rhoM = krakMod.RHO[krakMod.LOC[Medium] + 1];
                var P1 = -2.0 * G;
                var P2 = (krakMod.B1[ii] - H2K2) * G - 2.0 * krakMod.H[Medium] * F * rhoM;

                var P0 = 0.0;
                for (ii = krakMod.LOC[Medium] + krakMod.N[Medium]; ii >= krakMod.LOC[Medium] + 1; ii--)
                {
                    P0 = P1;
                    P1 = P2;
                    P2 = (H2K2 - krakMod.B1[ii]) * P1 - P0;

                    if (P0 * P1 <= 0.0)
                    {
                        krakMod.ModeCount++;
                    }

                    while (Math.Abs(P2) > Roof)
                    {
                        P0 = Floor * P0;
                        P1 = Floor * P1;
                        P2 = Floor * P2;
                        IPower -= IPowerF;
                    }
                }

                rhoM = krakMod.RHO[krakMod.LOC[Medium] + 1];
                F = -(P2 - P0) / (2.0 * krakMod.H[Medium]) / rhoM;
                G = -P1;
            }
        }

        private void NORMIZ(KrakMod krakMod, List<double> PHI, int ITP, int NTot1, double X)
        {
            Complex PERK = new Complex(0.0, 0.0);
            Complex DEL = new Complex();

            double SLOW, SQNRM;
            SQNRM = 0.0;
            SLOW = 0.0;

            if (krakMod.TopOpt[1] == 'A')
            {
                DEL = -0.5 * (krakMod.Omega2 / Complex.Pow(krakMod.CPT, 2) - (krakMod.Omega2 / Complex.Pow(krakMod.CPT, 2)).Real /
                            Math.Sqrt(X - (krakMod.Omega2 / Complex.Pow(krakMod.CPT, 2)).Real));
                /*DEL = krakMod.i * Complex.Sqrt(X - krakMod.Omega2/(Complex.Pow(krakMod.CPT,2)));*/
                PERK -= DEL * Math.Pow(PHI[1], 2) / krakMod.rhoT;
                SLOW += Math.Pow(PHI[1], 2) / (2 * Math.Sqrt(X - (krakMod.Omega2 / Complex.Pow(krakMod.CPT, 2)).Real))
                        / (krakMod.rhoT * Math.Pow(krakMod.CPT.Real, 2));
            }

            var L = krakMod.LOC[krakMod.FirstAcoustic];
            var J = 1;

            for (var Medium = krakMod.FirstAcoustic; Medium <= krakMod.LastAcoustic; Medium++)
            {
                L += 1;
                var rhoM = krakMod.RHO[L];
                var rhoOMH2 = rhoM * krakMod.Omega2 * Math.Pow(krakMod.H[Medium], 2);

                SQNRM += 0.5 * krakMod.H[Medium] * Math.Pow(PHI[J], 2) / rhoM;
                PERK += 0.5 * krakMod.H[Medium] * krakMod.i * krakMod.B1C[L] * Math.Pow(PHI[J], 2) / rhoM;
                SLOW += 0.5 * krakMod.H[Medium] * (krakMod.B1[L] + 2) * Math.Pow(PHI[J], 2) / rhoOMH2;

                var L1 = L + 1;
                L = L + krakMod.N[Medium] - 1;
                var J1 = J + 1;
                J = J + krakMod.N[Medium] - 1;

                var phiPowRange = PHI.GetRange(J1, J - J1 + 1).Select(x => x * x).ToList();
                phiPowRange.Insert(0, 0);

                var b1Range = krakMod.B1.GetRange(L1, L - L1 + 1).Select(x => x + 2).ToList();
                b1Range.Insert(0, 0);

                var b1CRange = krakMod.B1C.GetRange(L1, L - L1 + 1).ToList();
                b1CRange.Insert(0, 0);

                var phiPowSum = 0.0;
                var b1PhiSum = 0.0;
                var b1CPhiSum = 0.0;
                for (var i = 1; i < phiPowRange.Count; i++)
                {
                    phiPowSum += phiPowRange[i];
                    b1PhiSum += b1Range[i] * phiPowRange[i];
                    b1CPhiSum += b1CRange[i] * phiPowRange[i];
                }

                SQNRM += krakMod.H[Medium] * phiPowSum / rhoM;
                PERK += krakMod.H[Medium] * krakMod.i * b1CPhiSum / rhoM;
                SLOW += krakMod.H[Medium] * b1PhiSum / rhoOMH2;

                L += 1;
                J += 1;
                SQNRM += 0.5 * krakMod.H[Medium] * Math.Pow(PHI[J], 2) / rhoM;
                PERK += 0.5 * krakMod.H[Medium] * krakMod.i * krakMod.B1C[L] * Math.Pow(PHI[J], 2) / rhoM;
                SLOW += 0.5 * krakMod.H[Medium] * (krakMod.B1[L] + 2) * Math.Pow(PHI[J], 2) / rhoOMH2;
            }

            if (krakMod.BotOpt[0] == 'A')
            {
                DEL = -0.5 * (krakMod.Omega2 / Complex.Pow(krakMod.CPB, 2) - (krakMod.Omega2 / Complex.Pow(krakMod.CPB, 2)).Real /
                            Math.Sqrt(X - (krakMod.Omega2 / Complex.Pow(krakMod.CPB, 2)).Real));
                PERK -= DEL * Math.Pow(PHI[J], 2) / krakMod.rhoB;
                SLOW += Math.Pow(PHI[J], 2) / (2 * Math.Sqrt(X - (krakMod.Omega2 / Complex.Pow(krakMod.CPB, 2)).Real))
                        / (krakMod.rhoB * Math.Pow(krakMod.CPB.Real, 2));
            }

            var X1 = 0.9999999 * X;
            var X2 = 1.0000001 * X;

            var BCType = krakMod.TopOpt[1].ToString();
            double F1 = 0.0, G1 = 0.01, F2 = 0.0, G2 = 0.0;
            int IPower = 0;
            var bcimpMod = new BCIMPMod();
            bcimpMod.BCIMP(krakMod, X1, BCType, "TOP", krakMod.CPT, krakMod.CST, krakMod.rhoT, ref F1, ref G1, ref IPower);
            bcimpMod.BCIMP(krakMod, X2, BCType, "TOP", krakMod.CPT, krakMod.CST, krakMod.rhoT, ref F2, ref G2, ref IPower);
            var DrhoDX = 0.0;
            if (G1 != 0)
            {
                DrhoDX = -(F2 / G2 - F1 / G1) / (X2 - X1);
            }

            BCType = krakMod.BotOpt[0].ToString();
            bcimpMod.BCIMP(krakMod, X1, BCType, "BOT", krakMod.CPB, krakMod.CSB, krakMod.rhoB, ref F1, ref G1, ref IPower);
            bcimpMod.BCIMP(krakMod, X2, BCType, "BOT", krakMod.CPB, krakMod.CSB, krakMod.rhoB, ref F2, ref G2, ref IPower);
            var DetaDX = 0.0;
            if (G1 != 0)
            {
                DrhoDX = -(F2 / G2 - F1 / G1) / (X2 - X1);
            }

            var RN = SQNRM + DrhoDX * Math.Pow(PHI[1], 2) - DetaDX * Math.Pow(PHI[NTot1], 2);

            if (RN <= 0.0)
            {
                RN = -RN;
            }

            var SCALEF = 1.0 / Math.Sqrt(RN);
            if (PHI[ITP] < 0.0)
            {
                SCALEF = -SCALEF;
            }

            for (var i = 1; i <= NTot1; i++)
            {
                PHI[i] = SCALEF * PHI[i];
            }

            PERK = Math.Pow(SCALEF, 2) * PERK;
            SLOW = Math.Pow(SCALEF, 2) * SLOW * Math.Sqrt(krakMod.Omega2 / X);
            krakMod.VG[krakMod.Mode] = 1 / SLOW;

            SCAT(krakMod, ref PERK, PHI, X);
        }

        private void SCAT(KrakMod krakMod, ref Complex PERK, List<double> PHI, double X)
        {
            var OMEGA = Math.Sqrt(krakMod.Omega2);
            var KX = Math.Sqrt(X);
            double rho1 = 0.0, rho2 = 0.0, eta1SQ = 0.0, eta2SQ = 0.0,
                U = 0.0;

            var BCType = krakMod.TopOpt[1].ToString();
            double rhoINS;
            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                var Itop = krakMod.LOC[krakMod.FirstAcoustic] + krakMod.N[krakMod.FirstAcoustic] + 1;
                rhoINS = krakMod.RHO[Itop];
                var CINS = Math.Sqrt(krakMod.Omega2 * Math.Pow(krakMod.H[krakMod.FirstAcoustic], 2) / (2.0 + krakMod.B1[krakMod.FirstAcoustic]));
                var CImped = TWERSK(BCType[0], OMEGA, krakMod.BumDen, krakMod.xi, krakMod.eta, KX, rhoINS, CINS);
                CImped /= (-krakMod.i * OMEGA * rhoINS);
                var DPHIDZ = PHI[2] / krakMod.H[krakMod.FirstAcoustic];
                PERK -= CImped * Math.Pow(DPHIDZ, 2);
            }

            BCType = krakMod.BotOpt;
            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                var Ibot = krakMod.LOC[krakMod.FirstAcoustic] + krakMod.N[krakMod.FirstAcoustic] + 1;
                rhoINS = krakMod.RHO[Ibot];
                var CINS = Math.Sqrt(krakMod.Omega2 * Math.Pow(krakMod.H[krakMod.LastAcoustic], 2) / (2.0 + krakMod.B1[krakMod.LastAcoustic]));
                var CImped = TWERSK(BCType[0], OMEGA, krakMod.BumDen, krakMod.xi, krakMod.eta, KX, rhoINS, CINS);
                CImped /= (-krakMod.i * OMEGA * rhoINS);
                var DPHIDZ = PHI[2] / krakMod.H[krakMod.FirstAcoustic];
                PERK -= CImped * Math.Pow(DPHIDZ, 2);
            }

            var J = 1;
            var L = krakMod.LOC[krakMod.FirstAcoustic];

            for (var Medium = krakMod.FirstAcoustic - 1; Medium <= krakMod.LastAcoustic; Medium++)
            {
                if (Medium == krakMod.FirstAcoustic - 1)
                {
                    BCType = krakMod.TopOpt[1].ToString();
                    switch (BCType)
                    {
                        case "A":
                            rho1 = krakMod.rhoT;
                            eta1SQ = X - (krakMod.Omega2 / Complex.Pow(krakMod.CPT, 2)).Real;
                            U = Math.Sqrt(eta1SQ) * PHI[1] / krakMod.rhoT;
                            break;
                        case "V":
                            rho1 = Math.Pow(0.1, 9);
                            eta1SQ = 1.0;
                            rhoINS = krakMod.RHO[krakMod.LOC[krakMod.FirstAcoustic] + 1];
                            U = PHI[2] / krakMod.H[krakMod.FirstAcoustic] / rhoINS;
                            break;
                        case "R":
                            rho1 = Math.Pow(10, 9);
                            eta1SQ = 1.0;
                            U = 0.0;
                            break;
                    }
                }
                else
                {
                    var H2 = Math.Pow(krakMod.H[Medium], 2);
                    J += krakMod.N[Medium];
                    L = krakMod.LOC[Medium] + krakMod.N[Medium] + 1;

                    rho1 = krakMod.RHO[L];
                    eta1SQ = (2.0 + krakMod.B1[L]) / H2 - X;
                    U = (-PHI[J - 1] - 0.5 * (krakMod.B1[L] - H2 * X) * PHI[J]) / (krakMod.H[Medium] * rho1);
                }

                if (Medium == krakMod.LastAcoustic)
                {
                    BCType = krakMod.BotOpt[0].ToString();
                    switch (BCType)
                    {
                        case "A":
                            rho2 = krakMod.rhoB;
                            eta2SQ = (krakMod.Omega2 / Complex.Pow(krakMod.CPB, 2)).Real - X;
                            break;
                        case "V":
                            rho2 = Math.Pow(0.1, 9);
                            eta2SQ = 1.0;
                            break;
                        case "R":
                            rho2 = Math.Pow(10, 9);
                            eta2SQ = 1.0;
                            break;
                    }
                }
                else
                {
                    rho2 = krakMod.RHO[L + 1];
                    eta2SQ = (2.0 + krakMod.B1[L + 1]) / Math.Pow(krakMod.H[Medium + 1], 2) - X;
                }

                var PHIC = new Complex(PHI[J], 0.0);
                var kupingMod = new KupingMod();
                PERK += kupingMod.KUPING(krakMod.SIGMA[Medium + 1], eta1SQ, rho1, eta2SQ, rho2, PHIC, U);
            }

            krakMod.k[krakMod.Mode] = PERK;
        }

        private void BISECT(KrakMod krakMod, double XMin, double XMax, ref List<double> XL, ref List<double> XR)
        {
            int MaxBis = 50;
            for (var i = 0; i < XL.Count; i++)
            {
                XL[i] = XMin;
                XR[i] = XMax;
            }

            double DELTA = 0;
            int IPower = 0;

            FUNCT(krakMod, XMax, ref DELTA, ref IPower);
            var NZER1 = krakMod.ModeCount;

            if (krakMod.M == 1)
            {
                return;
            }

            for (krakMod.Mode = 1; krakMod.Mode <= krakMod.M - 1; krakMod.Mode++)
            {
                if (XL[krakMod.Mode] == XMin)
                {
                    double X2 = XR[krakMod.Mode];
                    var max = XL.GetRange(krakMod.Mode + 1, krakMod.M - krakMod.Mode).Max();
                    double X1 = Math.Max(max, XMin);

                    for (var J = 1; J <= MaxBis; J++)
                    {
                        double X = X1 + (X2 - X1) / 2;
                        FUNCT(krakMod, X, ref DELTA, ref IPower);
                        var NZeros = krakMod.ModeCount - NZER1;

                        if (NZeros < krakMod.Mode)
                        {
                            X2 = X;
                            XR[krakMod.Mode] = X;
                        }
                        else
                        {
                            X1 = X;
                            if (XR[NZeros + 1] >= X)
                            {
                                XR[NZeros + 1] = X;
                            }
                            if (XL[NZeros] <= X)
                            {
                                XL[NZeros] = X;
                            }
                        }

                        if (XL[krakMod.Mode] != XMin)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void ZSCEX(KrakMod krakMod, ref double x2, double TOL, int Iteration, int MAXIteration)
        {
            var x1 = x2 + 10.0 * TOL;
            double F1 = 0;
            int IPower1 = 1;

            FUNCT(krakMod, x1, ref F1, ref IPower1);

            for (Iteration = 1; Iteration <= MAXIteration; Iteration++)
            {
                double x0 = x1;
                double F0 = F1;
                int IPower0 = IPower1;
                x1 = x2;
                double shift;

                FUNCT(krakMod, x1, ref F1, ref IPower1);
                if (F1 == 0.0)
                {
                    shift = 0.0;
                }
                else
                {
                    shift = (x1 - x0) / (1.0 - F0 / F1 * Math.Pow(10.0, (IPower0 - IPower1)));
                }

                x2 = x1 - shift;
                if (Math.Abs(x2 - x1) < TOL || Math.Abs(x2 - x0) < TOL)
                {
                    return;
                }
            }
        }

        private void ZBRENTX(KrakMod krakMod, ref double X, ref double A, ref double B, double T)
        {
            double MACHEP = 1 / Math.Pow(10, 15);
            double TEN = 10.0;
            double FA = 0.0, FB = 0.0;
            int IEXPA = 0, IEXPB = 0;

            FUNCT(krakMod, A, ref FA, ref IEXPA);
            FUNCT(krakMod, B, ref FB, ref IEXPB);

            if ((FA > 0.0 && FB > 0.0) || (FA < 0.0 && FB < 0.0))
            {
                return;
            }
        Mark2000:
            double C = A;
            double FC = FA;
            int IEXPC = IEXPA;
            double E = B - A;
            double D = E;
            double F1;
            double F2;
            if (IEXPA < IEXPB)
            {
                F1 = FC * Math.Pow(TEN, (IEXPC - IEXPB));
                F2 = FB;
            }
            else
            {
                F1 = FC;
                F2 = FB * Math.Pow(TEN, (IEXPB - IEXPC));
            }
        Mark3000:
            if (Math.Abs(F1) < Math.Abs(F2))
            {
                A = B;
                B = C;
                C = A;
                FA = FB;
                IEXPA = IEXPB;
                FB = FC;
                IEXPB = IEXPC;
                FC = FA;
                IEXPC = IEXPA;
            }

            double TOL = 2.0 * MACHEP * Math.Abs(B) + T;
            double M = 0.5 * (C - B);
            if (Math.Abs(M) > TOL && FB != 0)
            {

                if (IEXPA < IEXPB)
                {
                    F1 = FA * Math.Pow(TEN, (IEXPA - IEXPB));
                    F2 = FB;
                }
                else
                {
                    F1 = FA;
                    F2 = FB * Math.Pow(TEN, (IEXPB - IEXPA));
                }

                if (Math.Abs(E) < TOL || Math.Abs(F1) <= Math.Abs(F2))
                {
                    E = M;
                    D = E;
                }
                else
                {
                    double S = FB / FA * Math.Pow(TEN, (IEXPB - IEXPA));
                    double P;
                    double Q;
                    if (A == C)
                    {
                        P = 2.0 * M * S;
                        Q = 1.0 - S;
                    }
                    else
                    {
                        Q = FA / FC * Math.Pow(TEN, (IEXPA - IEXPC));
                        double R = FB / FC * Math.Pow(TEN, (IEXPB - IEXPC));
                        P = S * (2.0 * M * Q * (Q - R) - (B - A) * (R - 1.0));
                        Q = (Q - 1.0) * (R - 1.0) * (S - 1.0);
                    }
                    if (P > 0.0)
                    {
                        Q = -Q;
                    }
                    else
                    {
                        P = -P;
                    }

                    S = E;
                    E = D;

                    if ((2.0 * P < 3.0 * M * Q - Math.Abs(TOL * Q)) && (P < Math.Abs(0.5 * S * Q)))
                    {
                        D = P / Q;
                    }
                    else
                    {
                        E = M;
                        D = E;
                    }
                }

                A = B;
                FA = FB;
                IEXPA = IEXPB;

                if (Math.Abs(D) > TOL)
                {
                    B += D;
                }
                else
                {
                    if (M > 0.0)
                    {
                        B += TOL;
                    }
                    else
                    {
                        B -= TOL;
                    }
                }

                FUNCT(krakMod, B, ref FB, ref IEXPB);
                if (FB > 0 == FC > 0) goto Mark2000;
                goto Mark3000;
            }

            X = B;
        }

        private Complex TWERSK(char OPT, double OMEGA, double BUMDEN, double XI, double ETA,
                                 double KX, double RHO0, double C0)
        {
            return new Complex();
        }
    }
}
