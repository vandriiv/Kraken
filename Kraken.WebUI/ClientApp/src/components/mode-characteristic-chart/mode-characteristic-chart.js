import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default class ModeCharacteristicChart extends Component{

    mapData = (data, chartName) => {
        const result = { name: chartName, data: [] };

        result.data = data.map((d, idx) => {
            return { x: (idx + 1), y: d };
        });

        return result;
    };

    dataFormater = (number) => {
        return number.toFixed(3).toString();
    }

    render() {
        const { data, yAxisLabelValue, chartName } = this.props;

        const chartData = this.mapData(data, chartName);       

        return (
            <div className="lg-chart-wrapper">
                <ResponsiveContainer height={700} width="100%">
                    <LineChart margin={{ left:10, top:35}}>
                        <CartesianGrid strokeDasharray="10 10" />
                        <XAxis dataKey="x" type="number" domain={['dataMin', 'dataMax']} label={{ value: '№ of mode', position: 'insideBottomRight', offset: 0, dy: 10 }} />
                        <YAxis dataKey="y" tickFormatter={this.dataFormater} type="number" domain={['dataMin', 'dataMax']} tickCount={20} label={{ value: yAxisLabelValue, position: 'insideTopLeft',dx:-10, dy:-35}} />
                        <Tooltip />
                        <Legend />
                        <Line dataKey="y" data={chartData.data} name={chartData.name} key={chartData.name} dot={false} stroke="#3030f0" />
                    </LineChart>
                </ResponsiveContainer>
            </div>);
    }
}