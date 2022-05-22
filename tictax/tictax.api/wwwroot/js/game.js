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
            GAME_BEGIN: 'gb',
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

        const backToLobby = () => {
            window.location.href = './lobby.html'
        }

        function resetBoard() {
            squareElements.forEach(square => {
                square.textContent = "";
            })
            overlayElement.style.display = "none";
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
        var opponent = ""

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
                let data = null;
                // Validate message to check if it really
                // is a game message.
                console.log('received:', event.data);
                try {
                    data = JSON.parse(event.data);
                    if (!data.hasOwnProperty('type')) {
                        throw Error('Message was parsed correctly, but it doesn\'t contain attribute "type"');
                    }
                } catch (error) {
                    console.log("Unrecognized message:", event.data);
                    console.log(error);
                    return;
                }

                // Process game logic
                switch (data.type) {
                    case protoMsgTypes.GAME_BEGIN:
                        {
                            // Game has just started
                            // Let the user know whether it's their time
                            // to make a move
                            $('#player1').text(username);
                            $('#player2').text(data.opponent);

                            opponent = data.opponent;

                            if (data.isYourTurn) {
                                $('#txtGameMessage').text('It\'s your turn');
                            } else {
                                $('#txtGameMessage').text('Waiting for your opponent\'s first move');
                            }

                            break;
                        }
                    case protoMsgTypes.GAME_STATE:
                        {
                            if (data.validMoves.length < 9) {
                                console.log('Valid moves less than 9');
                                // Hide message about whose turn it is
                                // once at least one move was made.
                                $('#txtGameMessage').text('');
                            }

                            // Render board based on the state
                            for (let i = 0; i < data.board.length; ++i) {
                                for (let j = 0; j < data.board[i].length; ++j) {
                                    squareElements[i * 3 + j].innerHTML = data.board[i][j];
                                }
                            }
                            break;
                        }
                    case protoMsgTypes.GAME_END:
                        {
                            // Game ended, display winner 

                            // Check if it's a tie or someone has won
                            if (data.isTie) {
                                $('#txtGameMessage').text('IT\'S A TIE!');
                            } else {
                                if (data.winner === username) {
                                    // You are a winner                                    
                                    $('#txtGameMessage').text(`YOU WON!`);
                                } else {
                                    $('#txtGameMessage').text(`${data.winner} WON!`);
                                }
                            }

                            // Update score
                            $('#player1Score').html(`<strong>${data.score[username]}</strong>`)
                            $('#player2Score').html(`<strong>${data.score[opponent]}</strong>`)

                            break;
                        }
                    case protoMsgTypes.PLAYER_DISCONNECTED:
                        {
                            // Opponent disconnected
                            if (data.gotoLobby) {
                                backToLobby();
                                return;
                            } else {
                                $('#txtGameMessage').text('Waiting for someone to join');
                            }
                            break;
                        }
                    case protoMsgTypes.ERROR:
                        {
                            // Received an error.
                            showError(data.message);
                            break;
                        }

                    default:
                }

            });

            // Listen for messages
            socket.addEventListener('close', function(event) {
                console.log('## CLOSED ##');
                const reason = event.reason;
                const code = event.code;

                console.log('code: ' + code);
                console.log('reason: ' + reason);

                backToLobby();
            });

            socket.addEventListener('error', function(event) {
                console.log('## ERROR ##');
                console.log(event);
                showError(event);
            });

        }

        $('button[cell]').click(function() {
            elem = $(this);
            let i = parseInt(elem.attr('cell'))

            // Client move validity check
            if (elem.html() !== '') {
                console.log("Client validity check")
                return;
            }

            socket.send(JSON.stringify({
                type: protoMsgTypes.PLAY_MOVE,
                matchId: MATCH_ID,
                cell: i
            }));

        })

        $('#cmdDisconnect').click(function() {
            socket.send(JSON.stringify({
                type: protoMsgTypes.LEAVE_MATCH
            }));
            backToLobby();
        });

        try {
            initSocket(WS_ADRESS);
        } catch (error) {
            console.log(error);
            showError(error);
        }

    });
});