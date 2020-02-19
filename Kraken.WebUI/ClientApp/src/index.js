import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import registerServiceWorker from './registerServiceWorker';
import App from './components/app';
import './index.css';

const rootElement = document.getElementById('root');

ReactDOM.render(  
    <App/>,
  rootElement);

registerServiceWorker();

