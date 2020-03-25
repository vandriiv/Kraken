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

    componentDidUpdate(prevProps){
        if(prevProps.error!=this.props.error)
        this.setState({
            visible:true
        });
    }

    mapValidationMessagesAsObjKeys(error) {
        return Object.keys(error).map((item, idx) => <li key={idx}>{`${error[item]}`}</li>);
    }

    mapValidationMessagesAsArray(error) {
        return error.map((e, idx) => <li key={idx}>{e}</li>);
    }

    mapValidationMessages(error) {
        if (Array.isArray(error)) {
            return this.mapValidationMessagesAsArray(error);
        }       
        return this.mapValidationMessagesAsObjKeys(error);
    }

    render() {
        const { visible } = this.state;
        const { error } = this.props;

        return (<div className='form-alert'>
            <Alert color="danger" isOpen={visible} toggle={this.hide}>
                <h5 className="alert-heading"> Following validation errors been occured:</h5>
                <ul>
                    {this.mapValidationMessages(error)}
                </ul>
            </Alert>
        </div>);
    }
}