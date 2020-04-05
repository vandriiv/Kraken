import React, { Component } from 'react';
import { Table, Button, ButtonGroup } from 'reactstrap';
import { exportTableToCsv } from '../../utilites/export-table-to-csv';
import { exportTableToExcel } from '../../utilites/export-table-to-excel';
import { exportAsJson } from '../../utilites/export-as-json';
import { exportAsXml } from '../../utilites/export-as-xml';
import { jsonToXml } from '../../utilites/json-to-xml-convert';

export default class ModesTable extends Component {
    tableId = "modes-table";
    tableName = "mode-amplidutes";

    exportJson = [];

    mapModes = (modes, modesCount) => {
        return [...Array(modesCount).keys()].map(idx => {
            return (<tr key={idx}>
                <td>{idx + 1}</td>
                {this.makeSingleRow(modes, idx)}
            </tr>);
        });
    };

    makeSingleRow = (modes, idx) => {
        return modes.map((m, i) => {
            return (<td key={i}>{m.modes[idx]}</td>)
        });
    }

    mapDepths = (modes) => {
        return modes.map((x, idx) => {
            return <th key={idx}>{x.depth.toFixed(3)}</th>;
        });
    }

    render() {
        const { modes, modesCount } = this.props;
        const jsonText = JSON.stringify(modes);

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
                    <Table bordered hover id={this.tableId} >
                        <thead>
                            <tr>
                                <th></th>
                                <th colSpan={modes.length}>Depth (m)</th>
                            </tr>
                            <tr>
                                <th colSpan="1">Number of mode</th>
                                {this.mapDepths(modes)}
                            </tr>
                        </thead>
                        <tbody>
                            {this.mapModes(modes, modesCount)}
                        </tbody>
                    </Table>
                </div>
            </div>
        );
    }
}
