﻿import React, { Component } from 'react';
import InitializedInputForm from '../initialized-input-form';
import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";

export default class Normal extends Component {
    acousticProblemData = {
        frequency: 10,
        nModes: 10,
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
        mediumInfo: '[300,0,5000],[200,0,5000]',
        ssp: '[0,1500,0,1,0,0], [3000,1500,0,1,0,0],[3000,1500,0,2,0,0],[5000,1500,0,2,0,0]',
        bottomBCType: 'A',
        sigma: 0,
        zb: 5000,
        cpb: 4000,
        csb: 0,
        rhob: 3,
        apb: 0,
        asb: 0,
        cLow: 1400,
        cHigh: 2000,
        rMax: 1000,
        nsd: 1,
        sd: '500',
        nrd: 1,
        rd: '2500'
    };

    render() {
        return (<Row>
            <Helmet>
                <title>Normal test problem</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <h5>Normal</h5>
                    <figure className="figure">
                        <img src="/images/figures/normal.gif" className="figure-img img-fluid rounded" alt="Normal problem figure" />
                        <figcaption className="figure-caption">Figure: Schematic of the Normal problem.</figcaption>
                    </figure>
                    <p>Mode normalization is checked using several density changes. Due to the shear in the lower halfspace, there is a Scholte wave with a phase velocity of about 1393 m/s. It has been excluded from the calculation.</p>
                </div>
                <InitializedInputForm acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}