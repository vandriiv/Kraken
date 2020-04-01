import React, { Component } from 'react';

import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";
import FormWrapper from '../form-wrapper';

export default class Munk extends Component {
    acousticProblemData = {
        frequency: 50,
        nModes: 102,
        nMedia:1,
        topBCType: 'V',
        interpolationType: 'N',
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
        mediumInfo: '[0,0,5000]',
        ssp: '[0.0, 1548.52, 0.0,1.0, 0.0, 0.0],\
        [200, 1530.29, 0.0,1.0, 0.0, 0.0],\
        [250, 1526.69, 0.0,1.0, 0.0, 0.0],\
        [400, 1517.78, 0.0,1.0, 0.0, 0.0],\
        [600, 1509.49, 0.0,1.0, 0.0, 0.0],\
        [800, 1504.30, 0.0,1.0, 0.0, 0.0],\
        [1000, 1501.38, 0.0,1.0, 0.0, 0.0],\
        [1200, 1500.14, 0.0,1.0, 0.0, 0.0],\
        [1400, 1500.12, 0.0,1.0, 0.0, 0.0],\
        [1600, 1501.02, 0.0,1.0, 0.0, 0.0],\
        [1800, 1502.57, 0.0,1.0, 0.0, 0.0],\
        [2000, 1504.62, 0.0,1.0, 0.0, 0.0],\
        [2200, 1507.02, 0.0,1.0, 0.0, 0.0],\
        [2400, 1509.69, 0.0,1.0, 0.0, 0.0],\
        [2600, 1512.55, 0.0,1.0, 0.0, 0.0],\
        [2800, 1515.56, 0.0,1.0, 0.0, 0.0],\
        [3000, 1518.67, 0.0,1.0, 0.0, 0.0],\
        [3200, 1521.85, 0.0,1.0, 0.0, 0.0],\
        [3400, 1525.10, 0.0,1.0, 0.0, 0.0],\
        [3600, 1528.38, 0.0,1.0, 0.0, 0.0],\
        [3800, 1531.70, 0.0,1.0, 0.0, 0.0],\
        [4000, 1535.04, 0.0,1.0, 0.0, 0.0],\
        [4200, 1538.39, 0.0,1.0, 0.0, 0.0],\
        [4400, 1541.76, 0.0,1.0, 0.0, 0.0],\
        [4600, 1545.14, 0.0,1.0, 0.0, 0.0],\
        [4800, 1548.52, 0.0,1.0, 0.0, 0.0],\
        [5000, 1551.91, 0.0,1.0, 0.0, 0.0]',
        bottomBCType: 'A',
        sigma: 0,
        zb: 5000,
        cpb:1600,
        csb: 0,
        rhob:1.8,
        apb: 0,
        asb: 0.8,
        cLow:1500,
        cHigh:1600,
        rMax: 0,
        nsd: 2,
        sd: '25,250',
        nrd:1001,
        rd: '0,5000',
        calculateTransmissionLoss: true,
        nModesForField: 9999,
        sourceType: 'R',
        modesTheory: 'A',
        nProf:1,
        rProf: '0,100',
        nr:1001,
        r: '0,100',
        nsdField:1,
        sdField: '1000',
        nrdField:501,
        rdField: '0,5000',
        nrr: 501,
        rr: '0'
    };

    render() {
        return (<Row>
            <Helmet>
                <title>Munk profile</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <p>BB Munk profile from <a href="https://oalib-acoustics.org/AcousticsToolbox/index_at.html">Acoustic Toolbox</a></p>
                </div>
                <FormWrapper acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}