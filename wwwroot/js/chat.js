var connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();
var idphong = document.getElementById('IdPhongInput').value;

document.getElementById('sendButton').disabled = true;

connection.on('InvalidUser', function (errorMessage) {
    alert(errorMessage);
});

connection
    .start()
    .then(function () {
        document.getElementById('sendButton').disabled = false;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

document
    .getElementById('sendButton')
    .addEventListener('click', function (event) {
        var user = document.getElementById('userInput').value;
        var idphong = document.getElementById('IdPhongInput').value;
        var message = document.getElementById('messageInput').value;
        connection
            .invoke('SendMessage', idphong, user, message)
            .catch(function (err) {
                return console.error(err.toString());
            });

        if (idphong == '2') {
            connection.on('ReceiveMessage', function (user, message) {
                var li = document.createElement('li');
                document.getElementById('messagesList').appendChild(li);
                li.textContent = `${user} says ${message}`;
            });
        } else {
            alert('sai phòng r');
        }
        event.preventDefault();
    });
