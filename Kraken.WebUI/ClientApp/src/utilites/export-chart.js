import domtoimage from 'dom-to-image';
import fileDownload from "js-file-download";

export const exportChart = (elementId, downloadFileName = null) => {
    const fileName = downloadFileName || elementId;
    domtoimage.toBlob(document.getElementById(elementId), { bgcolor: "#FFFFFF" })
        .then(function (blob) {
            fileDownload(blob, `${fileName}.png`);
        });
};