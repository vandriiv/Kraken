import { json2xml } from 'xml-js';

export const jsonToXml = (json) => {    
    const options = { compact: true, ignoreComment: true, spaces: 4 };   
    return json2xml(json, options);
};