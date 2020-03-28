import React, { Component } from 'react';
import { Row, Col } from 'reactstrap';
import FormWrapper from '../form-wrapper';
import Helmet from 'react-helmet';

export default class KrakenPage extends Component {

    render() {
        return (<Row>
            <Col xs={12}>
                <Helmet>Kraken</Helmet>
                <FormWrapper/>
            </Col>
        </Row>)
    }
}
