import React, { Component } from 'react';
import { Container } from 'reactstrap';
import NavMenu from '../nav-menu/';

export default class Layout extends Component {   

    render() {
        return (
            <div>
                <NavMenu/>
                <Container>
                    {this.props.children}
                </Container>
            </div>
        );
    }
}
