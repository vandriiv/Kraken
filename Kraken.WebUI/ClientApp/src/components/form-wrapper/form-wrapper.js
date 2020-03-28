import React, { Component, Fragment } from 'react';
import KrakenService from '../../services/kraken-service';
import ComputingResult from '../computing-result';
import AcousticProblemForm from '../acoustic-problem-form';
import InputErrorsList from '../input-errors-list';
import ErrorMessage from '../error-message';
import WarningsList from '../warnings-list';

export default class FormWrapper extends Component {

    state = {
        formData: null,
        isSuccess: false,
        computingResult: null,
        error: null
    }

    _krakenService = new KrakenService();

    onSubmit = (data) => {        
        this.setState({
            formData: data,
            isSuccess: false,
            error:null
        });

        this._krakenService.computeNormalModes(data)
            .then(res => {             
                    this.setState({
                        isSuccess: true,
                        computingResult: res
                    });              
            })
            .catch(err => {              
                const statusCode = err.error.response.status;
                const data = err.error.response.data;

                const error = { status: statusCode, data:null}

                if (statusCode === 500) {
                    if (data.expectedError) {
                        error.data = data.error;
                        this.setState({
                            error: error
                        });
                    }
                    else {
                        error.data = "Unexpected server error has occured";
                        this.setState({
                            error: error                            
                        });
                    }
                }
                else if (statusCode === 400) {
                    if (data.validationErrors) {
                        error.validationErrors = data.validationErrors;
                    }
                    else {
                        error.data = data;
                    }
                    this.setState({
                        error: error
                    });
                }
            });
    };

    onError = (err) => {
        this.setState({
            isSuccess:false
        });
    };

    render() {
        const { acousticProblemData } = this.props;        
        const { computingResult, error, isSuccess, formData } = this.state;
        const hasError = error !== null;
        const hasValidationError = hasError && error.validationErrors !== null && error.validationErrors !== undefined;

        return (<Fragment>
            <AcousticProblemForm acousticProblemData={acousticProblemData} onSubmit={this.onSubmit} onError={this.onError} />
            {isSuccess && computingResult.warnings.length>0 && <WarningsList warnings={computingResult.warnings} />}
            {isSuccess ? <ComputingResult computingResult={computingResult} ssp={formData.ssp} /> : null}
            {hasValidationError ? <InputErrorsList error={error.validationErrors} /> : null}
            {hasError && !hasValidationError ? <ErrorMessage header="An error been occured" errorMessage={error.data} /> : null}            
        </Fragment>);
    }
}