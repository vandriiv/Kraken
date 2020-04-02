import React, { Component } from 'react';
import { Alert } from 'reactstrap';

export default class ErrorMessage extends Component {

    state = {
        visible: true
    }

    hide = () => {
        this.setState({
            visible: false
        });
    };

    componentDidUpdate(prevProps) {
        if (prevProps.errorMessage !== this.props.errorMessage)
            this.setState({
                visible: true
            });
    }

    render() {
        const {header, errorMessage } = this.props;
        const { visible } = this.state;

        return (<div className='form-alert'>
            <Alert color="danger" isOpen={visible} toggle={this.hide}>
                <h5 className="alert-heading">{header}:</h5>
                <p>{errorMessage}</p>
            </Alert>
        </div>);
    }
}