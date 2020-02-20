using Kraken.Application.Models;
using Kraken.Application.Services.Interfaces;
using Kraken.NormalModesCalculation;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Application.Services.Implementation
{
    public class KrakenService : IKrakenService
    {
        private readonly KrakenNormalModesProgram _krakenNormalModeProgram;

        public KrakenService(KrakenNormalModesProgram krakenNormalModeProgram)
        {
            _krakenNormalModeProgram = krakenNormalModeProgram;
        }

        public NormalModes ComputeModes(AcousticProblemData acousticProblemData)
        {
            var result = new NormalModes();

            var options = acousticProblemData.InterpolationType  + acousticProblemData.TopBCType + acousticProblemData.AttenuationUnits + acousticProblemData.AddedVolumeAttenuation;
            var bottomBC = acousticProblemData.BottomBCType;

            var mediumInfo = new List<List<double>>(acousticProblemData.MediumInfo);
            mediumInfo.Insert(0, new List<double>());
            foreach(var list in mediumInfo)
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

            var cLowHight = new List<double> { 0, acousticProblemData.CLow, acousticProblemData.CHigh };

            var nz = acousticProblemData.NSD + acousticProblemData.NRD;

            var topAHSP = new List<double> { 0, acousticProblemData.ZT, acousticProblemData.CPT, acousticProblemData.CST,
                                            acousticProblemData.RHOT,acousticProblemData.APT, acousticProblemData.AST};

            var twerskyParams = new List<double> { 0, acousticProblemData.BumDen, acousticProblemData.Eta, acousticProblemData.Xi };
            var bottomAHSP = new List<double> { 0, acousticProblemData.ZB, acousticProblemData.CPB, acousticProblemData.CSB,
                                            acousticProblemData.RHOB,acousticProblemData.APB, acousticProblemData.ASB};

            var cg = new List<double>();
            var cp = new List<double>();
            var k = new List<Complex>();
            var zm = new List<double>();
            var modes = new List<List<double>>();

            _krakenNormalModeProgram.OceanAcousticNormalModes(acousticProblemData.NModes,acousticProblemData.Frequency,acousticProblemData.NMedia, options,
                mediumInfo,ssp.Count,ssp,bottomBC,acousticProblemData.Sigma,cLowHight,acousticProblemData.RMax,acousticProblemData.NSD,sd, acousticProblemData.NRD,
                rd,nz,topAHSP,twerskyParams,bottomAHSP, ref cg, ref cp, ref zm, ref modes,ref k);

            result.PhaseSpeed = cp; 
            result.GroupSpeed = cg;
            result.K = k;
            result.Modes = modes;
            result.ZM = zm;

            result.PhaseSpeed.RemoveAt(0);
            result.GroupSpeed.RemoveAt(0);
            result.K.RemoveAt(0);
            result.ZM.RemoveAt(0);

            result.Modes.RemoveAt(0);
            foreach(var mode in result.Modes)
            {
                mode.RemoveAt(0);
            }

            return result;
        }
    }
}
