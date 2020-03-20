import React, { Component } from 'react';
import BootstrapSwitchButton from 'bootstrap-switch-button-react';
import ModesChart from '../modes-chart';
import SoundSpeedChart from '../sound-speed-chart';
import ModeCharacteristicChart from '../mode-characteristic-chart';
import TransmissionLossChart from '../transmission-loss-chart';

export default class Charts extends Component {
    state = {
        showModesCharts: false,
        showTransmissionLossChart:false,
    };

    toggleModesChartsVisibility = () => {
        this.setState({
            showModesCharts: !this.state.showModesCharts
        });
    };

    toggleTransmissionLossChartsVisibility = () => {
        this.setState({
            showTransmissionLossChart: !this.state.showTransmissionLossChart
        });
    };

    render() {
        const { data, ssp } = this.props;
        const { showModesCharts, showTransmissionLossChart } = this.state;

        return (
            <div className="charts-wrapper">
                <div>
                    <div className="result-view-descr">
                        <BootstrapSwitchButton checked={showModesCharts} onstyle="outline-primary" offstyle="outline-secondary"
                            onlabel='Collapse'
                            offlabel='Expand'
                            onChange={this.toggleModesChartsVisibility}
                            size="sm" />
                        <span className="switch-button-descr">Modes amplitude, sound speed, wavenumbers, scatter losses, group and phase speed charts</span>
                    </div>
                </div>
                {showModesCharts &&
                    <>
                        <ModesChart data={data} />
                        <SoundSpeedChart data={ssp} />
                        <ModeCharacteristicChart data={data.k} chartName="Wavenumber (1/m)" yAxisLabelValue="k (1/m)" />
                        <ModeCharacteristicChart data={data.alpha} chartName="Scatter loss (1/m)" yAxisLabelValue="alpha (1/m)" />
                        <ModeCharacteristicChart data={data.groupSpeed} chartName="Group speed (m/s)" yAxisLabelValue="Group speed (m/s)" />
                    <ModeCharacteristicChart data={data.phaseSpeed} chartName="Phase speed (m/s)" yAxisLabelValue="Phase speed (m/s)" />
                    </>   
                }

                <div>
                    <div className="result-view-descr">
                        <BootstrapSwitchButton checked={showTransmissionLossChart} onstyle="outline-primary" offstyle="outline-secondary"
                            onlabel='Collapse'
                            offlabel='Expand'
                            onChange={this.toggleTransmissionLossChartsVisibility}
                            size="sm" />
                        <span className="switch-button-descr">Transmission loss</span>
                    </div>
                </div>
                {showTransmissionLossChart &&
                    <TransmissionLossChart transmissionLoss={data.transmissionLoss} sourceDepths={data.sourceDepths}
                        receiverDepths={data.receiverDepths} ranges={data.ranges}/> 
                }
            </div>
        );
    };
}