import React, { Component } from 'react';

import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";
import FormWrapper from '../form-wrapper';

export default class Arctic extends Component {
    acousticProblemData = {
        frequency: 200,
        nModes: 192,
        nMedia: 1,
        topBCType: 'V',
        interpolationType: 'C',
        attenuationUnits: 'F',
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
        mediumInfo: '[7500,0.0,3750.0]',
        ssp: '[0, 1436, 0, 1, 0, 0],\
[200, 1458.40, 0, 1, 0, 0],\
[300, 1460.50, 0, 1, 0, 0],\
[1000, 1466.70, 0, 1, 0, 0],\
[2000, 1479.60, 0, 1, 0, 0],\
[2500, 1487.90, 0, 1, 0, 0],\
[3750, 1510.40, 0, 1, 0, 0]',
        bottomBCType: 'A',
        sigma: 0,
        zb: 3750,
        cpb: 1510.4,
        csb: 0,
        rhob: 1,
        apb: 0,
        asb: 0,
        cLow: 0,
        cHigh: 1510,
        rMax: 0,
        nsd: 1,
        sd: '100',
        nrd: 751,
        rd: '0,3750',
        calculateTransmissionLoss: true,
        nModesForField: 9999,
        sourceType: 'R',
        modesTheory: 'A',
        nProf: 1,
        rProf: '0,100',
        nr: 1001,
        r: '0,100',
        nsdField: 1,
        sdField: '100',
        nrdField: 376,
        rdField: '0,3750',
        nrr: 376,
        rr: '0'
    };

    render() {
        return (<Row>
            <Helmet>
                <title>Arctic profile</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <p>Arctic profile from <a href="https://oalib-acoustics.org/AcousticsToolbox/index_at.html">Acoustic Toolbox</a></p>
                </div>
                <FormWrapper acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}