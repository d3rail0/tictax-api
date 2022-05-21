$(document).ready(function() {
    $('#includedContent').load('toast.html', function() {

        // Messages types defined by game protocol
        const protoMsgTypes = {
            AUTH: 'auth',
            CREATE_MATCH: 'create',
            JOIN_MATCH: 'join',
            LEAVE_MATCH: 'leave',
            ERROR: 'error',
            GAME_STATE: 'gs',
            GAME_END: 'ge',
            PLAYER_DISCONNECTED: 'p_disconn',
            PLAY_MOVE: 'move'
        }

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

        const back_to_lobby = () => {
            window.location.href = './lobby.html'
        }

        const boardElement = document.querySelector('.board');
        const squareElements = document.querySelectorAll('.square');

        let isXNext = false;
        let squares = Array(9).fill(null);
        let winner = null;

        const jwtToken = localStorage.getItem('tictax_jwt_token')
        const username = localStorage.getItem('tictax_username')
        const isOwner = localStorage.getItem('tictax_is_owner') === 'true'

        const WS_ADRESS = localStorage.getItem('tictax_game_server')
        const MATCH_ID = parseInt(localStorage.getItem('tictax_match_id'))

        var socket = undefined;

        // elem.html('<strong>abc</abc>');

        const initSocket = (address) => {

            if (socket != null && socket.readyState === socket.OPEN) {
                socket.close();
            }

            // Create WebSocket connection.
            socket = new WebSocket(address);

            // Connection opened
            socket.addEventListener('open', function(event) {
                socket.send(`{"type": \"${protoMsgTypes.AUTH}\", "token": "${jwtToken}"}`);

                if (isOwner) {
                    socket.send(JSON.stringify({
                        type: protoMsgTypes.CREATE_MATCH
                    }));
                } else {
                    console.log("Connecting -> ", JSON.stringify({
                        type: protoMsgTypes.JOIN_MATCH,
                        matchId: MATCH_ID
                    }));
                    socket.send(JSON.stringify({
                        type: protoMsgTypes.JOIN_MATCH,
                        matchId: MATCH_ID
                    }));
                }

            });

            // Listen for messages
            socket.addEventListener('message', function(event) {
                console.log(event.data);
            });

            // Listen for messages
            socket.addEventListener('close', function(event) {
                console.log('## CLOSED ##');
                const reason = event.reason;
                const code = event.code;

                console.log('code: ' + code);
                console.log('reason: ' + reason);

                back_to_lobby();
            });

            socket.addEventListener('error', function(event) {
                console.log('## ERROR ##');
                console.log(event);
                showError(event);
            });

        }


        function handleClick(e, i) {
            if (squares[i] === null && winner === null) {
                squares[i] = isXNext ? 'X' : 'O';
                isXNext = !isXNext;
                e.innerHTML = squares[i];
            }
        }

        function resetGame() {
            squareElements.forEach(square => {
                square.textContent = "";
            })
            overlayElement.style.display = "none";
        }

        $('button[cell]').click(function() {
            elem = $(this);
            let i = parseInt(elem.attr('cell'))

            try {

                socket.send(JSON.stringify({
                    type: protoMsgTypes.PLAY_MOVE,
                    matchId: MATCH_ID,
                    cell: i
                }));

            } catch (error) {
                console.log(error);
                showError(error);
            }

        })

        $('#cmdDisconnect').click(function() {
            socket.send(JSON.stringify({
                type: protoMsgTypes.LEAVE_MATCH
            }));
            back_to_lobby();
        });

        try {
            initSocket(WS_ADRESS);
        } catch (error) {
            console.log(error);
            showError(error);
        }

    });
});

// var socket = undefined;
// const WS_ADDRESS = 'ws://127.0.0.1:13024';

// const initSocket = (address) => {

//     if (socket != null && socket.readyState === socket.OPEN) {
//         socket.close();
//     }

//     // Create WebSocket connection.
//     socket = new WebSocket(address);

//     // Connection opened
//     socket.addEventListener('open', function(event) {
//         socket.send('{"type": "auth", "token": ""}');
//     });

//     // Listen for messages
//     socket.addEventListener('message', function(event) {
//         console.log('Message from server ', event.data);
//     });

//     // Listen for messages
//     socket.addEventListener('close', function(event) {
//         console.log('## CLOSED ##');
//         const reason = event.reason;
//         const code = event.code;

//         console.log('code: ' + code);
//         console.log('reason: ' + reason);
//     });

//     // Listen for messages
//     socket.addEventListener('error', function(event) {
//         console.log('## ERROR ##');
//         console.log(event)
//     });

// }

// initSocket(WS_ADDRESS);