import React, { Component } from "react";
import { Spinner as ReactstrapSpinner } from 'reactstrap';

import './spinner.css';

export default class Spinner extends Component {

    render() {
        return (
            <div className="spinner d-flex justify-content-center">
                <ReactstrapSpinner color="dark" style={{ width: '3rem', height: '3rem' }}/>
            </div>
            );
    }
}