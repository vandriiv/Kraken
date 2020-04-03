using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Kraken.Calculation.Field
{
    public class ReadModesMod
    {
        private Complex kTop2;
        private Complex kBot2;
        private double pi = Math.PI;

        public List<List<Complex>> GetModes(ModesOut modesOut, int MaxM, List<double> rd, int Nrd, string Comp, List<string> warnings)
        {
            var PhiR = new List<List<Complex>>(MaxM + 1);
            for (var i = 0; i <= MaxM; i++)
            {
                PhiR.Add(Enumerable.Repeat(new Complex(), Nrd + 1).ToList());
            }

            var W = Enumerable.Repeat(0d, Nrd + 1).ToList();
            var ird = Enumerable.Repeat(0, Nrd + 1).ToList();
            var weightMod = new WeightMod();
            weightMod.WEIGHT(modesOut.Z, modesOut.NTot, rd, Nrd, W, ird);

            if (modesOut.M > MaxM)
            {
                modesOut.M = MaxM;
            }

            if (modesOut.BCTop == "A")
            {
                kTop2 = Complex.Pow((2 * pi * modesOut.freqVec[1] / modesOut.cPT), 2);
            }

            if (modesOut.BCBot == "A")
            {
                kBot2 = Complex.Pow((2 * pi * modesOut.freqVec[1] / modesOut.cPB), 2);
            }

            var Tolerance = 1500 / modesOut.freqVec[1];
            for (var ir = 1; ir <= Nrd; ir++)
            {
                var iz = ird[ir];
                var WT = Math.Abs(Math.Min(W[ir], 1 - W[ir]));

                if (rd[ir] < modesOut.DepthT)
                {
                    if (modesOut.cST != 0 || modesOut.BCTop != "A")
                    {
                        warnings.Add($"Receiver depth: {rd[ir]}. Highest valid depth: {modesOut.DepthT}. Rcvr above depth of top.");
                    }
                }
                else if (rd[ir] > modesOut.DepthB)
                {
                    if (modesOut.cSB != 0 || modesOut.BCBot != "A")
                    {
                        warnings.Add($"Receiver depth: {rd[ir]}. Lowest valid depth: {modesOut.DepthB}. Rcvr below depth of top.");
                    }
                }
                else if (modesOut.NTot > 1)
                {
                    if (WT * (modesOut.Z[iz + 1] - modesOut.Z[iz]) > Tolerance)
                    {
                        warnings.Add($"Receiver depth: {rd[ir]}. Nearest depths: {modesOut.Z[iz]}, {modesOut.Z[iz+1]}. Modes not tabulated near requested pt.");
                    }
                }
                else
                {
                    if (Math.Abs(rd[iz] - modesOut.Z[iz]) > Tolerance)
                    {
                        warnings.Add($"Rd, Tabulation depth {rd[ir]}, {modesOut.Z[iz]}. Tolerance: {Tolerance}. Modes not tabulated near requested pt.");
                    }
                }
            }

            for (var Mode = 1; Mode <= modesOut.M; Mode++)
            {
                PhiR[Mode] = ReadOneMode(modesOut, W, ird, rd, Nrd, Mode, Comp);
            }

            return PhiR;
        }

        private List<Complex> ReadOneMode(ModesOut modesOut, List<double> W, List<int> ird, List<double> rd, int Nrd, int Mode, string Comp)
        {
            /* todo
              TufLuk = .FALSE. 
  IF ( ANY( Material( 1 : NMedia ) == 'ELASTIC' ) ) TufLuk = .TRUE.

  ! Extract the component specified by 'Comp'
  IF ( TufLuk ) CALL EXTRACT( Phi, N, Material, NMedia, Comp ) 
             */

            var TufLuk = false;
            if (modesOut.Material.Any(x => x == "ELASTIC"))
            {
                TufLuk = true;
            }

            if (TufLuk)
            {
                Extract(modesOut, Comp);
            }

            Complex gammaT = 0, gammaB = 0;

            if (modesOut.BCTop == "A")
            {
                var gamma2 = Math.Pow(modesOut.k[Mode].Real, 2) - kTop2;
                gammaT = PekerisRoot(gamma2);
            }

            if (modesOut.BCBot == "A")
            {
                var gamma2 = Math.Pow(modesOut.k[Mode].Real, 2) - kBot2;
                gammaB = PekerisRoot(gamma2);
            }
            //NRD CHECK
            var PhiR = Enumerable.Repeat(new Complex(0, 0), Nrd + 1).ToList();
            for (var ir = 1; ir <= Nrd; ir++)
            {
                if (rd[ir] < modesOut.DepthT)
                {
                    PhiR[ir] = modesOut.Phi[Mode][1] * Complex.Exp(-gammaT * (modesOut.DepthT - rd[ir]));
                }
                else if (rd[ir] > modesOut.DepthB)
                {
                    PhiR[ir] = modesOut.Phi[Mode][modesOut.NTot] * Complex.Exp(-gammaB * (rd[ir] - modesOut.DepthB));
                }
                else if (modesOut.NTot > 1)
                {
                    var iz = ird[ir];
                    PhiR[ir] = modesOut.Phi[Mode][iz] + W[ir] * (modesOut.Phi[Mode][iz + 1] - modesOut.Phi[Mode][iz]);
                }
                else
                {
                    var iz = ird[ir];
                    PhiR[ir] = modesOut.Phi[Mode][iz];
                }
            }

            return PhiR;
        }

        private void Extract(ModesOut modesOut, string Comp)
        {
            int i = 1, j = 1, k = 1;
            for (var Medium = 1; Medium <= modesOut.NMedia; Medium++)
            {
                if (modesOut.Material[Medium] == "ACOUSTIC")
                {
                    modesOut.Phi[j] = modesOut.Phi[k];
                    k = k + 1;
                }
                else if (modesOut.Material[Medium] == "ELASTIC")
                {
                    if (Comp == "H")
                    {
                        modesOut.Phi[j] = modesOut.Phi[k];
                    }
                    else if (Comp == "V")
                    {
                        modesOut.Phi[j] = modesOut.Phi[k + 1];
                    }
                    else if (Comp == "T")
                    {
                        modesOut.Phi[j] = modesOut.Phi[k + 2];
                    }
                    else if (Comp == "N")
                    {
                        modesOut.Phi[j] = modesOut.Phi[k + 3];
                    }

                    k += 4;
                }

                j++;
            }
        }

        private Complex PekerisRoot(Complex z)
        {
            if (z.Real > 0)
            {
                return Complex.Sqrt(z);
            }
            else
            {
                return new Complex(0, 1) * Complex.Sqrt(-z);
            }
        }
    }
}
