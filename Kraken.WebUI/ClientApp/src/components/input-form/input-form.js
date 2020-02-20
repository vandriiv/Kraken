import React, { Component } from 'react';
import { Form, FormGroup, Label, Input, Button,Row,Col } from 'reactstrap';
import Select from '../select';
import InputErrorsList from '../input-errors-list';
import './input-form.css';

export default class InputForm extends Component {

    state = {
        frequency: 0,
        nModes: 0,
        nMedia: 0,
        topBCType: '',
        interpolationType: '',
        attenuationUnits: '',
        isTopAcoustic: false,
        isTopTwersky: false,
        isBottomAcoustic: false,
        isVolumeAttenuatonAdded: false,
        zt: 0,
        cpt: 0,
        cst: 0,
        rhot: 0,
        apt: 0,
        ast: 0,
        bumDen: 0,
        eta: 0,
        xi: 0,
        mediumInfo: [],
        ssp: [],
        bottomBCType: '',
        zb: 0,
        cpb: 0,
        csb: 0,
        rhob: 0,
        apb: 0,
        asb: 0,
        cLow: 0,
        cHigh: 0,
        rMax: 0,
        nsd: 0,
        sd: [],
        nrd: 0,
        rd: [],
        error: null
    };

    interpolationTypes = [
        {
            key: 'C',
            name: 'C-linear'
        },
        {
            key: 'N',
            name: 'N2-linear'
        },
        {
            key: 'S',
            name: 'Cubic Spline'
        }];

    topBoundaryConditions = [
        {
            key: 'V',
            name: 'Vacuum above top'
        },
        {
            key: 'A',
            name: 'Acousto-elastic half-space'
        },
        {
            key: 'R',
            name: 'Perfectly rigid'
        },
        {
            key: 'S',
            name: 'Soft-boss Twersky scatter'
        },
        {
            key: 'H',
            name: 'Hard-boss Twersky scatter'
        },
        {
            key: 'T',
            name: 'Soft-boss Twersky scatter, amplitude only'
        },
        {
            key: 'I',
            name: 'Hard-boss Twersky scatter, amplitude only'
        }
    ];

    bottomBoundaryConditions = [
        {
            key: 'V',
            name: 'Vacuum above top'
        },
        {
            key: 'A',
            name: 'Acousto-elastic half-space'
        },
        {
            key: 'R',
            name: 'Perfectly rigid'
        }
    ];

    attenuationUnits = [
        {
            key: 'N',
            name: 'Nepers/m'
        },
        {
            key: 'F',
            name:'dB/(kmHz)'
        },
        {
            key: 'M',
            name:'dB/m'
        },
        {
            key: 'W',
            name:'dB/wavelength'
        },        
        {
            key: 'Q',
            name:'Quality factor'
        },
        {
            key: 'T',
            name:'Thorp attenuation formula'
        }
    ];

    //refactor
    handleTopBCTypeChange = (e) => {
        const { value } = e.target;
        if (value === 'A') {
            this.setState({
                isTopAcoustic: true,
                isTopTwersky: false,
                topBCType: value
            });
        }
        else if (value === 'S' || value === 'H' || value === 'T' || value === 'I') {
            this.setState({
                isTopAcoustic: false,
                isTopTwersky: true,
                topBCType: value
            });
        }
        else {
            this.setState({
                isTopAcoustic: false,
                isTopTwersky: false,
                topBCType: value
            });
        }        
    };

    handleBottomBCTypeChange = (e) => {
        const { value } = e.target;
        if (value === 'A') {
            this.setState({
                isBottomAcoustic: true,
                bottomBCType: value
            });
        }
        else {
            this.setState({
                isBottomAcoustic: false,
                bottomBCType: value
            });
        }
    };

    handleChange = (e) => {
        const { name, value } = e.target;

        this.setState({ [name]: value });
    }

    handleCheckboxChange = (e) => {
        const { name, checked } = e.target;

        this.setState({ [name]: checked });
    }

    onSubmit = (e) => {
        e.preventDefault();
        this.setState({ error: null });
        const data = this.validateAndFormatData();
        if (data !== null) {
            this.props.onSubmit(data);
        }
    }

    validateAndFormatData = () => {
        const error = {};
        let { frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, isVolumeAttenuatonAdded, zt, cpt,
            cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
            csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd } = this.state;

        if (frequency <= 0) {
            error.frequency = "Frequency ";
        }

        if (nModes <= 0) {
            error.nModes = "Number of modes ";
        }

        if (nMedia <= 0) {
            error.nModes = "Number of media ";
        }

        if (topBCType.length !== 1 || !this.topBoundaryConditions.some(x => x.key === topBCType)) {
            error.topBCType = "Top boundary conditions ";
        }

        if (interpolationType.length !== 1 || !this.interpolationTypes.some(x => x.key === interpolationType)) {
            error.interpolationType = "Interpolation type ";
        }

        if (topBCType === 'A' && zt < 0) {
            error.zt = "Z Top";
        }
        if (topBCType === 'A' && cpt < 0) {
            error.cpt = "CP Top";
        }
        if (topBCType === 'A' && cst < 0) {
            error.cst = "CP Top";
        }

        if (topBCType === 'A' && rhot < 0) {
            error.rhot = "RHO Top";
        }

        if (topBCType === 'A' && apt < 0) {
            error.apt = "AP Top";
        }

        if (topBCType === 'A' && ast < 0) {
            error.ast = "AS Top";
        }

        if ((topBCType === 'S' || topBCType === 'H' || topBCType === 'T' || topBCType === 'I') && bumDen < 0) {
            error.bumDen = "Bump density ";
        }

        if ((topBCType === 'S' || topBCType === 'H' || topBCType === 'T' || topBCType === 'I') && eta < 0) {
            error.eta = "Principal radius 1";
        }

        if ((topBCType === 'S' || topBCType === 'H' || topBCType === 'T' || topBCType === 'I') && xi < 0) {
            error.eta = "Principal radius 2";
        }

        if (bottomBCType.length !== 1 || !this.bottomBoundaryConditions.some(x => x.key === bottomBCType)) {
            error.bottomBCType = "Bottom boundary conditions";
        }

        if (bottomBCType === 'A' && zb < 0) {
            error.zb = "Z Bottom";
        }
        if (bottomBCType === 'A' && cpb < 0) {
            error.cpb = "CP Bottom";
        }
        if (bottomBCType === 'A' && csb < 0) {
            error.csb = "CP Bottom";
        }

        if (bottomBCType === 'A' && rhob < 0) {
            error.rhob = "RHO Bottom";
        }

        if (bottomBCType === 'A' && apb < 0) {
            error.apb = "AP Bottom";
        }

        if (bottomBCType === 'A' && asb < 0) {
            error.asb = "AS Bottom";
        }

        if (cLow <= 0) {
            error.cLow = "Lower phase speed limit";
        }

        if (cHigh <= 0) {
            error.cHigh = "Upper phase speed limit";
        }

        if (rMax <= 0) {
            error.rMax = "Maximum range";
        }

        if (nsd <= 0) {
            error.nsd = "Number of source depth";
        }

        if (nrd <= 0) {
            error.nrd = "Number of receiver depth";
        }

        try {
            sd = this.parseOneDimensionalArray(sd);
        }
        catch (e) {
            error.sd = "Source depth format";
        }

        try {
            rd = this.parseOneDimensionalArray(rd);
        }
        catch (e) {
            error.rd = "Source depth format";
        }

        try {
            mediumInfo = this.parseTwoDimensionalArray(mediumInfo);
        }
        catch (e) {
            error.mediumInfo = "Medium info format";
        }

        try {
            ssp = this.parseTwoDimensionalArray(ssp);
        }
        catch (e) {
            error.ssp = "SSP format";
        }

        if (!(Object.entries(error).length === 0 && error.constructor === Object)) {
            this.setState({ error: error });
            return null;
        }
        else {
            const addedVolumeAttenuation = isVolumeAttenuatonAdded === true ? 'T' : '';
            return {
                frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, addedVolumeAttenuation, zt, cpt,
                cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
                csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd
            };
        }
    }

    parseOneDimensionalArray = (str) => {
        const array = JSON.parse('[' + str + ']');
        if (!Array.isArray(array)) {
            throw "Given string is not array";
        }

        if (Array.isArray(array[0])) {
            throw "Given string is not one dimensional array";
        }

        return array;
    };


    parseTwoDimensionalArray = (str) => {
        const array = JSON.parse('[' + str + ']');
        if (!Array.isArray(array)) {
            throw "Given string is not array";
        }

        if (!Array.isArray(array[0])) {
            throw "Given string is not two dimensional array";
        }

        return array;
    };

    render() {

        const { isBottomAcoustic, isTopAcoustic, isTopTwersky, error } = this.state;        

        return (
            <Form onSubmit={this.onSubmit} >
                <FormGroup>
                    <Label for="frequency">Frequency (Hz)</Label>
                    <Input type="number" name="frequency" id="frequency" onChange={this.handleChange} placeholder="Frequency" required/>
                </FormGroup>
                <FormGroup>
                    <Label for="nModes">Number of modes</Label>
                    <Input type="number" name="nModes" id="nModes" onChange={this.handleChange} placeholder="Number of modes" required/>
                </FormGroup>                
                <FormGroup>
                    <Label for="nMedia">Number of media</Label>
                    <Input type="number" name="nMedia" id="nMedia" onChange={this.handleChange} placeholder="Number of media" required/>
                 </FormGroup>

                <Row form>                    
                    <Col md={6}>
                        <FormGroup>
                            <Select label={"Type of interpolation"} name={"interpolationType"} onChange={this.handleChange} options={this.interpolationTypes} />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Select label={"Type of top boundary condition"} name={"topBCType"} onChange={this.handleTopBCTypeChange} options={this.topBoundaryConditions} />
                        </FormGroup>
                    </Col>
                </Row>
                {isTopAcoustic ?
                    <Row form>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="zt">Depth (m)</Label>
                                <Input type="number" name="zt" id="zt" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="cpt">Top P-wave speed (m/s)</Label>
                                <Input type="number" name="cpt" id="cpt" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="cst">Top S-wave speed (m/s)</Label>
                                <Input type="number" name="cst" id="cst" onChange={this.handleChange} required />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="rhot">Top density (g/cm3)</Label>
                                <Input type="number" name="rhot" id="rhot" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="apt">Top P-wave attenuation</Label>
                                <Input type="number" name="apt" id="apt" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="ast">Top S-wave attenuation</Label>
                                <Input type="number" name="ast" id="ast" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                    </Row>
                    : null}
                {isTopTwersky ?
                    <Row form>
                        <Col md={4}>
                            <FormGroup>
                                <Label for="bumDen">Bump density (ridges/km)</Label>
                                <Input type="number" name="bumDen" id="bumDen" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={4}>
                            <FormGroup>
                                <Label for="eta">Principal radius 1 (m)</Label>
                                <Input type="number" name="eta" id="eta" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={4}>
                            <FormGroup>
                                <Label for="xi">Principal radius 2 (m)</Label>
                                <Input type="number" name="xi" id="xi" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                    </Row>
                    : null}
                <FormGroup>
                    <Select label={"Attenuation units"} name={"attenuationUnits"} onChange={this.handleChange} options={this.attenuationUnits} required />
                </FormGroup>
                <FormGroup check>
                    <Label check>
                        <Input type="checkbox" name="isVolumeAttenuatonAdded" id="isVolumeAttenuatonAdded" onChange={this.handleCheckboxChange} />{' '}
                        Add volume attenuation
                    </Label>
                </FormGroup>
                <FormGroup>
                    <Label for="mediumInfo">Medium info </Label>
                    <Input type="textarea" name="mediumInfo" id="mediumInfo" onChange={this.handleChange} required placeholder={"e.g. [300,0,3000], [200,0,500]"} />                  
                </FormGroup>
                <FormGroup>
                    <Label for="ssp">Sound speed profile</Label>
                    <Input type="textarea" name="ssp" id="ssp" onChange={this.handleChange} required placeholder={"e.g. [0,1500, 0, 1.0, 0, 0],[3000, 1500, 0, 1.0, 0, 0.00000],[3000, 1500, 0, 2.0000, 0, 0.00000],[5000, 1500, 0, 2.0000, 0, 0]"}/>
                </FormGroup>
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Select label={"Type of bottom boundary condition"} name={"bottomBCType"} onChange={this.handleBottomBCTypeChange} options={this.bottomBoundaryConditions} />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="sigma">Interfacial roughness (m)</Label>
                            <Input type="number" name="sigma" id="sigma" onChange={this.handleChange} required/>
                        </FormGroup>
                    </Col>
                </Row>
                {isBottomAcoustic ?
                    <Row form>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="zb">Depth (m)</Label>
                                <Input type="number" name="zb" id="zb" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="cpb">Bottom P-wave speed (m/s)</Label>
                                <Input type="text" name="cpb" id="cpb" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="csb">Bottom S-wave speed (m/s)</Label>
                                <Input type="text" name="csb" id="csb" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="rhob">Bottom density (g/cm3)</Label>
                                <Input type="text" name="rhob" id="rhob" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="apb">Bottom P-wave atten.</Label>
                                <Input type="text" name="apb" id="apb" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="asb">Bottom S-wave atten.</Label>
                                <Input type="text" name="asb" id="asb" onChange={this.handleChange} required/>
                            </FormGroup>
                        </Col>
                    </Row>
                    : null}
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="cLow">Lower phase speed limit (m/s)</Label>
                            <Input type="number" name="cLow" id="cLow" onChange={this.handleChange} required/>
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="cHigh">Upper phase speed limit (m/s)</Label>
                            <Input type="number" name="cHigh" id="cHigh" onChange={this.handleChange} required/>
                        </FormGroup>
                    </Col>
                </Row>
                <FormGroup>
                    <Label for="rMax">Maximum range (km)</Label>
                    <Input type="number" name="rMax" id="rMax" onChange={this.handleChange} required/>
                </FormGroup>
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="nsd">The number of soure depths</Label>
                            <Input type="number" name="nsd" id="nsd" onChange={this.handleChange} required/>
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="sd">The source depths (m)</Label>
                            <Input type="textarea" name="sd" id="sd" onChange={this.handleChange} required/>
                        </FormGroup>
                    </Col>
                </Row>
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="nrd">The number of receiver depths</Label>
                            <Input type="number" name="nrd" id="nrd" onChange={this.handleChange} required/>
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="rd">The receiver depths (m)</Label>
                            <Input type="textarea" name="rd" id="rd" onChange={this.handleChange} required/>
                        </FormGroup>
                    </Col>
                </Row>
                <Button outline color="secondary">Submit</Button>
                <div className="validation-errors-list">
                {error !== null ?
                    <InputErrorsList error={error} />
                        : null}
                </div>
            </Form>
            );
    }
}