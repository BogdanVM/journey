$('#edit-photo-btn').click(() => {
    const ImageEditor = new FilerobotImageEditor();
    const photoName = $('#edit-photo-btn').attr('name');

    ImageEditor.open('https://localhost:44363/Images/' + photoName);
});