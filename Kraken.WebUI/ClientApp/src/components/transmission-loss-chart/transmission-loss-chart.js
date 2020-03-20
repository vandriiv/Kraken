import React, { Component } from 'react';
import { Multiselect } from 'multiselect-react-dropdown';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default class TransmissionLossChart extends Component {

    state = {
        selectedReceiverDepth: null,
        selectedSourceDepth: null,
    };

    sourceDepthOptions = [];
    receiverDepthOptions = [];

    componentDidMount() {
        const { sourceDepths, receiverDepths } = this.props;
        this.sourceDepthOptions = this.mapOptions(sourceDepths);
        this.receiverDepthOptions = this.mapOptions(receiverDepths);
        this.setState({
            selectedReceiverDepth: this.receiverDepthOptions[0],
            selectedSourceDepth: this.sourceDepthOptions[0]
        });
    }

    componentDidUpdate(prevProps) {
        const { sourceDepths, receiverDepths } = this.props;

        if (sourceDepths !== prevProps.sourceDepths || receiverDepths !== prevProps.receiverDepths) {
            this.sourceDepthOptions = this.mapOptions(sourceDepths);
            this.receiverDepthOptions = this.mapOptions(receiverDepths);
            this.setState({
                selectedReceiverDepth: this.receiverDepthOptions[0],
                selectedSourceDepth: this.sourceDepthOptions[0]
            });
        }
    }

    mapOptions = (arr) => {
        return arr.map((x, idx) => {
            return { idx: idx, depth: x.toFixed(8) };
        })
    }

    onReceiverSelectChange = (selectedList, selectedItem) => {
        this.setState({
            selectedReceiverDepth: selectedItem
        });
    }

    onSourceSelectChange = (selectedList, selectedItem) => {
        this.setState({
            selectedSourceDepth: selectedItem
        });
    }

    mapData = (transmissionLoss, ranges, sourceIdx, receiverIdx) => {      

        return transmissionLoss[sourceIdx]
            .tlAtReceiverDepths[receiverIdx]
            .transmissionLoss.map((val, idx) => {
                return { x: ranges[idx], y: val };
        });
    }

    tLDataFormater = (number) => {
        return number.toFixed(3).toString();
    }

    rangeDataFormatter = (number) => {
        return number / 1000;
    }

    render() {
        const { transmissionLoss, ranges } = this.props;
        const { selectedSourceDepth, selectedReceiverDepth } = this.state;      

        if (selectedReceiverDepth === null || selectedReceiverDepth === null) {
            return null;
        }      

        console.log(this.state);
        const chartData = this.mapData(transmissionLoss, ranges, selectedSourceDepth.idx, selectedReceiverDepth.idx);
       
        return (<>
            <div className='tl-selects-wrapper'>
                <Multiselect className="single-select"
                    options={this.sourceDepthOptions}
                    singleSelect
                    displayValue="depth"
                    onSelect={this.onSourceSelectChange}
                    selectedValues={[selectedSourceDepth]}
                    avoidHighlightFirstOption={true}
                />
                <Multiselect className="single-select"
                    options={this.receiverDepthOptions}
                    singleSelect
                    displayValue="depth"
                    onSelect={this.onReceiverSelectChange}
                    selectedValues={[selectedReceiverDepth]}
                    avoidHighlightFirstOption={true}
                />
            </div>

            <div className="lg-chart-wrapper">
                <ResponsiveContainer height={700} width="100%">
                    <LineChart margin={{ left: 10, top: 35 }}>
                        <CartesianGrid strokeDasharray="10 10" />
                        <XAxis dataKey="x" type="number" tickFormatter={this.rangeDataFormatter} domain={['dataMin', 'dataMax']} label={{ value: 'Range (km)', position: 'insideBottomRight', offset: 0, dy: 10 }} />
                        <YAxis dataKey="y" type="number" tickFormatter={this.tLDataFormater} domain={['dataMin', 'dataMax']} tickCount={20} label={{ value: 'TL (dB)', position: 'insideTopLeft', dx: -10, dy: -35 }} />
                        <Tooltip />
                        <Legend />
                        <Line dataKey="y" data={chartData} name="Transmission loss" dot={false} stroke="#8884d8" />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </>);
    }
}