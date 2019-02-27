"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveNotification", function (hospitalID) {
    $$.ajax({
        url: '@Url.Action("Index", "Home")',
    });

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
    });


document.getElementById("sendButton").addEventListener("click", function (event) {
    //var hospitalID = document.getElementById("hospitalID").value;
    connection.invoke("SendNotification").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});