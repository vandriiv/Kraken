using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation
{
    class BCImpedanceSolver
    {
        public void ComputeBoundaryConditionImpedance(KrakenModule krakenModule, double x, string BCType, string botTop, Complex cPHS, Complex cSHS,
                          double rhoHS, ref double f, ref double g, ref int iPow)
        { 
            iPow = 0;
            var yV = Enumerable.Repeat(0d, 5 + 1).ToList();

            if (BCType[0] == 'V' || BCType[0] == 'S'
              || BCType[0] == 'H' || BCType[0] == 'T'
              || BCType[0] == 'I')
            {
                f = 1.0;
                g = 0.0;

                yV[1] = f;
                yV[2] = g;
                yV[3] = 0.0;
                yV[4] = 0.0;
                yV[5] = 0.0;
            }

            if (BCType[0] == 'R')
            {
                f = 0.0;
                g = 1.0;

                yV[1] = f;
                yV[2] = g;
                yV[3] = 0.0;
                yV[4] = 0.0;
                yV[5] = 0.0;
            }

            if (BCType[0] == 'A')
            {
                Complex gammaS2, gammaP2, gammaS, gammaP;
                double mu;
                if (cSHS.Real > 0)
                {
                    gammaS2 = x - krakenModule.Omega2 / Math.Pow(cSHS.Real, 2);
                    gammaP2 = x - krakenModule.Omega2 / Math.Pow(cPHS.Real, 2);
                    gammaS = Complex.Sqrt(gammaS2);
                    gammaP = Complex.Sqrt(gammaP2);
                    mu = rhoHS * Math.Pow(cSHS.Real, 2);

                    yV[1] = ((gammaS * gammaP - x) / mu).Real;
                    yV[2] = ((Complex.Pow((gammaS2 + x), 2) - 4.0 * gammaS * gammaP * x) * mu).Real;
                    yV[3] = (2.0 * gammaS * gammaP - gammaS2 - x).Real;
                    yV[4] = (gammaP * (x - gammaS2)).Real;
                    yV[5] = (gammaS * (gammaS2 - x)).Real;

                    f = krakenModule.Omega2 * yV[4];
                    g = yV[2];
                    if (g > 0)
                    {
                        krakenModule.ModeCount++;
                    }
                }
                else
                {
                    gammaP = Complex.Sqrt(x - krakenModule.Omega2 / Complex.Pow(cPHS, 2).Real);
                    f = gammaP.Real;
                    g = rhoHS;
                }
            }

            if (botTop == "TOP")
            {
                g = -g;
            }

            if (botTop == "TOP")
            {
                if (krakenModule.FirstAcoustic > 1)
                {
                    for (var medium = 1; medium <= krakenModule.FirstAcoustic - 1; medium++)
                    {
                        ELASDN(krakenModule, x, yV, ref iPow, medium);
                    }
                    f = krakenModule.Omega2 * yV[4];
                    g = yV[2];
                }

            }
            else
            {
                if (krakenModule.LastAcoustic < krakenModule.NMedia)
                {
                    for (var medium = krakenModule.NMedia; medium >= krakenModule.LastAcoustic + 1; medium--)
                    {
                        ELASUP(krakenModule, x, yV, ref iPow, medium);
                    }
                    f = krakenModule.Omega2 * yV[4];
                    g = yV[2];
                }
            }
        }

        private void ELASUP(KrakenModule krakenModule, double x, List<double> yV, ref int iPow, int medium)
        {
            var xV = Enumerable.Repeat(0d, 5 + 1).ToList();
            var zV = Enumerable.Repeat(0d, 5 + 1).ToList();

            double roof = Math.Pow(10, 50);
            double floor = Math.Pow(10, -50);
            int iPowR = 50;
            int iPowF = -50;

            double twoX, twoH, fourHX, xB3;
            int j;

            twoX = 2.0 * x;
            twoH = 2.0 * krakenModule.H[medium];
            fourHX = 4.0 * krakenModule.H[medium] * x;
            j = krakenModule.Loc[medium] + krakenModule.N[medium] + 1;
            xB3 = x * krakenModule.B3[j] - krakenModule.Rho[j];

            zV[1] = yV[1] - 0.5 * (krakenModule.B1[j] * yV[4] - krakenModule.B2[j] * yV[5]);
            zV[2] = yV[2] - 0.5 * (-krakenModule.Rho[j] * yV[4] - xB3 * yV[5]);
            zV[3] = yV[3] - 0.5 * (twoH * yV[4] + krakenModule.B4[j] * yV[5]);
            zV[4] = yV[4] - 0.5 * (xB3 * yV[1] + krakenModule.B2[j] * yV[2] - twoX * krakenModule.B4[j] * yV[3]);
            zV[5] = yV[5] - 0.5 * (krakenModule.Rho[j] * yV[1] - krakenModule.B1[j] * yV[2] - fourHX * yV[3]);

            for (var II = krakenModule.N[medium]; II >= 1; II--)
            {
                j -= 1;

                for (var i = 0; i < yV.Count; i++)
                {
                    xV[i] = yV[i];
                }
                for (var i = 0; i < zV.Count; i++)
                {
                    yV[i] = zV[i];
                }

                xB3 = x * krakenModule.B3[j] - krakenModule.Rho[j];

                zV[1] = xV[1] - (krakenModule.B1[j] * yV[4] - krakenModule.B2[j] * yV[5]);
                zV[2] = xV[2] - (-krakenModule.Rho[j] * yV[4] - xB3 * yV[5]);
                zV[3] = xV[3] - (twoH * yV[4] + krakenModule.B4[j] * yV[5]);
                zV[4] = xV[4] - (xB3 * yV[1] + krakenModule.B2[j] * yV[2] - twoX * krakenModule.B4[j] * yV[3]);
                zV[5] = xV[5] - (krakenModule.Rho[j] * yV[1] - krakenModule.B1[j] * yV[2] - fourHX * yV[3]);

                if (II != 1)
                {
                    if (Math.Abs(zV[2]) < floor)
                    {
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= roof;
                            zV[i] *= roof;
                        }
                        iPow -= iPowR;
                    }

                    if (Math.Abs(zV[2]) > roof)
                    {
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= floor;
                            zV[i] *= floor;
                        }
                        iPow -= iPowF;
                    }
                }
            }

            for (var i = 1; i < yV.Count; i++)
            {
                yV[i] = (xV[i] + 2 * yV[i] + zV[i]) / 4.0;
            }
        }

        private void ELASDN(KrakenModule krakenModule, double x, List<double> yV, ref int iPow, int medium)
        {
            var xV = Enumerable.Repeat(0d, 5 + 1).ToList();
            var zV = Enumerable.Repeat(0d, 5 + 1).ToList();

            double roof = Math.Pow(10,50);
            double floor = Math.Pow(10,-50);
            int iPowR = 50;
            int iPowF = -50;

            double twoX, twoH, fourHX, xB3;
            int j;

            twoX = 2.0 * x;
            twoH = 2.0 * krakenModule.H[medium];
            fourHX = 4.0 * krakenModule.H[medium] * x;
            j = krakenModule.Loc[medium] + 1;
            xB3 = x * krakenModule.B3[j] - krakenModule.Rho[j];

            zV[1] = yV[1] + 0.5 * (krakenModule.B1[j] * yV[4] - krakenModule.B2[j] * yV[5]);
            zV[2] = yV[2] + 0.5 * (-krakenModule.Rho[j] * yV[4] - xB3 * yV[5]);
            zV[3] = yV[3] + 0.5 * (twoH * yV[4] + krakenModule.B4[j] * yV[5]);
            zV[4] = yV[4] + 0.5 * (xB3 * yV[1] + krakenModule.B2[j] * yV[2] - twoX * krakenModule.B4[j] * yV[3]);
            zV[5] = yV[5] + 0.5 * (krakenModule.Rho[j] * yV[1] - krakenModule.B1[j] * yV[2] - fourHX * yV[3]);

            for (var II = 1; II <= krakenModule.N[medium]; II++)
            {
                j += 1;

                for (var i = 0; i < yV.Count; i++)
                {
                    xV[i] = yV[i];
                }
                for (var i = 0; i < zV.Count; i++)
                {
                    yV[i] = zV[i];
                }

                xB3 = x * krakenModule.B3[j] - krakenModule.Rho[j];

                zV[1] = xV[1] + (krakenModule.B1[j] * yV[4] - krakenModule.B2[j] * yV[5]);
                zV[2] = xV[2] + (-krakenModule.Rho[j] * yV[4] - xB3 * yV[5]);
                zV[3] = xV[3] + (twoH * yV[4] + krakenModule.B4[j] * yV[5]);
                zV[4] = xV[4] + (xB3 * yV[1] + krakenModule.B2[j] * yV[2] - twoX * krakenModule.B4[j] * yV[3]);
                zV[5] = xV[5] + (krakenModule.Rho[j] * yV[1] - krakenModule.B1[j] * yV[2] - fourHX * yV[3]);

                if (II != krakenModule.N[medium])
                {
                    if (Math.Abs(zV[2]) < floor)
                    {
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= roof;
                            zV[i] *= roof;
                        }
                        iPow -= iPowR;
                    }

                    if (Math.Abs(zV[2]) > roof)
                    {
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= floor;
                            zV[i] *= floor;
                        }
                        iPow -= iPowF;
                    }
                }
            }

            for (var i = 1; i < yV.Count; i++)
            {
                yV[i] = (xV[i] + 2 * yV[i] + zV[i]) / 4.0;
            }
        }
    }
}
