
$(document).on("click", ".delete-btn", function (e) {
    e.preventDefault();
    console.log($(this));
    let link = $(this).attr("href");
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    })
        .then((result) => {
            if (result.isConfirmed) {


                fetch(link)
                    .then(res => {
                        if (!res.ok) {
                            Swal.fire({
                                icon: 'error',
                                title: 'Oops...',
                                text: 'Something went wrong!',
                            })
                            throw new Error("something went wrong");
                            return
                        }
                        Swal.fire(
                            'Deleted!',
                            'Your file has been deleted.',
                            'success'
                        ).then(()=> location.reload())
                    }).catch(err => console.error(err));

            }
        })
})
