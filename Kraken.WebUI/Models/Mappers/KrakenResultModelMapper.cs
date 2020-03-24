using Kraken.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenResultModelMapper
    {
        public KrakenResultModel MapKrakenComputingResult(KrakenComputingResult computingResult)
        {
            var resultModel = new KrakenResultModel
            {
                Alpha = computingResult.K.Select(x=>x.Imaginary),
                GroupSpeed = computingResult.GroupSpeed,
                PhaseSpeed = computingResult.PhaseSpeed,
                K = computingResult.K.Select(x=>x.Real)
            };

            var depthsCount = computingResult.ZM.Count;

            resultModel.ModesCount = computingResult.ModesCount;

            resultModel.Modes = computingResult.ZM.Select((x, idx) => new DepthModes { Depth = x, Modes = computingResult.Modes[idx] });

            resultModel.TransmissionLossCalculated = computingResult.TransmissionLossCalculated;
            if (computingResult.TransmissionLossCalculated)
            {
                resultModel.Ranges = computingResult.Ranges;
                resultModel.SourceDepths = computingResult.SourceDepths;
                resultModel.ReceiverDepths = computingResult.ReceiverDepths;

                var sourceDepthsCount = computingResult.SourceDepths.Count;
                var receiverDepthsCount = computingResult.ReceiverDepths.Count;
                var rangesCount = computingResult.Ranges.Count;

                resultModel.TransmissionLoss = new List<TLAtSourceDepth>(sourceDepthsCount);
                for (var i = 0; i < sourceDepthsCount; i++)
                {
                    var tlAtSource = new TLAtSourceDepth();
                    tlAtSource.SourceDepth = computingResult.SourceDepths[i];
                    tlAtSource.TLAtReceiverDepths = new List<TLAtReceiverDepth>(receiverDepthsCount);

                    for (var j = 0; j < receiverDepthsCount; j++)
                    {
                        var tlAtReceiver = new TLAtReceiverDepth();
                        tlAtReceiver.ReceiverDepth = computingResult.ReceiverDepths[j];
                        tlAtReceiver.TransmissionLoss = new List<double>(rangesCount);

                        for (var k = 0; k < rangesCount; k++)
                        {
                            tlAtReceiver.TransmissionLoss.Add(computingResult.TransmissionLoss[i][j][k]);
                        }

                        tlAtSource.TLAtReceiverDepths.Add(tlAtReceiver);
                    }

                    resultModel.TransmissionLoss.Add(tlAtSource);
                }
            }

            resultModel.Warnings = computingResult.Warnings;

            return resultModel;
        }
    }
}
