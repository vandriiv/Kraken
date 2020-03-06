using Kraken.Application.Models;
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

            resultModel.Modes = normalModes.ZM.Select((x, idx) => new DepthModes { Depth = x, Modes = normalModes.Modes[idx] });

            resultModel.TransmissionLossCalculated = normalModes.TransmissionLossCalculated;
            resultModel.TransmissionLoss = normalModes.TransmissionLoss;
            resultModel.Ranges = normalModes.Ranges;
            resultModel.SourceDepths = normalModes.SourceDepths;

            return resultModel;
        }
    }
}
