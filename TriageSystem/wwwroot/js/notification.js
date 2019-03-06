var script = document.createElement('script');

script.src = '//code.jquery.com/jquery-1.11.0.min.js';
document.getElementsByTagName('head')[0].appendChild(script); 

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44375/notificationHub").build();

//Disable send button until connection is established
//document.getElementById("sendButton").disabled = true;
connection.start();


connection.on("ReceiveNotification", function (hospitalId) {
    $$.ajax({
        url: '@Url.Action("Index", "Home")',
    });

});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var hospitalID = document.getElementById("hospitalID").value;
    connection.invoke("SendNotification", hospitalID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
