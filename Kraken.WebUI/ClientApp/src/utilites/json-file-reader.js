export const readJsonFile = (file) => {    
    return new Promise((resolve, reject) => {
        let reader = new FileReader();     

        reader.onload = function () {
            const json = tryParseJSON(reader.result);

            resolve(json);
        };

        reader.onerror = function () {
            throw "File read error";
        };

        reader.readAsText(file);
    });
};

const tryParseJSON = (jsonString) =>{
    try {
        var o = JSON.parse(jsonString);

        if (o && typeof o === "object") {
            return o;
        }
    }
    catch (e) { }

    return false;
};