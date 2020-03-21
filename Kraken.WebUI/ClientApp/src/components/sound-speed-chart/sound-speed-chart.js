import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { exportChart } from '../../utilites/export-chart';
import { Button } from 'reactstrap';

export default class SoundSpeedChart extends Component {
    chartId = "sound-speed-chart";
    chartName = "Sound speed";

    render() {
        const { data } = this.props;                  

        return (
            <div className="lg-chart-wrapper">
                <div className='d-flex justify-content-end'>
                    <div className="align-self-end">
                        <Button outline color="success" onClick={() => exportChart(this.chartId, this.chartName)}>Save as image</Button>
                    </div>
                </div>
                <ResponsiveContainer height={700} width="100%" id={this.chartId}>
                    <LineChart margin={{ left: 10, top: 35 }} layout="vertical">
                        <CartesianGrid strokeDasharray="10 10" />
                        <XAxis dataKey="speed" type="number" domain={['dataMin', 'dataMax']} label={{ value: 'Sound speed (m/s)', position: 'insideBottomRight', offset: 0, dy: 10 }} />
                        <YAxis dataKey="depth" type="number" domain={['dataMin', 'dataMax']} tickCount={20} label={{ value: 'Depth (m)', position: 'insideTopLeft', dx: -10, dy: -35}} />
                        <Tooltip />
                        <Legend />
                        <Line dataKey="speed" data={data} name={this.chartName} key={this.chartName} dot={false} stroke="#3030f0" />
                    </LineChart>
                </ResponsiveContainer>
            </div>);
    }
}