import React, { Component } from 'react';
import { Row, Col } from 'reactstrap';
import FormWrapper from '../form-wrapper';

export default class KrakenPage extends Component {

    render() {
        return (<Row>
            <Col xs={12}>
                <FormWrapper/>
            </Col>
        </Row>)
    }
}
