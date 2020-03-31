import React,{Component} from 'react';
import { Row, Col } from 'reactstrap';
import {FaRegEnvelope, FaGithub, FaRegUser} from 'react-icons/fa';
 
import "./contacts-page.css";

export default class ContactsPage extends Component{

    render(){
        return (<Row>
            <Col className="contacts">
                <p><span><FaRegUser/> Author:</span> Andrii Voitovych</p>
                <p><span><a href="mailto:andrvoitovych@gmail.com"><FaRegEnvelope/></a> Email:</span> andrvoitovych@gmail.com </p>
                <p><span><FaGithub/> GitHub repo:</span> <a href="https://github.com/vandriiv/Kraken">github.com/vandriiv/Kraken</a></p>
            </Col>
        </Row>)
    }
}