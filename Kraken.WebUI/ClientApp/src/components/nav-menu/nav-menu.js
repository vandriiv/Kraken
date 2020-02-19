import React, { Component } from 'react';
import {
    Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Nav
} from 'reactstrap';
import { Link } from 'react-router-dom';
import './nav-menu.css';

export default class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }   

    render() {

        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                    <Container>
                        <NavbarBrand tag={Link} to="/"><img src="/images/squid.png" className="brand-img d-inline-block align-top" />
                            <span className="brand-title">Kraken normal modes</span>
                            </NavbarBrand>                       
                                <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                                    <Nav className="mr-auto" navbar>                                        
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/docs">Documentation</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/test-problems">Test problems</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/contacts">Contacts</NavLink>
                                </NavItem>                                
                                    </Nav>
                                </Collapse>                            
                    </Container>
                </Navbar>
            </header>
        );
    }
}
