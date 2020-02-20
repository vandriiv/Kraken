import React, { Component, Fragment } from 'react';
import KrakenService from '../../services/kraken-service';
export default class FormWrapper extends Component {

    _krakenService = new KrakenService();
    onSubmit = (data) => {
        console.log(data);
        console.log("--------------------");
        this._krakenService.computeNormalModes(data).then(r => console.log(r));
    };

    onError = (err) => {

    };

    render() {
        const { form } = this.props;
        const formWithProps = React.cloneElement(form, { onSubmit: this.onSubmit, onError: this.onError });
        return (<Fragment>
            {formWithProps}
        </Fragment>)
    }
}