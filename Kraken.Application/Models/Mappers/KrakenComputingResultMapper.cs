using Kraken.Common.Mappers;
using Kraken.Calculation.Models;
using System.Collections.Generic;

namespace Kraken.Application.Models.Mappers
{
    public class KrakenComputingResultMapper : IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult>
    {
        public KrakenComputingResult Map(KrakenResultAndAcousticFieldSnapshots source)
        {
            var result = new KrakenComputingResult();

            MapKrakenOnlyProperties(result, source.KrakenResult);

            if (source.AcousticFieldSnapshots != null)
            {
                result.TransmissionLossCalculated = true;

                result.SourceDepths.AddRange(source.AcousticFieldSnapshots.SourceDepths);
                result.ReceiverDepths.AddRange(source.AcousticFieldSnapshots.ReceiverDepths);
                result.Ranges.AddRange(source.AcousticFieldSnapshots.Ranges);

                if (result.SourceDepths.Count > 3 && result.SourceDepths[3] == -999.9)
                {
                    var val = result.SourceDepths[1];
                    result.SourceDepths.Clear();
                    result.SourceDepths.AddRange(new List<double>() { val });
                }
                else
                {
                    result.SourceDepths.RemoveAt(0);
                }

                if (result.ReceiverDepths.Count > 3 && result.ReceiverDepths[3] == -999.9)
                {
                    var val = result.ReceiverDepths[1];
                    result.ReceiverDepths.Clear();
                    result.ReceiverDepths.AddRange(new List<double>() { val });
                }
                else
                {
                    result.ReceiverDepths.RemoveAt(0);
                }

                if (result.Ranges.Count > 3 && result.Ranges[3] == -999.9)
                {
                    var val = result.Ranges[1];
                    result.Ranges.Clear();
                    result.Ranges.AddRange(new List<double>() { val });
                }
                else
                {
                    result.Ranges.RemoveAt(0);
                }

                result.TransmissionLoss.AddRange(source.TransmissionLoss);

                result.Warnings.AddRange(source.AcousticFieldSnapshots.Warnings);
            }
            else
            {
                result.TransmissionLossCalculated = false;
            }

            return result;
        }       

        private void MapKrakenOnlyProperties(KrakenComputingResult result, KrakenResult krakenResult)
        {
            result.GroupSpeed.AddRange(krakenResult.GroupSpeed);
            result.PhaseSpeed.AddRange(krakenResult.PhaseSpeed);
            result.K.AddRange(krakenResult.K);
            result.ModesCount = krakenResult.ModesCount;
            result.Modes.AddRange(krakenResult.Modes);
            result.ZM.AddRange(krakenResult.ZM);
            
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
