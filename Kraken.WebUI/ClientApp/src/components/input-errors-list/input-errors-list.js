import React, { Component } from 'react';
import { Alert } from 'reactstrap';

export default class InputErrorsList extends Component {
    state = {
        visible: true
    }

    hide = () => {
        this.setState({
            visible: false
        });
    };

    mapValidationMessages(error) {
        return Object.keys(error).map((item, idx) => <li key={idx}>{`${error[item]}`}</li>);
    }

    displayErrorMessages(error) {
        return this.mapValidationMessages(error);        
    }

    render() {
        const { visible } = this.state;
        const { error } = this.props;

        return (<div className='form-alert'>
            <Alert color="danger" isOpen={visible} toggle={this.hide}>
                <h5 className="alert-heading"> Following validation errors have been occured:</h5>
                <ul>
                    {this.displayErrorMessages(error)}
                </ul>
            </Alert>
        </div>);
    }
}