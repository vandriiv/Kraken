﻿import React, { Component } from 'react';

import { Row, Col } from 'reactstrap';
import { Helmet } from "react-helmet";
import FormWrapper from '../form-wrapper';

export default class Pekeris extends Component {
    acousticProblemData = {
        frequency: 10,
        nModes: 44,
        nMedia: 1,
        topBCType: 'V',
        interpolationType: 'N',
        attenuationUnits:'F',
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
        sigma:0,
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
                <title>Perekis test problem</title>
            </Helmet>
            <Col xs={12}>
                <div>
                    <h5>Pekeris</h5>
                    <figure className="figure">
                        <img src="/images/figures/pekeris.gif" className="figure-img img-fluid rounded" alt="Perekis problem figure"/>
                            <figcaption className="figure-caption">Figure: Schematic of the PEKERIS problem.</figcaption>
                    </figure>
                    <p>This problem involves a homogeneous fluid layer with a sound speed of 1500 m/s overlying a faster bottom with sound speed 2000 m/s  and density of 20 g/cm<sup>3</sup>.</p>
                </div>
                <FormWrapper acousticProblemData={this.acousticProblemData} />
            </Col>
        </Row>)
    }
}