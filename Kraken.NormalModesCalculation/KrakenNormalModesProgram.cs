using Kraken.Calculation.Exceptions;
using Kraken.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation
{
    public class KrakenNormalModesProgram
    {
        public (KrakenResult, CalculatedModesInfo) CalculateNormalModes(KrakenInputProfile profile)
        {
            var krakenModule = new KrakenModule();
            krakenModule.Init();

            krakenModule.NV = new List<int> { 0, 1, 2, 4, 8, 16 };

            krakenModule.Frequency = profile.Frequency;
            krakenModule.NMedia = profile.NMedia;
            krakenModule.BCTop = profile.Options.Substring(0, 3);
            krakenModule.BCBottom = profile.BCBottom;

            var rangedDataManager = new RangedDataManager();
            var meshInitializer = new MeshInitializer(krakenModule);

            krakenModule.Depth[1] = 0;
            for (var media = 1; media <= profile.NMedia; media++)
            {
                krakenModule.NG[media] = (int)profile.MediumInfo[media][1];
                krakenModule.Sigma[media] = profile.MediumInfo[media][2];
                krakenModule.Depth[media + 1] = profile.MediumInfo[media][3];
            }

            krakenModule.Sigma[profile.NMedia + 1] = profile.BottomSigma;
            meshInitializer.ProccedMesh(krakenModule, profile.SSP, profile.TopAcousticHSProperties, profile.TwerskyScatterParameters, profile.BottomAcousticHSProperties);

            krakenModule.CLow = profile.CLow;
            krakenModule.CHigh = profile.CHigh;

            krakenModule.RMax = profile.RMax;

            var zMin = krakenModule.Depth[1];
            var zMax = krakenModule.Depth[krakenModule.NMedia + 1];           

            rangedDataManager.ProceedSourceAndReceiverDepths(zMin, zMax, profile.Nsd, profile.Nrd, profile.SourceDepths, profile.ReceiverDepths);

            krakenModule.Omega2 = Math.Pow((2.0 * Math.PI * krakenModule.Frequency), 2);

            double error = 0;
            var isSuccess = false;
            var modesInfo = new CalculatedModesInfo();

            var zm = new List<double>();
            var modes = new List<List<double>>();

            for (krakenModule.ISet = 1; krakenModule.ISet <= krakenModule.NSets; krakenModule.ISet++)
            {
                krakenModule.N = krakenModule.NG.Select(x => x * krakenModule.NV[krakenModule.ISet]).ToList();
                for (var j = 1; j <= krakenModule.NMedia; j++)
                {
                    krakenModule.H[j] = (krakenModule.Depth[j + 1] - krakenModule.Depth[j]) / krakenModule.N[j];
                }
                krakenModule.HV[krakenModule.ISet] = krakenModule.H[1];
                Solve(modesInfo, krakenModule, rangedDataManager, meshInitializer, ref error, profile.NModes, ref zm, ref modes);

                if (error * 1000.0 * krakenModule.RMax < 1.0)
                {
                    isSuccess = true;
                    break;
                }
            }

            var result = new KrakenResult();

            if (isSuccess)
            {
                var omega = Math.Sqrt(krakenModule.Omega2);
                var minVal = krakenModule.Extrap[1].GetRange(1, krakenModule.M).Where(x => x > krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2)).Min();
                var minLoc = krakenModule.Extrap[1].FindIndex(x => x == minVal);
                krakenModule.M = minLoc;

                for (var i = 1; i <= krakenModule.M; i++)
                {
                    krakenModule.K[i] = Complex.Sqrt(krakenModule.Extrap[1][i] + krakenModule.K[i]);
                }

                var MMM = Math.Min(krakenModule.M, profile.NModes);
                modesInfo.ModesCount = MMM;
                modesInfo.K = new List<Complex>(krakenModule.K);

                var cp = Enumerable.Repeat(0d, MMM + 1).ToList();
                var cg = Enumerable.Repeat(0d, MMM + 1).ToList();
                var k = Enumerable.Repeat(new Complex(), MMM + 1).ToList();

                for (krakenModule.Mode = 1; krakenModule.Mode <= MMM; krakenModule.Mode++)
                {
                    cp[krakenModule.Mode] = (omega / krakenModule.K[krakenModule.Mode]).Real;
                    cg[krakenModule.Mode] = krakenModule.VG[krakenModule.Mode];
                    k[krakenModule.Mode] = krakenModule.K[krakenModule.Mode];
                }

                result.GroupSpeed = cg;
                result.PhaseSpeed = cp;
                result.K = k;
                result.ModesCount = MMM;
                result.Modes = modes;
                result.ZM = zm;
            }

            result.Warnings.AddRange(krakenModule.Warnings);

            return (result,modesInfo);
        }

        private void Initialize(KrakenModule krakenModule, MeshInitializer meshInitializer)
        {
            var ElasticFlag = false;           
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

            var cP = Enumerable.Repeat(new Complex(), NPTS + 1).ToList();
            var cS = Enumerable.Repeat(new Complex(), NPTS + 1).ToList();

            for (var medium = 1; medium <= krakenModule.NMedia; medium++)
            {
                if (medium != 1)
                {
                    krakenModule.Loc[medium] = krakenModule.Loc[medium - 1] + krakenModule.N[medium - 1] + 1;
                }
                var N1 = krakenModule.N[medium] + 1;
                var currentIdx = krakenModule.Loc[medium] + 1;

                var TASK = "TAB";
                meshInitializer.EvaluateSSP(krakenModule, krakenModule.Depth, cP, cS, krakenModule.Rho, medium, ref N1,
                                            currentIdx, krakenModule.Frequency, krakenModule.BCTop[0].ToString(),
                                            krakenModule.BCTop.Substring(2), TASK, dummy);

                if (cS[currentIdx].Real == 0)
                {
                    krakenModule.Material[medium] = "ACOUSTIC";
                    if (krakenModule.FirstAcoustic == 0)
                    {
                        krakenModule.FirstAcoustic = medium;
                    }
                    krakenModule.LastAcoustic = medium;

                    var cPMin = cP.GetRange(currentIdx, krakenModule.N[medium] + 1).Select(x => x.Real).Min();
                    krakenModule.CMin = Math.Min(krakenModule.CMin, cPMin);

                    for (var i = currentIdx; i <= currentIdx + krakenModule.N[medium]; i++)
                    {
                        krakenModule.B1[i] = -2.0 + Math.Pow(krakenModule.H[medium], 2) * (krakenModule.Omega2 / Complex.Pow(cP[i], 2)).Real;
                    }

                    for (var i = currentIdx; i <= currentIdx + krakenModule.N[medium]; i++)
                    {
                        krakenModule.B1C[i] = (krakenModule.Omega2 / Complex.Pow(cP[i], 2)).Imaginary;
                    }
                }
                else
                {
                    if (krakenModule.Sigma[medium] != 0)
                    {
                        throw new KrakenException("Rough elastic interfaces are not allowed");
                    }

                    krakenModule.Material[medium] = "ELASTIC";
                    ElasticFlag = true;
                    var twoH = 2.0 * krakenModule.H[medium];

                    for (var j = currentIdx; j <= currentIdx + krakenModule.N[medium]; j++)
                    {
                        krakenModule.CMin = Math.Min(cS[j].Real, krakenModule.CMin);

                        var cP2 = Complex.Pow(cP[j], 2).Real;
                        var cS2 = Complex.Pow(cS[j], 2).Real;

                        krakenModule.B1[j] = twoH / (krakenModule.Rho[j] * cS2);
                        krakenModule.B2[j] = twoH / (krakenModule.Rho[j] * cP2);
                        krakenModule.B3[j] = 4.0 * twoH * krakenModule.Rho[j] * cS2 * (cP2 - cS2) / cP2;
                        krakenModule.B4[j] = twoH * (cP2 - 2.0 * cS2) / cP2;
                        krakenModule.Rho[j] = twoH * krakenModule.Omega2 * krakenModule.Rho[j];
                    }
                }
            }

            if (krakenModule.BCBottom[0] == 'A')
            {
                if (krakenModule.CSBottom.Real > 0.0)
                {
                    ElasticFlag = true;
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
                        ElasticFlag = true;
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

            if (ElasticFlag)
            {
                krakenModule.CMin = 0.85 * krakenModule.CMin;
            }

            krakenModule.CLow = Math.Max(krakenModule.CLow, krakenModule.CMin);
        }

        private void Solve(CalculatedModesInfo modesInfo, KrakenModule krakenModule, RangedDataManager rangedDataManager, MeshInitializer meshInitializer, ref double error, int nm, ref List<double> zm, ref List<List<double>> modes)
        {
            var calculateModes = true;

            Initialize(krakenModule, meshInitializer);

            if (krakenModule.IProf > 1 && krakenModule.ISet <= 2 && krakenModule.BCTop[3] == 'C')
            {
                Solve3(krakenModule);
            }
            else if (krakenModule.ISet <= 2 && krakenModule.NMedia <= (krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1))
            {
                Solve1(krakenModule, nm);
            }
            else
            {
                Solve2(krakenModule);
            }

            for (var i = 1; i <= krakenModule.M; i++)
            {
                krakenModule.Extrap[krakenModule.ISet][i] = krakenModule.EVMat[krakenModule.ISet][i];
            }

            var minVal = krakenModule.Extrap[1].GetRange(1, krakenModule.M).Where(x => x > krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2)).Min();
            var minLoc = krakenModule.Extrap[1].FindIndex(x => x == minVal);
            krakenModule.M = minLoc;

            if (krakenModule.ISet == 1 && calculateModes)
            {
                Vector(modesInfo, krakenModule, rangedDataManager, nm, ref zm, ref modes);
            }

            error = 10;
            var key = 2 * krakenModule.M / 3 + 1;
            if (krakenModule.ISet > 1)
            {
                var t1 = krakenModule.Extrap[1][key];

                for (var j = krakenModule.ISet - 1; j >= 1; j--)
                {
                    for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
                    {
                        var x1 = Math.Pow(krakenModule.NV[j], 2);
                        var x2 = Math.Pow(krakenModule.NV[krakenModule.ISet], 2);
                        var f1 = krakenModule.Extrap[j][krakenModule.Mode];
                        var f2 = krakenModule.Extrap[j + 1][krakenModule.Mode];
                        krakenModule.Extrap[j][krakenModule.Mode] = f2 - (f1 - f2) / (x2 / x1 - 1.0);
                    }
                }

                var t2 = krakenModule.Extrap[1][key];
                error = Math.Abs(t2 - t1);
            }

        }

        private void Vector(CalculatedModesInfo modesInfo, KrakenModule krakenModule, RangedDataManager rangedDataManager, int nm, ref List<double> zm, ref List<List<double>> modes)
        {
            var BCTop = krakenModule.BCTop[1].ToString();
            var BCBottom = krakenModule.BCBottom[0].ToString();

            var NTot = krakenModule.N.GetRange(krakenModule.FirstAcoustic, krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1).Sum();
            var NTot1 = NTot + 1;

            //allocate
            var z = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var e = Enumerable.Repeat(0d, NTot1 + 1 + 1).ToList();
            var d = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var Phi = Enumerable.Repeat(0d, NTot1 + 1).ToList();
            var j = 1;
            z[1] = krakenModule.Depth[krakenModule.FirstAcoustic];

            var hRho = 0.0;
            for (var medium = krakenModule.FirstAcoustic; medium <= krakenModule.LastAcoustic; medium++)
            {
                hRho = krakenModule.H[medium] * krakenModule.Rho[krakenModule.Loc[medium] + 1];

                var temp = 1;
                for (var i = j + 1; i <= j + krakenModule.N[medium]; i++)
                {
                    e[i] = 1.0 / hRho;
                    z[i] = z[j] + krakenModule.H[medium] * temp;
                    temp++;
                }

                j += krakenModule.N[medium];
            }

            e[NTot1 + 1] = 1.0 / hRho;
            var vectorsManager = new VectorsManager();
            var (zTab, NzTab) = vectorsManager.MergeVectors(rangedDataManager.SourceDepths, rangedDataManager.Nsd, rangedDataManager.ReceiverDepths, rangedDataManager.Nrd);

            var PhiTab = Enumerable.Repeat(new Complex(), NzTab + 1).ToList();

            var weightsCalculator = new WeightsCalculator();
            var (weights,indicesTab) = weightsCalculator.CalculateWeightsAndIndices(z, NTot1, zTab, NzTab);

            var modesave = new List<List<double>>(NzTab + 1);
            for (var i = 0; i <= NzTab + 1; i++)
            {
                modesave.Add(Enumerable.Repeat(0d, krakenModule.M + 1).ToList());
            }
                     
            modesInfo.NMedia = krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1;
            modesInfo.NTot = NzTab;
            modesInfo.NMat = NzTab;

            modesInfo.N = new List<int>(krakenModule.N);

            modesInfo.Material = Enumerable.Repeat("", krakenModule.Material.Count).ToList();        

            modesInfo.Depth = Enumerable.Repeat(0d, krakenModule.Depth.Count).ToList();

            modesInfo.Rho = Enumerable.Repeat(0d, krakenModule.Rho.Count).ToList();

            for (var medium = krakenModule.FirstAcoustic; medium <= krakenModule.LastAcoustic; medium++)
            {
                modesInfo.Material[medium] = krakenModule.Material[medium];
                modesInfo.Depth[medium] = krakenModule.Depth[medium];
                modesInfo.Rho[medium] = krakenModule.Rho[krakenModule.Loc[medium] + 1];
            }

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

            var f = 0.0;
            var g = 0.0;
            var iPower = 0;
            modesInfo.Phi.Add(new List<Complex>());
            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
            {
                var x = krakenModule.EVMat[1][krakenModule.Mode];
                var bcimpSolver = new BCImpedanceSolver();
                bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x, BCTop, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref f, ref g, ref iPower);

                int l;
                double xH2;
                if (g == 0.0)
                {
                    d[1] = 1.0;
                    e[2] = 2.220446049250313 / Math.Pow(10, 16);
                }
                else
                {
                    l = krakenModule.Loc[krakenModule.FirstAcoustic] + 1;
                    xH2 = x * krakenModule.H[krakenModule.FirstAcoustic] * krakenModule.H[krakenModule.FirstAcoustic];
                    hRho = krakenModule.H[krakenModule.FirstAcoustic] * krakenModule.Rho[l];
                    d[1] = (krakenModule.B1[l] - xH2) / hRho / 2.0 + f / g;
                }

                var iTP = NTot;
                j = 1;
                l = krakenModule.Loc[krakenModule.FirstAcoustic] + 1;

                for (var medium = krakenModule.FirstAcoustic; medium <= krakenModule.LastAcoustic; medium++)
                {
                    xH2 = x * Math.Pow(krakenModule.H[medium], 2);
                    hRho = krakenModule.H[medium] * krakenModule.Rho[krakenModule.Loc[medium] + 1];

                    if (medium >= krakenModule.FirstAcoustic + 1)
                    {
                        l += 1;
                        d[j] = (d[j] + (krakenModule.B1[l] - xH2) / hRho) / 2.0;
                    }

                    for (var ii = 1; ii <= krakenModule.N[medium]; ii++)
                    {                        
                        j += 1;
                        l += 1;
                        d[j] = (krakenModule.B1[l] - xH2) / hRho;

                        if (krakenModule.B1[l] - xH2 + 2.0 > 0.0)
                        {
                            iTP = Math.Min(j, iTP);
                        }
                    }
                }
                
                bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x, BCBottom, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref f, ref g, ref iPower);
                if (g == 0.0)
                {
                    d[NTot1] = 1.0;
                    e[NTot1] = 2.220446049250313 / Math.Pow(10, 16);
                }
                else
                {
                    d[NTot1] = d[NTot1] / 2.0 - f / g;
                }

                var errorFlag = 0;
                var sinvitMod = new EigenvectorFinder();
                sinvitMod.FindEigenvectorUsingInverseIteration(NTot1, d, e,Phi, ref errorFlag);

                if (errorFlag != 0)
                {
                    krakenModule.Warnings.Add($"Inverse iteration failed to converge. Mode = {krakenModule.Mode}");
                    Phi = Phi.Select(p => 0d).ToList();
                }
                else
                {
                    NormalizeEigenvector(krakenModule, Phi, iTP, NTot1, x);
                }

                for (var i = 1; i <= NzTab; i++)
                {
                    PhiTab[i] = Phi[indicesTab[i]] + weights[i] * (Phi[indicesTab[i] + 1] - Phi[indicesTab[i]]);
                }

                modesInfo.Phi.Add(new List<Complex>(PhiTab));

                for (var i = 1; i <= NzTab; i++)
                {
                    modesave[i][krakenModule.Mode] = PhiTab[i].Real;
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

        private void Solve1(KrakenModule krakenModule, int nm)
        {
            var xMin = 1.00001 * krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2);
            var delta = 0.0;
            var iPower = 0;
            Funct(krakenModule, xMin, ref delta, ref iPower);
            krakenModule.M = krakenModule.ModeCount;

            var xL = Enumerable.Repeat(0d, krakenModule.M + 1).ToList();
            var xR = Enumerable.Repeat(0d, krakenModule.M + 1).ToList();

            if (krakenModule.ISet == 1)
            {
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

            var xMax = krakenModule.Omega2 / Math.Pow(krakenModule.CLow, 2);
            Funct(krakenModule, xMax, ref delta, ref iPower);
            krakenModule.M -= krakenModule.ModeCount;
            krakenModule.M = Math.Min(krakenModule.M, nm + 1);

            var NTot = krakenModule.N.GetRange(krakenModule.FirstAcoustic, krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1).Sum();
            if (krakenModule.M > NTot / 5)
            {
                krakenModule.Warnings.Add("Mesh too coarse to sample the modes adequately");
            }

            Bisection(krakenModule, xMin, xMax, ref xL, ref xR);

            krakenModule.M = Math.Min(krakenModule.M, nm);
            var x = 0.0;
            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
            {
                var x1 = xL[krakenModule.Mode];
                var x2 = xR[krakenModule.Mode];
                var eps = Math.Abs(x2) * 10 * Math.Pow(0.1, 14);

                var errorFlag = 0;
                CalculateZeroXOfFunction(krakenModule, ref x, ref x1, ref x2, eps, ref errorFlag);
                if (errorFlag != 0)
                {
                    krakenModule.Warnings.Add("Function sign is the same at the interval endpoints");
                }               

                krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode] = x;
            }

        }

        private void Solve2(KrakenModule krakenModule)
        {
            var x = krakenModule.Omega2 / Math.Pow(krakenModule.CLow, 2);
            var maxIteration = 1500;
            var p = Enumerable.Repeat(0d, 11).ToList();

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
                x = 1.00001 * x;
                if (krakenModule.ISet >= 2)
                {
                    for (var i = 1; i <= krakenModule.ISet - 1; i++)
                    {
                        p[i] = krakenModule.EVMat[i][krakenModule.Mode];
                    }

                    if (krakenModule.ISet >= 3)
                    {
                        for (var II = 1; II <= krakenModule.ISet - 2; II++)
                        {
                            for (var j = 1; j <= krakenModule.ISet - II - 1; j++)
                            {
                                var x1 = Math.Pow(krakenModule.HV[j], 2);
                                var x2 = Math.Pow(krakenModule.HV[j + II], 2);

                                p[j] = ((Math.Pow(krakenModule.HV[krakenModule.ISet], 2) - x2) * p[j] -
                                        (Math.Pow(krakenModule.HV[krakenModule.ISet], 2) - x1) * p[j + 1]) / (x1 - x2);
                            }
                        }
                        x = p[1];
                    }
                }
                var errorFlag = 0;  
                
                var tolerance = Math.Abs(x) * (krakenModule.B1.Count - 1) * Math.Pow(0.1, 15);
               
                RootFinderSecant(krakenModule, ref x, tolerance, maxIteration,ref errorFlag);
                if (errorFlag == -1)
                {
                    krakenModule.Warnings.Add("Root finder secant failure to converge.");
                    x = 1 / double.MaxValue;
                }

                krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode] = x;

                if (krakenModule.Omega2 / x > Math.Pow(krakenModule.CHigh, 2))
                {
                    krakenModule.M = krakenModule.Mode - 1;
                    return;
                }
            }


        }

        private void Solve3(KrakenModule krakenModule)
        {
            var maxIteration = 500;
            var xMin = 1.00001 * krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2);
            var delta = 0.0;
            var iPower = 0;           

            Funct(krakenModule, xMin, ref delta, ref iPower);
            krakenModule.M = krakenModule.ModeCount;

            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M; krakenModule.Mode++)
            {
                var x = krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode];
                var tolerance = Math.Abs(x) * Math.Pow(10, (2.0 - 15));

                var errorFlag = 0;

                RootFinderSecant(krakenModule, ref x, tolerance,maxIteration,ref errorFlag);
                if (errorFlag == -1)
                {
                    krakenModule.Warnings.Add("Root finder secant failure to converge.");
                    x = 1 / double.MaxValue;
                }

                krakenModule.EVMat[krakenModule.ISet][krakenModule.Mode] = x;
                if (krakenModule.Omega2 / x > Math.Pow(krakenModule.CHigh, 2))
                {
                    krakenModule.M = krakenModule.Mode - 1;
                    return;
                }
            }
        }

        private void Funct(KrakenModule krakenModule, double x, ref double delta, ref int iPower)
        {
            double roof = Math.Pow(10, 50);
            double floor = Math.Pow(0.1, 50);
            int iPowerR = 50;
            int iPowerF = -50;

            double f = 0.0, g = 0.0, f1 = 0.0, G1 = 0.0;
            int iPower1 = 0;

            var bcimpSolver = new BCImpedanceSolver();

            if (x <= krakenModule.Omega2 / Math.Pow(krakenModule.CHigh, 2))
            {
                delta = 0.0;
                iPower = 0;
                return;
            }

            krakenModule.ModeCount = 0;
            var BCType = krakenModule.BCBottom[0].ToString();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x, BCType, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref f, ref g, ref iPower);

            AcousticLayers(krakenModule, x, ref f, ref g, ref iPower);
            BCType = krakenModule.BCTop[1].ToString();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x, BCType, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref f1, ref G1, ref iPower1);

            delta = f * G1 - g * f1;
            if (g * delta > 0.0)
            {
                krakenModule.ModeCount++;
            }

            if (krakenModule.Mode > 1 && krakenModule.NMedia > (krakenModule.LastAcoustic - krakenModule.FirstAcoustic + 1))
            {
                for (var j = 1; j <= krakenModule.Mode - 1; j++)
                {
                    delta /= (x - krakenModule.EVMat[krakenModule.ISet][j]);

                    while (Math.Abs(delta) < floor && Math.Abs(delta) > 0.0)
                    {
                        delta = roof * delta;
                        iPower -= iPowerR;
                    }

                    while (Math.Abs(delta) > roof)
                    {
                        delta = floor * delta;
                        iPower -= iPowerF;
                    }
                }
            }
        }

        private void AcousticLayers(KrakenModule krakenModule, double x, ref double f, ref double g, ref int iPower)
        {
            var roof = Math.Pow(10, 50);
            var floor = Math.Pow(0.1, 50);
            var iPowerF = -50;

            if (krakenModule.FirstAcoustic == 0)
            {
                return;
            }

            for (var medium = krakenModule.LastAcoustic; medium >= krakenModule.FirstAcoustic; medium--)
            {
                var h2k2 = Math.Pow(krakenModule.H[medium], 2) * x;
                var ii = krakenModule.Loc[medium] + krakenModule.N[medium] + 1;
                var rhoM = krakenModule.Rho[krakenModule.Loc[medium] + 1];
                var p1 = -2.0 * g;
                var p2 = (krakenModule.B1[ii] - h2k2) * g - 2.0 * krakenModule.H[medium] * f * rhoM;

                var p0 = 0.0;
                for (ii = krakenModule.Loc[medium] + krakenModule.N[medium]; ii >= krakenModule.Loc[medium] + 1; ii--)
                {
                    p0 = p1;
                    p1 = p2;
                    p2 = (h2k2 - krakenModule.B1[ii]) * p1 - p0;

                    if (p0 * p1 <= 0.0)
                    {
                        krakenModule.ModeCount++;
                    }

                    if(Math.Abs(p2) > roof)
                    {
                        p0 = floor * p0;
                        p1 = floor * p1;
                        p2 = floor * p2;
                        iPower -= iPowerF;
                    }
                }

                rhoM = krakenModule.Rho[krakenModule.Loc[medium] + 1];
                f = -(p2 - p0) / (2.0 * krakenModule.H[medium]) / rhoM;
                g = -p1;
            }
        }

        private void NormalizeEigenvector(KrakenModule krakenModule, List<double> Phi, int iTP, int NTot1, double x)
        {
            Complex pertubationK = new Complex(0.0, 0.0);
            Complex del = new Complex();

            double slow, sqNorm;
            sqNorm = 0.0;
            slow = 0.0;

            if (krakenModule.BCTop[1] == 'A')
            {
                del = -0.5 * (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2) - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real /
                            Math.Sqrt(x - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real));
                /*del = krakenModule.i * Complex.Sqrt(x - krakenModule.Omega2/(Complex.Pow(krakenModule.CPTop,2)));*/
                pertubationK -= del * Math.Pow(Phi[1], 2) / krakenModule.RhoTop;
                slow += Math.Pow(Phi[1], 2) / (2 * Math.Sqrt(x - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real))
                        / (krakenModule.RhoTop * Math.Pow(krakenModule.CPTop.Real, 2));
            }

            var l = krakenModule.Loc[krakenModule.FirstAcoustic];
            var j = 1;

            for (var medium = krakenModule.FirstAcoustic; medium <= krakenModule.LastAcoustic; medium++)
            {
                l += 1;
                var rhoM = krakenModule.Rho[l];
                var rhoOmH2 = rhoM * krakenModule.Omega2 * Math.Pow(krakenModule.H[medium], 2);

                sqNorm += 0.5 * krakenModule.H[medium] * Math.Pow(Phi[j], 2) / rhoM;
                pertubationK += 0.5 * krakenModule.H[medium] * krakenModule.i * krakenModule.B1C[l] * Math.Pow(Phi[j], 2) / rhoM;
                slow += 0.5 * krakenModule.H[medium] * (krakenModule.B1[l] + 2) * Math.Pow(Phi[j], 2) / rhoOmH2;

                var L1 = l + 1;
                l = l + krakenModule.N[medium] - 1;
                var J1 = j + 1;
                j = j + krakenModule.N[medium] - 1;

                var phiPowRange = Phi.GetRange(J1, j - J1 + 1).Select(p => p * p).ToList();
                phiPowRange.Insert(0, 0);

                var b1Range = krakenModule.B1.GetRange(L1, l - L1 + 1).Select(p => p + 2).ToList();
                b1Range.Insert(0, 0);

                var b1CRange = krakenModule.B1C.GetRange(L1, l - L1 + 1).ToList();
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

                sqNorm += krakenModule.H[medium] * phiPowSum / rhoM;
                pertubationK += krakenModule.H[medium] * krakenModule.i * b1CPhiSum / rhoM;
                slow += krakenModule.H[medium] * b1PhiSum / rhoOmH2;

                l += 1;
                j += 1;
                sqNorm += 0.5 * krakenModule.H[medium] * Math.Pow(Phi[j], 2) / rhoM;
                pertubationK += 0.5 * krakenModule.H[medium] * krakenModule.i * krakenModule.B1C[l] * Math.Pow(Phi[j], 2) / rhoM;
                slow += 0.5 * krakenModule.H[medium] * (krakenModule.B1[l] + 2) * Math.Pow(Phi[j], 2) / rhoOmH2;
            }

            if (krakenModule.BCBottom[0] == 'A')
            {
                del = -0.5 * (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2) - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2)).Real /
                            Complex.Sqrt(x - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2))).Real);
                pertubationK -= del * Math.Pow(Phi[j], 2) / krakenModule.RhoBottom;
                slow += Complex.Pow(Phi[j], 2).Real / (2 * Complex.Sqrt(x - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2)))).Real
                        / (krakenModule.RhoBottom * Complex.Pow(krakenModule.CPBottom, 2)).Real;
            }

            var x1 = 0.9999999 * x;
            var x2 = 1.0000001 * x;

            var BCType = krakenModule.BCTop[1].ToString();
            double f1 = 0.0, G1 = 0.01, f2 = 0.0, G2 = 0.0;
            int iPower = 0;
            var bcimpSolver = new BCImpedanceSolver();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x1, BCType, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref f1, ref G1, ref iPower);
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x2, BCType, "TOP", krakenModule.CPTop, krakenModule.CSTop, krakenModule.RhoTop, ref f2, ref G2, ref iPower);
            var dRhoDx = 0.0;
            if (G1 != 0)
            {
                dRhoDx = -(f2 / G2 - f1 / G1) / (x2 - x1);
            }

            BCType = krakenModule.BCBottom[0].ToString();
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x1, BCType, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref f1, ref G1, ref iPower);
            bcimpSolver.ComputeBoundaryConditionImpedance(krakenModule, x2, BCType, "BOT", krakenModule.CPBottom, krakenModule.CSBottom, krakenModule.RhoBottom, ref f2, ref G2, ref iPower);
            var deltaDx = 0.0;
            if (G1 != 0)
            {
                dRhoDx = -(f2 / G2 - f1 / G1) / (x2 - x1);
            }

            var rN = sqNorm - dRhoDx * Math.Pow(Phi[1], 2) + deltaDx * Math.Pow(Phi[NTot1], 2);

            if (rN <= 0.0)
            {
                rN = -rN;
                krakenModule.Warnings.Add($"Mode = {krakenModule.Mode}. Normalization constant non-positive; suggests grid too coarse");
            }

            var scaleFactor = 1.0 / Math.Sqrt(rN);
            if (Phi[iTP] < 0.0)
            {
                scaleFactor = -scaleFactor;
            }

            for (var i = 1; i <= NTot1; i++)
            {
                Phi[i] = scaleFactor * Phi[i];
            }


            pertubationK = Math.Pow(scaleFactor, 2) * pertubationK;
            slow = Math.Pow(scaleFactor, 2) * slow * Math.Sqrt(krakenModule.Omega2 / x);
            krakenModule.VG[krakenModule.Mode] = 1 / slow;

            FigureScatterLoss(krakenModule, ref pertubationK, Phi, x);
        }

        private void FigureScatterLoss(KrakenModule krakenModule, ref Complex pertubationK, List<double> Phi, double x)
        {
            var omega = Math.Sqrt(krakenModule.Omega2);
            var kx = Math.Sqrt(x);
            double rho1 = 0.0, rho2 = 0.0, eta1SQ = 0.0, eta2SQ = 0.0,
                u = 0.0;

            var BCType = krakenModule.BCTop[1].ToString();
            double rhoInside;
            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                var iTop = krakenModule.Loc[krakenModule.FirstAcoustic] + krakenModule.N[krakenModule.FirstAcoustic] + 1;
                rhoInside = krakenModule.Rho[iTop];
                var cInside = Math.Sqrt(krakenModule.Omega2 * Math.Pow(krakenModule.H[krakenModule.FirstAcoustic], 2) / (2.0 + krakenModule.B1[krakenModule.FirstAcoustic]));
                var cImped = Twersky(BCType[0], omega, krakenModule.BumpDensity, krakenModule.Xi, krakenModule.Eta, kx, rhoInside, cInside);
                cImped /= (-krakenModule.i * omega * rhoInside);
                var dPhiDz = Phi[2] / krakenModule.H[krakenModule.FirstAcoustic];
                pertubationK -= cImped * Math.Pow(dPhiDz, 2);
            }

            BCType = krakenModule.BCBottom;
            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                var iBottom = krakenModule.Loc[krakenModule.FirstAcoustic] + krakenModule.N[krakenModule.FirstAcoustic] + 1;
                rhoInside = krakenModule.Rho[iBottom];
                var cInside = Math.Sqrt(krakenModule.Omega2 * Math.Pow(krakenModule.H[krakenModule.LastAcoustic], 2) / (2.0 + krakenModule.B1[krakenModule.LastAcoustic]));
                var cImped = Twersky(BCType[0], omega, krakenModule.BumpDensity, krakenModule.Xi, krakenModule.Eta, kx, rhoInside, cInside);
                cImped /= (-krakenModule.i * omega * rhoInside);
                var dPhiDz = Phi[2] / krakenModule.H[krakenModule.FirstAcoustic];
                pertubationK -= cImped * Math.Pow(dPhiDz, 2);
            }

            var j = 1;
            var l = krakenModule.Loc[krakenModule.FirstAcoustic];

            for (var medium = krakenModule.FirstAcoustic - 1; medium <= krakenModule.LastAcoustic; medium++)
            {
                if (medium == krakenModule.FirstAcoustic - 1)
                {
                    BCType = krakenModule.BCTop[1].ToString();
                    switch (BCType)
                    {
                        case "A":
                            rho1 = krakenModule.RhoTop;
                            eta1SQ = x - (krakenModule.Omega2 / Complex.Pow(krakenModule.CPTop, 2)).Real;
                            u = Math.Sqrt(eta1SQ) * Phi[1] / krakenModule.RhoTop;
                            break;
                        case "V":
                            rho1 = Math.Pow(0.1, 9);
                            eta1SQ = 1.0;
                            rhoInside = krakenModule.Rho[krakenModule.Loc[krakenModule.FirstAcoustic] + 1];
                            u = Phi[2] / krakenModule.H[krakenModule.FirstAcoustic] / rhoInside;
                            break;
                        case "R":
                            rho1 = Math.Pow(10, 9);
                            eta1SQ = 1.0;
                            u = 0.0;
                            break;
                    }
                }
                else
                {
                    var h2 = Math.Pow(krakenModule.H[medium], 2);
                    j += krakenModule.N[medium];
                    l = krakenModule.Loc[medium] + krakenModule.N[medium] + 1;

                    rho1 = krakenModule.Rho[l];
                    eta1SQ = (2.0 + krakenModule.B1[l]) / h2 - x;
                    u = (-Phi[j - 1] - 0.5 * (krakenModule.B1[l] - h2 * x) * Phi[j]) / (krakenModule.H[medium] * rho1);
                }

                if (medium == krakenModule.LastAcoustic)
                {
                    BCType = krakenModule.BCBottom[0].ToString();
                    switch (BCType)
                    {
                        case "A":
                            rho2 = krakenModule.RhoBottom;
                            eta2SQ = (krakenModule.Omega2 / Complex.Pow(krakenModule.CPBottom, 2)).Real - x;
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
                    rho2 = krakenModule.Rho[l + 1];
                    eta2SQ = (2.0 + krakenModule.B1[l + 1]) / Math.Pow(krakenModule.H[medium + 1], 2) - x;
                }

                var PhiComplex = new Complex(Phi[j], 0.0);
                var kupIngFormulation = new KupermanIngenitoFormulation();
                pertubationK += kupIngFormulation.EvaluateImaginaryPerturbation(krakenModule.Sigma[medium + 1], eta1SQ, rho1, eta2SQ, rho2, PhiComplex, u);
            }

            krakenModule.K[krakenModule.Mode] = pertubationK;
        }

        private void Bisection(KrakenModule krakenModule, double xMin, double xMax, ref List<double> xL, ref List<double> xR)
        {
            int maxBisection = 50;
            for (var i = 0; i < xL.Count; i++)
            {
                xL[i] = xMin;
                xR[i] = xMax;
            }

            double delta = 0;
            int iPower = 0;

            Funct(krakenModule, xMax, ref delta, ref iPower);
            var nZer1 = krakenModule.ModeCount;

            if (krakenModule.M == 1)
            {
                return;
            }

            for (krakenModule.Mode = 1; krakenModule.Mode <= krakenModule.M - 1; krakenModule.Mode++)
            {
                if (xL[krakenModule.Mode] == xMin)
                {
                    double x2 = xR[krakenModule.Mode];
                    var max = xL.GetRange(krakenModule.Mode + 1, krakenModule.M - krakenModule.Mode).Max();
                    double x1 = Math.Max(max, xMin);

                    for (var j = 1; j <= maxBisection; j++)
                    {
                        double x = x1 + (x2 - x1) / 2;
                        Funct(krakenModule, x, ref delta, ref iPower);
                        var nZeros = krakenModule.ModeCount - nZer1;

                        if (nZeros < krakenModule.Mode)
                        {
                            x2 = x;
                            xR[krakenModule.Mode] = x;
                        }
                        else
                        {
                            x1 = x;
                            if (xR[nZeros + 1] >= x)
                            {
                                xR[nZeros + 1] = x;
                            }
                            if (xL[nZeros] <= x)
                            {
                                xL[nZeros] = x;
                            }
                        }

                        if (xL[krakenModule.Mode] != xMin)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void RootFinderSecant(KrakenModule krakenModule, ref double x2, double tolerance, int MAXIteration, ref int errorFlag)
        {
            var x1 = x2 + 10.0 * tolerance;
            double f1 = 0;
            int iPower1 = 1;       

            Funct(krakenModule, x1, ref f1, ref iPower1);

            for (var iteration = 1; iteration <= MAXIteration; iteration++)
            {
                double x0 = x1;
                double F0 = f1;
                int iPower0 = iPower1;
                x1 = x2;
                double shift;

                Funct(krakenModule, x1, ref f1, ref iPower1);
                if (f1 == 0.0)
                {
                    shift = 0.0;
                }
                else
                {
                    shift = (x1 - x0) / (1.0 - F0 / f1 * Math.Pow(10.0, (iPower0 - iPower1)));
                }

                x2 = x1 - shift;
                if (Math.Abs(x2 - x1) < tolerance || Math.Abs(x2 - x0) < tolerance)
                {
                    return;
                }
            }

            errorFlag = -1;
        }

        /// <summary>
        /// legacy algol procedure
        /// </summary>        
        private void CalculateZeroXOfFunction(KrakenModule krakenModule, ref double x, ref double a, ref double b, double t, ref int errorFlag)
        {          

            double relativePrecision = Math.Pow(10, -16);
            double ten = 10.0;
            double fa = 0.0, fb = 0.0;
            int iExpA = 0, iExpB = 0;

            Funct(krakenModule, a, ref fa, ref iExpA);
            Funct(krakenModule, b, ref fb, ref iExpB);

            if ((fa > 0.0 && fb > 0.0) || (fa < 0.0 && fb < 0.0))
            {
                errorFlag = -1;
                return;
            }
        Mark2000:
            double c = a;
            double fc = fa;
            int iExpC = iExpA;
            double e = b - a;
            double d = e;
            double f1;
            double f2;
            if (iExpA < iExpB)
            {
                f1 = fc * Math.Pow(ten, (iExpC - iExpB));
                f2 = fb;
            }
            else
            {
                f1 = fc;
                f2 = fb * Math.Pow(ten, (iExpB - iExpC));
            }
        Mark3000:
            if (Math.Abs(f1) < Math.Abs(f2))
            {
                a = b;
                b = c;
                c = a;
                fa = fb;
                iExpA = iExpB;
                fb = fc;
                iExpB = iExpC;
                fc = fa;
                iExpC = iExpA;
            }

            double tolerance = 2.0 * relativePrecision * Math.Abs(b) + t;
            double m = 0.5 * (c - b);
            if (Math.Abs(m) > tolerance && fb != 0)
            {

                if (iExpA < iExpB)
                {
                    f1 = fa * Math.Pow(ten, (iExpA - iExpB));
                    f2 = fb;
                }
                else
                {
                    f1 = fa;
                    f2 = fb * Math.Pow(ten, (iExpB - iExpA));
                }

                if (Math.Abs(e) < tolerance || Math.Abs(f1) <= Math.Abs(f2))
                {
                    e = m;
                    d = e;
                }
                else
                {
                    double S = fb / fa * Math.Pow(ten, (iExpB - iExpA));
                    double p;
                    double q;
                    if (a == c)
                    {
                        p = 2.0 * m * S;
                        q = 1.0 - S;
                    }
                    else
                    {
                        q = fa / fc * Math.Pow(ten, (iExpA - iExpC));
                        double R = fb / fc * Math.Pow(ten, (iExpB - iExpC));
                        p = S * (2.0 * m * q * (q - R) - (b - a) * (R - 1.0));
                        q = (q - 1.0) * (R - 1.0) * (S - 1.0);
                    }
                    if (p > 0.0)
                    {
                        q = -q;
                    }
                    else
                    {
                        p = -p;
                    }

                    S = e;
                    e = d;

                    if ((2.0 * p < 3.0 * m * q - Math.Abs(tolerance * q)) && (p < Math.Abs(0.5 * S * q)))
                    {
                        d = p / q;
                    }
                    else
                    {
                        e = m;
                        d = e;
                    }
                }

                a = b;
                fa = fb;
                iExpA = iExpB;

                if (Math.Abs(d) > tolerance)
                {
                    b += d;
                }
                else
                {
                    if (m > 0.0)
                    {
                        b += tolerance;
                    }
                    else
                    {
                        b -= tolerance;
                    }
                }

                Funct(krakenModule, b, ref fb, ref iExpB);
                if (fb > 0 == fc > 0) goto Mark2000;
                goto Mark3000;
            }

            x = b;
        }

        private Complex Twersky(char OPT, double omega, double BUMDEN, double XI, double ETA,
                                 double kx, double RHO0, double C0)
        {
            double c1 = 0, c2= 0;
            //dummy version
            
            return new Complex(c1, c2);
        }
    }
}
