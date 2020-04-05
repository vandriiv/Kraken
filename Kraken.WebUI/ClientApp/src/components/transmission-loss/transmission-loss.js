import React, { Component } from 'react';
import { Multiselect } from 'multiselect-react-dropdown';
import TransmissionLossChart from '../transmission-loss-chart';
import TransmissionLossTable from '../transmission-loss-table';

import './transmission-loss.css';

export default class TransmissionLoss extends Component {
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

    onReceiverSelectChange = (_, selectedItem) => {
        this.setState({
            selectedReceiverDepth: selectedItem
        });
    }

    onSourceSelectChange = (_, selectedItem) => {
        this.setState({
            selectedSourceDepth: selectedItem
        });
    }

    mapData = (transmissionLoss, ranges, sourceIdx, receiverIdx) => {
        return transmissionLoss[sourceIdx]
            .tlAtReceiverDepths[receiverIdx]
            .transmissionLoss.map((val, idx) => {
                return { range: ranges[idx], tl: val };
            });
    }


    render() {
        const { transmissionLoss, ranges } = this.props;

        const { selectedSourceDepth, selectedReceiverDepth } = this.state;

        if (selectedReceiverDepth === null || selectedReceiverDepth === null) {
            return null;
        }

        const data = this.mapData(transmissionLoss, ranges, selectedSourceDepth.idx, selectedReceiverDepth.idx);

        return (<>
            <div className='d-flex justify-content-between'>
                <div className='tl-selects-wrapper'>
                    <div>
                        <p>Source depth (m)</p>
                        <Multiselect id="source-depth-select" className="single-select"
                            options={this.sourceDepthOptions}
                            singleSelect
                            displayValue="depth"
                            onSelect={this.onSourceSelectChange}
                            selectedValues={[selectedSourceDepth]}
                            avoidHighlightFirstOption={true}
                        />
                    </div>
                    <div>
                        <p>Receiver depth (m)</p>
                        <Multiselect id="receiver-depth-select" className="single-select"
                            options={this.receiverDepthOptions}
                            singleSelect
                            displayValue="depth"
                            onSelect={this.onReceiverSelectChange}
                            selectedValues={[selectedReceiverDepth]}
                            avoidHighlightFirstOption={true}
                        />
                    </div>
                </div>
            </div>
            <TransmissionLossTable data={data} sourceDepth={selectedSourceDepth.depth} receiverDepth={selectedReceiverDepth.depth} />
            <TransmissionLossChart data={data} sourceDepth={selectedSourceDepth.depth} receiverDepth={selectedReceiverDepth.depth} />
        </>);
    }
}