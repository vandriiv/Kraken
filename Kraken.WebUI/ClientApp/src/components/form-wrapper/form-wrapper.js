import React, { Component, Fragment } from 'react';
import KrakenService from '../../services/kraken-service';
import ComputingResultTables from '../computing-result-tables';
import ModesChart from '../modes-chart';
import SoundSpeedChart from '../sound-speed-chart';
import ModeCharacteristicChart from '../mode-characteristic-chart';
import Charts from '../charts';

export default class FormWrapper extends Component {

    state = {
        formData: null,
        isSuccess: false,
        computingResult: null,
        error:null
    }

    _krakenService = new KrakenService();

    onSubmit = (data) => {
        console.log(data);
        this.setState({
            formData: data
        });

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
        const { computingResult, error, isSuccess, formData } = this.state;
       
        const formWithProps = React.cloneElement(form, { onSubmit: this.onSubmit, onError: this.onError });

        return (<Fragment>
            {formWithProps}
            {isSuccess && <ComputingResultTables computingResult={computingResult} />}
            {isSuccess && <Charts modesData={computingResult} ssp={formData.ssp}/>}
        </Fragment>);
    }
}