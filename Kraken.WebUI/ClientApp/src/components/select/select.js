import React, { Component } from "react";
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
        select.dispatchEvent(new Event('change', { bubbles: true }));
    }

    render() {
        const { label, name, options, onChange } = this.props;
        return (
            <React.Fragment>
                <Label for="name">{label}</Label>
                <Input ref={"select"} type="select" name={name} id={name} onChange={onChange} required>
                    {options && this.renderOptions(options)}
                </Input>
            </React.Fragment>
        );
    }
}