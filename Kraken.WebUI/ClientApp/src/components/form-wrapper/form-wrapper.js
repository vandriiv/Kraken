import React, { Component, Fragment } from 'react';
import KrakenService from '../../services/kraken-service';
import ComputingResultTables from '../computing-result-tables';

export default class FormWrapper extends Component {

    state = {
        isSuccess: false,
        computingResult: null,
        error:null
    }

    _krakenService = new KrakenService();
    onSubmit = (data) => {
        this._krakenService.computeNormalModes(data)
            .then(res => {
                this.setState({
                    isSuccess: true,
                    computingResult: res
            })
        })
            .catch(err => {
                this.setState({
                    error: err
                });
            });
    };

    onError = (err) => {

    };

    render() {
        const { form } = this.props;
        const { computingResult, error, isSuccess } = this.state;
       
        const formWithProps = React.cloneElement(form, { onSubmit: this.onSubmit, onError: this.onError });

        return (<Fragment>
            {formWithProps}
            {isSuccess && < ComputingResultTables computingResult={computingResult}/>}
        </Fragment>)
    }
}