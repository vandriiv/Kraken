using Kraken.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenResultModelMapper
    {
        public KrakenResultModel MapKrakenComputingResult(KrakenComputingResult computingResult)
        {
            var resultModel = new KrakenResultModel();
            resultModel.Alpha.AddRange(computingResult.K.Select(x => x.Imaginary));
            resultModel.GroupSpeed.AddRange(computingResult.GroupSpeed);
            resultModel.PhaseSpeed.AddRange(computingResult.PhaseSpeed);
            resultModel.K.AddRange(computingResult.K.Select(x => x.Real));
         

            var depthsCount = computingResult.ZM.Count;

            resultModel.ModesCount = computingResult.ModesCount;

            resultModel.Modes.AddRange(computingResult.ZM.Select((x, idx) => new DepthModes(x, computingResult.Modes[idx])));

            resultModel.TransmissionLossCalculated = computingResult.TransmissionLossCalculated;
            if (computingResult.TransmissionLossCalculated)
            {
                resultModel.Ranges.AddRange(computingResult.Ranges);
                resultModel.SourceDepths.AddRange(computingResult.SourceDepths);
                resultModel.ReceiverDepths.AddRange(computingResult.ReceiverDepths);

                var sourceDepthsCount = computingResult.SourceDepths.Count;
                var receiverDepthsCount = computingResult.ReceiverDepths.Count;
                var rangesCount = computingResult.Ranges.Count;
               
                resultModel.TransmissionLoss.Capacity = sourceDepthsCount;
                for (var i = 0; i < sourceDepthsCount; i++)
                {
                    var tlAtSource = new TLAtSourceDepth();
                    tlAtSource.SourceDepth = computingResult.SourceDepths[i];

                    tlAtSource.TLAtReceiverDepths.Clear();
                    tlAtSource.TLAtReceiverDepths.Capacity = receiverDepthsCount;

                    for (var j = 0; j < receiverDepthsCount; j++)
                    {
                        var tlAtReceiver = new TLAtReceiverDepth();
                        tlAtReceiver.ReceiverDepth = computingResult.ReceiverDepths[j];

                        tlAtReceiver.TransmissionLoss.Clear();
                        tlAtReceiver.TransmissionLoss.Capacity = rangesCount;

                        for (var k = 0; k < rangesCount; k++)
                        {
                            tlAtReceiver.TransmissionLoss.Add(computingResult.TransmissionLoss[i][j][k]);
                        }

                        tlAtSource.TLAtReceiverDepths.Add(tlAtReceiver);
                    }

                    resultModel.TransmissionLoss.Add(tlAtSource);
                }
            }

            resultModel.Warnings.AddRange(computingResult.Warnings);

            return resultModel;
        }
    }
}
