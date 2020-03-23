import React, { Component } from 'react';
import { Table, Button, ButtonGroup } from 'reactstrap';
import { exportTableToCsv } from '../../utilites/export-table-to-csv';
import { exportTableToExcel } from '../../utilites/export-table-to-excel';
import { exportAsJson } from '../../utilites/export-as-json';
import { exportAsXml } from '../../utilites/export-as-xml';
import { jsonToXml } from '../../utilites/json-to-xml-convert';

export default class KrakenResultTable extends Component {
    tableId = "kraken-result-table";
    tableName = "wavenumber-scater_loss-group_speed-phase_speed";

    mapData = (data) => {
        return data.k.map((_, idx) => {
            return (<tr key={idx} >
                <td > {idx + 1}</td>
                <td>{data.k[idx]}</td>
                <td>{data.alpha[idx]}</td>
                <td>{data.phaseSpeed[idx]}</td>
                <td>{data.groupSpeed[idx]}</td>
            </tr>);
        });
    };

    mapDataForExport = (data) => {
        return data.k.map((kVal, idx) => {
            return {
                mode: idx + 1,
                k: kVal,
                alpha: data.alpha[idx],
                groupSpeed: data.groupSpeed[idx],
                phaseSpeed: data.phaseSpeed[idx]
            };
        });
    };

    render() {
        const { data } = this.props;
        const jsonText = JSON.stringify(this.mapDataForExport(data));

        return (
            <div>
                <div className='d-flex justify-content-end'>
                    <div>
                        <ButtonGroup>
                            <Button outline color="warning" onClick={() => exportAsJson(jsonText, this.tableName)}>Save as JSON</Button>
                            <Button outline color="primary" onClick={() => exportAsXml(jsonToXml(jsonText), this.tableName)}>Save as XML</Button>
                            <Button outline color="success" onClick={() => exportTableToCsv(this.tableId, this.tableName)}>Save as .csv</Button>
                            <Button outline color="success" onClick={() => exportTableToExcel(this.tableId, this.tableName)}>Save as .xls</Button>
                        </ButtonGroup>
                    </div>
                </div>
                <div className="overflow-table">
                    <Table responsive bordered hover id={this.tableId} >
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
                    </Table>
                </div>
            </div>);
    }
}