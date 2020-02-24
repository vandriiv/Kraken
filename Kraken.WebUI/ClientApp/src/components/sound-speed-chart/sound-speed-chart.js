import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default class SoundSpeedChart extends Component {

    mapData = (data) => {
        const result = {};
        result.name = "Sound speed";

        result.data = data.map(d => {
            return { y: d[0], x: d[1] };
        });

        return result;
    };

    render() {
        const { data } = this.props;       
        const chartData = this.mapData(data);      

        return (
            <div className="lg-chart-wrapper">
                <ResponsiveContainer height={700} width="100%">
                    <LineChart margin={{ left: 10, top: 35 }} layout="vertical">
                        <CartesianGrid strokeDasharray="10 10" />
                        <XAxis dataKey="x" type="number" domain={['dataMin', 'dataMax']} label={{ value: 'Sound speed (m/s)', position: 'insideBottomRight', offset: 0, dy: 10 }} />
                        <YAxis dataKey="y" type="number" domain={['dataMin', 'dataMax']} tickCount={20} label={{ value: 'Depth (m)', position: 'insideTopLeft', dx: -10, dy: -35}} />
                        <Tooltip />
                        <Legend />
                        <Line dataKey="x" data={chartData.data} name={chartData.name} key={chartData.name} dot={false} stroke="#3030f0" />
                    </LineChart>
                </ResponsiveContainer>
            </div>);
    }
}