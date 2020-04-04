using Kraken.Calculation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation
{
    class MeshInitializer
    {
        private const int maxSSP = 2001;

        private double alphaR;
        private double betaR;
        private double rhoR;
        private double alphaI;
        private double betaI;

        private readonly List<int> NSSPPts;
        private readonly List<double> z;

        private readonly List<Complex> alpha;
        private readonly List<Complex> beta;
        private readonly List<double> rho;

        private readonly List<List<Complex>> cpSpline;
        private readonly List<List<Complex>> csSpline;
        private readonly List<List<Complex>> rhoSpline;

        public MeshInitializer(KrakenModule krakenModule)
        {
            NSSPPts = Enumerable.Repeat(0, krakenModule.MaxMedium + 1).ToList();
            z = Enumerable.Repeat(0d, maxSSP + 1).ToList();

            if (krakenModule.BCTop[0] == 'S')
            {
                cpSpline = new List<List<Complex>>(4 + 1);
                for (var i = 0; i < 5; i++)
                {
                    cpSpline.Add(Enumerable.Repeat(new Complex(), maxSSP + 1).ToList());
                }

                csSpline = new List<List<Complex>>(4 + 1);
                for (var i = 0; i < 5; i++)
                {
                    csSpline.Add(Enumerable.Repeat(new Complex(), maxSSP + 1).ToList());
                }

                rhoSpline = new List<List<Complex>>(4 + 1);
                for (var i = 0; i < 5; i++)
                {
                    rhoSpline.Add(Enumerable.Repeat(new Complex(), maxSSP + 1).ToList());
                }
            }
            else
            {
                alpha = Enumerable.Repeat(new Complex(), maxSSP + 1).ToList();
                beta = Enumerable.Repeat(new Complex(), maxSSP + 1).ToList();
                rho = Enumerable.Repeat(0d, maxSSP + 1).ToList();
            }
        }


        public void ProccedMesh(KrakenModule krakenModule, int nc, List<List<double>> ssp,
                           List<double> tahsp, List<double> tsp, List<double> bahsp)
        {          
            string SSPType = krakenModule.BCTop[0].ToString();
            string BCType = krakenModule.BCTop[1].ToString();
            string attenUnit = krakenModule.BCTop[2].ToString();
            if (krakenModule.BCTop.Length > 3)
            {
                attenUnit += krakenModule.BCTop[3].ToString();
            }

            alphaR = 1500.0;
            betaR = 0.0;
            rhoR = 0.0;
            alphaI = 0.0;
            betaI = 0.0;
            var NElts = 0;
            var rho = new List<double>() { 0, 0 };
            if (BCType == "A")
            {
                alphaR = tahsp[2];
                betaR = tahsp[3];
                rhoR = tahsp[4];
                alphaI = tahsp[5];
                betaI = tahsp[6];

                if (alphaR == 0 || rhoR == 0)
                {
                    throw new Exception("Sound speed or density vanishes in halfspace");
                }

                krakenModule.CPTop = ConvertToSingleComplexWaveSpeed(alphaR, alphaI, krakenModule.Frequency, attenUnit);
                krakenModule.CSTop = ConvertToSingleComplexWaveSpeed(betaR, betaI, krakenModule.Frequency, attenUnit);
                krakenModule.RhoTop = rhoR;
            }

            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                krakenModule.BumpDensity = tsp[1];
                krakenModule.Eta = tsp[2];
                krakenModule.Xi = tsp[3];
            }

            var cP = new List<Complex>(maxSSP + 1);
            var cS = new List<Complex>(maxSSP + 1);

            for (var medium = 1; medium <= krakenModule.NMedia; medium++)
            {
                if ((25 * krakenModule.Frequency / 1500) * Math.Pow(krakenModule.Sigma[medium], 2) > 1)
                {
                    krakenModule.Warnings.Add("The Rayleigh roughness parameter might exceed the region of validity for the scatter approximation");
                }

                var task = "INIT";
                EvaluateSSP(krakenModule, krakenModule.Depth, cP, cS, rho, medium, ref NElts, 0, krakenModule.Frequency, SSPType,
                       attenUnit, task,ssp);

                var c = alphaR;
                if (betaR > 0)
                {
                    c = betaR;
                }
                var deltaZ = c / krakenModule.Frequency / 20;
                var nNeeded =(int) (krakenModule.Depth[medium + 1] - krakenModule.Depth[medium]) / deltaZ;
                nNeeded = Math.Max(nNeeded, 10);
                if (krakenModule.NG[medium] == 0)
                {
                    krakenModule.NG[medium] = (int)nNeeded;
                }
                else if (krakenModule.NG[medium] < nNeeded / 2)
                {
                    throw new KrakenException("Mesh is too coarse");
                }
            }

            BCType = krakenModule.BCBottom[0].ToString();
            if (BCType == "A")
            {
                alphaR = bahsp[2];
                betaR = bahsp[3];
                rhoR = bahsp[4];
                alphaI = bahsp[5];
                betaI = bahsp[6];

                if (alphaR == 0 || rhoR == 0)
                {
                    throw new Exception("Sound speed or density vanishes in halfspace");
                }

                krakenModule.CPBottom = ConvertToSingleComplexWaveSpeed(alphaR, alphaI, krakenModule.Frequency, attenUnit);
                krakenModule.CSBottom = ConvertToSingleComplexWaveSpeed(betaR, betaI, krakenModule.Frequency, attenUnit);
                krakenModule.RhoBottom = rhoR;
            }
        }

        public void EvaluateSSP(KrakenModule krakenModule, List<double> depth, List<Complex> cP, List<Complex> cS, List<double> RhoTop,
                          int medium, ref int n1, int offset, double Frequency, string SSPType, string attenUnit, string task, List<List<double>> ssp)
        {
            switch (SSPType)
            {
                case "N":
                    N2Linear(krakenModule, depth, cP, cS, RhoTop, medium, ref n1, offset, Frequency, attenUnit, task, ssp);
                    break;
                case "c":
                    CLinear(krakenModule, depth, cP, cS, RhoTop, medium, ref n1, offset, Frequency, attenUnit, task, ssp);
                    break;
                case "S":
                    CubicSpline(krakenModule, depth, cP, cS, RhoTop, medium, ref n1, offset, Frequency, attenUnit, task, ssp);
                    break;
                default:
                    throw new ArgumentException("Unknown profile option (SSPType)");
            }
        }

        private Complex ConvertToSingleComplexWaveSpeed(double c, double alpha, double Frequency, string attenUnit)
        {
            var omega = 2.0 * Math.PI * Frequency;
            var alphaT = 0.0;
            switch (attenUnit[0])
            {
                case 'N':
                    alphaT = alpha;
                    break;
                case 'M':
                    alphaT = alpha / 8.6858896;
                    break;
                case 'F':
                    alphaT = alpha * Frequency / 8685.8896;
                    break;
                case 'W':
                    if (c != 0)
                    {
                        alphaT = alpha * Frequency / (8.6858896 * c);
                    }
                    break;
                case 'Q':
                    if (c * alpha != 0)
                    {
                        alphaT = omega / (2.0 * c * alpha);
                    }
                    break;
                case 'L':
                    if (c != 0)
                    {
                        alphaT = alpha * omega / c;
                    }
                    break;
            }

            if (attenUnit.Length > 1)
            {
                switch (attenUnit[1])
                {
                    case 'T':
                        var f2 = Math.Pow((Frequency / 1000.0), 2);

                        var thorp = 3.3 * Math.Pow(10, -3) + 0.11 * f2 / (1.0 + f2) + 44.0 * f2 / (4100.0 + f2) + 3 * Math.Pow(10, -4) * f2;
                        thorp /= 8685.8896;

                        alphaT += thorp;
                        break;
                }
            }

            alphaT = alphaT * c * c / omega;
            var waveSpeed = new Complex(c, alphaT);
            return waveSpeed;
        }

        private void N2Linear(KrakenModule krakenModule, List<double> depth, List<Complex> cP, List<Complex> cS, List<double> RhoTop, int medium, ref int n1,
                          int offset, double Frequency, string attenUnit, string task, List<List<double>> ssp)
        {
          
            int ILoc;
            if (task.Contains("INIT"))
            {
                NSSPPts[medium] = n1;

                if (medium == 1)
                {
                    krakenModule.Loc[medium] = 0;
                }
                else
                {
                    krakenModule.Loc[medium] = krakenModule.Loc[medium - 1] + NSSPPts[medium - 1];
                }
                ILoc = krakenModule.Loc[medium];

                n1 = 1;
                for (var i = 1; i <= maxSSP; i++)
                {
                    int ind = ILoc + i;
                    z[ind] = ssp[ind][1];
                    alphaR = ssp[ind][2];
                    betaR = ssp[ind][3];
                    rhoR = ssp[ind][4];
                    alphaI = ssp[ind][5];
                    betaI = ssp[ind][6];

                    alpha[ILoc + i] = ConvertToSingleComplexWaveSpeed(alphaR, alphaI, Frequency, attenUnit);
                    beta[ILoc + i] = ConvertToSingleComplexWaveSpeed(betaR, betaI, Frequency, attenUnit);
                    rho[ILoc + i] = rhoR;

                    if (Math.Abs(z[ILoc + i] - depth[medium + 1]) < 0.000000119 * depth[medium + 1])
                    {
                        NSSPPts[medium] = n1;
                        if (medium == 1)
                        {
                            depth[1] = z[1];
                        }

                        return;
                    }

                    n1++;
                }

                throw new KrakenException("Number of SSP points exceeds limit");
            }
            else
            {
                ILoc = NSSPPts[medium - 1];
                var N = n1 - 1;
                var h = (z[ILoc + NSSPPts[medium]] - z[ILoc + 1]) / N;
                var Lay = 1;
                for (var i = offset; i <= n1 + offset - 1; i++)
                {
                    double zT = z[ILoc + 1] + (i - offset) * h;

                    if (i == n1 + offset)
                    {
                        zT = z[ILoc + NSSPPts[medium]];
                    }

                    while (zT > z[ILoc + Lay + 1])
                    {
                        Lay += 1;
                    }

                    var r = (zT - z[ILoc + Lay]) / (z[ILoc + Lay + 1] - z[ILoc + Lay]);

                    var N2Top = 1.0 / Complex.Pow(alpha[ILoc + Lay], 2);
                    var N2Bot = 1.0 / Complex.Pow(alpha[ILoc + Lay + 1], 2);
                    cP[i] = 1.0 / Complex.Sqrt((1.0 - r) * N2Top + r * N2Bot);

                    if (beta[ILoc + Lay] != 0)
                    {
                        N2Top = 1.0 / Complex.Pow(beta[ILoc + Lay], 2);
                        N2Bot = 1.0 / Complex.Pow(beta[ILoc + Lay + 1], 2);
                        cS[i] = 1.0 / Complex.Sqrt((1.0 - r) * N2Top + r * N2Bot);
                    }
                    else
                    {
                        cS[i] = new Complex(0.0, 0.0);
                    }

                    RhoTop[i] = (1.0 - r) * rho[ILoc + Lay] + r * rho[ILoc + Lay + 1];
                }
            }
        }

        private void CLinear(KrakenModule krakenModule, List<double> depth, List<Complex> cP, List<Complex> cS, List<double> RhoTop, int medium, ref int n1,
                          int offset, double Frequency, string attenUnit, string task, List<List<double>> ssp)
        {           
            int ILoc;
            if (task.Contains("INIT"))
            {
                NSSPPts[medium] = n1;

                if (medium == 1)
                {
                    krakenModule.Loc[medium] = 0;
                }
                else
                {
                    krakenModule.Loc[medium] = krakenModule.Loc[medium - 1] + NSSPPts[medium - 1];
                }
                ILoc = krakenModule.Loc[medium];

                n1 = 1;
                for (var i = 1; i <= maxSSP; i++)
                {
                    int ind = ILoc + i;
                    z[ind] = ssp[ind][1];
                    alphaR = ssp[ind][2];
                    betaR = ssp[ind][3];
                    rhoR = ssp[ind][4];
                    alphaI = ssp[ind][5];
                    betaI = ssp[ind][6];

                    alpha[ILoc + i] = ConvertToSingleComplexWaveSpeed(alphaR, alphaI, Frequency, attenUnit);
                    beta[ILoc + i] = ConvertToSingleComplexWaveSpeed(betaR, betaI, Frequency, attenUnit);
                    rho[ILoc + i] = rhoR;

                    if (Math.Abs(z[ILoc + i] - depth[medium + 1]) < 0.0000001 * depth[medium + 1])
                    {
                        NSSPPts[medium] = n1;
                        if (medium == 1)
                        {
                            depth[1] = z[1];
                        }

                        return;
                    }

                    n1++;
                }

                throw new KrakenException("Number of SSP points exceeds limit");
            }
            else
            {
                ILoc = NSSPPts[medium - 1];
                var N = n1 - 1;
                var h = (z[ILoc + NSSPPts[medium]] - z[ILoc + 1]) / N;
                var Lay = 1;
                for (var i = offset; i <= n1 + offset - 1; i++)
                {
                    double zT = z[ILoc + 1] + (i - offset) * h;

                    if (i == n1 + offset)
                    {
                        zT = z[ILoc + NSSPPts[medium]];
                    }

                    while (zT > z[ILoc + Lay + 1])
                    {
                        Lay += 1;
                    }

                    var r = (zT - z[ILoc + Lay]) / (z[ILoc + Lay + 1] - z[ILoc + Lay]);
                    cP[i] = (1.0 - r) * alpha[ILoc + Lay] + r * alpha[ILoc + Lay + 1];
                    cS[i] = (1.0 - r) * beta[ILoc + Lay] + r * beta[ILoc + Lay + 1];
                    RhoTop[i] = (1.0 - r) * rho[ILoc + Lay] + r * rho[ILoc + Lay + 1];
                }
            }
        }

        private void CubicSpline(KrakenModule krakenModule, List<double> depth, List<Complex> cP, List<Complex> cS, List<double> RhoTop, int medium,
            ref int n1, int offset, double Frequency, string attenUnit, string task, List<List<double>> ssp)
        {           
            int ILoc;
            if (task.Contains("INIT"))
            {
                NSSPPts[medium] = n1;

                if (medium == 1)
                {
                    krakenModule.Loc[medium] = 0;
                }
                else
                {
                    krakenModule.Loc[medium] = krakenModule.Loc[medium - 1] + NSSPPts[medium - 1];
                }
                ILoc = krakenModule.Loc[medium];

                n1 = 1;
                for (var i = 1; i <= maxSSP; i++)
                {
                    int ind = ILoc + i;
                    z[ind] = ssp[ind][1];
                    alphaR = ssp[ind][2];
                    betaR = ssp[ind][3];
                    rhoR = ssp[ind][4];
                    alphaI = ssp[ind][5];
                    betaI = ssp[ind][6];

                    cpSpline[1][ILoc + i] = ConvertToSingleComplexWaveSpeed(alphaR, alphaI, Frequency, attenUnit);
                    csSpline[1][ILoc + i] = ConvertToSingleComplexWaveSpeed(betaR, betaI, Frequency, attenUnit);
                    rhoSpline[1][ILoc + i] = rhoR;

                    if (Math.Abs(z[ILoc + i] - depth[medium + 1]) < 0.0000001 * depth[medium + 1])
                    {
                        NSSPPts[medium] = n1;
                        if (medium == 1)
                        {
                            depth[1] = z[1];
                        }                       

                        var IBCBEG = 0;
                        var IBCEND = 0;
                        var splineC = new SplineCalculator();
                        splineC.CalculateCubicSpline(z, cpSpline, ILoc, NSSPPts[medium], IBCBEG, IBCEND);
                        splineC.CalculateCubicSpline(z, csSpline, ILoc, NSSPPts[medium], IBCBEG, IBCEND);
                        splineC.CalculateCubicSpline(z, rhoSpline, ILoc, NSSPPts[medium], IBCBEG, IBCEND);

                        return;
                    }

                    n1++;
                }

                throw new KrakenException("Number of SSP points exceeds limit");
            }
            else
            {
                ILoc = NSSPPts[medium - 1];
                var N = n1 - 1;
                var h = (z[ILoc + NSSPPts[medium]] - z[ILoc + 1]) / N;
                var Lay = 1;
                for (var i = offset; i <= n1 + offset - 1; i++)
                {
                    double zT = z[ILoc + 1] + (i - offset) * h;
                    if (i == n1 + offset)
                    {
                        zT = z[ILoc + NSSPPts[medium]];
                    }

                    while (zT > z[ILoc + Lay + 1])
                    {
                        Lay += 1;
                    }
                    var splineCalculator = new SplineCalculator();
                    var hSpline = zT - z[ILoc + Lay];
                    cP[i] = splineCalculator.CalculateExponentialSpline(cpSpline, ILoc + Lay, hSpline);
                    cS[i] = splineCalculator.CalculateExponentialSpline(csSpline, ILoc + Lay, hSpline);
                    RhoTop[i] = splineCalculator.CalculateExponentialSpline(rhoSpline, ILoc + Lay, hSpline).Real;
                }
            }
        }

    }
}
