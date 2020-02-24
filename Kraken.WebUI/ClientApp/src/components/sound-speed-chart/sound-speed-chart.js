import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default class SoundSpeedChart extends Component {

    transformData = (data) => {
        const result = {};
        result.name = "Sound speed";

        result.data = data.map(d => {
            return { y: d[0], x: d[1] };
        });

        return result;
    };

    render() {
        const { data } = this.props;       
        const transformedData = this.transformData(data);      

        return (
            <div className="lg-chart-wrapper">
                <ResponsiveContainer height={700} width="100%">
                    <LineChart margin={{ left: 10 }} layout="vertical">
                        <CartesianGrid strokeDasharray="10 10" />
                        <XAxis dataKey="x" type="number" domain={[1460, 1600]} label={{ value: 'Sound speed (m/s)', position: 'insideBottomRight', offset: 0, dy: 10 }} />
                        <YAxis dataKey="y" type="number" tickCount={20} label={{ value: 'Depth (m)', angle: -90, position: 'insideTopLeft', dx: -10, dy: 65 }} />
                        <Tooltip />
                        <Legend />
                        <Line dataKey="x" data={transformedData.data} name={transformedData.name} key={transformedData.name} dot={false} stroke="#3030f0" />
                    </LineChart>
                </ResponsiveContainer>
            </div>);
    }
}