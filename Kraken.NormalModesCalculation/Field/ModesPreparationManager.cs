using Kraken.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation.Field
{
    public class ModesPreparationManager
    {
        private Complex kTop2;
        private Complex kBot2;      

        public List<List<Complex>> GetPreparedModes(CalculatedModesInfo modesInfo, int MaxM, List<double> receiverDepths, int Nrd, string Comp, List<string> warnings)
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
                kTop2 = Complex.Pow((2 * Math.PI * modesInfo.Frequency / modesInfo.CPTop), 2);
            }

            if (modesInfo.BCBottom == "A")
            {
                kBot2 = Complex.Pow((2 * Math.PI * modesInfo.Frequency / modesInfo.CPBottom), 2);
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

            for (var mode = 1; mode <= modesInfo.ModesCount; mode++)
            {
                PhiR[mode] = PrepareOneMode(modesInfo, W, ird, receiverDepths, Nrd, mode, Comp);
            }

            return PhiR;
        }

        private List<Complex> PrepareOneMode(CalculatedModesInfo modesInfo, List<double> W, List<int> ird, List<double> receiverDepths, int Nrd, int mode, string Comp)
        {

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
                var gamma2 = Math.Pow(modesInfo.K[mode].Real, 2) - kTop2;
                gammaT = PekerisRoot(gamma2);
            }

            if (modesInfo.BCBottom == "A")
            {
                var gamma2 = Math.Pow(modesInfo.K[mode].Real, 2) - kBot2;
                gammaB = PekerisRoot(gamma2);
            }
            
            var PhiR = Enumerable.Repeat(new Complex(0, 0), Nrd + 1).ToList();
            for (var ir = 1; ir <= Nrd; ir++)
            {
                if (receiverDepths[ir] < modesInfo.DepthTop)
                {
                    PhiR[ir] = modesInfo.Phi[mode][1] * Complex.Exp(-gammaT * (modesInfo.DepthTop - receiverDepths[ir]));
                }
                else if (receiverDepths[ir] > modesInfo.DepthBottom)
                {
                    PhiR[ir] = modesInfo.Phi[mode][modesInfo.NTot] * Complex.Exp(-gammaB * (receiverDepths[ir] - modesInfo.DepthBottom));
                }
                else if (modesInfo.NTot > 1)
                {
                    var iz = ird[ir];
                    PhiR[ir] = modesInfo.Phi[mode][iz] + W[ir] * (modesInfo.Phi[mode][iz + 1] - modesInfo.Phi[mode][iz]);
                }
                else
                {
                    var iz = ird[ir];
                    PhiR[ir] = modesInfo.Phi[mode][iz];
                }
            }

            return PhiR;
        }

        private void Extract(CalculatedModesInfo modesInfo, string Comp)
        {
            int j = 1, k = 1;
            for (var Medium = 1; Medium <= modesInfo.NMedia; Medium++)
            {
                for (var i = 1; i < modesInfo.N[Medium] + 1; i++)
                {
                    if (modesInfo.Material[Medium] == "ACOUSTIC")
                    {
                        modesInfo.Phi[j] = modesInfo.Phi[k];
                        k += 1;
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
