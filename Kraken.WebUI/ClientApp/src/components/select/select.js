﻿import React, { Component } from "react";
import { Label, Input } from "reactstrap";

export default class Select extends Component {

    componentDidMount() {
        this.dispatchChangeEvent();
    }

    renderOptions(options) {
        return options.map(x => <option key={x.key} value={x.key}>{x.name}</option>);
    }

    dispatchChangeEvent() {
        const select = document.getElementById(this.props.name);
        select.dispatchEvent(new Event('change', { bubbles: false }));
    }

    render() {
        const { label, name, options, onChange, initValue } = this.props;       
        return (
            <>
                <Label for="name">{label}</Label>
                <Input ref={"select"} type="select" name={name} id={name} onChange={onChange} value={initValue} required>
                    {options && this.renderOptions(options)}
                </Input>
            </>
        );
    }
}