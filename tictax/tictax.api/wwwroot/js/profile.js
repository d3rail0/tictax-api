$(document).ready(function() {

    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });

    let value = params.player;
    const username = localStorage.getItem('tictax_username');

    console.log("player", value);

    const getProfileData = async(username) => {

        await fetch('/api/profile' + new URLSearchParams({
                player: username
            }))
            .then(resp => {
                if (!resp.ok) {
                    throw 'Something went wrong';
                }
                return resp.json();
            })
            .then(data => {
                console.log("##### recv:", data);

            })
            .catch(err => {
                console.log(err);
            });


        // await fetch('/api/matches', {
        //     method: 'POST',
        //     body: JSON.stringify(body),
        //     headers: {
        //         'Content-type': 'application/json',
        //         'Authorization': 'bearer ' + stored_token
        //     }
        // })
        // .then(resp => {
        //     if (!resp.ok) {
        //         throw 'Something went wrong';
        //     }
        //     return resp.json();
        // })
        // .then(data => {
        //     console.log("##### recv:", data);

        // })
        // .catch(err => {
        //     console.log(err);
        // });
    }

    $('#txtPlayerName').text(value);



    // if (value !== username) {
    //     // Don't show "Update status" button 
    //     // if player is viewing someone elses profile
    //     $('#cmdUpdateStatus').remove();
    // } else {
    //     $('#cmdLike').remove();
    // }

});