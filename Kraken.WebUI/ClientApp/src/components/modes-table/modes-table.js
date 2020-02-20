import React, { Component } from 'react';
import { Table } from 'reactstrap';

export default class ModesTable extends Component {

    mapModes = (modes) => {
        return Object.keys(modes).map((key,idx) => {
            return (<tr>
                <td>{idx + 1}</td>
                <td>{key}</td>
                <td><ol>{modes[key].map(m => <li>{m}</li>)}</ol></td>
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
