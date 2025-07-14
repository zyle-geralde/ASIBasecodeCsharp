export function updateImagePreview(file, previewElement, placeholderElement, fileNameDisplay) {
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            previewElement.src = e.target.result;
            previewElement.style.display = 'block';
            placeholderElement.style.display = 'none';
            fileNameDisplay.value = file.name;
        };
        reader.readAsDataURL(file);
    } else {
        previewElement.src = '';
        previewElement.style.display = 'none';
        placeholderElement.style.display = 'block';
        fileNameDisplay.value = '';
    }
}