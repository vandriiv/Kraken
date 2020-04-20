using Kraken.Common.Mappers;
using Kraken.Application.Models;
using System.Linq;

namespace Kraken.WebUI.Models.Mappers
{
    public class KrakenInputModelMapper : IMapper<KrakenInputModel, AcousticProblemData>
    {
        public AcousticProblemData Map(KrakenInputModel source)
        {
            var acousticProblemData = new AcousticProblemData
            {
                Frequency = source.Frequency,
                NModes = source.NModes,
                NMedia = source.NMedia,
                TopBCType = source.TopBCType,
                InterpolationType = source.InterpolationType,
                AttenuationUnits = source.AttenuationUnits,
                AddedVolumeAttenuation = source.AddedVolumeAttenuation,

                ZT = source.ZT,
                CPT = source.CPT,
                CST = source.CST,
                RHOT = source.RHOT,
                APT = source.APT,
                AST = source.AST,

                BumDen = source.BumDen,
                Eta = source.Eta,
                Xi = source.Xi,

                BottomBCType = source.BottomBCType,
                Sigma = source.Sigma,

                ZB = source.ZB,
                CPB = source.CPB,
                CSB = source.CSB,
                RHOB = source.RHOB,
                APB = source.APB,
                ASB = source.ASB,

                CLow = source.CLow,
                CHigh = source.CHigh,

                RMax = source.RMax,

                NSD = source.NSD,

                NRD = source.NRD,

                CalculateTransmissionLoss = source.CalculateTransmissionLoss,
                NModesForField = source.NModesForField,
                SourceType = source.SourceType,
                ModesTheory = source.ModesTheory,
                NR = source.NR,
                NSDField = source.NSDField,
                NRDField = source.NRDField,
                NRR = source.NRR
            };


            foreach (var m in source.MediumInfo)
            {
                acousticProblemData.MediumInfo.Add(m.ToList());
            }

            foreach (var ssp in source.SSP)
            {
                acousticProblemData.SSP.Add(ssp.ToList());
            }

            acousticProblemData.SD.AddRange(source.SD);
            acousticProblemData.RD.AddRange(source.RD);

            if (acousticProblemData.CalculateTransmissionLoss)
            {
                acousticProblemData.SDField.AddRange(source.SDField);
                acousticProblemData.RDField.AddRange(source.RDField);
                acousticProblemData.R.AddRange(source.R);
                acousticProblemData.RR.AddRange(source.RR);
            }

            return acousticProblemData;
        }       
    }
}
