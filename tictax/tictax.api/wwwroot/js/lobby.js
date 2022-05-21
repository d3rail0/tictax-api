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

        $("#roomTable").on('click', 'button[class*=\'join-btn\']', function() {
            elem = $(this);
            match_id = elem.attr('data')
            console.log("join ->", match_id);
        })

        $('#cmdRefresh').click(function() {

            $("#roomTable").empty();

            fetch('api/matches', {
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


        // Refresh automatically when the page is opened
        $('#cmdRefresh').click();

    });
});