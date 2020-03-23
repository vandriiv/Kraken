export const exportAsXml = (content, fileName) => {
    const a = document.createElement("a");
    const file = new Blob([content], { type: 'application/xml' });
    a.href = URL.createObjectURL(file);
    a.download = fileName;
    a.click();
}