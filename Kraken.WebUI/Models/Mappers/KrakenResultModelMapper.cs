using Kraken.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenResultModelMapper
    {
        public KrakenResultModel MapNormalModes(KrakenComputingResult normalModes)
        {
            var resultModel = new KrakenResultModel
            {
                Alpha = normalModes.K.Select(x=>x.Imaginary),
                GroupSpeed = normalModes.GroupSpeed,
                PhaseSpeed = normalModes.PhaseSpeed,
                K = normalModes.K.Select(x=>x.Real)
            };

            var depthsCount = normalModes.ZM.Count;

            resultModel.ModesCount = normalModes.ModesCount;

            resultModel.Modes = normalModes.ZM.Select((x, idx) => new DepthModes { Depth = x, Modes = normalModes.Modes[idx] });

            resultModel.TransmissionLossCalculated = normalModes.TransmissionLossCalculated;
            if (normalModes.TransmissionLossCalculated)
            {
                resultModel.Ranges = normalModes.Ranges;
                resultModel.SourceDepths = normalModes.SourceDepths;
                resultModel.ReceiverDepths = normalModes.ReceiverDepths;

                var sourceDepthsCount = normalModes.SourceDepths.Count;
                var receiverDepthsCount = normalModes.ReceiverDepths.Count;
                var rangesCount = normalModes.Ranges.Count;

                resultModel.TransmissionLoss = new List<TLAtSourceDepth>(sourceDepthsCount);
                for (var i = 0; i < sourceDepthsCount; i++)
                {
                    var tlAtSource = new TLAtSourceDepth();
                    tlAtSource.SourceDepth = normalModes.SourceDepths[i];
                    tlAtSource.TLAtReceiverDepths = new List<TLAtReceiverDepth>(receiverDepthsCount);

                    for (var j = 0; j < receiverDepthsCount; j++)
                    {
                        var tlAtReceiver = new TLAtReceiverDepth();
                        tlAtReceiver.ReceiverDepth = normalModes.ReceiverDepths[j];
                        tlAtReceiver.TransmissionLoss = new List<double>(rangesCount);

                        for (var k = 0; k < rangesCount; k++)
                        {
                            tlAtReceiver.TransmissionLoss.Add(normalModes.TransmissionLoss[i][j][k]);
                        }

                        tlAtSource.TLAtReceiverDepths.Add(tlAtReceiver);
                    }

                    resultModel.TransmissionLoss.Add(tlAtSource);
                }
            }

            return resultModel;
        }
    }
}
