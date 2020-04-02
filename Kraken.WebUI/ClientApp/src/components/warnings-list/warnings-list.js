import React, { Component } from 'react';
import { Alert } from 'reactstrap';

export default class WarningsList extends Component {
    state = {
        visible: true
    }

    hide = () => {
        this.setState({
            visible: false
        });
    };

    componentDidUpdate(prevProps) {
        if (prevProps.warnings !== this.props.warnings)
            this.setState({
                visible: true
            });
    }

    render() {
        const { visible } = this.state;
        const { warnings } = this.props;

        return (<div className='form-alert'>
            <Alert color="warning" isOpen={visible} toggle={this.hide}>
                <h5 className="alert-heading"> Following warnings been occured - the result may not be correct:</h5>
                <ul>
                    {warnings.map((w, idx) => <li key={idx}>{w}</li>)}
                </ul>
            </Alert>
        </div>);
    }
}