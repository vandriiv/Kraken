import React, { Component } from 'react';
import { Table } from 'reactstrap';

export default class ModesTable extends Component {

    mapModes = (modes) => {
        return modes.map((val, idx) => {
            return (<tr key={idx}>
                <td>{idx + 1}</td>
                <td>{val.depth}</td>
                <td><ol>{val.modes.map((m, li_idx) => <li key={li_idx}>{m}</li>)}</ol></td>
            </tr>)
        });
    };

    render() {
        const { modes } = this.props;

        return (<Table>
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
        </Table>);
    }
}
