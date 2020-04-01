import React, { Component } from 'react';

import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";
import FormWrapper from '../form-wrapper';

export default class Dickins extends Component {
    acousticProblemData = {
        frequency: 230,
        nModes: 128,
        nMedia: 1,
        topBCType: 'V',
        interpolationType: 'C',
        attenuationUnits: 'W',
        isVolumeAttenuatonAdded: false,
        zt: 0,
        cpt: 0,
        cst: 0,
        rhot: 0,
        apt: 0,
        ast: 0,
        bumDen: 0,
        eta: 0,
        xi: 0,
        mediumInfo: '[0, 0.0, 3000.0]',
        ssp:'[0.0,1476.7,0.0,1.0,0.000,0.0000],\
[38.0,1476.7,0.0,1.0,0.000,0.0000],\
[50.0,1472.6,0.0,1.0,0.000,0.0000],\
[70.0,1468.8,0.0,1.0,0.000,0.0000],\
[100.0,1467.2,0.0,1.0,0.000,0.0000],\
[140.0,1471.6,0.0,1.0,0.000,0.0000],\
[160.0,1473.6,0.0,1.0,0.000,0.0000],\
[170.0,1473.6,0.0,1.0,0.000,0.0000],\
[200.0,1472.7,0.0,1.0,0.000,0.0000],\
[215.0,1472.2,0.0,1.0,0.000,0.0000],\
[250.0,1471.6,0.0,1.0,0.000,0.0000],\
[300.0,1471.6,0.0,1.0,0.000,0.0000],\
[370.0,1472.0,0.0,1.0,0.000,0.0000],\
[450.0,1472.7,0.0,1.0,0.000,0.0000],\
[500.0,1473.1,0.0,1.0,0.000,0.0000],\
[700.0,1474.9,0.0,1.0,0.000,0.0000],\
[900.0,1477.0,0.0,1.0,0.000,0.0000],\
[1000.0,1478.1,0.0,1.0,0.000,0.0000],\
[1250.0,1480.7,0.0,1.0,0.000,0.0000],\
[1500.0,1483.8,0.0,1.0,0.000,0.0000],\
[2000.0,1490.5,0.0,1.0,0.000,0.0000],\
[2500.0,1498.3,0.0,1.0,0.000,0.0000],\
[3000.0,1506.5,0.0,1.0,0.000,0.0000]',
        bottomBCType: 'A',
        sigma: 0,
        zb: 3000,
        cpb: 1506.50,
        csb: 0,
        rhob: 1.5,
        apb: 0.5,
        asb: 0,
        cLow: 1400,
        cHigh: 2000,
        rMax: 0,
        nsd: 1,
        sd: '18',
        nrd: 201,
        rd: '0,3000',
        calculateTransmissionLoss: true,
        nModesForField: 9999,
        sourceType: 'R',
        modesTheory: 'A',
        nProf: 1,
        rProf: '0',
        nr: 501,
        r: '0,100',
        nsdField: 1,
        sdField: '18',
        nrdField: 201,
        rdField: '0,3000',
        nrr: 201,
        rr: '0'
    };

    render() {
        return (<Row>
            <Helmet>
                <title>Dickins seamount</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <p>Dickins seamount profile from <a href="https://oalib-acoustics.org/AcousticsToolbox/index_at.html">Acoustic Toolbox</a></p>
                </div>
                <FormWrapper acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}