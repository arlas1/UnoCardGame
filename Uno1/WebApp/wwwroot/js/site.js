// site.js

document.addEventListener("DOMContentLoaded", function () {
    var isConnectedElement = document.getElementById("isConnected");

    if (isConnectedElement) {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl("/gameHub")
            .build();

        connection.start()
            .then(function () {
                isConnectedElement.innerText = "Yes";
            })
            .catch(function (error) {
                isConnectedElement.innerText = "No";
                console.error("Error establishing SignalR connection:", error);
            });
    }
});
