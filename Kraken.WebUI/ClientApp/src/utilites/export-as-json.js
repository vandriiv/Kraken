export const exportAsJson = (content, fileName)  => {
    const a = document.createElement("a");
    const file = new Blob([content], { type: 'application/json' });
    a.href = URL.createObjectURL(file);
    a.download = fileName;
    a.click();
}