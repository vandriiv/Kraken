using Kraken.Application.Models;
using System.Linq;

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

                NRD = model.NRD,            

                CalculateTransmissionLoss = model.CalculateTransmissionLoss,
                NModesForField = model.NModesForField,
                SourceType = model.SourceType,
                ModesTheory = model.ModesTheory,              
                NR = model.NR,               
                NSDField = model.NSDField,               
                NRDField = model.NRDField,               
                NRR = model.NRR               
            };

            
            foreach(var m in model.MediumInfo)
            {
                acousticProblemData.MediumInfo.Add(m.ToList());
            }

            foreach(var ssp in model.SSP)
            {
                acousticProblemData.SSP.Add(ssp.ToList());
            }           

            acousticProblemData.SD.AddRange(model.SD);
            acousticProblemData.RD.AddRange(model.RD);

            if (acousticProblemData.CalculateTransmissionLoss)
            {
                acousticProblemData.SDField.AddRange(model.SDField);
                acousticProblemData.RDField.AddRange(model.RDField);
                acousticProblemData.R.AddRange(model.R);
                acousticProblemData.RR.AddRange(model.RR);
            }

            return acousticProblemData;
        }
    }
}
