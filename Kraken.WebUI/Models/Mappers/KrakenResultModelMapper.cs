using Kraken.Common.Mappers;
using Kraken.Application.Models;
using System.Linq;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenResultModelMapper : IMapper<KrakenComputingResult, KrakenResultModel>
    {
        public KrakenResultModel Map(KrakenComputingResult source)
        {
            var resultModel = new KrakenResultModel();
            resultModel.Alpha.AddRange(source.K.Select(x => x.Imaginary));
            resultModel.GroupSpeed.AddRange(source.GroupSpeed);
            resultModel.PhaseSpeed.AddRange(source.PhaseSpeed);
            resultModel.K.AddRange(source.K.Select(x => x.Real));

            var depthsCount = source.ZM.Count;

            resultModel.ModesCount = source.ModesCount;

            resultModel.Modes.AddRange(source.ZM.Select((x, idx) => new DepthModes(x, source.Modes[idx])));

            resultModel.TransmissionLossCalculated = source.TransmissionLossCalculated;
            if (source.TransmissionLossCalculated)
            {
                resultModel.Ranges.AddRange(source.Ranges);
                resultModel.SourceDepths.AddRange(source.SourceDepths);
                resultModel.ReceiverDepths.AddRange(source.ReceiverDepths);

                var sourceDepthsCount = source.SourceDepths.Count;
                var receiverDepthsCount = source.ReceiverDepths.Count;
                var rangesCount = source.Ranges.Count;

                resultModel.TransmissionLoss.Capacity = sourceDepthsCount;
                for (var i = 0; i < sourceDepthsCount; i++)
                {
                    var tlAtSource = new TLAtSourceDepth();
                    tlAtSource.SourceDepth = source.SourceDepths[i];

                    tlAtSource.TLAtReceiverDepths.Clear();
                    tlAtSource.TLAtReceiverDepths.Capacity = receiverDepthsCount;

                    for (var j = 0; j < receiverDepthsCount; j++)
                    {
                        var tlAtReceiver = new TLAtReceiverDepth();
                        tlAtReceiver.ReceiverDepth = source.ReceiverDepths[j];

                        tlAtReceiver.TransmissionLoss.Clear();
                        tlAtReceiver.TransmissionLoss.Capacity = rangesCount;

                        for (var k = 0; k < rangesCount; k++)
                        {
                            tlAtReceiver.TransmissionLoss.Add(source.TransmissionLoss[i][j][k]);
                        }

                        tlAtSource.TLAtReceiverDepths.Add(tlAtReceiver);
                    }

                    resultModel.TransmissionLoss.Add(tlAtSource);
                }
            }

            resultModel.Warnings.AddRange(source.Warnings);

            return resultModel;
        }
    }
}
