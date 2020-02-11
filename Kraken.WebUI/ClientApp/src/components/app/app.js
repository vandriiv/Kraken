import React, { Component } from 'react';
import Layout from '../layout';
import { BrowserRouter as Router, Switch, Route } from 'react-router-dom';
import KrakenPage from '../kraken-page';
import './app.css';
export default class App extends Component {
    render() {
        return (
            <Router>
                <Layout>
                    <Switch>
                        <Route path={"/"} exact component={KrakenPage}/>
                    </Switch>
                </Layout>
            </Router>);
    }
}