


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


$(document).on("click", ".reply",async function (e){
    e.preventDefault();
    let link = $(this).attr("href");

    const { value: text } = await Swal.fire({
        input: 'textarea',
        inputLabel: 'Message',
        inputPlaceholder: 'Type your message here...',
        inputAttributes: {
            'aria-label': 'Type your message here'
        },
        showCancelButton: true
    })
    if (text) {
    link += `?replyMessage=${text}`;
    fetch(link)
        .then(res => {
            if (!res.ok) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!',
                })
               
            }
            else {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Your reply is sent',
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    location.reload();
                })
            }
        })


      
    }
})

$(document).on("click", ".replied", function (e) {
    e.preventDefault();
    let link = $(this).attr("href");
    fetch(link)
        .then(res => res.json())
        .then(data => {
            Swal.fire({
                icon: 'info',
                text: data.replyMessage,
            })
        })

            


    
})

