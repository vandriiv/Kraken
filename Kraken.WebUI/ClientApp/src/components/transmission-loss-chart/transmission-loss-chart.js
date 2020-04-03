import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { exportChart } from '../../utilites/export-chart';
import { Button } from 'reactstrap';

export default class TransmissionLossChart extends Component {
    chartId = "transmission-loss-chart";

    tLDataFormater = (number) => {
        const toFixed3 = number.toFixed(3).toString()
        if (toFixed3.includes("0.000")) {
            return number.toExponential(3);
        }
        return toFixed3;
    }

    rangeDataFormatter = (number) => {
        return number / 1000;
    }

    render() {
        const { data, receiverDepth, sourceDepth } = this.props;

        const chartExportName = `transmission-loss-sd-${parseFloat(sourceDepth).toFixed(5)}-rd-${parseFloat(receiverDepth).toFixed(5)}`;

        return (<>
            <div className="lg-chart-wrapper">
                <div className='d-flex justify-content-end'>
                    <div>
                        <Button outline color="success" onClick={() => exportChart(this.chartId, chartExportName)}>Save as image</Button>
                    </div>
                </div>
                <ResponsiveContainer height={700} width="100%" id={this.chartId}>
                    <LineChart margin={{ left: 10, top: 35 }}>
                        <CartesianGrid strokeDasharray="10 10" />
                        <XAxis dataKey="range" type="number" tickFormatter={this.rangeDataFormatter} domain={['dataMin', 'dataMax']} label={{ value: 'Range (km)', position: 'insideBottomRight', offset: 0, dy: 10 }} />
                        <YAxis dataKey="tl" type="number" tickFormatter={this.tLDataFormater} domain={[dataMin => dataMin/1.1, 'dataMax']} tickCount={20} label={{ value: 'TL (dB)', position: 'insideTopLeft', dx: -10, dy: -35 }} />
                        <Tooltip />
                        <Legend />
                        <Line dataKey="tl" data={data} name="Transmission loss" dot={false} stroke="#8884d8" />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </>);
    }
}