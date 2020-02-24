import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { distinctColors } from '../../utilites/color-generator';

export default class ModesChart extends Component{

    colors = [];

    mapLines = (data) => {           
        return data.map((s, idx) => {
            return <Line dataKey="y" data={s.data} name={s.name} key={s.name} dot={false} stroke={this.colors[idx]} />
        }
      );
    };

    transformData = (data) => {
        const result = [];
        for (let i = 0; i < data[0].modes.length; i++) {
            result.push([]);
        }

        data.forEach(d => {
            d.modes.forEach((m, idx) => {
                result[idx].push({ x: d.depth, y: m });
            });
        }); 

        return result.map((val, idx) => {
            return { name: "Mode " + (idx+1), data: val }
        });     
    };


    render() {
        const { data } = this.props;
        const transformedData = this.transformData(data);
        this.colors = distinctColors(data[0].modes.length).map(x => `rgb(${x[0]}, ${x[1]},${x[2]})`);      

        return (
            <div className="lg-chart-wrapper">
                <ResponsiveContainer height={700} width="100%">
                    <LineChart margin={{left: 10 }}> 
                    <CartesianGrid strokeDasharray="10 10" />
                        <XAxis dataKey="x" type="number" domain={['dataMin', 'dataMax']} label={{ value: 'Depth (m)', position: 'insideBottomRight', offset: 0, dy: 10 }}/>
                        <YAxis dataKey="y" type="number" tickCount={20} label={{ value: 'Mode amplitude', angle: -90, position: 'insideTopLeft', dx: -15, dy:115 }} />
                    <Tooltip />
                    <Legend />
                    {this.mapLines(transformedData)}
                    </LineChart>
                </ResponsiveContainer>
            </div>);
    }
}