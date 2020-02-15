using Kraken.Application.Models;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenResultModelMapper
    {
        public KrakenResultModel MapNormalModes(NormalModes normalModes)
        {
            var resultModel = new KrakenResultModel
            {
                Alpha = normalModes.Alpha,
                GroupSpeed = normalModes.GroupSpeed,
                PhaseSpeed = normalModes.PhaseSpeed,
                K = normalModes.K,
                Modes = normalModes.Modes,
                ZM = normalModes.ZM
            };

            return resultModel;
        }
    }
}
