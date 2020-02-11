import React, { Component } from 'react';
import InputForm from '../input-form';
import { Row, Col } from 'reactstrap';

export default class KrakenPage extends Component {

    render() {
        return (<Row>
            <Col xs={12}>
                <InputForm/>
            </Col>
        </Row>)
    }
}
