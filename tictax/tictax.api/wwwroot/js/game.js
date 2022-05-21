const boardElement = document.querySelector(".board");
const squareElements = document.querySelectorAll(".square");

let isXNext = false;
let squares = Array(9).fill(null);
let winner = null;

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


var socket = undefined;
const WS_ADDRESS = 'ws://127.0.0.1:13024';

const initSocket = (address) => {

    if (socket != null && socket.readyState === socket.OPEN) {
        socket.close();
    }

    // Create WebSocket connection.
    socket = new WebSocket(address);

    // Connection opened
    socket.addEventListener('open', function(event) {
        socket.send('{"type": "auth", "token": ""}');
    });

    // Listen for messages
    socket.addEventListener('message', function(event) {
        console.log('Message from server ', event.data);
    });

    // Listen for messages
    socket.addEventListener('close', function(event) {
        console.log('## CLOSED ##');
        const reason = event.reason;
        const code = event.code;

        console.log('code: ' + code);
        console.log('reason: ' + reason);
    });

    // Listen for messages
    socket.addEventListener('error', function(event) {
        console.log('## ERROR ##');
        console.log(event)
    });

}

initSocket(WS_ADDRESS);