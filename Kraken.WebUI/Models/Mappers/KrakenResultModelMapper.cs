using Kraken.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenResultModelMapper
    {
        public KrakenResultModel MapNormalModes(NormalModes normalModes)
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

            return resultModel;
        }
    }
}
