using Kraken.Calculation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation
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
        public CalculatedModesInfo OceanAcousticNormalModes(int nm, double frq, int nl, string note1, List<List<double>> bb, int nc,
                                             List<List<double>> ssp, string note2, double bsig, List<double> clh, double rng, int nsr, List<double> zsr,
                                             int nrc, List<double> zrc, int nz, List<double> tahsp, List<double> tsp, List<double> bahsp, ref List<double> cg, ref List<double> cp,
                                             ref List<double> zm, ref List<List<double>> modes, ref List<Complex> k, List<string> warnings)
        {
            var krakenModule = new KrakenModule();
            krakenModule.Init();

            krakenModule.NV = new List<int> { 0, 1, 2, 4, 8, 16 };

            krakenModule.Frequency = frq;
            krakenModule.NMedia = nl;
            krakenModule.BCTop = note1.Substring(0, 3);
            krakenModule.BCBottom = note2[0].ToString();

            var rangedDataManager = new RangedDataManager();
            var meshInitializer = new MeshInitializer(krakenModule);

            krakenModule.Depth[1] = 0;
            for (var il = 1; il <= nl; il++)
            {
                krakenModule.NG[il] = (int)bb[il][1];
                krakenModule.Sigma[il] = bb[il][2];
                krakenModule.Depth[il + 1] = bb[il][3];
            }

            krakenModule.Sigma[nl + 1] = bsig;
            meshInitializer.ProccedMesh(krakenModule, nc, ssp, tahsp, tsp, bahsp);

            krakenModule.CLow = clh[1];
            krakenModule.CHigh = clh[2];

            krakenModule.RMax = rng;

            var zMin = krakenModule.Depth[1];
            var zMax = krakenModule.Depth[krakenModule.NMedia + 1];           

            rangedDataManager.ProceedSourceAndReceiverDepths(zMin, zMax, nsr, nrc, zsr, zrc);

            krakenModule.Omega2 = Math.Pow((2.0 * Math.PI * krakenModule.Frequency), 2);

            double error = 0;
            var isSuccess = false;
            var modesInfo = new CalculatedModesInfo();
            for (krakenModule.ISet = 1; krakenModule.ISet <= krakenModule.NSets; krakenModule.ISet++)
            {
                krakenModule.N = krakenModule.NG.Select(x => x * krakenModule.NV[krakenModule.ISet]).ToList();
                for (var j = 1; j <= krakenModule.NMedia; j++)
                {
                    krakenModule.H[j] = (krakenModule.Depth[j + 1] - krakenModule.Depth[j]) / krakenModule.N[j];
                }
                krakenModule.HV[krakenModule.ISet] = krakenModule.H[1];
                SOLVE(modesInfo, krakenModule, rangedDataManager, meshInitializer, ref error, nm, nz, ref zm, ref modes);

                if (error * 1000.0 * krakenModule.RMax < 1.0)
                {
                    isSuccess = true;
                    break;
                }
            }

            if (isSuccess)
            {
                var OMEGA = Math.Sqrt(krakenModule.Omega2);
                var minVal = krakenModule.Extrap[1].GetRange(1, krakenModule.M).Where(x => x > krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2)).Min();
                var Min_Loc = krakenModule.Extrap[1].FindIndex(x => x == minVal);
                krakenModule.M = Min_Loc;

                for (var i = 1; i <= krakenModule.M; i++)
                {
                    krakenModule.K[i] = Complex.Sqrt(krakenModule.Extrap[1][i] + krakenModule.K[i]);
                }

                var MMM = Math.Min(krakenModule.M, nm);
                modesInfo.ModesCount = MMM;
                modesInfo.K = new List<Complex>(krakenModule.K);

                cp = Enumerable.Repeat(0d, MMM + 1).ToList();
                cg = Enumerable.Repeat(0d, MMM + 1).ToList();
                k = Enumerable.Repeat(new Complex(), MMM + 1).ToList();

                for (krakenModule.Mode = 1; krakenModule.Mode <= MMM; krakenModule.Mode++)
                {
                    cp[krakenModule.Mode] = (OMEGA / krakenModule.K[krakenModule.Mode]).Real;
                    cg[krakenModule.Mode] = krakenModule.VG[krakenModule.Mode];
                    k[krakenModule.Mode] = krakenModule.K[krakenModule.Mode];
                }
            }

            warnings.AddRange(krakenModule.Warnings);

            return modesInfo;
        }

        private void INIT(KrakenModule krakenModule, MeshInitializer meshInitializer)
        {
            var ELFLAG = false;
            double CP2 = 0.0, CS2 = 0.0;
            var dummy = new List<List<double>>(7);

            krakenModule.CMin = double.MaxValue;
            krakenModule.FirstAcoustic = 0;
            krakenModule.Loc[1] = 0;

            var NPTS = krakenModule.N.GetRange(1, krakenModule.NMedia).Sum() + krakenModule.NMedia;

            krakenModule.B1 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakenModule.B1C = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakenModule.B2 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakenModule.B3 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakenModule.B4 = Enumerable.Repeat(0d, NPTS + 1).ToList();
            krakenModule.Rho = Enumerable.Repeat(0d, NPTS + 1).ToList();

            var CP = Enumerable.Repeat(new Complex(), NPTS + 1).ToList();
            var CS = Enumerable.Repeat(new Complex(), NPTS + 1).ToList();

            for (var Medium = 1; Medium <= krakenModule.NMedia; Medium++)
            {
                if (Medium != 1)
                {
                    krakenModule.Loc[Medium] = krakenModule.Loc[Medium - 1] + krakenModule.N[Medium - 1] + 1;
                }
                var N1 = krakenModule.N[Medium] + 1;
                var II = krakenModule.Loc[Medium] + 1;

                var TASK = "TAB";
                meshInitializer.EvaluateSSP(krakenModule, krakenModule.Depth, CP, CS, krakenModule.Rho, Medium, ref N1, II, krakenModule.Frequency, krakenModule.BCTop[0].ToString(), krakenModule.BCTop.Substring(2), TASK, dummy);

                if (CS[II].Real == 0)
                {
                    krakenModule.Material[Medium] = "ACOUSTIC";
                    if (krakenModule.FirstAcoustic == 0)
                    {
                        krakenModule.FirstAcoustic = Medium;
                    }
                    krakenModule.LastAcoustic = Medium;

                    var CPMin = CP.GetRange(II, krakenModule.N[Medium] + 1).Select(x => x.Real).Min();
                    krakenModule.CMin = Math.Min(krakenModule.CMin, CPMin);

                    for (var i = II; i <= II + krakenModule.N[Medium]; i++)
                    {
                        krakenModule.B1[i] = -2.0 + Math.Pow(krakenModule.H[Medium], 2) * (krakenModule.Omega2 / Complex.Pow(CP[i], 2)).Real;
                    }

                    for (var i = II; i <= II + krakenModule.N[Medium]; i++)
                    {
                        krakenModule.B1C[i] = (krakenModule.Omega2 / Complex.Pow(CP[i], 2)).Imaginary;
                    }
                }
                else
                {
                    if (krakenModule.Sigma[Medium] != 0)
                    {
                        throw new KrakenException("Rough elastic interfaces are not allowed");
                    }

                    krakenModule.Material[Medium] = "ELASTIC";
                    ELFLAG = true;
                    var TWOH = 2.0 * krakenModule.H[Medium];

                    for (var j = II; j <= II + krakenModule.N[Medium]; j++)
                    {
                        krakenModule.CMin = Math.Min(CS[j].Real, krakenModule.CMin);

                        CP2 = Complex.Pow(CP[j], 2).Real;
                        CS2 = Complex.Pow(CS[j], 2).Real;

                        krakenModule.B1[j] = TWOH / (krakenModule.Rho[j] * CS2);
                        krakenModule.B2[j] = TWOH / (krakenModule.Rho[j] * CP2);
                        krakenModule.B3[j] = 4.0 * TWOH * krakenModule.Rho[j] * CS2 * (CP2 - CS2) / CP2;
                        krakenModule.B4[j] = TWOH * (CP2 - 2.0 * CS2) / CP2;
                        krakenModule.Rho[j] = TWOH * krakenModule.Omega2 * krakenModule.Rho[j];
                    }
                }
            }

            if (krakenModule.BCBottom[0] == 'A')
            {
                if (krakenModule.CSBottom.Real > 0.0)
                {
                    ELFLAG = true;
                    krakenModule.CMin = Math.Min(krakenModule.CMin, krakenModule.CSBottom.Real);
                    krakenModule.CHigh = Math.Min(krakenModule.CHigh, krakenModule.CSBottom.Real);
                }
                else
                {
                    krakenModule.CMin = Math.Min(krakenModule.CMin, krakenModule.CPBottom.Real);
                    // krakenModule.CHigh = Math.Min(krakenModule.CHigh, krakenModule.CPBottom.Real); //commented 27.03.2020
                }
            }

            if (krakenModule.BCTop.Length > 1)
            {
                if (krakenModule.BCTop[1] == 'A')
                {
                    if (krakenModule.CSTop.Real > 0.0)
                    {
                        ELFLAG = true;
                        krakenModule.CMin = Math.Min(krakenModule.CMin, krakenModule.CSTop.Real);
                        krakenModule.CHigh = Math.Min(krakenModule.CHigh, krakenModule.CSTop.Real);
                    }
                    else
                    {
                        krakenModule.CMin = Math.Min(krakenModule.CMin, krakenModule.CPTop.Real);
                        //krakenModule.CHigh = Math.Min(krakenModule.CHigh, krakenModule.CPTop.Real);//commented 27.03.2020
                    }
                }
            }

            if (ELFLAG)
            {
                krakenModule.CMin = 0.85 * krakenModule.CMin;
            }

            krakenModule.CLow = Math.Max(krakenModule.CLow, krakenModule.CMin);
        }

        private void SOLVE(CalculatedModesInfo modesInfo, KrakenModule krakenModule, RangedDataManager rangedDataManager, MeshInitializer meshInitializer, ref double error, int nm, int nz, ref List<double> zm, ref List<List<double>> modes)
        {
            var NOMODES = 0;

            INIT(krakenModule, meshInitializer);

            if (krakenModule.IProf > 1 && krakenModule.ISet <= 2 && krakenModule.BCTop[3] == 'C')
            {
                SOLVE3(krakenModule);
            }
            else if (krakenModule.ISet <= 2 && krakenModule.NMedia <= (krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1))
            {
                SOLVE1(krakenModule, nm);
            }
            else
            {
                SOLVE2(krakenModule);
            }

            for (var i = 1; i <= krakenModule.M; i++)
            {
                krakenModule.Extrap[krakenModule.ISet][i] = krakenModule.EVMat[krakenModule.ISet][i];
            }

            var minVal = krakenModule.Extrap[1].GetRange(1, krakenModule.M).Where(x => x > krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2)).Min();
            var Min_Loc = krakenModule.Extrap[1].FindIndex(x => x == minVal);
            krakenModule.M = Min_Loc;

            if (krakenModule.ISet == 1 && NOMODES == 0)
            {
                VECTOR(modesInfo, krakenModule, rangedDataManager, nm, nz, ref zm, ref modes);
            }

            error = 10;
            var KEY = 2 * krakenModule.M / 3 + 1;
            if (krakenModule.ISet > 1)
            {
                var T1 = krakenModule.Extrap[1][KEY];

                for (var j = krakenModule.ISet - 1; j >= 1; j--)
                {
                    for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
                    {
                        var X1 = Math.Pow(krakenModule.NV[j], 2);
                        var X2 = Math.Pow(krakenModule.NV[krakenModule.ISet], 2);
                        var F1 = krakenModule.Extrap[j][krakenModule.Mode];
                        var F2 = krakenModule.Extrap[j + 1][krakenModule.Mode];
                        krakenModule.Extrap[j][krakenModule.Mode] = F2 - (F1 - F2) / (X2 / X1 - 1.0);
                    }
                }

                var T2 = krakenModule.Extrap[1][KEY];
                error = Math.Abs(T2 - T1);
            }

        }

        private void VECTOR(CalculatedModesInfo modesInfo, KrakenModule krakenModule, RangedDataManager rangedDataManager, int nm, int nz, ref List<double> zm, ref List<List<double>> modes)
        {
            var BCTop = krakenModule.BCTop[1].ToString();
            var BCBottom = krakenModule.BCBottom[0].ToString();

            var NTot = krakenModule.N.GetRange(krakenModule.FirstAcoustic, krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1).Sum();
            var NTot1 = NTot + 1;

            //allocate
            var Z = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var E = Enumerable.Repeat(0d, NTot1 + 1 + 1).ToList();
            var D = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var PHI = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var j = 1;
            Z[1] = krakenModule.Depth[krakenModule.FirstAcoustic];

            var Hrho = 0.0;
            for (var Medium = krakenModule.FirstAcoustic; Medium <= krakenModule.LastAcoustic; Medium++)
            {
                Hrho = krakenModule.H[Medium] * krakenModule.Rho[krakenModule.Loc[Medium] + 1];

                var temp = 1;
                for (var i = j + 1; i <= j + krakenModule.N[Medium]; i++)
                {
                    E[i] = 1.0 / Hrho;
                    Z[i] = Z[j] + krakenModule.H[Medium] * temp;
                    temp++;
                }

                j += krakenModule.N[Medium];
            }

            E[NTot1 + 1] = 1.0 / Hrho;
            var vectorsManager = new VectorsManager();
            var (zTab, NzTab) = vectorsManager.MergeVectors(rangedDataManager.SourceDepths, rangedDataManager.Nsd, rangedDataManager.ReceiverDepths, rangedDataManager.Nrd);

            var PHITAB = Enumerable.Repeat(new Complex(), NzTab + 1).ToList();

            var weightsCalculator = new WeightsCalculator();
            var (WTS,IZTAB) = weightsCalculator.CalculateWeightsAndIndices(Z, NTot1, zTab, NzTab);

            var modesave = new List<List<double>>(NzTab + 1);
            for (var i = 0; i <= NzTab + 1; i++)
            {
                modesave.Add(Enumerable.Repeat(0d, krakenModule.M + 1).ToList());
            }
                     
            modesInfo.NMedia = krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1;
            modesInfo.NTot = NzTab;
            modesInfo.NMat = NzTab;

            modesInfo.N = new List<int>(krakenModule.N);
            modesInfo.Material = new List<string>(krakenModule.Material);
            modesInfo.Depth = new List<double>(krakenModule.Depth);
            modesInfo.Rho = new List<double>(krakenModule.Rho);
            modesInfo.Frequency = krakenModule.Frequency;
            modesInfo.Z = new List<double>(zTab);

            modesInfo.BCTop = krakenModule.BCTop[1].ToString();
            modesInfo.CPTop = krakenModule.CPTop;
            modesInfo.CSTop = krakenModule.CSTop;
            modesInfo.RhoTop = krakenModule.RhoTop;
            modesInfo.DepthTop = krakenModule.Depth[1];

            modesInfo.BCBottom = krakenModule.BCBottom[0].ToString();
            modesInfo.CPBottom = krakenModule.CPBottom;
            modesInfo.CSBottom = krakenModule.CSBottom;
            modesInfo.RhoBottom = krakenModule.RhoBottom;
            modesInfo.DepthBottom = krakenModule.Depth[krakenModule.NMedia + 1];

            var F = 0.0;
            var G = 0.0;
            var iPower = 0;
            modesInfo.Phi.Add(new List<Complex>());
            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
            {
                var X = krakenModule.EVMat[1][krakenModule.Mode];
                var bcimpSolver = new BCImpedanceSolver();
                bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X, BCTop, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref F, ref G, ref iPower);

                int L;
                double XH2;
                if (G == 0.0)
                {
                    D[1] = 1.0;
                    E[2] = 2.220446049250313 / Math.Pow(10, 16);
                }
                else
                {
                    L = krakenModule.Loc[krakenModule.FirstAcoustic] + 1;
                    XH2 = X * krakenModule.H[krakenModule.FirstAcoustic] * krakenModule.H[krakenModule.FirstAcoustic];
                    Hrho = krakenModule.H[krakenModule.FirstAcoustic] * krakenModule.Rho[L];
                    D[1] = (krakenModule.B1[L] - XH2) / Hrho / 2.0 + F / G;
                }

                var ITP = NTot;
                j = 1;
                L = krakenModule.Loc[krakenModule.FirstAcoustic] + 1;

                for (var Medium = krakenModule.FirstAcoustic; Medium <= krakenModule.LastAcoustic; Medium++)
                {
                    XH2 = X * Math.Pow(krakenModule.H[Medium], 2);
                    Hrho = krakenModule.H[Medium] * krakenModule.Rho[krakenModule.Loc[Medium] + 1];

                    if (Medium >= krakenModule.FirstAcoustic + 1)
                    {
                        L += 1;
                        D[j] = (D[j] + (krakenModule.B1[L] - XH2) / Hrho) / 2.0;
                    }

                    for (var ii = 1; ii <= krakenModule.N[Medium]; ii++)
                    {                        
                        j += 1;
                        L += 1;
                        D[j] = (krakenModule.B1[L] - XH2) / Hrho;

                        if (krakenModule.B1[L] - XH2 + 2.0 > 0.0)
                        {
                            ITP = Math.Min(j, ITP);
                        }
                    }
                }
                
                bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X, BCBottom, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref F, ref G, ref iPower);
                if (G == 0.0)
                {
                    D[NTot1] = 1.0;
                    E[NTot1] = 2.220446049250313 / Math.Pow(10, 16);
                }
                else
                {
                    D[NTot1] = D[NTot1] / 2.0 - F / G;
                }

                var IERR = 0;
                var sinvitMod = new EigenvectorFinder();
                sinvitMod.FindEigenvectorUsingInverseIteration(NTot1, D, E,PHI, ref IERR);

                if (IERR != 0)
                {
                    krakenModule.Warnings.Add($"Inverse iteration failed to converge. Mode = {krakenModule.Mode}");
                    PHI = PHI.Select(x => 0d).ToList();
                }
                else
                {
                    NORMIZ(krakenModule, PHI, ITP, NTot1, X);
                }

                for (var i = 1; i <= NzTab; i++)
                {
                    PHITAB[i] = PHI[IZTAB[i]] + WTS[i] * (PHI[IZTAB[i] + 1] - PHI[IZTAB[i]]);
                }

                modesInfo.Phi.Add(new List<Complex>(PHITAB));

                for (var i = 1; i <= NzTab; i++)
                {
                    modesave[i][krakenModule.Mode] = PHITAB[i].Real;
                }
            }

            var MMM = Math.Min(krakenModule.M, nm);
            modes = new List<List<double>>(NzTab + 1);
            for (var i = 0; i <= NzTab; i++)
            {
                modes.Add(Enumerable.Repeat(0d, MMM + 1).ToList());
            }
            zm = Enumerable.Repeat(0d, NzTab + 1).ToList();
            
            for (var MZ = 1; MZ <= NzTab; MZ++)
            {
                for (krakenModule.Mode = 1; krakenModule.Mode <= MMM; krakenModule.Mode++)
                {
                    modes[MZ][krakenModule.Mode] = modesave[MZ][krakenModule.Mode];
                }
                zm[MZ] = zTab[MZ];
            }
        }

        private void SOLVE1(KrakenModule krakenModule, int nm)
        {
            var XMin = 1.00001 * krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2);
            var DELTA = 0.0;
            var iPower = 0;
            FUNCT(krakenModule, XMin, ref DELTA, ref iPower);
            krakenModule.M = krakenModule.ModeCount;

            var XL = Enumerable.Repeat(0d, krakenModule.M + 1).ToList();
            var XR = Enumerable.Repeat(0d, krakenModule.M + 1).ToList();

            if (krakenModule.ISet == 1)
            {//check
                krakenModule.EVMat = new List<List<double>>(krakenModule.NSets + 1);
                for (var i = 0; i <= krakenModule.NSets; i++)
                {
                    krakenModule.EVMat.Add(Enumerable.Repeat(0d, krakenModule.M + 1).ToList());
                }

                krakenModule.Extrap = new List<List<double>>(krakenModule.NSets + 1);
                for (var i = 0; i <= krakenModule.NSets; i++)
                {
                    krakenModule.Extrap.Add(Enumerable.Repeat(0d, krakenModule.M + 1).ToList());
                }

                krakenModule.K = Enumerable.Repeat(new Complex(), krakenModule.M + 1).ToList();
                krakenModule.VG = Enumerable.Repeat(0d, krakenModule.M + 1).ToList();
            }

            var XMax = krakenModule.Omega2 / Math.Pow(krakenModule.CLow, 2);
            FUNCT(krakenModule, XMax, ref DELTA, ref iPower);
            krakenModule.M -= krakenModule.ModeCount;
            krakenModule.M = Math.Min(krakenModule.M, nm + 1);

            var NTot = krakenModule.N.GetRange(krakenModule.FirstAcoustic, krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1).Sum();
            if (krakenModule.M > NTot / 5)
            {
                krakenModule.Warnings.Add("Mesh too coarse to sample the modes adequately");
            }

            BISECT(krakenModule, XMin, XMax, ref XL, ref XR);

            krakenModule.M = Math.Min(krakenModule.M, nm);
            var X = 0.0;
            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
            {
                var X1 = XL[krakenModule.Mode];
                var X2 = XR[krakenModule.Mode];
                var EPS = Math.Abs(X2) * 10 * Math.Pow(0.1, 14);

                var errorMsg = "";
                ZBRENTX(krakenModule, ref X, ref X1, ref X2, EPS,errorMsg);
                if (errorMsg != "")
                {
                    krakenModule.Warnings.Add(errorMsg);
                }               

                krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode] = X;
            }

        }

        private void SOLVE2(KrakenModule krakenModule)
        {
            var X = krakenModule.Omega2 / Math.Pow(krakenModule.CLow, 2);
            var MaxIT = 1500;
            var P = Enumerable.Repeat(0d, 11).ToList();

            if (krakenModule.K == null)
            {
                krakenModule.M = 3000;
                krakenModule.EVMat = new List<List<double>>(krakenModule.NSets + 1);
                for (var i = 0; i <= krakenModule.NSets; i++)
                {
                    krakenModule.EVMat.Add(Enumerable.Repeat(0d, krakenModule.M + 1).ToList());
                }

                krakenModule.Extrap = new List<List<double>>(krakenModule.NSets + 1);
                for (var i = 0; i <= krakenModule.NSets; i++)
                {
                    krakenModule.Extrap.Add(Enumerable.Repeat(0d, krakenModule.M + 1).ToList());
                }

                krakenModule.K = Enumerable.Repeat(new Complex(), krakenModule.M + 1).ToList();
                krakenModule.VG = Enumerable.Repeat(0d, krakenModule.M + 1).ToList();
            }

            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
            {
                X = 1.00001 * X;
                if (krakenModule.ISet >= 2)
                {
                    for (var i = 1; i <= krakenModule.ISet - 1; i++)
                    {
                        P[i] = krakenModule.EVMat[i][krakenModule.Mode];
                    }

                    if (krakenModule.ISet >= 3)
                    {
                        for (var II = 1; II <= krakenModule.ISet - 2; II++)
                        {
                            for (var j = 1; j <= krakenModule.ISet - II - 1; j++)
                            {
                                var X1 = Math.Pow(krakenModule.HV[j], 2);
                                var X2 = Math.Pow(krakenModule.HV[j + II], 2);

                                P[j] = ((Math.Pow(krakenModule.HV[krakenModule.ISet], 2) - X2) * P[j] -
                                        (Math.Pow(krakenModule.HV[krakenModule.ISet], 2) - X1) * P[j + 1]) / (X1 - X2);
                            }
                        }
                        X = P[1];
                    }
                }
                var IERR = 0;
                //var TOL = Math.Abs(X) * 10 * Math.Pow(0.1, 11);
                var TOL = Math.Abs(X) * (krakenModule.B1.Count - 1) * Math.Pow(0.1, 15);
               
                ZSCEX(krakenModule, ref X, TOL, MaxIT,ref IERR);
                if (IERR == -1)
                {
                    krakenModule.Warnings.Add("Root finder secant failure to converge.");
                    X = 1 / double.MaxValue;
                }

                krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode] = X;

                if (krakenModule.Omega2 / X > Math.Pow(krakenModule.CHigh, 2))
                {
                    krakenModule.M = krakenModule.Mode - 1;
                    return;
                }
            }


        }

        private void SOLVE3(KrakenModule krakenModule)
        {
            var MaxIT = 500;
            var XMin = 1.00001 * krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2);
            var DELTA = 0.0;
            var iPower = 0;           

            FUNCT(krakenModule, XMin, ref DELTA, ref iPower);
            krakenModule.M = krakenModule.ModeCount;

            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
            {
                var X = krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode];
                var TOL = Math.Abs(X) * Math.Pow(10, (2.0 - 15));

                var IERR = 0;

                ZSCEX(krakenModule, ref X, TOL,MaxIT,ref IERR);
                if (IERR == -1)
                {
                    krakenModule.Warnings.Add("Root finder secant failure to converge.");
                    X = 1 / double.MaxValue;
                }

                krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode] = X;
                if (krakenModule.Omega2 / X > Math.Pow(krakenModule.CHigh, 2))
                {
                    krakenModule.M = krakenModule.Mode - 1;
                    return;
                }
            }
        }

        private void FUNCT(KrakenModule krakenModule, double X, ref double DELTA, ref int iPower)
        {
            double roof = Math.Pow(10, 50);
            double floor = Math.Pow(0.1, 50);
            int iPowerR = 50;
            int iPowerF = -50;

            double F = 0.0, G = 0.0, F1 = 0.0, G1 = 0.0;
            int iPower1 = 0;

            var bcimpSolver = new BCImpedanceSolver();

            if (X <= krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2))
            {
                DELTA = 0.0;
                iPower = 0;
                return;
            }

            krakenModule.ModeCount = 0;
            var BCType = krakenModule.BCBottom[0].ToString();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X, BCType, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref F, ref G, ref iPower);

            ACOUST(krakenModule, X, ref F, ref G, ref iPower);
            BCType = krakenModule.BCTop[1].ToString();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X, BCType, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref F1, ref G1, ref iPower1);

            DELTA = F * G1 - G * F1;
            if (G * DELTA > 0.0)
            {
                krakenModule.ModeCount++;
            }

            if (krakenModule.Mode > 1 && krakenModule.NMedia > (krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1))
            {
                for (var j = 1; j <= krakenModule.Mode - 1; j++)
                {
                    DELTA /= (X - krakenModule.EVMat[krakenModule.ISet][j]);

                    while (Math.Abs(DELTA) < floor && Math.Abs(DELTA) > 0.0)
                    {
                        DELTA = roof * DELTA;
                        iPower -= iPowerR;
                    }

                    while (Math.Abs(DELTA) > roof)
                    {
                        DELTA = floor * DELTA;
                        iPower -= iPowerF;
                    }
                }
            }
        }

        private void ACOUST(KrakenModule krakenModule, double X, ref double F, ref double G, ref int iPower)
        {
            var roof = Math.Pow(10, 50);
            var floor = Math.Pow(0.1, 50);
            var iPowerF = -50;

            if (krakenModule.FirstAcoustic == 0)
            {
                return;
            }

            for (var Medium = krakenModule.LastAcoustic; Medium >= krakenModule.FirstAcoustic; Medium--)
            {
                var H2K2 = Math.Pow(krakenModule.H[Medium], 2) * X;
                var ii = krakenModule.Loc[Medium] + krakenModule.N[Medium] + 1;
                var rhoM = krakenModule.Rho[krakenModule.Loc[Medium] + 1];
                var P1 = -2.0 * G;
                var P2 = (krakenModule.B1[ii] - H2K2) * G - 2.0 * krakenModule.H[Medium] * F * rhoM;

                var P0 = 0.0;
                for (ii = krakenModule.Loc[Medium] + krakenModule.N[Medium]; ii >= krakenModule.Loc[Medium] + 1; ii--)
                {
                    P0 = P1;
                    P1 = P2;
                    P2 = (H2K2 - krakenModule.B1[ii]) * P1 - P0;

                    if (P0 * P1 <= 0.0)
                    {
                        krakenModule.ModeCount++;
                    }

                    if(Math.Abs(P2) > roof)
                    {
                        P0 = floor * P0;
                        P1 = floor * P1;
                        P2 = floor * P2;
                        iPower -= iPowerF;
                    }
                }

                rhoM = krakenModule.Rho[krakenModule.Loc[Medium] + 1];
                F = -(P2 - P0) / (2.0 * krakenModule.H[Medium]) / rhoM;
                G = -P1;
            }
        }

        private void NORMIZ(KrakenModule krakenModule, List<double> PHI, int ITP, int NTot1, double X)
        {
            Complex PERK = new Complex(0.0, 0.0);
            Complex DEL = new Complex();

            double SLOW, SQNRM;
            SQNRM = 0.0;
            SLOW = 0.0;

            if (krakenModule.BCTop[1] == 'A')
            {
                DEL = -0.5 * (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2) - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real /
                            Math.Sqrt(X - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real));
                /*DEL = krakenModule.i * Complex.Sqrt(X - krakenModule.Omega2/(Complex.Pow(krakenModule.CPTop,2)));*/
                PERK -= DEL * Math.Pow(PHI[1], 2) / krakenModule.RhoTop;
                SLOW += Math.Pow(PHI[1], 2) / (2 * Math.Sqrt(X - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real))
                        / (krakenModule.RhoTop * Math.Pow(krakenModule.CPTop.Real, 2));
            }

            var L = krakenModule.Loc[krakenModule.FirstAcoustic];
            var j = 1;

            for (var Medium = krakenModule.FirstAcoustic; Medium <= krakenModule.LastAcoustic; Medium++)
            {
                L += 1;
                var rhoM = krakenModule.Rho[L];
                var rhoOMH2 = rhoM * krakenModule.Omega2 * Math.Pow(krakenModule.H[Medium], 2);

                SQNRM += 0.5 * krakenModule.H[Medium] * Math.Pow(PHI[j], 2) / rhoM;
                PERK += 0.5 * krakenModule.H[Medium] * krakenModule.i * krakenModule.B1C[L] * Math.Pow(PHI[j], 2) / rhoM;
                SLOW += 0.5 * krakenModule.H[Medium] * (krakenModule.B1[L] + 2) * Math.Pow(PHI[j], 2) / rhoOMH2;

                var L1 = L + 1;
                L = L + krakenModule.N[Medium] - 1;
                var J1 = j + 1;
                j = j + krakenModule.N[Medium] - 1;

                var phiPowRange = PHI.GetRange(J1, j - J1 + 1).Select(x => x * x).ToList();
                phiPowRange.Insert(0, 0);

                var b1Range = krakenModule.B1.GetRange(L1, L - L1 + 1).Select(x => x + 2).ToList();
                b1Range.Insert(0, 0);

                var b1CRange = krakenModule.B1C.GetRange(L1, L - L1 + 1).ToList();
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

                SQNRM += krakenModule.H[Medium] * phiPowSum / rhoM;
                PERK += krakenModule.H[Medium] * krakenModule.i * b1CPhiSum / rhoM;
                SLOW += krakenModule.H[Medium] * b1PhiSum / rhoOMH2;

                L += 1;
                j += 1;
                SQNRM += 0.5 * krakenModule.H[Medium] * Math.Pow(PHI[j], 2) / rhoM;
                PERK += 0.5 * krakenModule.H[Medium] * krakenModule.i * krakenModule.B1C[L] * Math.Pow(PHI[j], 2) / rhoM;
                SLOW += 0.5 * krakenModule.H[Medium] * (krakenModule.B1[L] + 2) * Math.Pow(PHI[j], 2) / rhoOMH2;
            }

            if (krakenModule.BCBottom[0] == 'A')
            {
                DEL = -0.5 * (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2) - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2)).Real /
                            Complex.Sqrt(X - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2))).Real);
                PERK -= DEL * Math.Pow(PHI[j], 2) / krakenModule.RhoBottom;
                SLOW += Complex.Pow(PHI[j], 2).Real / (2 * Complex.Sqrt(X - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2)))).Real
                        / (krakenModule.RhoBottom * Complex.Pow(krakenModule.CPBottom, 2)).Real;
            }

            var X1 = 0.9999999 * X;
            var X2 = 1.0000001 * X;

            var BCType = krakenModule.BCTop[1].ToString();
            double F1 = 0.0, G1 = 0.01, F2 = 0.0, G2 = 0.0;
            int iPower = 0;
            var bcimpSolver = new BCImpedanceSolver();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X1, BCType, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref F1, ref G1, ref iPower);
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X2, BCType, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref F2, ref G2, ref iPower);
            var DrhoDX = 0.0;
            if (G1 != 0)
            {
                DrhoDX = -(F2 / G2 - F1 / G1) / (X2 - X1);
            }

            BCType = krakenModule.BCBottom[0].ToString();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X1, BCType, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref F1, ref G1, ref iPower);
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, X2, BCType, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref F2, ref G2, ref iPower);
            var DetaDX = 0.0;
            if (G1 != 0)
            {
                DrhoDX = -(F2 / G2 - F1 / G1) / (X2 - X1);
            }

            var RN = SQNRM - DrhoDX * Math.Pow(PHI[1], 2) + DetaDX * Math.Pow(PHI[NTot1], 2);

            if (RN <= 0.0)
            {
                RN = -RN;
                krakenModule.Warnings.Add($"Mode = {krakenModule.Mode}. Normalization constant non-positive; suggests grid too coarse");
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
            SLOW = Math.Pow(SCALEF, 2) * SLOW * Math.Sqrt(krakenModule.Omega2 / X);
            krakenModule.VG[krakenModule.Mode] = 1 / SLOW;

            SCAT(krakenModule, ref PERK, PHI, X);
        }

        private void SCAT(KrakenModule krakenModule, ref Complex PERK, List<double> PHI, double X)
        {
            var OMEGA = Math.Sqrt(krakenModule.Omega2);
            var KX = Math.Sqrt(X);
            double rho1 = 0.0, rho2 = 0.0, eta1SQ = 0.0, eta2SQ = 0.0,
                U = 0.0;

            var BCType = krakenModule.BCTop[1].ToString();
            double rhoINS;
            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                var Itop = krakenModule.Loc[krakenModule.FirstAcoustic] + krakenModule.N[krakenModule.FirstAcoustic] + 1;
                rhoINS = krakenModule.Rho[Itop];
                var CINS = Math.Sqrt(krakenModule.Omega2 * Math.Pow(krakenModule.H[krakenModule.FirstAcoustic], 2) / (2.0 + krakenModule.B1[krakenModule.FirstAcoustic]));
                var CImped = TWERSK(BCType[0], OMEGA, krakenModule.BumpDensity, krakenModule.Xi, krakenModule.Eta, KX, rhoINS, CINS);
                CImped /= (-krakenModule.i * OMEGA * rhoINS);
                var DPHIDZ = PHI[2] / krakenModule.H[krakenModule.FirstAcoustic];
                PERK -= CImped * Math.Pow(DPHIDZ, 2);
            }

            BCType = krakenModule.BCBottom;
            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                var Ibot = krakenModule.Loc[krakenModule.FirstAcoustic] + krakenModule.N[krakenModule.FirstAcoustic] + 1;
                rhoINS = krakenModule.Rho[Ibot];
                var CINS = Math.Sqrt(krakenModule.Omega2 * Math.Pow(krakenModule.H[krakenModule.LastAcoustic], 2) / (2.0 + krakenModule.B1[krakenModule.LastAcoustic]));
                var CImped = TWERSK(BCType[0], OMEGA, krakenModule.BumpDensity, krakenModule.Xi, krakenModule.Eta, KX, rhoINS, CINS);
                CImped /= (-krakenModule.i * OMEGA * rhoINS);
                var DPHIDZ = PHI[2] / krakenModule.H[krakenModule.FirstAcoustic];
                PERK -= CImped * Math.Pow(DPHIDZ, 2);
            }

            var j = 1;
            var L = krakenModule.Loc[krakenModule.FirstAcoustic];

            for (var Medium = krakenModule.FirstAcoustic - 1; Medium <= krakenModule.LastAcoustic; Medium++)
            {
                if (Medium == krakenModule.FirstAcoustic - 1)
                {
                    BCType = krakenModule.BCTop[1].ToString();
                    switch (BCType)
                    {
                        case "A":
                            rho1 = krakenModule.RhoTop;
                            eta1SQ = X - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real;
                            U = Math.Sqrt(eta1SQ) * PHI[1] / krakenModule.RhoTop;
                            break;
                        case "V":
                            rho1 = Math.Pow(0.1, 9);
                            eta1SQ = 1.0;
                            rhoINS = krakenModule.Rho[krakenModule.Loc[krakenModule.FirstAcoustic] + 1];
                            U = PHI[2] / krakenModule.H[krakenModule.FirstAcoustic] / rhoINS;
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
                    var H2 = Math.Pow(krakenModule.H[Medium], 2);
                    j += krakenModule.N[Medium];
                    L = krakenModule.Loc[Medium] + krakenModule.N[Medium] + 1;

                    rho1 = krakenModule.Rho[L];
                    eta1SQ = (2.0 + krakenModule.B1[L]) / H2 - X;
                    U = (-PHI[j - 1] - 0.5 * (krakenModule.B1[L] - H2 * X) * PHI[j]) / (krakenModule.H[Medium] * rho1);
                }

                if (Medium == krakenModule.LastAcoustic)
                {
                    BCType = krakenModule.BCBottom[0].ToString();
                    switch (BCType)
                    {
                        case "A":
                            rho2 = krakenModule.RhoBottom;
                            eta2SQ = (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2)).Real - X;
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
                    rho2 = krakenModule.Rho[L + 1];
                    eta2SQ = (2.0 + krakenModule.B1[L + 1]) / Math.Pow(krakenModule.H[Medium + 1], 2) - X;
                }

                var PHIC = new Complex(PHI[j], 0.0);
                var kupIngFormulation = new KupermanIngenitoFormulation();
                PERK += kupIngFormulation.EvaluateImaginaryPerturbation(krakenModule.Sigma[Medium + 1], eta1SQ, rho1, eta2SQ, rho2, PHIC, U);
            }

            krakenModule.K[krakenModule.Mode] = PERK;
        }

        private void BISECT(KrakenModule krakenModule, double XMin, double XMax, ref List<double> XL, ref List<double> XR)
        {
            int MaxBis = 50;
            for (var i = 0; i < XL.Count; i++)
            {
                XL[i] = XMin;
                XR[i] = XMax;
            }

            double DELTA = 0;
            int iPower = 0;

            FUNCT(krakenModule, XMax, ref DELTA, ref iPower);
            var NZER1 = krakenModule.ModeCount;

            if (krakenModule.M == 1)
            {
                return;
            }

            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M - 1; krakenModule.Mode++)
            {
                if (XL[krakenModule.Mode] == XMin)
                {
                    double X2 = XR[krakenModule.Mode];
                    var max = XL.GetRange(krakenModule.Mode + 1, krakenModule.M - krakenModule.Mode).Max();
                    double X1 = Math.Max(max, XMin);

                    for (var j = 1; j <= MaxBis; j++)
                    {
                        double X = X1 + (X2 - X1) / 2;
                        FUNCT(krakenModule, X, ref DELTA, ref iPower);
                        var NZeros = krakenModule.ModeCount - NZER1;

                        if (NZeros < krakenModule.Mode)
                        {
                            X2 = X;
                            XR[krakenModule.Mode] = X;
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

                        if (XL[krakenModule.Mode] != XMin)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void ZSCEX(KrakenModule krakenModule, ref double x2, double TOL, int MAXIteration, ref int IERR)
        {
            var x1 = x2 + 10.0 * TOL;
            double F1 = 0;
            int iPower1 = 1;
            if(krakenModule.Mode >= 40)
            {

            }

            FUNCT(krakenModule, x1, ref F1, ref iPower1);

            for (var Iteration = 1; Iteration <= MAXIteration; Iteration++)
            {
                double x0 = x1;
                double F0 = F1;
                int iPower0 = iPower1;
                x1 = x2;
                double shift;

                FUNCT(krakenModule, x1, ref F1, ref iPower1);
                if (F1 == 0.0)
                {
                    shift = 0.0;
                }
                else
                {
                    shift = (x1 - x0) / (1.0 - F0 / F1 * Math.Pow(10.0, (iPower0 - iPower1)));
                }

                x2 = x1 - shift;
                if (Math.Abs(x2 - x1) < TOL || Math.Abs(x2 - x0) < TOL)
                {
                    return;
                }
            }

            IERR = -1;
        }

        private void ZBRENTX(KrakenModule krakenModule, ref double X, ref double A, ref double B, double T, string errorMessage)
        {
            double MACHEP = 1 / Math.Pow(10, 15);
            double TEN = 10.0;
            double FA = 0.0, FB = 0.0;
            int IEXPA = 0, IEXPB = 0;

            FUNCT(krakenModule, A, ref FA, ref IEXPA);
            FUNCT(krakenModule, B, ref FB, ref IEXPB);

            if ((FA > 0.0 && FB > 0.0) || (FA < 0.0 && FB < 0.0))
            {
                errorMessage = "Function sign is the same at the interval endpoints";
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

                FUNCT(krakenModule, B, ref FB, ref IEXPB);
                if (FB > 0 == FC > 0) goto Mark2000;
                goto Mark3000;
            }

            X = B;
        }

        private Complex TWERSK(char OPT, double OMEGA, double BUMDEN, double XI, double ETA,
                                 double KX, double RHO0, double C0)
        {
            double C1 = 0, C2 = 0;
            //call twersky

            return new Complex(C1, C2);
        }
    }
}
