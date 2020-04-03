using Kraken.Calculation.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Kraken.Calculation
{
    public class Readin
    {
        private double alphaR;
        private double betaR;
        private double rhoR;
        private double alphaI;
        private double betaI;

        private readonly List<int> NSSPPts;
        private readonly List<double> Z;

        private readonly List<Complex> alpha;
        private readonly List<Complex> beta;
        private readonly List<double> rho;

        private readonly List<List<Complex>> alphaC;
        private readonly List<List<Complex>> betaC;
        private readonly List<List<Complex>> rhoC;

        public Readin(KrakMod krakMod)
        {
            var MaxSSP = 2001;
            NSSPPts = Enumerable.Repeat(0, krakMod.MaxMedium + 1).ToList();
            Z = Enumerable.Repeat(0d, MaxSSP + 1).ToList();

            if (krakMod.TopOpt[0] == 'S')
            {
                alphaC = new List<List<Complex>>(4 + 1);
                for (var i = 0; i < 5; i++)
                {
                    alphaC.Add(Enumerable.Repeat(new Complex(), MaxSSP + 1).ToList());
                }

                betaC = new List<List<Complex>>(4 + 1);
                for (var i = 0; i < 5; i++)
                {
                    betaC.Add(Enumerable.Repeat(new Complex(), MaxSSP + 1).ToList());
                }

                rhoC = new List<List<Complex>>(4 + 1);
                for (var i = 0; i < 5; i++)
                {
                    rhoC.Add(Enumerable.Repeat(new Complex(), MaxSSP + 1).ToList());
                }
            }
            else
            {
                alpha = Enumerable.Repeat(new Complex(), MaxSSP + 1).ToList();
                beta = Enumerable.Repeat(new Complex(), MaxSSP + 1).ToList();
                rho = Enumerable.Repeat(0d, MaxSSP + 1).ToList();
            }
        }


        public void READIN(KrakMod krakMod, int nc, List<List<double>> ssp,
                           List<double> tahsp, List<double> tsp, List<double> bahsp)
        {
            int MaxSSP = 2001;
            string SSPType = krakMod.TopOpt[0].ToString();
            string BCType = krakMod.TopOpt[1].ToString();
            string AttenUnit = krakMod.TopOpt[2].ToString();
            if (krakMod.TopOpt.Length > 3)
            {
                AttenUnit += krakMod.TopOpt[3].ToString();
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

                krakMod.CPT = CRCI(alphaR, alphaI, krakMod.Freq, AttenUnit);
                krakMod.CST = CRCI(betaR, betaI, krakMod.Freq, AttenUnit);
                krakMod.rhoT = rhoR;
            }

            if (BCType == "S" || BCType == "H" || BCType == "T" || BCType == "I")
            {
                krakMod.BumDen = tsp[1];
                krakMod.eta = tsp[2];
                krakMod.xi = tsp[3];
            }

            var CP = new List<Complex>(MaxSSP + 1);
            var CS = new List<Complex>(MaxSSP + 1);

            for (var Medium = 1; Medium <= krakMod.NMedia; Medium++)
            {
                if ((25 * krakMod.Freq / 1500) * Math.Pow(krakMod.SIGMA[Medium], 2) > 1)
                {
                    krakMod.Warnings.Add("The Rayleigh roughness parameter might exceed the region of validity for the scatter approximation");
                }

                var Task = "INIT";
                PROFIL(krakMod, krakMod.Depth, CP, CS, rho, Medium, ref NElts, 0, krakMod.Freq, SSPType,
                AttenUnit, Task, nc, ssp);

                var C = alphaR;
                if (betaR > 0)
                {
                    C = betaR;
                }
                var deltaZ = C / krakMod.Freq / 20;
                var Nneeded = (krakMod.Depth[Medium + 1] - krakMod.Depth[Medium]) / deltaZ;
                Nneeded = Math.Max(Nneeded, 10);
                if (krakMod.NG[Medium] == 0)
                {
                    krakMod.NG[Medium] = (int)Nneeded;
                }
                else if (krakMod.NG[Medium] < Nneeded / 2)
                {
                    throw new KrakenException("Mesh is too coarse");
                }
            }

            BCType = krakMod.BotOpt[0].ToString();
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

                krakMod.CPB = CRCI(alphaR, alphaI, krakMod.Freq, AttenUnit);
                krakMod.CSB = CRCI(betaR, betaI, krakMod.Freq, AttenUnit);
                krakMod.rhoB = rhoR;
            }
        }

        public void PROFIL(KrakMod krakMod, List<double> Depth, List<Complex> CP, List<Complex> CS, List<double> rhoT,
                          int Medium, ref int N1, int next, double Freq, string SSPType, string AttenUnit, string Task, int nc, List<List<double>> ssp)
        {
            switch (SSPType)
            {
                case "N":
                    N2LIN(krakMod, Depth, CP, CS, rhoT, Medium, ref N1, next, Freq, AttenUnit, Task, nc, ssp);
                    break;
                case "C":
                    CLIN(krakMod, Depth, CP, CS, rhoT, Medium, ref N1, next, Freq, AttenUnit, Task, nc, ssp);
                    break;
                case "S":
                    CCUBIC(krakMod, Depth, CP, CS, rhoT, Medium, ref N1, next, Freq, AttenUnit, Task, nc, ssp);
                    break;
                default:
                    throw new ArgumentException("Unknown profile option (SSPType)");
            }
        }

        public Complex CRCI(double C, double alpha, double Freq, string AttenUnit)
        {
            var Omega = 2.0 * Math.PI * Freq;
            var alphaT = 0.0;
            switch (AttenUnit[0])
            {
                case 'N':
                    alphaT = alpha;
                    break;
                case 'M':
                    alphaT = alpha / 8.6858896;
                    break;
                case 'F':
                    alphaT = alpha * Freq / 8685.8896;
                    break;
                case 'W':
                    if (C != 0)
                    {
                        alphaT = alpha * Freq / (8.6858896 * C);
                    }
                    break;
                case 'Q':
                    if (C * alpha != 0)
                    {
                        alphaT = Omega / (2.0 * C * alpha);
                    }
                    break;
                case 'L':
                    if (C != 0)
                    {
                        alphaT = alpha * Omega / C;
                    }
                    break;
            }

            if (AttenUnit.Length > 1)
            {
                switch (AttenUnit[1])
                {
                    case 'T':
                        var F2 = Math.Pow((Freq / 1000.0), 2);
                        var Thorpe = 40.0 * F2 / (4100 + F2) + 0.1 * F2 / (1.0 + F2);
                        Thorpe /= 914.4;
                        Thorpe /= 8.6858896;
                        alphaT += Thorpe;
                        break;
                }
            }

            alphaT = alphaT * C * C / Omega;
            var CRCI = new Complex(C, alphaT);
            return CRCI;
        }

        public void N2LIN(KrakMod krakMod, List<double> Depth, List<Complex> CP, List<Complex> CS, List<double> rhoT, int Medium, ref int N1,
                          int next, double Freq, string AttenUnit, string Task, int nc, List<List<double>> ssp)
        {
            var MaxSSP = 2001;

            int ILoc;
            if (Task.Contains("INIT"))
            {
                NSSPPts[Medium] = N1;

                if (Medium == 1)
                {
                    krakMod.LOC[Medium] = 0;
                }
                else
                {
                    krakMod.LOC[Medium] = krakMod.LOC[Medium - 1] + NSSPPts[Medium - 1];
                }
                ILoc = krakMod.LOC[Medium];

                N1 = 1;
                for (var I = 1; I <= MaxSSP; I++)
                {
                    int ind = ILoc + I;
                    Z[ind] = ssp[ind][1];
                    alphaR = ssp[ind][2];
                    betaR = ssp[ind][3];
                    rhoR = ssp[ind][4];
                    alphaI = ssp[ind][5];
                    betaI = ssp[ind][6];

                    alpha[ILoc + I] = CRCI(alphaR, alphaI, Freq, AttenUnit);
                    beta[ILoc + I] = CRCI(betaR, betaI, Freq, AttenUnit);
                    rho[ILoc + I] = rhoR;

                    if (Math.Abs(Z[ILoc + I] - Depth[Medium + 1]) < 0.000000119 * Depth[Medium + 1])
                    {
                        NSSPPts[Medium] = N1;
                        if (Medium == 1)
                        {
                            Depth[1] = Z[1];
                        }

                        return;
                    }

                    N1++;
                }

                throw new KrakenException("Number of SSP points exceeds limit");
            }
            else
            {
                ILoc = NSSPPts[Medium - 1];
                var N = N1 - 1;
                var H = (Z[ILoc + NSSPPts[Medium]] - Z[ILoc + 1]) / N;
                var Lay = 1;
                for (var I = next; I <= N1 + next - 1; I++)
                {
                    double ZT = Z[ILoc + 1] + (I - next) * H;

                    if (I == N1 + next)
                    {
                        ZT = Z[ILoc + NSSPPts[Medium]];
                    }

                    while (ZT > Z[ILoc + Lay + 1])
                    {
                        Lay += 1;
                    }

                    var R = (ZT - Z[ILoc + Lay]) / (Z[ILoc + Lay + 1] - Z[ILoc + Lay]);

                    var N2TOP = 1.0 / Complex.Pow(alpha[ILoc + Lay], 2);
                    var N2BOT = 1.0 / Complex.Pow(alpha[ILoc + Lay + 1], 2);
                    CP[I] = 1.0 / Complex.Sqrt((1.0 - R) * N2TOP + R * N2BOT);

                    if (beta[ILoc + Lay] != 0)
                    {
                        N2TOP = 1.0 / Complex.Pow(beta[ILoc + Lay], 2);
                        N2BOT = 1.0 / Complex.Pow(beta[ILoc + Lay + 1], 2);
                        CS[I] = 1.0 / Complex.Sqrt((1.0 - R) * N2TOP + R * N2BOT);
                    }
                    else
                    {
                        CS[I] = new Complex(0.0, 0.0);
                    }

                    rhoT[I] = (1.0 - R) * rho[ILoc + Lay] + R * rho[ILoc + Lay + 1];
                }
            }
        }

        public void CLIN(KrakMod krakMod, List<double> Depth, List<Complex> CP, List<Complex> CS, List<double> rhoT, int Medium, ref int N1,
                          int next, double Freq, string AttenUnit, string Task, int nc, List<List<double>> ssp)
        {
            var MaxSSP = 2001;

            int ILoc;
            if (Task.Contains("INIT"))
            {
                NSSPPts[Medium] = N1;

                if (Medium == 1)
                {
                    krakMod.LOC[Medium] = 0;
                }
                else
                {
                    krakMod.LOC[Medium] = krakMod.LOC[Medium - 1] + NSSPPts[Medium - 1];
                }
                ILoc = krakMod.LOC[Medium];

                N1 = 1;
                for (var I = 1; I <= MaxSSP; I++)
                {
                    int ind = ILoc + I;
                    Z[ind] = ssp[ind][1];
                    alphaR = ssp[ind][2];
                    betaR = ssp[ind][3];
                    rhoR = ssp[ind][4];
                    alphaI = ssp[ind][5];
                    betaI = ssp[ind][6];

                    alpha[ILoc + I] = CRCI(alphaR, alphaI, Freq, AttenUnit);
                    beta[ILoc + I] = CRCI(betaR, betaI, Freq, AttenUnit);
                    rho[ILoc + I] = rhoR;

                    if (Math.Abs(Z[ILoc + I] - Depth[Medium + 1]) < 0.0000001 * Depth[Medium + 1])
                    {
                        NSSPPts[Medium] = N1;
                        if (Medium == 1)
                        {
                            Depth[1] = Z[1];
                        }

                        return;
                    }

                    N1++;
                }

                throw new KrakenException("Number of SSP points exceeds limit");
            }
            else
            {
                ILoc = NSSPPts[Medium - 1];
                var N = N1 - 1;
                var H = (Z[ILoc + NSSPPts[Medium]] - Z[ILoc + 1]) / N;
                var Lay = 1;
                for (var I = next; I <= N1 + next - 1; I++)
                {
                    double ZT = Z[ILoc + 1] + (I - next) * H;

                    if (I == N1 + next)
                    {
                        ZT = Z[ILoc + NSSPPts[Medium]];
                    }

                    while (ZT > Z[ILoc + Lay + 1])
                    {
                        Lay += 1;
                    }

                    var R = (ZT - Z[ILoc + Lay]) / (Z[ILoc + Lay + 1] - Z[ILoc + Lay]);
                    CP[I] = (1.0 - R) * alpha[ILoc + Lay] + R * alpha[ILoc + Lay + 1];
                    CS[I] = (1.0 - R) * beta[ILoc + Lay] + R * beta[ILoc + Lay + 1];
                    rhoT[I] = (1.0 - R) * rho[ILoc + Lay] + R * rho[ILoc + Lay + 1];
                }
            }
        }

        public void CCUBIC(KrakMod krakMod, List<double> Depth, List<Complex> CP, List<Complex> CS, List<double> rhoT, int Medium,
            ref int N1, int next, double Freq, string AttenUnit, string Task, int nc, List<List<double>> ssp)
        {
            var MaxSSP = 2001;

            int ILoc;
            if (Task.Contains("INIT"))
            {
                NSSPPts[Medium] = N1;

                if (Medium == 1)
                {
                    krakMod.LOC[Medium] = 0;
                }
                else
                {
                    krakMod.LOC[Medium] = krakMod.LOC[Medium - 1] + NSSPPts[Medium - 1];
                }
                ILoc = krakMod.LOC[Medium];

                N1 = 1;
                for (var I = 1; I <= MaxSSP; I++)
                {
                    int ind = ILoc + I;
                    Z[ind] = ssp[ind][1];
                    alphaR = ssp[ind][2];
                    betaR = ssp[ind][3];
                    rhoR = ssp[ind][4];
                    alphaI = ssp[ind][5];
                    betaI = ssp[ind][6];

                    alphaC[1][ILoc + I] = CRCI(alphaR, alphaI, Freq, AttenUnit);
                    betaC[1][ILoc + I] = CRCI(betaR, betaI, Freq, AttenUnit);
                    rhoC[1][ILoc + I] = rhoR;

                    if (Math.Abs(Z[ILoc + I] - Depth[Medium + 1]) < 0.0000001 * Depth[Medium + 1])
                    {
                        NSSPPts[Medium] = N1;
                        if (Medium == 1)
                        {
                            Depth[1] = Z[1];
                        }                       

                        var IBCBEG = 0;
                        var IBCEND = 0;
                        var splineC = new Splinec();
                        splineC.CSPLINE(Z, alphaC, ILoc, NSSPPts[Medium], IBCBEG, IBCEND, NSSPPts[Medium]);
                        splineC.CSPLINE(Z, betaC, ILoc, NSSPPts[Medium], IBCBEG, IBCEND, NSSPPts[Medium]);
                        splineC.CSPLINE(Z, rhoC, ILoc, NSSPPts[Medium], IBCBEG, IBCEND, NSSPPts[Medium]);

                        return;
                    }

                    N1++;
                }

                throw new KrakenException("Number of SSP points exceeds limit");
            }
            else
            {
                ILoc = NSSPPts[Medium - 1];
                var N = N1 - 1;
                var H = (Z[ILoc + NSSPPts[Medium]] - Z[ILoc + 1]) / N; //check
                var Lay = 1;
                for (var I = next; I <= N1 + next - 1; I++)
                {
                    double ZT = Z[ILoc + 1] + (I - next) * H;
                    if (I == N1 + next)
                    {
                        ZT = Z[ILoc + NSSPPts[Medium]];
                    }

                    while (ZT > Z[ILoc + Lay + 1])
                    {
                        Lay += 1;
                    }
                    var splineC = new Splinec();
                    var HSPLNE = ZT - Z[ILoc + Lay];
                    CP[I] = splineC.ESPLINE(alphaC, ILoc + Lay, HSPLNE);
                    CS[I] = splineC.ESPLINE(betaC, ILoc + Lay, HSPLNE);
                    rhoT[I] = splineC.ESPLINE(rhoC, ILoc + Lay, HSPLNE).Real;
                }
            }
        }

    }
}
