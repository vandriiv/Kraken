using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.NormalModesCalculation
{
    class BCIMPMod
    {
        public void BCIMP(KrakMod krakMod, double x, string BCType, string BOTTOP, Complex CPHS, Complex CSHS,
                          double rhoHS, ref double F, ref double G, ref int IPow)
        {
            IPow = 0;
            var yV = Enumerable.Repeat(0d, 5+1).ToList();

            if (BCType[0] == 'V' || BCType[0] == 'S'
              || BCType[0] == 'H' || BCType[0] == 'T'
              || BCType[0] == 'I')
            {
                F = 1.0;
                G = 0.0;

                yV[1] = F;
                yV[2] = G;
                yV[3] = 0.0;
                yV[4] = 0.0;
                yV[5] = 0.0;
            }

            if (BCType[0] == 'R')
            {
                F = 0.0;
                G = 1.0;

                yV[1] = F;
                yV[2] = G;
                yV[3] = 0.0;
                yV[4] = 0.0;
                yV[5] = 0.0;
            }

            if (BCType[0] == 'A')
            {
                double gammaS2, gammaP2, gammaS, gammaP, RMU;
                if (CSHS.Real > 0)
                {
                    gammaS2 = x - krakMod.Omega2 / Math.Pow(CSHS.Real, 2);
                    gammaP2 = x - krakMod.Omega2 / Math.Pow(CPHS.Real, 2);
                    gammaS = Math.Sqrt(gammaS2);
                    gammaP = Math.Sqrt(gammaP2);
                    RMU = rhoHS * Math.Pow(CSHS.Real, 2);

                    yV[1] = (gammaS * gammaP - x) / RMU;
                    yV[2] = (Math.Pow((gammaS2 + x), 2) - 4.0 * gammaS * gammaP * x) * RMU;
                    yV[3] = 2.0 * gammaS * gammaP - gammaS2 - x;
                    yV[4] = gammaP * (x - gammaS2);
                    yV[5] = gammaS * (gammaS2 - x);

                    F = krakMod.Omega2 * yV[4];
                    G = yV[2];
                    if (G > 0)
                    {
                        krakMod.ModeCount++;
                    }
                }
                else
                {
                    gammaP = Math.Sqrt(x - krakMod.Omega2 / Complex.Pow(CPHS, 2).Real);
                    F = gammaP;
                    G = rhoHS;
                }
            }

            if (BOTTOP == "TOP")
            {
                G = -G;
            }

            if (BOTTOP == "TOP")
            {
                if (krakMod.FirstAcoustic > 1)
                {
                    for (var Medium = 1; Medium <= krakMod.FirstAcoustic - 1; Medium++)
                    {
                        ELASDN(krakMod, x, yV,ref IPow, Medium);
                    } 
                    F = krakMod.Omega2 * yV[4];
                    G = yV[2];
                }
               
            }
            else{
                if(krakMod.LastAcoustic < krakMod.NMedia){
                    for(var Medium = krakMod.NMedia; Medium>=krakMod.LastAcoustic+1;Medium--){
                        ELASUP(krakMod,x,yV,ref IPow,Medium);
                    }
                    F = krakMod.Omega2 * yV[4];
                    G = yV[2];
                }
            }
        }

        private void ELASUP(KrakMod krakMod, double x, List<double> yV, ref int IPow, int Medium)
        {
            var xV = Enumerable.Repeat(0d, 5+1).ToList();
            var zV = Enumerable.Repeat(0d, 5+1).ToList();

            double Roof = Math.Pow(10,4);
            double Floor = Math.Pow(0.1,4);
            int IPowR = 5;
            int IPowF = -5;

            double TWOx, TWOH, FOURHx, xB3;
            int J;

            TWOx = 2.0 * x;
            TWOH = 2.0 * krakMod.H[Medium];
            FOURHx = 4.0 * krakMod.H[Medium] * x;
            J = krakMod.LOC[Medium] + krakMod.N[Medium] + 1;
            xB3 = x * krakMod.B3[J] - krakMod.RHO[J];

            zV[1] = yV[1] - 0.5 * (krakMod.B1[J] * yV[4] - krakMod.B2[J] * yV[5]);
            zV[2] = yV[2] - 0.5 * (-krakMod.RHO[J] * yV[4] - xB3 * yV[5]);
            zV[3] = yV[3] - 0.5 * (TWOH * yV[4] + krakMod.B4[J] * yV[5]);
            zV[4] = yV[4] - 0.5 * (xB3 * yV[1] + krakMod.B2[J] * yV[2] - TWOx * krakMod.B4[J] * yV[3]);
            zV[5] = yV[5] - 0.5 * (krakMod.RHO[J] * yV[1] - krakMod.B1[J] * yV[2] - FOURHx * yV[3]);

            for (var II = krakMod.N[Medium]; II >= 1; II--)
            {
                J -= 1;
               
                for(var i = 0; i < yV.Count; i++)
                {
                    xV[i] = yV[i];
                }
                for (var i = 0; i < zV.Count; i++)
                {
                    yV[i] = zV[i];
                }

                xB3 = x * krakMod.B3[J] - krakMod.RHO[J];

                zV[1] = xV[1] - (krakMod.B1[J] * yV[4] - krakMod.B2[J] * yV[5]);
                zV[2] = xV[2] - (-krakMod.RHO[J] * yV[4] - xB3 * yV[5]);
                zV[3] = xV[3] - (TWOH * yV[4] + krakMod.B4[J] * yV[5]);
                zV[4] = xV[4] - (xB3 * yV[1] + krakMod.B2[J] * yV[2] - TWOx * krakMod.B4[J] * yV[3]);
                zV[5] = xV[5] - (krakMod.RHO[J] * yV[1] - krakMod.B1[J] * yV[2] - FOURHx * yV[3]);

                if(II!=1){
                    if (Math.Abs(zV[2]) < Floor)
                    {
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= Roof;
                            zV[i] *= Roof;
                        }
                        IPow -= IPowR;
                    }

                    if (Math.Abs(zV[2]) > Roof)
                    {
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= Floor;
                            zV[i] *= Floor;
                        }
                        IPow -= IPowF;
                    }
                }
            }

            for(var i =1;i<yV.Count;i++){
                yV[i] = (xV[i] +2*yV[i] + zV[i]) / 4.0;
            }
        }

        private void ELASDN(KrakMod krakMod, double x, List<double> yV, ref int IPow, int Medium)
        {
            var xV = Enumerable.Repeat(0d, 5+1).ToList();
            var zV = Enumerable.Repeat(0d, 5+1).ToList();

            double Roof = 100000;
            double Floor = 0.00001;
            int IPowR = 5;
            int IPowF = -5;

            double TWOx, TWOH, FOURHx, xB3;
            int J;

            TWOx = 2.0 * x;
            TWOH = 2.0 * krakMod.H[Medium];
            FOURHx = 4.0 * krakMod.H[Medium] * x;
            J = krakMod.LOC[Medium] + 1;
            xB3 = x * krakMod.B3[J] - krakMod.RHO[J];

            zV[1] = yV[1] + 0.5 * (krakMod.B1[J] * yV[4] - krakMod.B2[J] * yV[5]);
            zV[2] = yV[2] + 0.5 * (-krakMod.RHO[J] * yV[4] - xB3 * yV[5]);
            zV[3] = yV[3] + 0.5 * (TWOH * yV[4] + krakMod.B4[J] * yV[5]);
            zV[4] = yV[4] + 0.5 * (xB3 * yV[1] + krakMod.B2[J] * yV[2] - TWOx * krakMod.B4[J] * yV[3]);
            zV[5] = yV[5] + 0.5 * (krakMod.RHO[J] * yV[1] - krakMod.B1[J] * yV[2] - FOURHx * yV[3]);

            for (var II = 1; II <= krakMod.N[Medium]; II++)
            {
                J += 1;

                for (var i = 0; i < yV.Count; i++)
                {
                    xV[i] = yV[i];
                }
                for (var i = 0; i < zV.Count; i++)
                {
                    yV[i] = zV[i];
                }

                xB3 = x * krakMod.B3[J] - krakMod.RHO[J];

                zV[1] = xV[1] + (krakMod.B1[J] * yV[4] - krakMod.B2[J] * yV[5]);
                zV[2] = xV[2] + (-krakMod.RHO[J] * yV[4] - xB3 * yV[5]);
                zV[3] = xV[3] + (TWOH * yV[4] + krakMod.B4[J] * yV[5]);
                zV[4] = xV[4] + (xB3 * yV[1] + krakMod.B2[J] * yV[2] - TWOx * krakMod.B4[J] * yV[3]);
                zV[5] = xV[5] + (krakMod.RHO[J] * yV[1] - krakMod.B1[J] * yV[2] - FOURHx * yV[3]);

                if(II!=krakMod.N[Medium]){
                    if(Math.Abs(zV[2])<Floor){
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= Roof;
                            zV[i] *= Roof;
                        }
                        IPow -= IPowR;
                    }

                    if(Math.Abs(zV[2])>Roof){
                        for (var i = 0; i < zV.Count; i++)
                        {
                            yV[i] *= Floor;
                            zV[i] *= Floor;
                        }
                        IPow -= IPowF;
                    }
                }
            }

            for(var i =1;i<yV.Count;i++){
                yV[i] = (xV[i] +2*yV[i] + zV[i]) / 4.0;
            }
        }
    }
}
