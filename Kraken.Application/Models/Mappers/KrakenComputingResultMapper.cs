﻿using Kraken.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Application.Models.Mappers
{
    public class KrakenComputingResultMapper
    {
        public KrakenComputingResult MapFromKrakenResult(KrakenResult krakenResult)
        {
            var result = new KrakenComputingResult();

            MapKrakenOnlyProperties(result, krakenResult);

            result.TransmissionLossCalculated = false;

            return result;
        }

        public KrakenComputingResult MapFromKrakenAndFieldResult(KrakenResult krakenResult, AcousticFieldSnapshots fieldResult)
        {
            var result = new KrakenComputingResult();

            MapKrakenOnlyProperties(result, krakenResult);

            result.TransmissionLossCalculated = true;

            result.SourceDepths = fieldResult.SourceDepths;
            result.ReceiverDepths = fieldResult.ReceiverDepths;
            result.Ranges = fieldResult.Ranges;           

            if (result.SourceDepths.Count > 3 && result.SourceDepths[3] == -999.9)
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

            if (result.Ranges.Count > 3 && result.Ranges[3] == -999.9)
            {
                result.Ranges = new List<double>() { result.Ranges[1] };
            }
            else
            {
                result.Ranges.RemoveAt(0);
            }

            result.Warnings.AddRange(fieldResult.Warnings);

            return result;
        }

        private void MapKrakenOnlyProperties(KrakenComputingResult result, KrakenResult krakenResult)
        {
            result.GroupSpeed = krakenResult.GroupSpeed;
            result.PhaseSpeed = krakenResult.PhaseSpeed;
            result.K = krakenResult.K;
            result.ModesCount = krakenResult.ModesCount;
            result.Modes = krakenResult.Modes;
            result.ZM = krakenResult.ZM;
            
            result.Warnings.AddRange(krakenResult.Warnings);

            result.GroupSpeed.RemoveAt(0);
            result.PhaseSpeed.RemoveAt(0);
            result.K.RemoveAt(0);
            result.ZM.RemoveAt(0);

            result.Modes.RemoveAt(0);
            foreach (var modes in result.Modes)
            {
                modes.RemoveAt(0);
            }
        }
    }
}