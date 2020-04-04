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

        public List<List<Complex>> GetModes(CalculatedModesInfo modesInfo, int MaxM, List<double> receiverDepths, int Nrd, string Comp, List<string> warnings)
        {
            var PhiR = new List<List<Complex>>(MaxM + 1);
            for (var i = 0; i <= MaxM; i++)
            {
                PhiR.Add(Enumerable.Repeat(new Complex(), Nrd + 1).ToList());
            }

            var weightsCalculator = new WeightsCalculator();
            var (W, ird) = weightsCalculator.CalculateWeightsAndIndices(modesInfo.Z, modesInfo.NTot, receiverDepths, Nrd);          

            if (modesInfo.ModesCount > MaxM)
            {
                modesInfo.ModesCount = MaxM;
            }

            if (modesInfo.BCTop == "A")
            {
                kTop2 = Complex.Pow((2 * pi * modesInfo.Frequency / modesInfo.CPTop), 2);
            }

            if (modesInfo.BCBottom == "A")
            {
                kBot2 = Complex.Pow((2 * pi * modesInfo.Frequency / modesInfo.CPBottom), 2);
            }

            var Tolerance = 1500 / modesInfo.Frequency;
            for (var ir = 1; ir <= Nrd; ir++)
            {
                var iz = ird[ir];
                var WT = Math.Abs(Math.Min(W[ir], 1 - W[ir]));

                if (receiverDepths[ir] < modesInfo.DepthTop)
                {
                    if (modesInfo.CSTop != 0 || modesInfo.BCTop != "A")
                    {
                        warnings.Add($"Receiver depth: {receiverDepths[ir]}. Highest valid depth: {modesInfo.DepthTop}. Rcvr above depth of top.");
                    }
                }
                else if (receiverDepths[ir] > modesInfo.DepthBottom)
                {
                    if (modesInfo.CSBottom != 0 || modesInfo.BCBottom != "A")
                    {
                        warnings.Add($"Receiver depth: {receiverDepths[ir]}. Lowest valid depth: {modesInfo.DepthBottom}. Rcvr below depth of top.");
                    }
                }
                else if (modesInfo.NTot > 1)
                {
                    if (WT * (modesInfo.Z[iz + 1] - modesInfo.Z[iz]) > Tolerance)
                    {
                        warnings.Add($"Receiver depth: {receiverDepths[ir]}. Nearest depths: {modesInfo.Z[iz]}, {modesInfo.Z[iz+1]}. Modes not tabulated near requested pt.");
                    }
                }
                else
                {
                    if (Math.Abs(receiverDepths[iz] - modesInfo.Z[iz]) > Tolerance)
                    {
                        warnings.Add($"Rd, Tabulation depth {receiverDepths[ir]}, {modesInfo.Z[iz]}. Tolerance: {Tolerance}. Modes not tabulated near requested pt.");
                    }
                }
            }

            for (var Mode = 1; Mode <= modesInfo.ModesCount; Mode++)
            {
                PhiR[Mode] = ReadOneMode(modesInfo, W, ird, receiverDepths, Nrd, Mode, Comp);
            }

            return PhiR;
        }

        private List<Complex> ReadOneMode(CalculatedModesInfo modesInfo, List<double> W, List<int> ird, List<double> receiverDepths, int Nrd, int Mode, string Comp)
        {
            /* todo
              TufLuk = .FALSE. 
  IF ( ANY( Material( 1 : NMedia ) == 'ELASTIC' ) ) TufLuk = .TRUE.

  ! Extract the component specified by 'Comp'
  IF ( TufLuk ) CALL EXTRACT( Phi, N, Material, NMedia, Comp ) 
             */

            var TufLuk = false;
            if (modesInfo.Material.Any(x => x == "ELASTIC"))
            {
                TufLuk = true;
            }

            if (TufLuk)
            {
                Extract(modesInfo, Comp);
            }

            Complex gammaT = 0, gammaB = 0;

            if (modesInfo.BCTop == "A")
            {
                var gamma2 = Math.Pow(modesInfo.K[Mode].Real, 2) - kTop2;
                gammaT = PekerisRoot(gamma2);
            }

            if (modesInfo.BCBottom == "A")
            {
                var gamma2 = Math.Pow(modesInfo.K[Mode].Real, 2) - kBot2;
                gammaB = PekerisRoot(gamma2);
            }
            //NRD CHECK
            var PhiR = Enumerable.Repeat(new Complex(0, 0), Nrd + 1).ToList();
            for (var ir = 1; ir <= Nrd; ir++)
            {
                if (receiverDepths[ir] < modesInfo.DepthTop)
                {
                    PhiR[ir] = modesInfo.Phi[Mode][1] * Complex.Exp(-gammaT * (modesInfo.DepthTop - receiverDepths[ir]));
                }
                else if (receiverDepths[ir] > modesInfo.DepthBottom)
                {
                    PhiR[ir] = modesInfo.Phi[Mode][modesInfo.NTot] * Complex.Exp(-gammaB * (receiverDepths[ir] - modesInfo.DepthBottom));
                }
                else if (modesInfo.NTot > 1)
                {
                    var iz = ird[ir];
                    PhiR[ir] = modesInfo.Phi[Mode][iz] + W[ir] * (modesInfo.Phi[Mode][iz + 1] - modesInfo.Phi[Mode][iz]);
                }
                else
                {
                    var iz = ird[ir];
                    PhiR[ir] = modesInfo.Phi[Mode][iz];
                }
            }

            return PhiR;
        }

        private void Extract(CalculatedModesInfo modesInfo, string Comp)
        {
            int i = 1, j = 1, k = 1;
            for (var Medium = 1; Medium <= modesInfo.NMedia; Medium++)
            {
                if (modesInfo.Material[Medium] == "ACOUSTIC")
                {
                    modesInfo.Phi[j] = modesInfo.Phi[k];
                    k = k + 1;
                }
                else if (modesInfo.Material[Medium] == "ELASTIC")
                {
                    if (Comp == "H")
                    {
                        modesInfo.Phi[j] = modesInfo.Phi[k];
                    }
                    else if (Comp == "V")
                    {
                        modesInfo.Phi[j] = modesInfo.Phi[k + 1];
                    }
                    else if (Comp == "T")
                    {
                        modesInfo.Phi[j] = modesInfo.Phi[k + 2];
                    }
                    else if (Comp == "N")
                    {
                        modesInfo.Phi[j] = modesInfo.Phi[k + 3];
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
