"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44375/notificationHub").build();
connection.logging = true;
//Disable send button until connection is established
//document.getElementById("sendButton").disabled = true;
connection.start().catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function (event) {

    var hospitalID = document.getElementById("hospitalID").value;
    var patient = {};

    patient.PPS = document.getElementById("PPS").value;
    patient.Condition = document.getElementById("Condition").value;
    patient.HospitalID = parseInt(hospitalID);
    console.log(patient);
    console.log(typeof (patient.HospitalID));
    $.ajax({
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        url: "/Home/RegisterPatient",
        data: JSON.stringify(patient),
        success: function (response) {



        }
        //error: function (response) {
        //    console.log("wrong");
        //}
    });

    connection.invoke("SendNotification", hospitalID).catch(function (err) {
        console.log(err);
        return console.error(err.toString());
    });
    event.preventDefault();

    setTimeout(redirect, 100)
    function redirect() {
        window.location.href = '@Url.Action("Index", "Home")';
    }
});


