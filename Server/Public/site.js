// load client.js first!
elem = id => document.getElementById(id);

// Get a reference to the body element, and create a new image object
var body = document.querySelector('body');
var myImage = new Image();
// Call the function with the URL we want to load, but then chain the
// promise then() method on to the end of it. This contains two callbacks
imgLoad('placeholder.png').then(function (response) {
    // The first runs when the promise resolves, with the request.response
    // specified within the resolve() method.
    var imageURL = window.URL.createObjectURL(response);
    myImage.src = imageURL;
    body.appendChild(myImage);
    // The second runs when the promise
    // is rejected, and logs the Error specified with the reject() method.
}, function (Error) {
    console.log(Error);
});

(status_disconnected = () => elem("status").className = (elem("status").innerText = "Disconnected").toLowerCase())();
status_badAuth = () => (elem("status").className = "disconnected") && (elem("status").innerText = "Bad password... retrying in 3 seconds");
status_connecting = () => elem("status").className = (elem("status").innerText = "Connecting").toLowerCase();
status_connected = () => elem("status").className = (elem("status").innerText = "Connected").toLowerCase();

let cb = {
    connectFail: function () {
        status_disconnected();
        connectFail();
    },
    authSuccess: function () {
        clearTimeout(connectLoop);
        status_connected();
    },
    authFail: function () {
        status_badAuth();
        connectFail();
    },
}

let connectLoop;
let shakeLoop;

function connectFail() {
    clearTimeout(connectLoop);
    connectLoop = setTimeout(function () {
        status_connecting();
        connect(cb);
    }, 3000);
}

elem("message").addEventListener("keyup", function (event) {
    event.preventDefault();
    switch (event.keyCode) {
        case 13:
            elem("message").value.trim() && elem("send").click();
        case 27:
            elem("message").value = "";
            break;
    }
});

elem("nextActionButton").addEventListener("click", function () {
    nextSlide();
})

elem("prevActionButton").addEventListener("click", function () {
    prevSlide();
})

elem("send").addEventListener("click", function () {
    let message;
    if (!(message = elem("message").value.trim())) return false;
    stageMessageSend(message);

    clearTimeout(shakeLoop)
    elem("send").className = "shake";
    shakeLoop = setTimeout(() => elem("send").className = "", 200);


    var oldMessage = elem("currentMessage");
    var newMessage = oldMessage.cloneNode(true);
    newMessage.innerText = message;
    newMessage.className = "blink";
    oldMessage.parentNode.replaceChild(newMessage, oldMessage);

})
// elem("clear").addEventListener("click", function() {
//     stageMessageHide();
//     elem("currentMessage").innerText = " ";
//     elem("currentMessage").className = "";
// })

function showPreferences() {
    // todo move elsewhere
    if (screenfull.enabled) {
        screenfull.request();
    }

    elem("preferences").showModal();
    elem("pref_addr").value = localStorage.getItem("host") || default_host;
    elem("pref_control_port").value = localStorage.getItem("control_port") || default_control_port;
    elem("pref_web_port").value = localStorage.getItem("web_port") || default_web_port;
    elem("pref_pass").value = localStorage.getItem("pass") || default_pass;
}
elem("pref_open").addEventListener("click", showPreferences);

elem("pref_save").addEventListener("click", function () {
    localStorage.setItem("host", elem("pref_addr").value || default_host);
    localStorage.setItem("control_port", elem("pref_control_port").value || default_control_port);
    localStorage.setItem("web_port", elem("pref_web_port").value || default_web_port);
    localStorage.setItem("pass", elem("pref_pass").value || default_pass);
    socket.close()
    status_disconnected();
    connect(cb);
    status_connecting();
    elem("preferences").close();
})

status_connecting();
connect(cb);