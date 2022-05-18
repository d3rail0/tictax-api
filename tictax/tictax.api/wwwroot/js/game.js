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