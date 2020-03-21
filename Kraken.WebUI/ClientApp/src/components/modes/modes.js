import React, { Component } from 'react';
import ModesTable from '../modes-table/modes-table';
import ModesChart from '../modes-chart';

export default class Modes extends Component {

    render() {
        const { modes, modesCount } = this.props;

        return (<>
            <ModesTable modes={modes} modesCount={modesCount} />
            <ModesChart modes={modes} modesCount={modesCount} />
        </>);
    }
}