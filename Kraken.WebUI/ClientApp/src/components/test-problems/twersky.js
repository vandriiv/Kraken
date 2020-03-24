import React, { Component } from 'react';

import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";
import FormWrapper from '../form-wrapper';

export default class Twersky extends Component {
    acousticProblemData = {
        frequency: 10,
        nModes: 10,
        nMedia: 1,
        topBCType: 'S',
        interpolationType: 'N',
        attenuationUnits: 'F',
        isVolumeAttenuatonAdded: false,
        zt: 0,
        cpt: 0,
        cst: 0,
        rhot: 0,
        apt: 0,
        ast: 0,
        bumDen: 0.092,
        eta: 8.2,
        xi: 5.1,
        mediumInfo: '[500,0,5000]',
        ssp: '[0,1500,0,1,0,0], [5000,1500,0,1,0,0]',
        bottomBCType: 'A',
        sigma: 0,
        zb: 5000,
        cpb: 2000,
        csb: 0,
        rhob: 2,
        apb: 0,
        asb: 0,
        cLow: 1400,
        cHigh: 2000,
        rMax: 1000,
        nsd: 1,
        sd: '500',
        nrd: 1,
        rd: '2500',
        calculateTransmissionLoss: true,
        nModesForField: 9999,
        sourceType: 'R',
        modesTheory: 'A',
        nProf: 1,
        rProf: '0',
        nr: 501,
        r: '200,220',
        nsdField: 1,
        sdField: '500',
        nrdField: 1,
        rdField: '2500',
        nrr: 1,
        rr: '0'
    };

    render() {
        return (<Row>
            <Helmet>
                <title>Twersky test problem</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <h5>Twersky</h5>
                    <figure className="figure">
                        <img src="/images/figures/twersky.gif" className="figure-img img-fluid rounded" alt="Twersky problem figure" />
                        <figcaption className="figure-caption">Figure: Schematic of the Twersky problem.</figcaption>
                    </figure>
                    <p>The Pekeris problem is modified by the inclusion of surface scatter. The rough surface involves a density of 0.092 bosses per km of width 8.2 m and height 5.1 m</p>
                </div>
                <FormWrapper acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}