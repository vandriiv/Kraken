import React, { Component } from 'react';
import { Table } from 'reactstrap';

export default class ModesTable extends Component {

    mapModes = (modes,modesCount) => {       
        return [...Array(modesCount).keys()].map(idx => {
            return (<tr key={idx}>
                <td>{idx + 1}</td>
                {this.makeSingleRow(modes, idx)}
            </tr>);          
        });       
    };

    makeSingleRow = (modes, idx) => {
        return modes.map((m,i) => {
            return (<td key={i}>{m.modes[idx]}</td>)
        });
    }

    mapDepths = (modes) => {
        return modes.map((x, idx) => {
            return <th key={idx}>{x.depth.toFixed(3)}</th>;
        });
    }

    render() {
        const { modes, modesCount } = this.props.data;       

        return (
            <Table responsive bordered hover>
            <thead>
                    <tr>
                        <th></th>
                        <th colSpan={modes.length}>Depth (m)</th>
                </tr>
                <tr>
                        <th colSpan="1">№ mode</th>
                        {this.mapDepths(modes)}
                 </tr>
                </thead>
                <tbody>
                    {this.mapModes(modes, modesCount)}
                </tbody>
        </Table>);
    }
}
