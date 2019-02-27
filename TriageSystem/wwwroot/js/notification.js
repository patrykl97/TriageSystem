﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44375/notificationHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveNotification", function (hospitalID) {
    $.ajax({
        url: '@Url.Action("Index", "Home")',
        type: 'POST',
        success: function (data) {
            $("#divContent").html(data);
        }
    });
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var hospitalID = document.getElementById("hospitalID").value;
    connection.invoke("SendNotification", hospitalID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});