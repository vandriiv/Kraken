const downloadCsv = (csv, filename) => {
    const csvFile = new Blob([csv], { type: "text/csv" });

    const downloadLink = document.createElement("a");

    downloadLink.download = filename;
    downloadLink.href = window.URL.createObjectURL(csvFile);
    downloadLink.style.display = "none";
    document.body.appendChild(downloadLink);
    downloadLink.click();
};

export const exportTableToCsv = (tableID, filename = '') => {
    const csv = [];

    filename = filename ? filename + '.csv' : tableID + '.csv';

    let rows = document.querySelectorAll(`#${tableID} tr`);

    for (let i = 0; i < rows.length; i++) {
        let row = [], cols = rows[i].querySelectorAll("td, th");

        for (let j = 0; j < cols.length; j++)
            row.push(cols[j].innerText);

        csv.push(row.join(","));
    }

    downloadCsv(csv.join("\n"), filename);
}