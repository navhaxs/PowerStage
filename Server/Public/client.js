// C:\Program Files (x86)\Renewed Vision\ProPresenter 6\ProPresenter.UI.Plugin.dll
// ProPresenter.UI.Plugin.ProNetwork.RVProRemoteWebSocketServiceHandler

const debug = false;
const default_host = window.location.hostname;
const default_control_port = '50003';
const default_web_port = '50004';
const default_pass = 'control';

let socket;
let cb_connectSuccess;
let cb_connectFail;
let cb_authSuccess;
let cb_authFail;

function err(s) {
    debug && console.error(`Error: ${s}`);
    return false;
}

function check_socket() {
    if (!socket) return err('SOCKET NOT CONNECTED');
    if (socket.readyState != 1) return err('SOCKET NOT READY');
    return true;
}

function connect(obj) {
    obj.connectSuccess && (cb_connectSuccess = obj.connectSuccess);
    obj.connectFail && (cb_connectFail = obj.connectFail);
    obj.authSuccess && (cb_authSuccess = obj.authSuccess);
    obj.authFail && (cb_authFail = obj.authFail);

    var host = localStorage.getItem("host") || default_host;
    var port = localStorage.getItem("control_port") || default_control_port;
    debug && console.log(`Connecting to ws://${host}:${port}/remote`);
    socket = new WebSocket(`ws://${host}:${port}/remote`);
    // Above operation is non-blocking. Have to wait for the socket to connect before we authenticate()
    socket.onclose = socket.onerror = cb_connectFail;
    socket.onopen = function() {
        cb_connectSuccess && cb_connectSuccess();
        debug && console.log("Connection success, authenticating...");
        authenticate()
    };
    listen();
}

function authenticate() {
    emit({
        action: 'authenticate',
        protocol: '600',
        password: localStorage.getItem("pass") || default_pass
    })
}

function listen() {
    socket.onmessage = function(event) {
        var msg = JSON.parse(event.data);
        var div = document.getElementById('debugLog');

        // div.innerHTML = JSON.stringify(msg) + "<br />" + div.innerHTML;
        // console.log(msg);

        if (msg.status) {
            var this_web_port = localStorage.getItem("web_port") || default_web_port;
            var this_host = localStorage.getItem("host") || default_host;
            var this_slide = "http://" + this_host + ":" + this_web_port + "/slides/slide_" + msg.status.currentSlideIndex + ".png";
            document.getElementById("slide").src = this_slide;
            var next_slide = "http://" + this_host + ":" + this_web_port + "/slides/slide_" + (msg.status.currentSlideIndex + 1) + ".png";
            document.getElementById("nextslide").src = next_slide;
            document.getElementById("currentSlideIndex").textContent = msg.status.currentSlideIndex;
            document.getElementById("totalSlidesCount").textContent = msg.status.totalSlidesCount;
        }


        switch (msg.action) {
            case "authenticate":
                debug && console.log("Authentication " + (msg.authenticated ? "success" : "failed"));
                msg.authenticated ? (cb_authSuccess && cb_authSuccess()) : (cb_authFail && cb_authFail());
                break;
            default:
                break;
        }
    }
}

function nextSlide() {
    emit({action: 'nextSlide'})
}

function prevSlide() {
    emit({action: 'prevSlide'})
}

function stageMessageSend(msg) {
    emit({
        action: 'stageDisplaySendMessage',
        stageDisplayMessage: msg
    })
}

function stageMessageHide() {
    emit({
        action: 'stageDisplayHideMessage'
    })
}

function emit(obj) {
    window.navigator.vibrate(50);
    var json = JSON.stringify(obj);
    if (check_socket()) socket.send(json);
    else return err('SOCKET EMIT FAILED');
}