﻿

$(document).on("click", ".load-more-btn", function (e) {
    e.preventDefault();
    let link = $(this).attr("href");
    let skipCount = parseInt($(this).attr("skipCount"))
    let takeCount = parseInt($(this).attr("takeCount"))
    let commentCount = parseInt($(this).attr("commentCount"))


    let newSkipCount = skipCount + takeCount;
    if (newSkipCount + takeCount >= commentCount) {
        e.target.classList.add("d-none");
    }
    link += "&skipCount=" + `${newSkipCount}`



    console.log(skipCount)

    fetch(link)
        .then(res => {
            if (!res.ok) {
                throw new Error("something Went Wrong")
                return
            }
            return res.text();
        }).then(data => {
            console.log(data.length)
            if (data.length == 0) {
                console.log("salam")
            }

            document.getElementById("comment-container").innerHTML += data;

            console.log(data);
        })

        .then(() => {
            $(this).attr("skipCount", newSkipCount)
          
        })
        .catch(err => console.log(err))


})



let link = "/house/GetSearchRecommendation?search=";
let timeout = null;

$(document).on("keyup", ".searchInput", function (e) {

    clearTimeout(timeout);
    timeout = setTimeout(() => {
    let newLink = link + e.target.value
    
    fetch(newLink)
        .then(res => res.text())
        .then(data => {
            console.log(data)
            $("#search-holder").html(data);
        })

    },1000)
})



