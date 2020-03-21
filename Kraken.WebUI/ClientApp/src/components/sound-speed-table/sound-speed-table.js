import React, { Component } from 'react';
import { Table } from 'reactstrap';
import { exportTableToCsv } from '../../utilites/export-table-to-csv';
import { exportTableToExcel } from '../../utilites/export-table-to-excel';

export default class SoundSpeedTable extends Component {
    tableId = "sound-speed-table";
    tableName = "Sound speed";

    render() {
        const { data } = this.props;

        return (
            <Table responsive bordered hover id={this.tableId}>
            <thead>
                <tr>
                    <th>Depth (m)</th>
                    <th>Sound speed (m/s)</th>
                </tr>
            </thead>
            <tbody>
                    {data.map((d, idx) => {
                        return (<tr key={idx}>
                        <td>{d.depth}</td>
                        <td>{d.speed}</td>
                    </tr>);
                })}                
            </tbody>
        </Table>);
    }
}