import React, { Component } from 'react';
import SoundSpeedChart from '../sound-speed-chart';
import SoundSpeedTable from '../sound-speed-table';

export default class SoundSpeed extends Component {

    mapSSP = (data) => {
        return data.map(d => {
            return { depth: d[0], speed: d[1] };
        });
    };

    render() {
        const { data } = this.props;

        const mappedData = this.mapSSP(data);

        return (<>       
            <SoundSpeedTable data={mappedData} />         
            <SoundSpeedChart data={mappedData} />
        </>);
    }
}