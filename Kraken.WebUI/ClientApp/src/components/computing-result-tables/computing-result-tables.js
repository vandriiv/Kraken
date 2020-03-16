import React, { Component } from 'react';
import BootstrapSwitchButton from 'bootstrap-switch-button-react';
import KrakenResultTable from '../kraken-result-table';
import ModesTable from '../modes-table/modes-table';

export default class ComputingResultTables extends Component {
    state = {
        showModes: true,
        showKrakenResult: true
    };

    toggleKrakenResultTable = () => {
        this.setState({
            showKrakenResult: !this.state.showKrakenResult
        });
    };

    toggleModesTable = () =>{
        this.setState({
            showModes: !this.state.showModes
        });
    };

    render() {
        const { showModes, showKrakenResult } = this.state;
        const { computingResult } = this.props;

        return (<div className='tables-wrapper'>
            <div>
                <div className="table-descr">
                    <BootstrapSwitchButton checked={showKrakenResult} onstyle="outline-primary" offstyle="outline-secondary"
                onlabel='Collapse'
                offlabel='Expand'
                onChange={this.toggleKrakenResultTable}    
                size="sm"/>
                <span className="switch-button-descr">Wavenumber, scatter loss, phase and group speed for each mode table</span> 
             </div>
            </div>
            {showKrakenResult ?
                <KrakenResultTable data={computingResult} />
                : null}
            <div>
                <div className="table-descr">
                    <BootstrapSwitchButton checked={showModes} onstyle="outline-primary" offstyle="outline-secondary"
                    onlabel='Collapse'
                    offlabel='Expand'
                    onChange={this.toggleModesTable}
                        size="sm" />
                    <span className="switch-button-descr">Normal modes amplitudes table</span>                    
                </div>
            </div>
            {showModes ?
                <ModesTable data={computingResult, computingResult} />
                : null}
        </div>)
    }
}