$(document).ready(function() {

    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });

    let loadedProfileUsername = params.player;
    const loggedInUsername = localStorage.getItem('tictax_username');
    const storedToken = localStorage.getItem('tictax_jwt_token');

    let isAvailable = false;

    const fetchAndRenderProfileData = (username) => {
        console.log("fetching profile data for:", username);

        fetch('/api/profile?' + new URLSearchParams({
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

                $('#txtPlayerName').text(data.username);
                $('#txtWins').text(data.totalWins);
                $('#txtDraws').text(data.totalDraws);
                $('#txtLosses').text(data.totalLosses);

                isAvailable = data.isAvailable;

                if (data.isAvailable) {
                    $('#txtIsAvailable').text('YES');
                } else {
                    $('#txtIsAvailable').text('NO');
                }

                const activityFeedElem = $('#activityFeed');
                activityFeedElem.empty();
                data.activityFeed.forEach(activity => {

                    // Check if the user in current iteration has liked this 
                    // profile
                    if (activity.username == loggedInUsername) {
                        $('#cmdLike span').text('Dislike');
                    }


                    const liElem = document.createElement('li');
                    const glyphElem = document.createElement('i');
                    glyphElem.classList.add(['activity__list__icon', 'fa', 'fa-question-circle-o'])
                    const divElem = document.createElement('div');
                    divElem.classList.add('activity__list__header')
                    const avatarElem = document.createElement('img');
                    avatarElem.setAttribute('src', '/img/avatar.png');
                    avatarElem.setAttribute('alt', '');
                    const linkToUser = document.createElement('a');
                    linkToUser.classList.add('userLink');
                    linkToUser.setAttribute('href', '#');
                    linkToUser.innerText = activity.username;

                    const spanElem = document.createElement('span');
                    spanElem.innerText = ' liked this profile';
                    divElem.appendChild(avatarElem);
                    divElem.appendChild(linkToUser)
                    divElem.appendChild(spanElem);

                    liElem.appendChild(glyphElem);
                    liElem.appendChild(divElem);
                    activityFeedElem.append(liElem);
                });

                // $('$').text(data.);

            })
            .catch(err => {
                console.log(err);
            });

    }


    fetchAndRenderProfileData(loadedProfileUsername);


    $('#cmdUpdateStatus').click(async function() {
        //Update user status 
        body = {
            "newProfileState": !isAvailable // boolean as switch
        }

        await fetch('/api/profile/', {
                method: 'PUT',
                body: JSON.stringify(body),
                headers: {
                    'Content-type': 'application/json',
                    'Authorization': 'bearer ' + storedToken
                }
            })
            .then(resp => {
                if (!resp.ok) {
                    throw 'Something went wrong';
                }
                return resp.text();
            })
            .then(data => {
                console.log("Successful profile update");
            })
            .catch(err => {
                console.log(err);
            });

        await fetchAndRenderProfileData(loadedProfileUsername);
    });

    $('#cmdLike').click(async function() {
        elem = $(this);
        console.log(elem);

        method = 'POST'

        if (elem.text().includes("Dislike")) {
            // Delete like if logged in user
            // has already liked this profile.
            method = 'DELETE'
        }

        // Like this profile
        body = {
            "username": loadedProfileUsername
        }

        await fetch('/api/profile/action/like', {
                method: method,
                body: JSON.stringify(body),
                headers: {
                    'Content-type': 'application/json',
                    'Authorization': 'bearer ' + storedToken
                }
            })
            .then(resp => {
                if (!resp.ok) {
                    throw 'Something went wrong';
                }
                return resp.text();
            })
            .then(data => {
                console.log("Successful like action");
                if (method === 'POST') {
                    // Logged in user has just liked this profile
                    $('#cmdLike span').text('Dislike');
                } else {
                    // Just disliked profile
                    $('#cmdLike span').text('Like');
                }
            })
            .catch(err => {
                console.log(err);
            });

        await fetchAndRenderProfileData(loadedProfileUsername);
    });

    $("#activityFeed").on('click', 'a[class*=\'userLink\']', async function() {
        elem = $(this);
        // User link was clicked, redirect to new profile
        let searchParams = new URLSearchParams();
        searchParams.set('player', elem.text());
        window.location.href = './profile.html?' + searchParams.toString();
    });

    $('#cmdBackToLobby').click(function() {
        window.location.href = './lobby.html';
    });

    if (loadedProfileUsername !== loggedInUsername) {
        // Don't show "Update status" button 
        // if player is viewing someone elses profile
        $('#cmdUpdateStatus').remove();
    } else {
        $('#cmdLike').remove();
    }

});