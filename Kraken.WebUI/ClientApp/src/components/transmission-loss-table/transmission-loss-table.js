﻿import React, { Component } from 'react';
import { Table, Button, ButtonGroup } from 'reactstrap';
import { exportTableToCsv } from '../../utilites/export-table-to-csv';
import { exportTableToExcel } from '../../utilites/export-table-to-excel';
import { exportAsJson } from '../../utilites/export-as-json';
import { exportAsXml } from '../../utilites/export-as-xml';
import { jsonToXml } from '../../utilites/json-to-xml-convert';

export default class TransmissionLossTable extends Component {
    tableId = "transmission-loss-table";

    render() {
        const { data, receiverDepth, sourceDepth } = this.props;      

        const tableExportName = `transmission-loss-sd-${parseFloat(sourceDepth).toFixed(5)}-rd-${parseFloat(receiverDepth).toFixed(5)}`;
        const jsonText = JSON.stringify(data);

        return (
            <div>
                <div className='d-flex justify-content-end'>
                    <div>
                        <ButtonGroup>
                            <Button outline color="warning" onClick={() => exportAsJson(jsonText, tableExportName)}>Save as JSON</Button>
                            <Button outline color="primary" onClick={() => exportAsXml(jsonToXml(jsonText), tableExportName)}>Save as XML</Button>
                            <Button outline color="success" onClick={() => exportTableToCsv(this.tableId, tableExportName)}>Save as .csv</Button>
                            <Button outline color="success" onClick={() => exportTableToExcel(this.tableId, tableExportName)}>Save as .xls</Button>
                        </ButtonGroup>
                    </div>
                </div>
                <div className="overflow-table">
                    <Table responsive bordered hover id={this.tableId}>
                        <thead>
                            <tr>
                                <th>Range (m)</th>
                                <th>Transmission loss (dB)</th>
                            </tr>
                        </thead>
                        <tbody>
                            {data.map((d, idx) => {
                                return (<tr key={idx}>
                                    <td>{d.range}</td>
                                    <td>{d.tl}</td>
                                </tr>);
                            })}
                        </tbody>
                    </Table>
                </div>
            </div>);
    }
}