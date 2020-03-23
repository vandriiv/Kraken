import React, { Component } from 'react';
import { Table, Button, ButtonGroup } from 'reactstrap';
import { exportTableToCsv } from '../../utilites/export-table-to-csv';
import { exportTableToExcel } from '../../utilites/export-table-to-excel';
import { exportAsJson } from '../../utilites/export-as-json';
import { exportAsXml } from '../../utilites/export-as-xml';
import { jsonToXml } from '../../utilites/json-to-xml-convert';

export default class SoundSpeedTable extends Component {
    tableId = "sound-speed-table";
    tableName = "Sound speed";

    render() {
        const { data } = this.props;
        const jsonText = JSON.stringify(data);

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
                    </Table>
                </div>
            </div>
        );
    }
}