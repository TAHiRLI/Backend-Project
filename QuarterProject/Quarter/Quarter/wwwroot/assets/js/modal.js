$(document).on("click", ".modalBtn", function (e) {
    e.preventDefault();
    let modal_link = $(this).attr("href");
    fetch(modal_link)
        .then(res => {
            if (!res.ok) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!',
                })
                return
            }
           return  res.text()
        })
        .then(html => {
            $("#house-modal-container").html(html);
        })
})
