import React, { Component } from 'react';
import BootstrapSwitchButton from 'bootstrap-switch-button-react';
import ModesChart from '../modes-chart';
import SoundSpeedChart from '../sound-speed-chart';
import ModeCharacteristicChart from '../mode-characteristic-chart';

export default class Charts extends Component {
    state = {
        showCharts:false
    };

    toggleChartsVisibility = () => {
        this.setState({
            showCharts: !this.state.showCharts
        });
    };

    render() {
        const { modesData, ssp } = this.props;
        const { showCharts } = this.state;

        return (
            <div className="charts-wrapper">
                <div>
                    <div className="chart-button">
                        <BootstrapSwitchButton checked={showCharts} onstyle="outline-primary" offstyle="outline-secondary"
                            onlabel='Collapse'
                            offlabel='Expand'
                            onChange={this.toggleChartsVisibility}
                            size="sm" />
                        <span className="switch-button-descr">Modes amplitude, sound speed, wavenumbers, scatter losses, group and phase speed charts</span>
                    </div>
                </div>
                {showCharts &&
                    <>
                        <ModesChart data={modesData.modes} />
                        <SoundSpeedChart data={ssp} />
                        <ModeCharacteristicChart data={modesData.k} chartName="Wavenumber (1/m)" yAxisLabelValue="k (1/m)" />
                        <ModeCharacteristicChart data={modesData.alpha} chartName="Scatter loss (1/m)" yAxisLabelValue="alpha (1/m)" />
                        <ModeCharacteristicChart data={modesData.groupSpeed} chartName="Group speed (m/s)" yAxisLabelValue="Group speed (m/s)" />
                        <ModeCharacteristicChart data={modesData.phaseSpeed} chartName="Phase speed (m/s)" yAxisLabelValue="Phase speed (m/s)" />
                </>   
                }
            </div>
        );
    };
}