import React, { Component } from 'react';
import KrakenComputingResult from '../kraken-computing-result';
import Modes from '../modes';
import TransmissionLoss from '../transmission-loss';
import SoundSpeed from '../sound-speed/sound-speed';
import BootstrapSwitchButton from 'bootstrap-switch-button-react';

import './computing-result.css';

export default class ComputingResult extends Component {
    state = {
        showKrakenComputing:false,
        showModes: false,
        showSoundSpeed:false,
        showTransmissionLoss:false
    };

    toggleModesVisibility = () => {
        this.setState({
            showModes: !this.state.showModes
        });
    }

    toggleKrakenComputingVisibility = () => {
        this.setState({
            showKrakenComputing: !this.state.showKrakenComputing
        });
    }

    toggleSoundSpeedVisibility = () => {
        this.setState({
            showSoundSpeed: !this.state.showSoundSpeed
        });
    }

    toggleTransmissionLossVisibility = () => {
        this.setState({
            showTransmissionLoss: !this.state.showTransmissionLoss
        });
    }

    render() {
        const { computingResult,ssp } = this.props;
        const krakenComputingResult = {
            k: computingResult.k,
            alpha: computingResult.alpha,
            groupSpeed: computingResult.groupSpeed,
            phaseSpeed: computingResult.phaseSpeed
        };

        const { transmissionLossCalculated } = computingResult;

        const { showKrakenComputing, showModes, showSoundSpeed, showTransmissionLoss } = this.state;

        return (<>
            <div className="switch-with-descr">
                <BootstrapSwitchButton checked={showKrakenComputing} onstyle="outline-primary" offstyle="outline-secondary"
                    onlabel='Collapse'
                    offlabel='Expand'
                    onChange={this.toggleKrakenComputingVisibility}                    
                    width="100" 
                    size="sm"/>  
                <span className="switch-button-descr">Wavenumber, scatter loss, phase and group speed for each mode</span>
            </div>
            {showKrakenComputing && < KrakenComputingResult data={krakenComputingResult} />}
            <div className="switch-with-descr">
                <BootstrapSwitchButton checked={showSoundSpeed} onstyle="outline-primary" offstyle="outline-secondary"
                    onlabel='Collapse'
                    offlabel='Expand'
                    onChange={this.toggleSoundSpeedVisibility}
                    width="100" 
                    size="sm"/>                    
                <span className="switch-button-descr">Sound speed</span>
            </div>
            {showSoundSpeed && < SoundSpeed data={ssp} />}
            <div className="switch-with-descr">
                <BootstrapSwitchButton checked={showModes} onstyle="outline-primary" offstyle="outline-secondary"
                    onlabel='Collapse'
                    offlabel='Expand'
                    onChange={this.toggleModesVisibility}
                    width="100" 
                    size="sm"/>  
                <span className="switch-button-descr">Normal modes</span>
            </div>
            {showModes && < Modes modes={computingResult.modes} modesCount={computingResult.modesCount} />}

            {transmissionLossCalculated &&
                <div className="switch-with-descr">
                    <BootstrapSwitchButton checked={showTransmissionLoss} onstyle="outline-primary" offstyle="outline-secondary"
                        onlabel='Collapse'
                        offlabel='Expand'
                        onChange={this.toggleTransmissionLossVisibility}
                        width="100" 
                        size="sm"/>  
                    <span className="switch-button-descr">Transmission loss</span>
                </div>
                }
            {showTransmissionLoss && <TransmissionLoss transmissionLoss={computingResult.transmissionLoss} ranges={computingResult.ranges} sourceDepths={computingResult.sourceDepths} receiverDepths={computingResult.receiverDepths} />}
        </>);
    }
}