using Kraken.Application.Models;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenInputModelMapper
    {
        public AcousticProblemData MapAcousticProblemData(KrakenInputModel model)
        {
            var acousticProblemData = new AcousticProblemData
            {
                Frequency = model.Frequency,
                NModes = model.NModes,
                NMedia = model.NMedia,
                TopBCType = model.TopBCType,
                InterpolationType = model.InterpolationType,
                AttenuationUnits = model.AttenuationUnits,
                AddedVolumeAttenuation = model.AddedVolumeAttenuation,

                ZT = model.ZT,
                CPT = model.CPT,
                CST = model.CST,
                RHOT = model.RHOT,
                APT = model.APT,
                AST = model.AST,

                BumDen = model.BumDen,
                Eta = model.Eta,
                Xi = model.Xi,

                MediumInfo = model.MediumInfo,
                SSP = model.SSP,

                BottomBCType = model.BottomBCType,
                Sigma = model.Sigma,

                ZB = model.ZB,
                CPB = model.CPB,
                CSB = model.CSB,
                RHOB = model.RHOB,
                APB = model.APB,
                ASB = model.ASB,

                CLow = model.CLow,
                CHigh = model.CHigh,

                RMax = model.RMax,

                NSD = model.NSD,
                SD = model.SD,

                NRD = model.NRD,
                RD = model.RD, 

                CalculateTransmissionLoss = model.CalculateTransmissionLoss,
                NModesForField = model.NModesForField,
                SourceType = model.SourceType,
                ModesTheory = model.ModesTheory,              
                NR = model.NR,
                R = model.R,
                NSDField = model.NSDField,
                SDField = model.SDField,
                NRDField = model.NRDField,
                RDField = model.RDField,
                NRR = model.NRR,
                RR = model.RR
            };

            return acousticProblemData;
        }
    }
}
