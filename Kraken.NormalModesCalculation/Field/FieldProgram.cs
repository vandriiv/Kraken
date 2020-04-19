using Kraken.Calculation.Field.Interfaces;
using Kraken.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation.Field
{
    public class FieldProgram : IFieldProgram
    {
        public AcousticFieldSnapshots CalculateFieldPressure(FieldInputData fieldData)
        {
            var maxM = Math.Min(fieldData.ModesLimit, fieldData.ModesInfo.ModesCount);

            string comp = "";
            if (fieldData.Options.Length > 2)
            {
                comp = fieldData.Options[2].ToString();
            }

            var rangedDataManager = new RangedDataManager();
                  
            rangedDataManager.ProceedReceiverRanges(fieldData.Nr, fieldData.ReceiverRanges);

            var zMin = -3.40282347E+38;
            var zMax = 3.40282347E+38;

            rangedDataManager.ProceedSourceAndReceiverDepths(zMin, zMax, fieldData.Nsd, fieldData.Nrd, fieldData.SourceDepths, fieldData.ReceiverDepths);

            var result = new AcousticFieldSnapshots();

            result.Ranges = new List<double>(rangedDataManager.ReceiverRanges);
            result.SourceDepths = new List<double>(rangedDataManager.SourceDepths);
            result.ReceiverDepths = new List<double>(rangedDataManager.ReceiverDepths);

            var C = Enumerable.Repeat(new Complex(), maxM + 1).ToList();

            var receiverDisplacements = Enumerable.Repeat(0d, fieldData.Nrr + 1).ToList();

            var Nrr = fieldData.Nrr;

            if (fieldData.Nrr != rangedDataManager.Nrd)
            {
                Nrr = rangedDataManager.Nrd;
                receiverDisplacements = Enumerable.Repeat(0d, Nrr + 1).ToList();
            }

            for (var i = 0; i < fieldData.ReceiverDisplacements.Count; i++)
            {
                receiverDisplacements[i] = fieldData.ReceiverDisplacements[i];
            }

            if (Nrr > 1)
            {
                receiverDisplacements[2] = -999.9;
            }
            if (Nrr > 2)
            {
                receiverDisplacements[3] = -999.9;
            }

            var subTabMod = new SubTabulator();

            subTabMod.SubTabulate(receiverDisplacements, Nrr);

            var readModesMod = new ModesPreparationManager();

            var phiS = readModesMod.GetPreparedModes(fieldData.ModesInfo, maxM, rangedDataManager.SourceDepths, rangedDataManager.Nsd, "N", result.Warnings);
            var phiR = readModesMod.GetPreparedModes(fieldData.ModesInfo, maxM, rangedDataManager.ReceiverDepths, rangedDataManager.Nrd, comp, result.Warnings);
            var pressureFieldCalculator = new PressueFieldCalculator();
            
            result.Snapshots = new List<List<List<Complex>>>();
            result.Snapshots.Add(new List<List<Complex>>());

            for (var IS = 1; IS <= rangedDataManager.Nsd; IS++)
            {
                for (var i = 1; i <= fieldData.ModesInfo.ModesCount; i++)
                {
                    C[i] = phiS[i][IS];
                }

                var P = pressureFieldCalculator.Evaluate(C, phiR, rangedDataManager.Nrd, rangedDataManager.ReceiverRanges, 
                                            rangedDataManager.Nr, receiverDisplacements, fieldData.ModesInfo.K, fieldData.ModesInfo.ModesCount, fieldData.Options);
                result.Snapshots.Add(P);
            }

            return result;
        }
    }
}
