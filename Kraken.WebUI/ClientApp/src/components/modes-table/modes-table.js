import React, { Component } from 'react';
import { Table } from 'reactstrap';

export default class ModesTable extends Component {

    mapModes = (modes) => {
        const modesCount = modes[0].modes.length;
        return [...Array(modesCount).keys()].map(idx => {
            return (<tr>
                <td>{idx + 1}</td>
                {this.makeSingleRow(modes, idx)}
            </tr>);          
        });       
    };

    makeSingleRow = (modes, idx) => {
        return modes.map(m => {
            return (<td>{m.modes[idx]}</td>)
        });
    }

    mapDepths = (modes) => {
        return modes.map(x => {
            return <th>{x.depth.toFixed(3)}</th>;
        });
    }

    render() {
        const { modes } = this.props;
        console.log(modes);

       /* return (<Table>
            <thead>                
                <tr>
                    <th>#</th>
                    <th>Depth</th>
                    <th>Modes</th>
                </tr>
            </thead>
            <tbody>
                {this.mapModes(modes)}
            </tbody>
        </Table>);*/

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
                    {this.mapModes(modes)}
                </tbody>
        </Table>);
    }
}
