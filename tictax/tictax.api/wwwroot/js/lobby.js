$(document).ready(function() {

    $("#includedContent").load("toast.html", function() {
        var toastLiveExample = document.getElementById('liveToast');
        var toastMessageElem = document.getElementById('toastMessage');
        var toastHeaderElem = document.getElementById('toastHeader');
        var toastHeaderTextElem = document.getElementById('toastHeaderText');

        const showToast = (headerTxt, msg) => {
            const toast = new bootstrap.Toast(toastLiveExample);
            toastMessageElem.innerText = msg;
            toastHeaderTextElem.innerText = headerTxt;
            toast.show()
        }

        const showError = (errMsg) => {
            toastLiveExample.classList.remove('bg-success');
            toastLiveExample.classList.add('bg-danger');
            toastHeaderElem.classList.remove('bg-success');
            toastHeaderElem.classList.add('bg-danger');
            showToast("Error", errMsg);
        }

        const showInfo = (msg) => {
            toastLiveExample.classList.remove('bg-danger');
            toastLiveExample.classList.add('bg-success');
            toastHeaderElem.classList.remove('bg-danger');
            toastHeaderElem.classList.add('bg-success');
            showToast("Success", msg);
        }

        const b64_to_utf8 = (str) => {
            return decodeURIComponent(escape(window.atob(str)));
        }

        const decode_token = (token) => {
            token = token.slice(token.indexOf('.') + 1, token.lastIndexOf('.'));
            decoded = JSON.parse(b64_to_utf8(token))
            return {
                username: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
                exp: decoded['exp']
            }
        }

        // Verify if token already exists in local storage
        // If it exists, check whether it has expired.
        const stored_token = localStorage.getItem('tictax_jwt_token');
        let needs_to_login = true;
        if (stored_token !== null) {
            try {
                decoded = decode_token(stored_token);
                const ts = Math.round((new Date()).getTime() / 1000);
                if (ts < decoded.exp) {
                    needs_to_login = false
                }
            } catch (error) {
                console.log(error);
            }
        }

        if (needs_to_login) {
            window.location.href = './index.html';
            return
        }

        const initGameData = async(matchId, isOwner) => {
            // Initializes required local storage items to prepare
            // client for connection with the game server.
            // Returns true if successful at doing so.
            localStorage.setItem('tictax_is_owner', isOwner);

            // Get WS Host
            const body = {
                matchId: matchId
            };

            let isError = false;

            await fetch('/api/matches', {
                    method: 'POST',
                    body: JSON.stringify(body),
                    headers: {
                        'Content-type': 'application/json',
                        'Authorization': 'bearer ' + stored_token
                    }
                })
                .then(resp => {
                    if (!resp.ok) {
                        throw 'Couldn\'t fetch websocket server host';
                    }
                    return resp.json();
                })
                .then(data => {
                    console.log("##### recv:", data);
                    localStorage.setItem('tictax_game_server', data.wsServerHost);
                    localStorage.setItem('tictax_match_id', data.matchId);
                })
                .catch(err => {
                    isError = true;
                    console.log("#### ROOM JOIN ERROR");
                    console.log(err);
                    showError(err);
                });

            return !isError;
        }

        $('#cmdLogout').click(function() {
            localStorage.setItem('tictax_jwt_token', null);
            window.location.href = './index.html';
        });
        $('#cmdProfile').click(function() {
            let searchParams = new URLSearchParams();
            let username = localStorage.getItem('tictax_username');
            searchParams.set('player', username);
            window.location.href = './profile.html?' + searchParams.toString();
        });

        $("#roomTable").on('click', 'button[class*=\'join-btn\']', async function() {
            elem = $(this);
            matchId = parseInt(elem.attr('data'));
            console.log("join ->", matchId);

            if (await initGameData(matchId, false)) {
                // Redirect to game.html
                window.location.href = './game.html';
            }
        })

        $('#cmdRefresh').click(function() {

            $("#roomTable").empty();

            fetch('/api/matches', {
                    method: 'GET',
                    headers: {
                        'Authorization': 'bearer ' + stored_token
                    }
                })
                .then(resp => {
                    if (!resp.ok) {
                        throw 'Couldn\'t fetch available rooms';
                    }
                    return resp.json();
                })
                .then(data => {
                    // Display available matches within the server browser table
                    $('#totalRoomsText').text(`Total rooms: ${data.totalMatches}`);
                    $('#activeRoomsText').text(`Available rooms: ${data.availableMatches.length}`);
                    const serverBrowser = $('#roomTable');
                    data.availableMatches.forEach(match => {
                        const table_row = document.createElement('tr');
                        const td_room_id = document.createElement('td');
                        const td_creator = document.createElement('td');
                        const td_join_button = document.createElement('td');

                        table_row.appendChild(td_room_id);
                        table_row.appendChild(td_creator);
                        table_row.appendChild(td_join_button);

                        td_room_id.innerText = match.matchId;
                        td_creator.innerText = match.owner;

                        const join_button = document.createElement('button');
                        join_button.classList.add('btn');
                        join_button.classList.add('btn-warning');
                        join_button.classList.add('join-btn');
                        join_button.setAttribute('data', `${match.matchId}`);
                        join_button.innerText = 'Join';

                        td_join_button.appendChild(join_button);
                        serverBrowser.append(table_row);
                    });
                })
                .catch(err => {
                    console.log(err);
                    showError(err);
                })
        });

        $('#cmdCreateRoom').click(async function() {
            if (await initGameData(0, true)) {
                // Redirect to game.html
                window.location.href = './game.html';
            }
        });


        // Refresh server list automatically when the page is loaded
        $('#cmdRefresh').click();

    });
});