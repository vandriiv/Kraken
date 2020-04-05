using Kraken.Calculation.Models;
using System.Collections.Generic;

namespace Kraken.Application.Models.Mappers
{
    public class AcousticProblemDataMapper
    {
        public KrakenInputProfile MapKrakenInputProfile(AcousticProblemData acousticProblemData)
        {
            var options = acousticProblemData.InterpolationType + acousticProblemData.TopBCType + acousticProblemData.AttenuationUnits + acousticProblemData.AddedVolumeAttenuation;
            var bcBottom = acousticProblemData.BottomBCType;

            var mediumInfo = new List<List<double>>(acousticProblemData.MediumInfo);
            mediumInfo.Insert(0, new List<double>());
            foreach (var list in mediumInfo)
            {
                list.Insert(0, 0);
            }

            var ssp = new List<List<double>>(acousticProblemData.SSP);
            ssp.Insert(0, new List<double>());
            foreach (var list in ssp)
            {
                list.Insert(0, 0);
            }

            var sd = new List<double>(acousticProblemData.SD);
            sd.Insert(0, 0);

            var rd = new List<double>(acousticProblemData.RD);
            rd.Insert(0, 0);

            var topAHSP = new List<double> { 0, acousticProblemData.ZT, acousticProblemData.CPT, acousticProblemData.CST,
                                            acousticProblemData.RHOT,acousticProblemData.APT, acousticProblemData.AST};

            var twerskyParams = new List<double> { 0, acousticProblemData.BumDen, acousticProblemData.Eta, acousticProblemData.Xi };
            var bottomAHSP = new List<double> { 0, acousticProblemData.ZB, acousticProblemData.CPB, acousticProblemData.CSB,
                                            acousticProblemData.RHOB,acousticProblemData.APB, acousticProblemData.ASB};

            var krakenInputProfile = new KrakenInputProfile(acousticProblemData.NModes, acousticProblemData.Frequency, acousticProblemData.NMedia,
                options, bcBottom, mediumInfo, ssp, acousticProblemData.Sigma, acousticProblemData.CLow, acousticProblemData.CHigh,
                acousticProblemData.RMax, acousticProblemData.NSD, sd, acousticProblemData.NRD, rd, topAHSP, twerskyParams, bottomAHSP);

            return krakenInputProfile;
        }

        public FieldInputData MapFieldInputData(AcousticProblemData acousticProblemData, CalculatedModesInfo modesInfo)
        {
            var options = acousticProblemData.SourceType + acousticProblemData.ModesTheory;

            var r = new List<double>(acousticProblemData.R);
            r.Insert(0, 0);

            var rr = new List<double>(acousticProblemData.RR);
            rr.Insert(0, 0);

            var sd = new List<double>(acousticProblemData.SDField);
            sd.Insert(0, 0);

            var rd = new List<double>(acousticProblemData.RDField);
            rd.Insert(0, 0);

            var fieldInputData = new FieldInputData(modesInfo, options, acousticProblemData.NModesForField,
                acousticProblemData.NR, acousticProblemData.R, acousticProblemData.NSDField, sd,
                acousticProblemData.NRDField, rd, acousticProblemData.NRR, acousticProblemData.RR);

            return fieldInputData;
        }
    }
}
