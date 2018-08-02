window.nativeApp = {};

window.nativeApp.onLogin = function() {
    window.webkit.messageHandlers.onLogin.postMessage(null);
};

window.nativeApp.onLogout = function() {
    window.webkit.messageHandlers.onLogout.postMessage(null);
};

window.nativeApp.updateHeaderText = function(header) {
    window.webkit.messageHandlers.updateHeaderText.postMessage(header);
};

window.nativeApp.clearMenuBarItem = function(header) {
    window.webkit.messageHandlers.clearMenuBarItem.postMessage(null);
};

window.nativeApp.checkSymptoms = function(header) {
    window.webkit.messageHandlers.checkSymptoms.postMessage(null);
};
