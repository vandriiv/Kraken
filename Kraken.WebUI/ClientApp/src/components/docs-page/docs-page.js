import React, { Component } from 'react';
import { Row, Col, Table} from 'reactstrap';
import ReactJson from 'react-json-view';
import { HashLink as Link } from 'react-router-hash-link';
import { FaGithub } from 'react-icons/fa';

import './docs-page.css';

export default class DocsPage extends Component {

    render() {
        const uploadJsonExample = {
            "frequency": 10,
            "nModes": 10,
            "nMedia": 1,
            "topBCType": "V",
            "interpolationType": "N",
            "attenuationUnits": "F",
            "isVolumeAttenuatonAdded": false,
            "zt": 0,
            "cpt": 0,
            "cst": 0,
            "rhot": 0,
            "apt": 0,
            "ast": 0,
            "bumDen": 0,
            "eta": 0,
            "xi": 0,
            "mediumInfo": "[500,0,5000]",
            "ssp": "[0,1500,0,1,0,0], [5000,1500,0,1,0,0]",
            "bottomBCType": "A",
            "sigma": 0,
            "zb": 5000,
            "cpb": 2000,
            "csb": 0,
            "rhob": 2,
            "apb": 0,
            "asb": 0,
            "cLow": 1400,
            "cHigh": 2000,
            "rMax": 1000,
            "nsd": 1,
            "sd": "500",
            "nrd": 1,
            "rd": "2500",
            "calculateTransmissionLoss": true,
            "sourceType": "R",
            "modesTheory": "A",
            "nModesForField": 9999,
            "nr": 501,
            "r": "200,220",
            "nsdField": 1,
            "sdField": "500",
            "nrdField": 1,
            "rdField": "2500",
            "nrr": 1,
            "rr": "0"
        };

        return (<Row>
            <Col>
                <div id="docs-nav" className="d-flex docs-nav">                  
                   <Link smooth to="#about-project">About project</Link>
                   <Link smooth to="#differences">Differences from original KRAKEN</Link>
                   <Link smooth to="#form-description">Description of form inputs</Link>
                   <Link smooth to="#file-upload">File upload</Link>
                </div>
                <hr/>
                <section id="about-project">
                    <h2>About project</h2>
                    <hr />
                    <p>
                        This project is web application for predicting acoustic transmission-loss in the ocean based on <a href="https://oalib-acoustics.org/AcousticsToolbox/manual/kraken.html">KRAKEN normal mode program</a>.
                        <br />
                        This project was developed for education purposes, so results of the calculations may differ from the results of the original program, use it on your own risks.                        
                    </p>
                    <p>The source code is available <a href="https://github.com/vandriiv/Kraken">here</a> <FaGithub/></p>
                </section>
                <section id="differences">
                    <h2>Differences from original KRAKEN</h2>
                    <hr />
                    <p>List of features that are available in original KRAKEN but not in this web application:</p>
                    <ul>
                        <li>Analytic type of interpolation</li>
                        <li>Reflection coefficient from a FILE (btw, available in KRAKENC only)</li>
                        <li>Multiple profiles proceeding, so you can solve only range-independent problem</li>
                    </ul>
                </section>
                <section id="form-description">
                    <h2>Description of form inputs</h2>
                    <hr />
                    <div>
                        <h5>Frequency</h5>
                        <p>Frequency in Hz.</p>
                    </div>
                    <div>
                        <h5>Number of modes</h5>
                        <p>Number of modes to calculate. If the number of modes specified exceeds the
                        number computed then the program uses all the
                    computed modes.</p>
                    </div>
                    <div>
                        <h5>Numer of media</h5>
                        <p>Number of media.
                        The problem is divided into media within which it is
                        assumed that the material properties vary smoothly. A new
                        medium should be used at fluid/elastic interfaces or at
                        interfaces where the density changes discontinuously. The
                        number of media in the problem is defined excluding the
                    upper and lower half-space. </p>
                    </div>
                    <div>
                        <h5>Type of interpolation</h5>
                        <p>Type of interpolation to be used for the SSP. If your not sure which option to take, I'd suggest
                        you use 'C-Linear' or 'N2-Liner'.  Practically, you can pick
                        either one: the choice has been implemented to
                        facilitate precise intermodel comparisons.
                        Option 'Cubic Spline' is a little dangerous because splines
                        yield a poor fit to certain kinds of curves,
                        e.g. curves with sharp bends.  If you insist
                        on splines, you can fix a bad fit by dividing the
                    water column into two 'media' at the bend.</p>
                    </div>
                    <div>
                        <h5>Type of top boundary condition</h5>
                        <p>Type of top boundary condition. For open ocean problems option 'Vacuum above top' should be
                        used for the top BC. The Twersky options
                    are intended for under-ice modeling.</p>
                    </div>
                    <div>
                        <h5>Attenuation units</h5>
                        <p>Attenuation units. If Thorp attenuation formula selected, this overrides
                    any other attenuations specified.</p>
                    </div>
                    <div>
                        <h5>Add volume attenuation</h5>
                        <p>Thorp attenuation formula added.</p>
                    </div>
                    <div>
                        <h5>Top half-space properties:</h5>
                        <h6>Depth</h6>
                        <p>Depth (m).</p>
                        <h6>Top P-wave speed</h6>
                        <p>Top P-wave speed (m/s).</p>
                        <h6>Top S-wave speed</h6>
                        <p>Top S-wave speed (m/s).</p>
                        <h6>Top density</h6>
                        <p>Top density (g/cm<sup>3</sup>)</p>
                        <h6>Top P-wave attenuation</h6>
                        <p>Top P-wave attenuation (units as in Attenuation units input).</p>
                        <h6>Top S-wave attenuation</h6>
                        <p>Top S-wave attenuation.</p>
                        <div className="alert alert-info" role="alert">
                            Top half-space properties are required when "Acousto-elastic half-space" top boundary condition is selected.
                        </div>
                    </div>
                    <div>
                        <h5>Twersky scatter parameters:</h5>
                        <h6>Bump density</h6>
                        <p>Bump density (ridges/km).</p>
                        <h6>Principal radius 1</h6>
                        <p>Principal radius 1 (m).</p>
                        <h6>Principal radius 2</h6>
                        <p>Principal radius 2 (m).</p>
                        <div className="alert alert-info" role="alert">
                            This parameters are required when one of the Twersky-scatter options is selected.
                        </div>
                    </div>
                    <div>
                        <h5>Medium info</h5>
                        <p>Medium info consists of list(s) in the following format: [NMesh,Sigma,Z(NSSP)] , divided by comma (,). For instance: [100,0,1000],[200,0,3000],[200,0,5000] .</p>
                        <p><strong>NMesh:</strong> Number of mesh points to use initially.
                    The number of mesh points should be about 10
                    per vertical wavelength in acoustic media. In
                    elastic media, the number needed can vary quite
                    a bit; 20 per wavelength is a reasonable
                    starting point.
                    The maximum allowable number of mesh points is
                    given by 'MAXN' in the dimension statements.
                    At present 'MAXN' is 50000.  The number of mesh
                    points used depends on the initial mesh and the
                    number of times it is refined (doubled).  The
                    number of mesh doublings can vary from 1 to 5
                    depending on the parameter RMAX described
                    below.
                    If you type 0 for the number of mesh points,
                    the code will calculated NMESH automatically.
                    <br />
                            <strong>Sigma:</strong> RMS roughness at the interface.
                    <br />
                            <strong>Z(NSSP):</strong> Depth at bottom of medium (m).
                    This value is used to detect the last SSP point
                    when reading in the profile that follows. </p>
                    </div>
                    <div>
                        <h5>Sound speed profile</h5>
                        <p>Sound speed profile consists of lists in the following format: [Z(i),CP(i),CS(i),RHO(i),AP(i),AS(i)] (i=1,NSSP) , divided by comma (,). For instance: [0,1500,0,1,0,0], [1000,1550,0,1,0,0],[1000,1550,0,1,0,0],[3000,1500,0,1,0,0],[3000,1500,0,1,0,0],[5000,1550,0,1,0,0] .</p>
                        <p><strong>Z():</strong> Depth (m).
                    The surface starts at the first depth point
                    specified. Thus if you have say, XBT data which
                    starts at 50 m below the surface, then you'll
                    need to put in some SSP point at 0 m, otherwise
                    the free-surface would be placed at 50 m giving
                    erroneous results. The points Z(1) and Z(NSSP)
                    MUST correspond to the depths of interfaces
                    between media.
                    <br />
                            <strong>CP():</strong> P-wave speed (m/s).
                    <br />
                            <strong>CS():</strong> S-wave speed (m/s).
                    <br />
                            <strong>RHO():</strong> Density (g/cm<sup>3</sup>).
                        Density variations within an acoustic medium
                        are at present ignored.
                    <br />
                            <strong>AP():</strong> P-wave attenuation (units as in Attenuation units input).
                    <br />
                            <strong>AS():</strong> S-wave attenuation.
                    </p>
                    </div>
                    <div>
                        <h5>Type of bottom boundary condition</h5>
                        <p>Type of bottom boundary condition. Option 'Acousto-elastic half-space' is generally used for ocean bottom modeling</p>
                    </div>
                    <div>
                        <h5>Interfacial roughness</h5>
                        <p>Interfacial roughness (m)</p>
                    </div>
                    <div>
                        <h5>Bottom half-space properties:</h5>
                        <h6>Depth</h6>
                        <p>Depth (m).</p>
                        <h6>Bottom P-wave speed.</h6>
                        <p>Bottom P-wave speed (m/s).</p>
                        <h6>Bottom S-wave speed</h6>
                        <p>Bottom S-wave speed (m/s).</p>
                        <h6>Bottom density</h6>
                        <p>Bottom density (g/cm<sup>3</sup>).</p>
                        <h6>Bottom P-wave attenuation</h6>
                        <p>Bottom P-wave attenuation (units as in Attenuation units input).</p>
                        <h6>Bottom S-wave attenuation</h6>
                        <p>Bottom S-wave attenuation.</p>
                        <div className="alert alert-info" role="alert">
                            Bottom half-space properties are required when "Acousto-elastic half-space" bottom boundary condition is selected.
                        </div>
                    </div>
                    <div>
                        <h5>Lower phase speed limit</h5>
                        <p>
                            Lower phase speed limit (m/s).It will be computed automatically if you set it to zero. However, by using a nonzero lower phase limit you can skip the computation of slower modes. Mainly
                            this is used to exclude interfacial modes (e.g.a Scholte wave). The root finder is especially slow in converging to these interfacial
                            modes and when the source and receiver are sufficiently are far from the interface the interfacial modes are negligible.
                    </p>
                    </div>
                    <div>
                        <h5>Upper phase speed limit</h5>
                        <p>
                            Upper phase speed limit (m/s). The larger upper phase speed limit is, the more modes are calculated and the longer the execution time.
                            Therefore upper phase speed limit should be set as small as possible to minimize execution time.
                        <br />
                        On the other hand, upper phase speed limit controls the maximum ray angle included in a subsequent field calculation – ray paths are included which turn
                        at the depth corresponding to upper phase speed limit in the SSP. Thus a larger upper phase speed limit means more deeply
                        penetrating rays are included.
                        <br />
                        Choice of upper phase speed limit then becomes a matter of experience. In the far-field and at high-frequencies, rays travelling in the ocean
                        bottom are severely attenuated and one may set upper phase speed limit to the sound speed at the ocean bottom. In
                        the near-field, low-frequency case, rays refracted in the bottom may contribute significantly to the field and upper phase speed limit should be
                        chosen to include such ray paths.
                        <br />
                        KRAKEN will (if necessary) reduce upper phase speed limit so that
                        only trapped (non-leaky) modes are computed.
                    </p>
                    </div>
                    <div>
                        <h5>Maximum range</h5>
                        <p>
                            Maximum range (km). This parameter should be set to the largest range for which a field calculation will be desired.
                            During the mode calculation the mesh is doubled successively until the eigenvalues are sufficiently accurate at this range. If you set
                            it to zero, then no mesh doublings will be performed. You don't need to worry too much about this parameter – even if you set it to
                            zero the results will usually be reasonable.
                    </p>
                    </div>
                    <div>
                        <h5>The number of soure depths</h5>
                        <p>The number of source depths.</p>
                    </div>
                    <div>
                        <h5>The source depths</h5>
                        <p>The source depths (m). Source depths is a list of depths in the following format: depth(1),depth(2)[,depth(N)]. For instance: 500 .</p>
                    </div>
                    <div>
                        <h5>The number of receiver depths</h5>
                        <p>The number of receiver depths.</p>
                    </div>
                    <div>
                        <h5>The receiver depths</h5>
                        <p>The receiver depths (m). Receiver depths is a list of depths in the following format: depth(1),depth(2)[,depth(N)]. For instance: 0,2500 .</p>
                    </div>
                    <div>
                        <h5>Calculate transmission loss</h5>
                        <p>Set if you need to calculate transmission loss (using Field model).</p>
                    </div>
                    <div>
                        <h5>Source type</h5>
                        <p>Source type. Two options available:</p>
                        <ul>
                            <li>Point source (cylindrical (R-Z) coordinates);</li>
                            <li>Line source (cartesian   (X-Z) coordinates).</li>
                        </ul>
                    </div>
                    <div>
                        <h5>Modes theory</h5>
                        <p>Selects coupled or adiabatic mode theory.</p>
                    </div>
                    <div>
                        <h5>Number of modes in field computation</h5>
                        <p>
                            Number of modes to use in the field computation. If the number of modes specified exceeds the number computed then the program uses all the
                            computed modes.
                    </p>
                    </div>
                    <div>
                        <h5>The number of receiver ranges</h5>
                        <p>Number of receiver ranges.</p>
                    </div>
                    <div>
                        <h5>The receiver ranges</h5>
                        <p>The receiver ranges (km). Receiver ranges is a list of ranges in the following format: range(1),range(2)[,range(N)]. For instance: 200,220 .</p>
                    </div>
                    <div>
                        <h5>The number of soure depths</h5>
                        <p>The number of source depths.</p>
                    </div>
                    <div>
                        <h5>The source depths</h5>
                        <p>The source depths (m). Source depths is a list of depths in the following format: depth(1),depth(2)[,depth(N)]. For instance: 500 .</p>
                    </div>
                    <div>
                        <h5>The number of receiver depths</h5>
                        <p>The number of receiver depths.</p>
                    </div>
                    <div>
                        <h5>The receiver depths</h5>
                        <p>The receiver depths (m). Receiver depths is a list of depths in the following format: depth(1),depth(2)[,depth(N)]. For instance: 0,2500 .</p>
                    </div>
                    <div>
                        <h5>The number of receiver range-displacements</h5>
                        <p>The number of receiver range-displacements. Must equal "Number of receiver depths".</p>
                    </div>
                    <div>
                        <h5>The receiver displacements</h5>
                        <p>
                            The receiver displacements (m). Receiver displacements is a list of numbers in the following format: displ(1),displ(2)[,displ(N)]. For instance: 0 .
                        <br />
                        This vector should be all zeros for a perfectly vertical array.
                    </p>
                    </div>
                </section>
                <section id="file-upload">
                    <h2>File upload</h2>
                    <hr />
                    <p>
                        This section contains description of data structure for upload data for form inputs from file.
                    </p>
                    <div>
                        <p>The acoustic problem data is described in <code>JSON</code> format. Data structure sample:</p>
                        <ReactJson src={uploadJsonExample} displayDataTypes={false} displayObjectSize={false} name={false} />
                    </div>

                    <div className="file-upload-table">
                        <p>The table contains comparisons between properties in JSON object for upload and <Link smooth to="#form-description">form inputs</Link>.</p>
                        <Table responsive bordered hover>
                            <thead>
                                <tr>
                                    <th>Object property</th>
                                    <th>Form input</th>
                                    <th>Note</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>frequency</td>
                                    <td>Frequency</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nModes</td>
                                    <td>Number of modes</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nMedia</td>
                                    <td>Number of media</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>topBCType</td>
                                    <td>Type of top boundary condition</td>
                                    <td>
                                        <span>Comprasion between values:</span>
                                        <ul>
                                            <li>"V" – Vacuum above top</li>
                                            <li>"A" – Acousto-elastic half-space</li>
                                            <li>"R" – Perfectly rigid</li>
                                            <li>"S" – Soft-boss Twersky scatter</li>
                                            <li>"H" – Hard-boss Twersky scatter</li>
                                            <li>"T" – Soft-boss Twersky scatter, amplitude only</li>
                                            <li>"I" – Hard-boss Twersky scatter, amplitude only</li>
                                        </ul>
                                    </td>
                                </tr>
                                <tr>
                                    <td>interpolationType</td>
                                    <td>Type of interpolation</td>
                                    <td>
                                        <span>Comprasion between values:</span>
                                        <ul>
                                            <li>"N" – N2-linear</li>
                                            <li>"C" – C-linear</li>
                                            <li>"S" – Cubic Spline</li>
                                        </ul>
                                    </td>
                                </tr>
                                <tr>
                                    <td>attenuationUnits</td>
                                    <td>AttenuationUnits</td>
                                    <td>
                                        <span>Comprasion between values:</span>
                                        <ul>
                                            <li>"N" – Nepers/m</li>
                                            <li>"F" – dB/(kmHz)</li>
                                            <li>"M" – dB/m</li>
                                            <li>"W" – dB/wavelength</li>
                                            <li>"Q" – quality factor</li>
                                            <li>"T" – Thorp attenuation formula</li>
                                        </ul>
                                    </td>
                                </tr>
                                <tr>
                                    <td>isVolumeAttenuatonAdded</td>
                                    <td>Add volume attenuation</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>zt</td>
                                    <td>Depth</td>
                                    <td>One of top half-space properties</td>
                                </tr>
                                <tr>
                                    <td>cpt</td>
                                    <td>Top P-wave speed</td>
                                    <td>One of top half-space properties</td>
                                </tr>
                                <tr>
                                    <td>cst</td>
                                    <td>Top S-wave speed</td>
                                    <td>One of top half-space properties</td>
                                </tr>
                                <tr>
                                    <td>rhot</td>
                                    <td>Top density</td>
                                    <td>One of top half-space properties</td>
                                </tr>
                                <tr>
                                    <td>apt</td>
                                    <td>Top P-wave attenuation</td>
                                    <td>One of top half-space properties</td>
                                </tr>
                                <tr>
                                    <td>ast</td>
                                    <td>Top S-wave attenuation</td>
                                    <td>One of top half-space properties</td>
                                </tr>
                                <tr>
                                    <td>bumDen</td>
                                    <td>Bump density</td>
                                    <td>One of Twersky scatter parameters</td>
                                </tr>
                                <tr>
                                    <td>eta</td>
                                    <td>Principal radius 1</td>
                                    <td>One of Twersky scatter parameters</td>
                                </tr>
                                <tr>
                                    <td>xi</td>
                                    <td>Principal radius 2</td>
                                    <td>One of Twersky scatter parameters</td>
                                </tr>
                                <tr>
                                    <td>mediumInfo</td>
                                    <td>Medium info</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>ssp</td>
                                    <td>Sound speed profile</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>bottomBCType</td>
                                    <td>Type of bottom boundary condition</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>sigma</td>
                                    <td>Interfacial roughness</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>zb</td>
                                    <td>Depth</td>
                                    <td>One of bottom half-space properties</td>
                                </tr>
                                <tr>
                                    <td>cpb</td>
                                    <td>Bottom P-wave speed</td>
                                    <td>One of bottom half-space properties</td>
                                </tr>
                                <tr>
                                    <td>csb</td>
                                    <td>Bottom S-wave speed</td>
                                    <td>One of bottom half-space properties</td>
                                </tr>
                                <tr>
                                    <td>rhob</td>
                                    <td>Bottom density</td>
                                    <td>One of bottom half-space properties</td>
                                </tr>
                                <tr>
                                    <td>apb</td>
                                    <td>Bottom P-wave attenuation</td>
                                    <td>One of bottom half-space properties</td>
                                </tr>
                                <tr>
                                    <td>asb</td>
                                    <td>Bottom S-wave attenuation</td>
                                    <td>One of bottom half-space properties</td>
                                </tr>
                                <tr>
                                    <td>сLow</td>
                                    <td>Lower phase speed limit</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>cHigh</td>
                                    <td>Upper phase speed limit</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>rMax</td>
                                    <td>Maximum range</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nsd</td>
                                    <td>The number of soure depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>sd</td>
                                    <td>The source depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nrd</td>
                                    <td>The number of receiver depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>rd</td>
                                    <td>The receiver depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>calculateTransmissionLoss</td>
                                    <td>Calculate transmission loss</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>sourceType</td>
                                    <td>Source type</td>
                                    <td>
                                        <span>Comprasion between values:</span>
                                        <ul>
                                            <li>"R" – Point source</li>
                                            <li>"X" – Line source</li>
                                        </ul>
                                    </td>
                                </tr>
                                <tr>
                                    <td>modesTheory</td>
                                    <td>Modes theory</td>
                                    <td>
                                        <span>Comprasion between values:</span>
                                        <ul>
                                            <li>"C" – Coupled mode theory</li>
                                            <li>"A" – Adiabatic mode theory</li>
                                        </ul>
                                    </td>
                                </tr>
                                <tr>
                                    <td>nModesForField</td>
                                    <td>Number of modes in field computation</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nr</td>
                                    <td>The number of receiver ranges</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>r</td>
                                    <td>The receiver ranges</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nsdField</td>
                                    <td>The number of soure depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>sdField</td>
                                    <td>The source depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nrdField</td>
                                    <td>The number of receiver depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>rdField</td>
                                    <td>The receiver depths</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>nrr</td>
                                    <td>The number of receiver range-displacements</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>rr</td>
                                    <td>The receiver displacements</td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </Table>
                    </div>
                </section>
            </Col>
        </Row>);
    }
}