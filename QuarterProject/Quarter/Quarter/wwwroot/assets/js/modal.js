$(document).on("click", ".modalBtn", function (e) {
    e.preventDefault();
    let link = $(this).attr("href");
    fetch(link)
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
