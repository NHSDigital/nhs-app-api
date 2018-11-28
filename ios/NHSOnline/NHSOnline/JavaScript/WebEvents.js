window.nativeApp = {};

window.nativeApp.onLogin = function() {
    window.webkit.messageHandlers.onLogin.postMessage(null);
};

window.nativeApp.onLogout = function() {
    window.webkit.messageHandlers.onLogout.postMessage(null);
};

window.nativeApp.hideHeader = function() {
    window.webkit.messageHandlers.hideHeader.postMessage(null);
};

window.nativeApp.hideMenuBar = function() {
    window.webkit.messageHandlers.hideMenuBar.postMessage(null);
};

window.nativeApp.setMenuBarItem = function(index) {
    window.webkit.messageHandlers.setMenuBarItem.postMessage(index);
}

window.nativeApp.hideWhiteScreen = function() {
    window.webkit.messageHandlers.hideWhiteScreen.postMessage(null);
};

window.nativeApp.showHeader = function() {
    window.webkit.messageHandlers.showHeader.postMessage(null);
};

window.nativeApp.showHeaderSlim = function() {
    window.webkit.messageHandlers.showHeaderSlim.postMessage(null);
};

window.nativeApp.hideHeaderSlim = function() {
    window.webkit.messageHandlers.hideHeaderSlim.postMessage(null);
};

window.nativeApp.resetPageFocus = function() {
    window.webkit.messageHandlers.resetPageFocus.postMessage(null);
};

window.nativeApp.updateHeaderText = function(header) {
    window.webkit.messageHandlers.updateHeaderText.postMessage(header);
};

window.nativeApp.postNdopToken = function(token) {
    window.webkit.messageHandlers.postNdopToken.postMessage(token);
};

window.nativeApp.clearMenuBarItem = function(header) {
    window.webkit.messageHandlers.clearMenuBarItem.postMessage(null);
};

window.nativeApp.completeAppIntro = function(header) {
    window.webkit.messageHandlers.completeAppIntro.postMessage(null);
};

window.nativeApp.goToBiometrics = function(header) {
    window.webkit.messageHandlers.goToBiometrics.postMessage(null);
};

window.nativeApp.onSessionExpiring = function(sessionDuration) {
    window.webkit.messageHandlers.onSessionExpiring.postMessage(sessionDuration);
};
