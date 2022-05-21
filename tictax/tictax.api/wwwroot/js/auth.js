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
        stored_token = localStorage.getItem('tictax_jwt_token');
        if (stored_token !== null) {
            try {
                decoded = decode_token(stored_token);
                const ts = Math.round((new Date()).getTime() / 1000);
                if (ts < decoded.exp) {
                    // Token hasn't expired, we can immediately redirect this user
                    // to lobby.html
                    setTimeout(function() {
                        window.location.href = './lobby.html';
                    }, 1000);
                }
            } catch (error) {
                console.log(error);
            }
        }


        $('#cmdSignup').click(function() {

            const body = {
                username: $('input#signupUsername').val(),
                password: $('input#signupPassword').val()
            };

            console.log(body);

            fetch('/api/auth/register', {
                    method: 'POST',
                    body: JSON.stringify(body),
                    headers: {
                        'Content-type': 'application/json'
                    }
                })
                .then(resp => {
                    if (!resp.ok) {
                        throw 'Signup was unsuccessful!';
                    }
                    return resp.text();
                })
                .then(data => {
                    showInfo('You have successfully registered an account with username "' + body.username + '"');
                    $('input#signupUsername').val('');
                    $('input#signupPassword').val('');
                    $('#btnradio1').click();
                })
                .catch(err => {
                    console.log(err);
                    showError(err);
                })
        });


        $('#cmdLogin').click(function() {
            const body = {
                username: $('input#loginUsername').val(),
                password: $('input#loginPassword').val()
            };

            console.log(body);

            fetch('/api/auth/login', {
                    method: 'POST',
                    body: JSON.stringify(body),
                    headers: {
                        'Content-type': 'application/json'
                    }
                })
                .then(resp => {
                    if (!resp.ok) {
                        throw 'Login was unsuccessful!';
                    }
                    return resp.json();
                })
                .then(data => {
                    // Redirect to game lobby with newly received token
                    showInfo('You have successfully logged in');
                    localStorage.setItem('tictax_jwt_token', data.token);
                    console.log('Received token: ' + localStorage.getItem('tictax_jwt_token') + ' ' + typeof(data.token));
                    setTimeout(function() {
                        window.location.href = './lobby.html';
                    }, 1000);
                })
                .catch(err => {
                    console.log(err);
                    showError(err);
                })
        });
    });
});