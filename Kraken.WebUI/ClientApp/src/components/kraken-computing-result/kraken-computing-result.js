import React, { Component } from 'react';
import KrakenResultTable from '../kraken-result-table';
import ModeCharacteristicChart from '../mode-characteristic-chart';

export default class KrakenComputingResult extends Component {

    render() {
        const { data } = this.props;

        return (<>            
            <KrakenResultTable data={data} />       
            <ModeCharacteristicChart data={data.k} chartName="Wavenumber (1/m)" chartId="wavenumber" yAxisLabelValue="k (1/m)" />
            <ModeCharacteristicChart data={data.alpha} chartName="Scatter loss (1/m)" chartId="scatter-loss" yAxisLabelValue="alpha (1/m)" />
            <ModeCharacteristicChart data={data.groupSpeed} chartName="Group speed (m/s)" chartId="group-speed" yAxisLabelValue="Group speed (m/s)" />
            <ModeCharacteristicChart data={data.phaseSpeed} chartName="Phase speed (m/s)" chartId="phase-speed" yAxisLabelValue="Phase speed (m/s)" />
        </>);
    }
}