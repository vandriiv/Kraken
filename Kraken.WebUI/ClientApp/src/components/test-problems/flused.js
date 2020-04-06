import React, { Component } from 'react';

import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";
import FormWrapper from '../form-wrapper';

export default class Flused extends Component {
    acousticProblemData = {
        frequency: 10,
        nModes: 46,
        nMedia: 2,
        topBCType: 'V',
        interpolationType: 'N',
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
        mediumInfo: '[500,0,5000],[200,0,5100]',
        ssp: '[0,1500,0,1,0,0], [5000,1500,0,1,0,0],[5000,1400,0,1.5,0,0],[5100,1400,0,1.5,0,0]',
        bottomBCType: 'A',
        sigma: 0,
        zb: 5100,
        cpb: 4000,
        csb: 2000,
        rhob: 2,
        apb: 0,
        asb: 0,
        cLow: 1300,
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
                <title>Flused test problem</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <h5>Flused</h5>
                    <figure className="figure">
                        <img src="/images/figures/flused.gif" className="figure-img img-fluid rounded" alt="Flused problem figure" />
                        <figcaption className="figure-caption">Figure: Schematic of the Flused problem.</figcaption>
                    </figure>
                    <p>A fluid sediment is inserted between the bottom half-space and the ocean.</p>
                </div>
                <FormWrapper acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}