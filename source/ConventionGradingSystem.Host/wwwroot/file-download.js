window.downloadFileFromStream = async (fileName, fileType, fileStream) => {
  const arrayBuffer = await fileStream.arrayBuffer();
  const blob = new Blob([arrayBuffer], { type: fileType });

  const objectUrl = URL.createObjectURL(blob);
  const anchorElement = document.createElement('a');
  anchorElement.href = objectUrl;
  anchorElement.download = fileName;

  anchorElement.click();
  anchorElement.remove();
  URL.revokeObjectURL(objectUrl);
}
