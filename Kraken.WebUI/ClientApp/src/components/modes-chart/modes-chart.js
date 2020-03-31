import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { distinctColors } from '../../utilites/color-generator';
import { Multiselect } from 'multiselect-react-dropdown';
import { exportChart } from '../../utilites/export-chart';
import { Button } from 'reactstrap';

import './modes-chart.css';

export default class ModesChart extends Component {

    state = {
        modesToDisplay: []
    };

    colors = [];
    modesOptions = [];
    chartId = "modes-chart";

    componentDidMount() {
        const { modesCount } = this.props;
        this.updateModesDependentData(modesCount);
    }

    componentDidUpdate(prevProps) {
        const { modesCount } = this.props;
        if (prevProps.modesCount !== modesCount) {
            this.updateModesDependentData(modesCount);
        }
    }

    updateModesDependentData = (modesCount) => {
        this.modesOptions = [...Array(modesCount).keys()].map(x => (x + 1).toString());
        const maxModes = Math.min(10, modesCount);
        this.setState({
            modesToDisplay: [...Array(maxModes).keys()].map(x => (x + 1).toString())
        });

        this.colors = distinctColors(modesCount).map(x => `rgb(${x[0]}, ${x[1]},${x[2]})`);
    }

    mapLines = (data) => {
        return data.map((s, idx) => {
            return <Line dataKey="y" data={s.data} name={s.name} key={s.name} dot={false} stroke={this.colors[idx]} />
        }
        );
    };

    mapData = (data, modesCount, toDisplay) => {
        const result = [];
        for (let i = 0; i < modesCount; i++) {
            result.push([]);
        }

        data.forEach(d => {
            d.modes.forEach((m, idx) => {
                result[idx].push({ x: d.depth, y: m });
            });
        });

        return result
            .filter((_, idx) => {
                return toDisplay.includes((idx + 1).toString())
            })
            .map((val, idx) => {
                return { name: "Mode " + toDisplay[idx], data: val }
            });
    };

    onSelect = (selectedList) => {
        this.setState({
            modesToDisplay: selectedList
        });
    }

    onRemove = (selectedList) => {
        this.setState({
            modesToDisplay: selectedList
        })
    }


    render() {
        const { modes, modesCount } = this.props;
        const { modesToDisplay } = this.state;

        const chartData = this.mapData(modes, modesCount, modesToDisplay);

        return (
            <>
                <div className='d-flex justify-content-between'>
                    <div className="select-wrapper">
                        <p>Select modes to display on chart</p>
                        <Multiselect
                            placeHolder="Select modes to display on chart"
                            options={this.modesOptions}
                            onSelect={this.onSelect}
                            onRemove={this.onRemove}
                            selectedValues={modesToDisplay}
                            isObject={false}
                        />
                    </div>
                    <div className="align-self-end">
                        <Button outline color="success" onClick={() => exportChart(this.chartId)}>Save as image</Button>
                    </div>
                </div>
                <div className="lg-chart-wrapper">
                    <ResponsiveContainer height={700} width="100%" id={this.chartId} >
                        <LineChart margin={{ left: 10, top: 35 }}>
                            <CartesianGrid strokeDasharray="10 10" />
                            <XAxis dataKey="x" type="number" domain={['dataMin', 'dataMax']} label={{ value: 'Depth (m)', position: 'insideBottomRight', offset: 0, dy: 10 }} />
                            <YAxis dataKey="y" type="number" tickCount={20} label={{ value: 'Mode amplitude', position: 'insideTopLeft', dx: -10, dy: -35 }} />
                            <Tooltip />
                            <Legend className="chart-legend" />
                            {this.mapLines(chartData)}
                        </LineChart>
                    </ResponsiveContainer>
                </div>
            </>);
    }
}