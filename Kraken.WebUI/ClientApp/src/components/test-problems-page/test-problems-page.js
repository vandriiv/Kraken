import React, { Component } from 'react';
import { Row, Col, ListGroup, ListGroupItem } from 'reactstrap';
import { Link } from 'react-router-dom';

export default class TestProblemsPage extends Component {

    render() {
        return (<Row>
            <Col xs={12}>
                <p>The following test problems have been developed to validate the model by exercising various components of the code and to illustrate the input structure required for various kinds of scenarios. In brief, we have:</p>
                <ListGroup>
                    <ListGroupItem><Link to="/pekeris">PEKERIS:</Link> A simple (two-layer) Pekeris waveguide.</ListGroupItem>
                    <ListGroupItem><Link to="/twersky">TWERSKY:</Link> The Pekeris wave guide with surface roughness. Demonstrates that the Twersky scatter works properly.</ListGroupItem>
                    <ListGroupItem><Link to="/scholte">SCHOLTE:</Link> A two-layer waveguide with an elastic bottom which leads to a Scholte wave. Demonstrates that the elastic half-space condition functions correctly.</ListGroupItem>
                    <ListGroupItem><Link to="/double">DOUBLE:</Link> A double-duct problem demonstrating that gradients are handled properly.</ListGroupItem>
                    <ListGroupItem><Link to ="/flused">FLUSED:</Link> A three-layer problem involving ocean, sediment and half-space. Demonstrates that multiple layers are treated properly.</ListGroupItem>
                    <ListGroupItem><Link to ="/elsed">ELSED:</Link> A three-layer problem with shear properties in the sediment. Demonstrates that elastic media are handled properly.</ListGroupItem>
                    <ListGroupItem><Link to="/atten">ATTEN:</Link> A two-layer problem with volume attenuation. Demonstrates that attenuation is handled properly.</ListGroupItem>
                    <ListGroupItem><Link to="/normal">NORMAL:</Link> A problem with several density changes to check out the modal normalization in a severe case.</ListGroupItem>
                    <ListGroupItem><Link to="/ice">ICE:</Link> A problem with an elastic ice layer to demonstrate that elastic layers above the water column are handled properly.</ListGroupItem>
                </ListGroup>
            </Col>
        </Row>)
    }
}