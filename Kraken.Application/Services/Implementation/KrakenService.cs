using Kraken.Application.Exceptions;
using Kraken.Application.Models;
using Kraken.Application.Services.Interfaces;
using Kraken.Calculation;
using Kraken.Calculation.Exceptions;
using Kraken.Calculation.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Application.Services.Implementation
{
    public class KrakenService : IKrakenService
    {
        private readonly KrakenNormalModesProgram _krakenNormalModeProgram;
        private readonly FieldModel _fieldModel; 

        public KrakenService(KrakenNormalModesProgram krakenNormalModeProgram, FieldModel fieldModel)
        {
            _krakenNormalModeProgram = krakenNormalModeProgram;
            _fieldModel = fieldModel;
        }

        public KrakenComputingResult ComputeModes(AcousticProblemData acousticProblemData)
        {          
            var result = new KrakenComputingResult();

            var options = acousticProblemData.InterpolationType  + acousticProblemData.TopBCType + acousticProblemData.AttenuationUnits + acousticProblemData.AddedVolumeAttenuation;
            var bottomBC = acousticProblemData.BottomBCType;

            var mediumInfo = new List<List<double>>(acousticProblemData.MediumInfo);
            mediumInfo.Insert(0, new List<double>());
            foreach(var list in mediumInfo)
            {
                list.Insert(0, 0);
            }

            var ssp = new List<List<double>>(acousticProblemData.SSP);
            ssp.Insert(0, new List<double>());
            foreach (var list in ssp)
            {
                list.Insert(0, 0);
            }

            var sd = new List<double>(acousticProblemData.SD);
            sd.Insert(0, 0);

            var rd = new List<double>(acousticProblemData.RD);
            rd.Insert(0, 0);

            var cLowHight = new List<double> { 0, acousticProblemData.CLow, acousticProblemData.CHigh };

            var nz = acousticProblemData.NSD + acousticProblemData.NRD;

            var topAHSP = new List<double> { 0, acousticProblemData.ZT, acousticProblemData.CPT, acousticProblemData.CST,
                                            acousticProblemData.RHOT,acousticProblemData.APT, acousticProblemData.AST};

            var twerskyParams = new List<double> { 0, acousticProblemData.BumDen, acousticProblemData.Eta, acousticProblemData.Xi };
            var bottomAHSP = new List<double> { 0, acousticProblemData.ZB, acousticProblemData.CPB, acousticProblemData.CSB,
                                            acousticProblemData.RHOB,acousticProblemData.APB, acousticProblemData.ASB};

            var cg = new List<double>();
            var cp = new List<double>();
            var k = new List<Complex>();
            var zm = new List<double>();
            var modes = new List<List<double>>();

            var warnings = new List<string>();

            CalculatedModesInfo modesInfo;
            try
            {
                 modesInfo = _krakenNormalModeProgram.OceanAcousticNormalModes(acousticProblemData.NModes, acousticProblemData.Frequency, acousticProblemData.NMedia, options,
                    mediumInfo, ssp.Count, ssp, bottomBC, acousticProblemData.Sigma, cLowHight, acousticProblemData.RMax, acousticProblemData.NSD, sd, acousticProblemData.NRD,
                    rd, nz, topAHSP, twerskyParams, bottomAHSP, ref cg, ref cp, ref zm, ref modes, ref k, warnings);
            }
            catch(KrakenException ex)
            {
                throw new KrakenComputingException(ex.Message);
            }

            result.PhaseSpeed = cp; 
            result.GroupSpeed = cg;
            result.K = k;
            result.Modes = modes;
            result.ZM = zm;
            result.ModesCount = modesInfo.ModesCount;

            result.PhaseSpeed.RemoveAt(0);
            result.GroupSpeed.RemoveAt(0);
            result.K.RemoveAt(0);
            result.ZM.RemoveAt(0);

            result.Modes.RemoveAt(0);
            foreach(var mode in result.Modes)
            {
                mode.RemoveAt(0);
            }           

            if (acousticProblemData.CalculateTransmissionLoss)
            {
                var fieldOptions = acousticProblemData.SourceType + acousticProblemData.ModesTheory;              

                var r = new List<double>(acousticProblemData.R);
                r.Insert(0, 0);

                var rr = new List<double>(acousticProblemData.RR);
                rr.Insert(0, 0);

                var sdField = new List<double>(acousticProblemData.SDField);
                sdField.Insert(0, 0);

                var rdField = new List<double>(acousticProblemData.RDField);
                rdField.Insert(0, 0);

                var ranges = new List<double>();
                var sourceDepths = new List<double>();
                var receiverDepths = new List<double>();
                var fieldPressure = new List<List<List<Complex>>>();

                _fieldModel.CalculateFieldPressure(modesInfo, fieldOptions, acousticProblemData.NModesForField, acousticProblemData.NR, r, 
                                                    acousticProblemData.NSDField, sdField, acousticProblemData.NRDField,
                                                    rdField, acousticProblemData.NRR, rr, ref ranges, ref sourceDepths, ref receiverDepths,
                                                    ref fieldPressure, warnings);

                var transmissionLoss = fieldPressure.GetRange(1, fieldPressure.Count - 1)
                                       .Select(x => x.GetRange(1, x.Count - 1)
                                       .Select(y => y.GetRange(1, y.Count - 1)
                                       .Select(z => z.Real == 0 ? 1E-6 : z.Real)
                                       .Select(z => -20 * Math.Log10(Math.Abs(z))).ToList()).ToList()).ToList();

                result.TransmissionLossCalculated = true;
                result.TransmissionLoss = transmissionLoss;
                result.SourceDepths = sourceDepths;
                result.ReceiverDepths = receiverDepths;
                result.Ranges = ranges;

                result.Ranges.RemoveAt(0);
                if(result.SourceDepths.Count>3 && result.SourceDepths[3] == -999.9)
                {
                    result.SourceDepths = new List<double>() { result.SourceDepths[1] };
                }
                else
                {
                    result.SourceDepths.RemoveAt(0);
                }

                if (result.ReceiverDepths.Count > 3 && result.ReceiverDepths[3] == -999.9)
                {
                    result.ReceiverDepths = new List<double>() { result.ReceiverDepths[1] };
                }
                else
                {
                    result.ReceiverDepths.RemoveAt(0);
                }
            }

            result.Warnings = warnings;

            return result;
        }
    }
}
