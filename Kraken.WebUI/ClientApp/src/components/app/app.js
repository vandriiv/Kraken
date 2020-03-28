import React, { Component } from 'react';
import Layout from '../layout';
import { BrowserRouter as Router, Switch, Route } from 'react-router-dom';
import Helmet from 'react-helmet';

import KrakenPage from '../kraken-page';
import TestProblemsPage from '../test-problems-page';

import Atten from '../test-problems/atten';
import Double from '../test-problems/double';
import Elsed from '../test-problems/elsed';
import Flused from '../test-problems/flused';
import Ice from '../test-problems/ice';
import Normal from '../test-problems/normal';
import Pekeris from '../test-problems/pekeris';
import Scholte from '../test-problems/scholte';
import Twersky from '../test-problems/twersky';

import MatlabKrakenTestOne from '../test-problems/matlab-kraken-test-one';
import MatlabKrakenTestTwo from '../test-problems/matlab-kraken-test-two';
import MatlabKrakenTestThree from '../test-problems/matlab-kraken-test-three';
import Munk from '../test-problems/munk';
import Dickins from '../test-problems/dickins';

import './app.css';

export default class App extends Component {
    render() {
        return (<>
            <Helmet>
                <title>Kraken</title>
            </Helmet>
            <Router>               
                <Layout>                
                    <Switch>
                        <Route path={"/"} exact component={KrakenPage} />
                        <Route path={"/test-problems"} exact component={TestProblemsPage} />
                        <Route path={"/atten"} exact component={Atten} />
                        <Route path={"/double"} exact component={Double} />
                        <Route path={"/elsed"} exact component={Elsed} />
                        <Route path={"/flused"} exact component={Flused} />
                        <Route path={"/ice"} exact component={Ice} />
                        <Route path={"/normal"} exact component={Normal} />
                        <Route path={"/pekeris"} exact component={Pekeris} />
                        <Route path={"/scholte"} exact component={Scholte} />
                        <Route path={"/twersky"} exact component={Twersky} />
                        <Route path={"/matlab-kraken-test-1"} exact component={MatlabKrakenTestOne} />
                        <Route path={"/matlab-kraken-test-2"} exact component={MatlabKrakenTestTwo} />
                        <Route path={"/matlab-kraken-test-3"} exact component={MatlabKrakenTestThree} />
                        <Route path={"/munk"} exact component={Munk} />
                        <Route path={"/dickins"} exact component={Dickins}/>
                    </Switch>
                </Layout>
            </Router>
            </>);
    }
}