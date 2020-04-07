import React, { Component } from 'react';

import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";
import FormWrapper from '../form-wrapper';

export default class Scholte extends Component {
    acousticProblemData = {
        frequency: 10,
        nModes: 45,
        nMedia: 1,
        topBCType: 'V',
        interpolationType: 'N',
        attenuationUnits: 'M',
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
        mediumInfo: '[500,0,5000]',
        ssp: '[0,1500,0,1,0,0], [5000,1500,0,1,0,0]',
        bottomBCType: 'A',
        sigma: 0,
        zb: 5000,
        cpb: 4000,
        csb: 2000,
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
                <title>Scholte test problem</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <h5>Scholte</h5>
                    <figure className="figure">
                        <img src="/images/figures/scholte.gif" className="figure-img img-fluid rounded" alt="Scholte problem figure" />
                        <figcaption className="figure-caption">Figure: Schematic of the Scholte problem.</figcaption>
                    </figure>
                    <p>This problem is a version of the Pekeris waveguide but with an elastic half-space as the bottom. This type of problem has a Scholte mode with a phase velocity less than the slowest speed in the problem. (Since the source and receiver are many wavelenghts from the interface the Scholte mode is not actually important for the transmission loss calculation.</p>
                </div>
                <FormWrapper acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}