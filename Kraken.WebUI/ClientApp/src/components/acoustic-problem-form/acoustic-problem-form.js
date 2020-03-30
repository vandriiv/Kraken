import React, { Component } from 'react';
import { Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import Select from '../select';
import InputErrorsList from '../input-errors-list';
import './acoustic-problem-form.css';
import { readJsonFile } from '../../utilites/json-file-reader';
import { exportAsJson } from '../../utilites/export-as-json';
import ErrorMessage from '../error-message';


export default class AcousticProblemForm extends Component {
    state = {
        frequency: 0,
        nModes: 0,
        nMedia: 0,
        topBCType: '',
        interpolationType: '',
        attenuationUnits: '',       
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
        calculateTransmissionLoss: false,
        sourceType: '',
        modesTheory: '',
        nModesForField: 0,       
        nr: 0,
        r: [],
        nsdField: 0,
        sdField: [],
        nrdField: 0,
        rdField: [],
        nrr: 0,
        rr: [],
        error: null,
        uploadFileError:null
    };   

    fileInputRef = React.createRef();

    constructor(props) {
        super(props);
        const { acousticProblemData } = props;

        if (acousticProblemData) {            

            this.state = { ...acousticProblemData, error: null, uploadFileError: null, hasInitValue: true };
        }
    }

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
            name: 'dB/(kmHz)'
        },
        {
            key: 'M',
            name: 'dB/m'
        },
        {
            key: 'W',
            name: 'dB/wavelength'
        },
        {
            key: 'Q',
            name: 'Quality factor'
        },
        {
            key: 'T',
            name: 'Thorp attenuation formula'
        }
    ];

    sourceTypes = [
        {
            key: 'R',
            name: 'Point source'
        },
        {
            key: 'X',
            name: 'Line source'
        }
    ];

    modesTheories = [
        {
            key: 'A',
            name: 'Adiabatic mode theory'
        },
        {
            key: 'C',
            name: 'Coupled mode theory'
        }
    ];    

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

    isTwersky = (bcType) => (bcType === 'S' || bcType === 'H' || bcType === 'T' || bcType === 'I');

    validateAndFormatData = () => {
        const error = {};
        let { frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, isVolumeAttenuatonAdded, zt, cpt,
            cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
            csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd,
            calculateTransmissionLoss, sourceType, modesTheory, nModesForField,
            nr, r, nsdField, sdField, nrdField, rdField, nrr, rr } = this.state;      
       

        if (frequency <= 0) {
            error.frequency = "Frequency must be greater than 0";
        }

        if (nModes <= 0) {
            error.nModes = "Number of modes must be greater than 0";
        }

        if (nMedia <= 0) {
            error.nModes = "Number of media must be greater than 0";
        }

        if (topBCType.length !== 1 || !this.topBoundaryConditions.some(x => x.key === topBCType)) {
            error.topBCType = "Top boundary condition is required";
        }

        if (interpolationType.length !== 1 || !this.interpolationTypes.some(x => x.key === interpolationType)) {
            error.interpolationType = "Interpolation type is required";
        }

        if (topBCType === 'A' && zt < 0) {
            error.zt = "Z Top must be greater than or equal 0";
        }
        if (topBCType === 'A' && cpt < 0) {
            error.cpt = "CP Top must be greater than or equal 0";
        }
        if (topBCType === 'A' && cst < 0) {
            error.cst = "CP Top must be greater than or equal 0";
        }

        if (topBCType === 'A' && rhot < 0) {
            error.rhot = "RHO Top must be greater than or equal 0";
        }

        if (topBCType === 'A' && apt < 0) {
            error.apt = "AP Top must be greater than or equal 0";
        }

        if (topBCType === 'A' && ast < 0) {
            error.ast = "AS Top must be greater than or equal 0";
        }

        if (this.isTwersky(topBCType) && bumDen < 0) {
            error.bumDen = "Bump density must be greater than or equal 0";
        }

        if (this.isTwersky(topBCType) && eta < 0) {
            error.eta = "Principal radius 1 must be greater than or equal 0";
        }

        if (this.isTwersky(topBCType) && xi < 0) {
            error.eta = "Principal radius 2 must be greater than or equal 0";
        }

        if (bottomBCType.length !== 1 || !this.bottomBoundaryConditions.some(x => x.key === bottomBCType)) {
            error.bottomBCType = "Bottom boundary conditions are required";
        }

        if (bottomBCType === 'A' && zb < 0) {
            error.zb = "Z Bottom must be greater than or equal 0";
        }
        if (bottomBCType === 'A' && cpb < 0) {
            error.cpb = "CP Bottom must be greater than or equal 0";
        }
        if (bottomBCType === 'A' && csb < 0) {
            error.csb = "CP Bottom must be greater than or equal 0";
        }

        if (bottomBCType === 'A' && rhob < 0) {
            error.rhob = "RHO Bottom must be greater than or equal 0";
        }

        if (bottomBCType === 'A' && apb < 0) {
            error.apb = "AP Bottom must be greater than or equal 0";
        }

        if (bottomBCType === 'A' && asb < 0) {
            error.asb = "AS Bottom must be greater than or equal 0";
        }

        if (cLow <= 0) {
            error.cLow = "Lower phase speed limit must be greater than 0";
        }

        if (cHigh <= 0) {
            error.cHigh = "Upper phase speed limit must be greater than 0";
        }

        if (rMax < 0) {
            error.rMax = "Maximum range must be greater than or equal 0";
        }

        if (nsd <= 0) {
            error.nsd = "Number of source depth must be greater than 0";
        }

        if (nrd <= 0) {
            error.nrd = "Number of receiver depth must be greater than 0";
        }

        try {
            sd = this.parseOneDimensionalArray(sd);
            if (sd.some(x => isNaN(x))) {
                error.sd = "Source depth format is not valid";
            }
            else if (sd.some(x => x < 0)) {
                error.sd = "Source depth must consist of non-negative numbers";
            }
        }
        catch (e) {
            error.sd = "Source depth format is not valid";
        }

        try {
            rd = this.parseOneDimensionalArray(rd);
            if (rd.some(x => isNaN(x))) {
                error.rd = "Receiver depth format is not valid";
            }
            else if (rd.some(x => x < 0)) {
                error.rd = "Receiver depth must consist of non-negative numbers";
            }
        }
        catch (e) {
            error.rd = "Source depth format is not valid";
        }

        try {
            mediumInfo = this.parseTwoDimensionalArray(mediumInfo);
            if (mediumInfo.length === 0) {
                error.mediumInfo = "Medium info is required";
            }
            else {
                mediumInfo.forEach(mi => {
                    if (mi.length !== 3) {
                        error.mediumInfo = "Medium info must consist of lists with 3 elements each";
                        return;
                    }
                    if (mi.some(x => isNaN(x))) {
                        error.mediumInfo = "Medium info format is not valid";
                        return;
                    }
                });
            }

        }
        catch (e) {
            error.mediumInfo = "Medium info format is not valid";
        }

        try {
            ssp = this.parseTwoDimensionalArray(ssp);
            if (ssp.length === 0) {
                error.ssp = "Sound speed profile is required";
            }
            else {
                ssp.forEach(s => {
                    if (s.length !== 6) {
                        error.ssp = "Sound speed profile must consist of lists with 6 elements each";
                        return;
                    }

                    if (s.some(x => isNaN(x))) {
                        error.ssp = "Sound speed format is not valid";
                        return;
                    }
                });
            }
            
        }
        catch (e) {
            error.ssp = "SSP format is not valid";
        }

        if (calculateTransmissionLoss === true) {
            if (nModesForField <= 0) {
                error.nModesForField = "Number of modes for field computing must be greater than 0";
            }

            if (sourceType.length !== 1 || !this.sourceTypes.some(x => x.key === sourceType)) {
                error.sourceType = "Source type is required";
            }

            if (modesTheory.length !== 1 || !this.modesTheories.some(x => x.key === modesTheory)) {
                error.modesTheory = "Mode theory is required";
            }

            if (nsdField <= 0) {
                error.nsdField = "Number of source depth (for field) must be greater than 0";
            }

            if (nrdField <= 0) {
                error.nrdField = "Number of receiver depth (for field) must be greater than 0";
            }

            if (nr <= 0) {
                error.nr = "The number of receiver ranges must be greater than 0";
            }

            if (nrr <= 0) {
                error.nrr = "The number of receiver range-displacements must be greater than 0";
            }

            try {
                rr = this.parseOneDimensionalArray(rr);
                if (rr.some(x => isNaN(x))) {
                    error.rr = "The receiver displacements format is not valid";
                }
                else if (rr.some(x => x < 0)) {
                    error.rr = "The receiver displacements must consist of non-negative numbers";
                }
            }
            catch (e) {
                error.rr = "The receiver displacements format is invalid";
            }

            try {
                r = this.parseOneDimensionalArray(r);
            }
            catch (e) {
                error.r = "The receiver ranges format is invalid";
            }          

            try {
                sdField = this.parseOneDimensionalArray(sdField);
                if (sdField.some(x => isNaN(x))) {
                    error.sdField = "Source depth (for field) format is not valid";
                }
                else if (sdField.some(x => x < 0)) {
                    error.sdField = "Source depth must (for field) consist of non-negative numbers";
                }
            }
            catch (e) {
                error.sdField = "Source depth format (for field) is invalid";
            }

            try {
                rdField = this.parseOneDimensionalArray(rdField);
                if (rdField.some(x => isNaN(x))) {
                    error.rdField = "Receiver depth (for field) format is not valid";
                }
                else if (rdField.some(x => x < 0)) {
                    error.rdField = "Receiver depth (for field) must consist of non-negative numbers";
                }
            }
            catch (e) {
                error.rdField = "Receiver depth format (for field) is invalid";
            }
        }

        if (!(Object.entries(error).length === 0 && error.constructor === Object)) {
            this.setState({ error: error });
            this.props.onError(error);
            return null;
        }

        else {
            const addedVolumeAttenuation = isVolumeAttenuatonAdded === true ? 'T' : '';

            if (calculateTransmissionLoss === true) {
                return {
                    frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, addedVolumeAttenuation, zt, cpt,
                    cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
                    csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd, nModesForField,
                    nr, r, nsdField, sdField, nrdField, rdField, nrr, rr, calculateTransmissionLoss, sourceType, modesTheory
                };
            }
            else {
                return {
                    frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, addedVolumeAttenuation, zt, cpt,
                    cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
                    csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd
                };
            }
        }
    }

    parseOneDimensionalArray = (str) => {
        const array = JSON.parse('[' + str + ']');
        if (!Array.isArray(array)) {
            throw "Given string is not array";
        }

        if (array.some(x => Array.isArray(x))) {
            throw "Given string is not one dimensional array";
        }

        return array;
    };

    parseTwoDimensionalArray = (str) => {
        const array = JSON.parse('[' + str + ']');
        if (!Array.isArray(array)) {
            throw "Given string is not array";
        }

        if (array.some(x => !Array.isArray(x))) {
            throw "Given string is not two dimensional array";
        }

        return array;
    };

    loadFileButtonClick = () => {
        this.fileInputRef.current["value"] = "";
        this.fileInputRef.current.click();     
    };

    uploadFile = (e) => {
        this.setState({
            uploadFileError: null
        });

        if (e.target.files.length == 0) {
            return;
        }

        let file = e.target.files[0];       
        readJsonFile(file).then(formData => {
            if (formData === false) {
                this.setState({
                    uploadFileError: 'File content is not valid JSON object.'
                });
            }
            else {
                const { isValid, missedProps } = this.isValidJsonStructureForFormData(formData);            
                if (isValid === true) {
                    this.hasInitValue = true;
                    this.setState({                        
                        ...formData,
                        hasInitValue:true
                    });
                }
                else {
                    const props = missedProps.join(", ");
                    this.setState({
                        uploadFileError: 'Following properties are missed or set to undefined: ' + props + '.'
                    });
                }
            }
        }).catch(() => {           
            this.setState({
                uploadFileError: 'An unexected error while reading the file.'
            });
        });                
    };

    isValidJsonStructureForFormData = (data) => {       
        const missedProps = [];

        const krakenProps = ['frequency', 'nModes', 'nMedia', 'topBCType', 'interpolationType',
            'attenuationUnits', 'isVolumeAttenuatonAdded', 'zt', 'cpt',
            'cst', 'rhot', 'apt', 'ast', 'bumDen', 'eta', 'xi',
            'mediumInfo', 'ssp', 'bottomBCType', 'sigma', 'zb', 'cpb',
            'csb', 'rhob', 'apb', 'asb', 'cLow', 'cHigh', 'rMax', 'nsd', 'sd', 'nrd', 'rd',
            'calculateTransmissionLoss'];

        const tlProps = ['sourceType', 'modesTheory', 'nModesForField',
              'nr', 'r', 'nsdField', 'sdField', 'nrdField', 'rdField', 'nrr', 'rr'];


        krakenProps.forEach(prop => {
            if (!data.hasOwnProperty(prop) || data[prop] === undefined) {
               missedProps.push(prop);
            }
        });

        if (data.calculateTransmissionLoss === true) {
            tlProps.forEach(prop => {
                if (!data.hasOwnProperty(prop) || data[prop] === undefined) {
                  missedProps.push(prop);
                }
            });
        }

        if (missedProps.length === 0) {
            return { isValid: true };
        }

        return { isValid: false, missedProps };
    };

    saveFormData = () => {
        const { frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, isVolumeAttenuatonAdded, zt, cpt,
            cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
            csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd,
            calculateTransmissionLoss, sourceType, modesTheory, nModesForField,
            nr, r, nsdField, sdField, nrdField, rdField, nrr, rr } = this.state;

        const jsonText = JSON.stringify({
            frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, isVolumeAttenuatonAdded, zt, cpt,
            cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
            csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd,
            calculateTransmissionLoss, sourceType, modesTheory, nModesForField,
            nr, r, nsdField, sdField, nrdField, rdField, nrr, rr
        });

        exportAsJson(jsonText, "acoustic-problem");
    };

    render() {
        const { error, uploadFileError, hasInitValue } = this.state;
        const { frequency, nModes, nMedia, topBCType, interpolationType, attenuationUnits, isVolumeAttenuatonAdded, zt, cpt,
            cst, rhot, apt, ast, bumDen, eta, xi, mediumInfo, ssp, bottomBCType, sigma, zb, cpb,
            csb, rhob, apb, asb, cLow, cHigh, rMax, nsd, sd, nrd, rd,
            calculateTransmissionLoss, sourceType, modesTheory, nModesForField,
            nr, r, nsdField, sdField, nrdField, rdField, nrr, rr } = this.state;      

        const isTopAcoustic = topBCType === 'A';
        const isTopTwersky = (topBCType === 'T' || topBCType === 'S'
            || topBCType === 'I' || topBCType === 'H');
        const isBottomAcoustic = bottomBCType === 'A';

        return (
            <>
                <div className='d-flex justify-content-end'>
                    <input type="file" accept=".json" style={{ display: "none" }} ref={this.fileInputRef} onChange={this.uploadFile} />
                    <Button onClick={this.loadFileButtonClick} outline color="primary">Load data from file</Button>
                </div>
                {uploadFileError && <ErrorMessage header="An error occured while uploading file" errorMessage={uploadFileError}/>}
           <Form onSubmit={this.onSubmit}>
               {hasInitValue ? <>
                <FormGroup>
                    <Label for="frequency">Frequency (Hz)</Label>
                    <Input type="number" name="frequency" id="frequency" onChange={this.handleChange} placeholder="Frequency" defaultValue={frequency} required />
                </FormGroup>
                <FormGroup>
                    <Label for="nModes">Number of modes</Label>
                    <Input type="number" name="nModes" id="nModes" onChange={this.handleChange} placeholder="Number of modes" defaultValue={nModes} required />
                </FormGroup>
                <FormGroup>
                    <Label for="nMedia">Number of media</Label>
                    <Input type="number" name="nMedia" id="nMedia" onChange={this.handleChange} placeholder="Number of media" defaultValue={nMedia} required />
                </FormGroup>

                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Select label={"Type of interpolation"} name={"interpolationType"} onChange={this.handleChange} options={this.interpolationTypes} initValue={interpolationType} />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Select label={"Type of top boundary condition"} name={"topBCType"} onChange={this.handleChange} options={this.topBoundaryConditions} initValue={topBCType} />
                        </FormGroup>
                    </Col>
                </Row>
                {isTopAcoustic ?
                    <Row form>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="zt">Depth (m)</Label>
                                <Input type="number" name="zt" id="zt" onChange={this.handleChange} required defaultValue={zt} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="cpt">Top P-wave speed (m/s)</Label>
                                <Input type="number" name="cpt" id="cpt" onChange={this.handleChange} required defaultValue={cpt} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="cst">Top S-wave speed (m/s)</Label>
                                <Input type="number" name="cst" id="cst" onChange={this.handleChange} required defaultValue={cst} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="rhot">Top density (g/cm3)</Label>
                                <Input type="number" name="rhot" id="rhot" onChange={this.handleChange} required defaultValue={rhot} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="apt">Top P-wave attenuation</Label>
                                <Input type="number" name="apt" id="apt" onChange={this.handleChange} required defaultValue={apt} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="ast">Top S-wave attenuation</Label>
                                <Input type="number" name="ast" id="ast" onChange={this.handleChange} required defaultValue={ast} />
                            </FormGroup>
                        </Col>
                    </Row>
                    : null}
                {isTopTwersky ?
                    <Row form>
                        <Col md={4}>
                            <FormGroup>
                                <Label for="bumDen">Bump density (ridges/km)</Label>
                                <Input type="number" name="bumDen" id="bumDen" onChange={this.handleChange} required defaultValue={bumDen} />
                            </FormGroup>
                        </Col>
                        <Col md={4}>
                            <FormGroup>
                                <Label for="eta">Principal radius 1 (m)</Label>
                                <Input type="number" name="eta" id="eta" onChange={this.handleChange} required defaultValue={eta} />
                            </FormGroup>
                        </Col>
                        <Col md={4}>
                            <FormGroup>
                                <Label for="xi">Principal radius 2 (m)</Label>
                                <Input type="number" name="xi" id="xi" onChange={this.handleChange} required defaultValue={xi} />
                            </FormGroup>
                        </Col>
                    </Row>
                    : null}
                <FormGroup>
                    <Select label={"Attenuation units"} name={"attenuationUnits"} onChange={this.handleChange} options={this.attenuationUnits} required initValue={attenuationUnits} />
                </FormGroup>
                <FormGroup check>
                            <Label check>
                                <Input type="checkbox" name="isVolumeAttenuatonAdded" id="isVolumeAttenuatonAdded" onChange={this.handleCheckboxChange}
                                    checked={isVolumeAttenuatonAdded}  />{' '}
                        Add volume attenuation
                    </Label>
                </FormGroup>
                <FormGroup>
                    <Label for="mediumInfo">Medium info </Label>
                    <Input type="textarea" name="mediumInfo" id="mediumInfo" onChange={this.handleChange} required placeholder={"e.g. [300,0,3000], [200,0,500]"} defaultValue={mediumInfo} />
                </FormGroup>
                <FormGroup>
                    <Label for="ssp">Sound speed profile</Label>
                    <Input type="textarea" name="ssp" id="ssp" onChange={this.handleChange} required defaultValue={ssp} placeholder={"e.g. [0,1500, 0, 1.0, 0, 0],[3000, 1500, 0, 1.0, 0, 0.00000],[3000, 1500, 0, 2.0000, 0, 0.00000],[5000, 1500, 0, 2.0000, 0, 0]"} />
                </FormGroup>
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Select label={"Type of bottom boundary condition"} name={"bottomBCType"} onChange={this.handleChange} options={this.bottomBoundaryConditions} initValue={bottomBCType} />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="sigma">Interfacial roughness (m)</Label>
                            <Input type="number" name="sigma" id="sigma" onChange={this.handleChange} required defaultValue={sigma} />
                        </FormGroup>
                    </Col>
                </Row>
                {isBottomAcoustic ?
                    <Row form>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="zb">Depth (m)</Label>
                                <Input type="number" name="zb" id="zb" onChange={this.handleChange} required defaultValue={zb} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="cpb">Bottom P-wave speed (m/s)</Label>
                                <Input type="text" name="cpb" id="cpb" onChange={this.handleChange} required defaultValue={cpb} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="csb">Bottom S-wave speed (m/s)</Label>
                                <Input type="text" name="csb" id="csb" onChange={this.handleChange} required defaultValue={csb} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="rhob">Bottom density (g/cm3)</Label>
                                <Input type="text" name="rhob" id="rhob" onChange={this.handleChange} required defaultValue={rhob} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="apb">Bottom P-wave atten.</Label>
                                <Input type="text" name="apb" id="apb" onChange={this.handleChange} required defaultValue={apb} />
                            </FormGroup>
                        </Col>
                        <Col md={2}>
                            <FormGroup>
                                <Label for="asb">Bottom S-wave atten.</Label>
                                <Input type="text" name="asb" id="asb" onChange={this.handleChange} required defaultValue={asb} />
                            </FormGroup>
                        </Col>
                    </Row>
                    : null}
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="cLow">Lower phase speed limit (m/s)</Label>
                            <Input type="number" name="cLow" id="cLow" onChange={this.handleChange} required defaultValue={cLow} />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="cHigh">Upper phase speed limit (m/s)</Label>
                            <Input type="number" name="cHigh" id="cHigh" onChange={this.handleChange} required defaultValue={cHigh} />
                        </FormGroup>
                    </Col>
                </Row>
                <FormGroup>
                    <Label for="rMax">Maximum range (km)</Label>
                    <Input type="number" name="rMax" id="rMax" onChange={this.handleChange} required defaultValue={rMax} />
                </FormGroup>
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="nsd">The number of soure depths</Label>
                            <Input type="number" name="nsd" id="nsd" onChange={this.handleChange} required defaultValue={nsd} />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="sd">The source depths (m)</Label>
                            <Input type="textarea" name="sd" id="sd" onChange={this.handleChange} required defaultValue={sd} />
                        </FormGroup>
                    </Col>
                </Row>
                <Row form>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="nrd">The number of receiver depths</Label>
                            <Input type="number" name="nrd" id="nrd" onChange={this.handleChange} required defaultValue={nrd} />
                        </FormGroup>
                    </Col>
                    <Col md={6}>
                        <FormGroup>
                            <Label for="rd">The receiver depths (m)</Label>
                            <Input type="textarea" name="rd" id="rd" onChange={this.handleChange} required defaultValue={rd} />
                        </FormGroup>
                    </Col>
                </Row>
                <FormGroup check>
                    <Label check>
                                <Input type="checkbox" name="calculateTransmissionLoss" id="calculateTransmissionLoss"
                                    checked={calculateTransmissionLoss}
                                    defaultValue={calculateTransmissionLoss} onChange={this.handleCheckboxChange} />{' '}
                        Calculate transmission loss
                    </Label>
                </FormGroup>
                {calculateTransmissionLoss ?
                    <>
                        <Row form>
                            <Col md={6}>
                                <FormGroup>
                                    <Select label={"Source type"} name={"sourceType"} onChange={this.handleChange} options={this.sourceTypes} initValue={sourceType} />
                                </FormGroup>
                            </Col>
                            <Col md={6}>
                                <FormGroup>
                                    <Select label={"Mode theory"} name={"modesTheory"} onChange={this.handleChange} options={this.modesTheories} initValue={modesTheory} />
                                </FormGroup>
                            </Col>
                        </Row>
                        <FormGroup>
                            <Label for="nModesForField">Number of modes in field computation</Label>
                            <Input type="number" name="nModesForField" id="nModesForField" onChange={this.handleChange} defaultValue={nModesForField} placeholder="Number of modes to use in field computation" required />
                        </FormGroup>                     

                        <Row form>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="nr">The number of receiver ranges</Label>
                                    <Input type="number" name="nr" id="nr" onChange={this.handleChange} defaultValue={nr} required />
                                </FormGroup>
                            </Col>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="r">The receiver ranges (km)</Label>
                                    <Input type="textarea" name="r" id="r" onChange={this.handleChange} defaultValue={r} required />
                                </FormGroup>
                            </Col>
                        </Row>

                        <Row form>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="nsdField">The number of soure depths</Label>
                                    <Input type="number" name="nsdField" id="nsdField" onChange={this.handleChange} defaultValue={nsdField} required />
                                </FormGroup>
                            </Col>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="sdField">The source depths (m)</Label>
                                    <Input type="textarea" name="sdField" id="sdField" onChange={this.handleChange} defaultValue={sdField} required />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row form>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="nrdField">The number of receiver depths</Label>
                                    <Input type="number" name="nrdField" id="nrdField" onChange={this.handleChange} defaultValue={nrdField} required />
                                </FormGroup>
                            </Col>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="rdField">The receiver depths (m)</Label>
                                    <Input type="textarea" name="rdField" id="rdField" onChange={this.handleChange} defaultValue={rdField} required />
                                </FormGroup>
                            </Col>
                        </Row>

                        <Row form>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="nrr">The number of receiver range-displacements</Label>
                                    <Input type="number" name="nrr" id="nrr" onChange={this.handleChange} defaultValue={nrr} required />
                                </FormGroup>
                            </Col>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="rr">The receiver displacements (m)</Label>
                                    <Input type="textarea" name="rr" id="rr" onChange={this.handleChange} defaultValue={rr} required />
                                </FormGroup>
                            </Col>
                        </Row>
                    </>
                    : null
                   }
               </>
                :
                   <>
                       <FormGroup>
                           <Label for="frequency">Frequency (Hz)</Label>
                           <Input type="number" name="frequency" id="frequency" onChange={this.handleChange} placeholder="Frequency" required />
                       </FormGroup>
                       <FormGroup>
                           <Label for="nModes">Number of modes</Label>
                           <Input type="number" name="nModes" id="nModes" onChange={this.handleChange} placeholder="Number of modes" required />
                       </FormGroup>
                       <FormGroup>
                           <Label for="nMedia">Number of media</Label>
                           <Input type="number" name="nMedia" id="nMedia" onChange={this.handleChange} placeholder="Number of media" required />
                       </FormGroup>

                       <Row form>
                           <Col md={6}>
                               <FormGroup>
                                   <Select label={"Type of interpolation"} name={"interpolationType"} onChange={this.handleChange} options={this.interpolationTypes} />
                               </FormGroup>
                           </Col>
                           <Col md={6}>
                               <FormGroup>
                                   <Select label={"Type of top boundary condition"} name={"topBCType"} onChange={this.handleChange} options={this.topBoundaryConditions} />
                               </FormGroup>
                           </Col>
                       </Row>
                       {isTopAcoustic ?
                           <Row form>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="zt">Depth (m)</Label>
                                       <Input type="number" name="zt" id="zt" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="cpt">Top P-wave speed (m/s)</Label>
                                       <Input type="number" name="cpt" id="cpt" onChange={this.handleChange} required />
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
                                       <Input type="number" name="rhot" id="rhot" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="apt">Top P-wave attenuation</Label>
                                       <Input type="number" name="apt" id="apt" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="ast">Top S-wave attenuation</Label>
                                       <Input type="number" name="ast" id="ast" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                           </Row>
                           : null}
                       {isTopTwersky ?
                           <Row form>
                               <Col md={4}>
                                   <FormGroup>
                                       <Label for="bumDen">Bump density (ridges/km)</Label>
                                       <Input type="number" name="bumDen" id="bumDen" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={4}>
                                   <FormGroup>
                                       <Label for="eta">Principal radius 1 (m)</Label>
                                       <Input type="number" name="eta" id="eta" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={4}>
                                   <FormGroup>
                                       <Label for="xi">Principal radius 2 (m)</Label>
                                       <Input type="number" name="xi" id="xi" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                           </Row>
                           : null}
                       <FormGroup>
                           <Select label={"Attenuation units"} name={"attenuationUnits"} onChange={this.handleChange} options={this.attenuationUnits} required />
                       </FormGroup>
                       <FormGroup check>
                                <Label check>
                                    <Input type="checkbox" name="isVolumeAttenuatonAdded" id="isVolumeAttenuatonAdded" checked={isVolumeAttenuatonAdded} value={isVolumeAttenuatonAdded}
                                        onChange={this.handleCheckboxChange} />{' '}
                               Add volume attenuation
                    </Label>
                       </FormGroup>
                       <FormGroup>
                           <Label for="mediumInfo">Medium info </Label>
                           <Input type="textarea" name="mediumInfo" id="mediumInfo" onChange={this.handleChange} required placeholder={"e.g. [300,0,3000], [200,0,500]"} />
                       </FormGroup>
                       <FormGroup>
                           <Label for="ssp">Sound speed profile</Label>
                           <Input type="textarea" name="ssp" id="ssp" onChange={this.handleChange} required placeholder={"e.g. [0,1500, 0, 1.0, 0, 0],[3000, 1500, 0, 1.0, 0, 0.00000],[3000, 1500, 0, 2.0000, 0, 0.00000],[5000, 1500, 0, 2.0000, 0, 0]"} />
                       </FormGroup>
                       <Row form>
                           <Col md={6}>
                               <FormGroup>
                                   <Select label={"Type of bottom boundary condition"} name={"bottomBCType"} onChange={this.handleChange} options={this.bottomBoundaryConditions} />
                               </FormGroup>
                           </Col>
                           <Col md={6}>
                               <FormGroup>
                                   <Label for="sigma">Interfacial roughness (m)</Label>
                                   <Input type="number" name="sigma" id="sigma" onChange={this.handleChange} required />
                               </FormGroup>
                           </Col>
                       </Row>
                       {isBottomAcoustic ?
                           <Row form>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="zb">Depth (m)</Label>
                                       <Input type="number" name="zb" id="zb" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="cpb">Bottom P-wave speed (m/s)</Label>
                                       <Input type="text" name="cpb" id="cpb" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="csb">Bottom S-wave speed (m/s)</Label>
                                       <Input type="text" name="csb" id="csb" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="rhob">Bottom density (g/cm3)</Label>
                                       <Input type="text" name="rhob" id="rhob" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="apb">Bottom P-wave atten.</Label>
                                       <Input type="text" name="apb" id="apb" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                               <Col md={2}>
                                   <FormGroup>
                                       <Label for="asb">Bottom S-wave atten.</Label>
                                       <Input type="text" name="asb" id="asb" onChange={this.handleChange} required />
                                   </FormGroup>
                               </Col>
                           </Row>
                           : null}
                       <Row form>
                           <Col md={6}>
                               <FormGroup>
                                   <Label for="cLow">Lower phase speed limit (m/s)</Label>
                                   <Input type="number" name="cLow" id="cLow" onChange={this.handleChange} required />
                               </FormGroup>
                           </Col>
                           <Col md={6}>
                               <FormGroup>
                                   <Label for="cHigh">Upper phase speed limit (m/s)</Label>
                                   <Input type="number" name="cHigh" id="cHigh" onChange={this.handleChange} required />
                               </FormGroup>
                           </Col>
                       </Row>
                       <FormGroup>
                           <Label for="rMax">Maximum range (km)</Label>
                           <Input type="number" name="rMax" id="rMax" onChange={this.handleChange} required />
                       </FormGroup>
                       <Row form>
                           <Col md={6}>
                               <FormGroup>
                                   <Label for="nsd">The number of soure depths</Label>
                                   <Input type="number" name="nsd" id="nsd" onChange={this.handleChange} required />
                               </FormGroup>
                           </Col>
                           <Col md={6}>
                               <FormGroup>
                                   <Label for="sd">The source depths (m)</Label>
                                   <Input type="textarea" name="sd" id="sd" onChange={this.handleChange} required />
                               </FormGroup>
                           </Col>
                       </Row>
                       <Row form>
                           <Col md={6}>
                               <FormGroup>
                                   <Label for="nrd">The number of receiver depths</Label>
                                   <Input type="number" name="nrd" id="nrd" onChange={this.handleChange} required />
                               </FormGroup>
                           </Col>
                           <Col md={6}>
                               <FormGroup>
                                   <Label for="rd">The receiver depths (m)</Label>
                                   <Input type="textarea" name="rd" id="rd" onChange={this.handleChange} required />
                               </FormGroup>
                           </Col>
                       </Row>
                       <FormGroup check>
                           <Label check>
                                    <Input type="checkbox" name="calculateTransmissionLoss" id="calculateTransmissionLoss"
                                        checked={calculateTransmissionLoss}
                                        defaultValue={calculateTransmissionLoss} onChange={this.handleCheckboxChange} />{' '}
                               Calculate transmission loss
                    </Label>
                       </FormGroup>
                       {calculateTransmissionLoss ?
                           <>
                               <Row form>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Select label={"Source type"} name={"sourceType"} onChange={this.handleChange} options={this.sourceTypes} />
                                       </FormGroup>
                                   </Col>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Select label={"Mode theory"} name={"modesTheory"} onChange={this.handleChange} options={this.modesTheories} />
                                       </FormGroup>
                                   </Col>
                               </Row>
                               <FormGroup>
                                   <Label for="nModesForField">Number of modes in field computation</Label>
                                   <Input type="number" name="nModesForField" id="nModesForField" onChange={this.handleChange} placeholder="Number of modes to use in field computation" required />
                               </FormGroup>
                               
                               <Row form>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="nr">The number of receiver ranges</Label>
                                           <Input type="number" name="nr" id="nr" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="r">The receiver ranges (km)</Label>
                                           <Input type="textarea" name="r" id="r" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                               </Row>

                               <Row form>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="nsdField">The number of soure depths</Label>
                                           <Input type="number" name="nsdField" id="nsdField" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="sdField">The source depths (m)</Label>
                                           <Input type="textarea" name="sdField" id="sdField" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                               </Row>
                               <Row form>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="nrdField">The number of receiver depths</Label>
                                           <Input type="number" name="nrdField" id="nrdField" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="rdField">The receiver depths (m)</Label>
                                           <Input type="textarea" name="rdField" id="rdField" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                               </Row>

                               <Row form>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="nrr">The number of receiver range-displacements</Label>
                                           <Input type="number" name="nrr" id="nrr" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                                   <Col md={6}>
                                       <FormGroup>
                                           <Label for="rr">The receiver displacements (m)</Label>
                                           <Input type="textarea" name="rr" id="rr" onChange={this.handleChange} required />
                                       </FormGroup>
                                   </Col>
                               </Row>
                           </>
                           : null
                            } </>}
                    <div>
                        <Button outline color="success" onClick={this.saveFormData}>Save form data to file</Button>
                    </div>
                <Button outline color="secondary">Submit</Button>                
           </Form>
           {
            error !== null ?
            <div className="validation-errors-list">
                <InputErrorsList error={error} />
            </div>
            : null
        }
        </>
        );
    }
}