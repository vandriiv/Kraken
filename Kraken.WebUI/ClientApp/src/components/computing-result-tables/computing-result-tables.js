import React, { Component } from 'react';
import BootstrapSwitchButton from 'bootstrap-switch-button-react'
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
                <BootstrapSwitchButton checked={showKrakenResult} onstyle="outline-primary" offstyle="outline-secondary"
                onlabel='Show'
                offlabel='Hide'
                onChange={this.toggleKrakenResultTable}    
                size="sm"
                />
            </div>
            {showKrakenResult ?
                <KrakenResultTable data={computingResult} />
                : null}
            <div>
            <BootstrapSwitchButton checked={showModes} onstyle="outline-primary" offstyle="outline-secondary"
                onlabel='Show'
                offlabel='Hide'
                onChange={this.toggleModesTable}
                    size="sm" />
            </div>
            {showModes ?
                <ModesTable modes={computingResult.modes} />
                : null}
        </div>)
    }
}