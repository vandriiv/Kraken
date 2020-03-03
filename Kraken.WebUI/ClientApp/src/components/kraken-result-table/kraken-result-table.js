import React, { Component } from 'react';
import { Table } from 'reactstrap';

export default class KrakenResultTable extends Component {

    mapData = (data) => {
        return data.k.map((_, idx) => {
            return (<tr key={idx} >
                <td > { idx + 1}</td>
                <td>{data.k[idx]}</td>
                <td>{data.alpha[idx]}</td>
                <td>{data.phaseSpeed[idx]}</td>
                <td>{data.groupSpeed[idx]}</td>
            </tr>);
        });
    };

    render() {
        const { data } = this.props;
        return (<Table responsive bordered hover>
            <thead>
                <tr>
                    <th>#</th>
                    <th>k (1/m)</th>
                    <th>alpha (1/m)</th>
                    <th>Phase speed (m/s)</th>
                    <th>Group speed (m/s)</th>
                </tr>
            </thead>
            <tbody>
                {this.mapData(data)}
            </tbody>
        </Table>);
    }
}