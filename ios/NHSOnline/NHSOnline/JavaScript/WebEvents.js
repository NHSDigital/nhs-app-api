window.nativeApp = {};

window.nativeApp.attemptBiometricLogin = function() {
    window.webkit.messageHandlers.attemptBiometricLogin.postMessage(null);
};

window.nativeApp.clearMenuBarItem = function(header) {
    window.webkit.messageHandlers.clearMenuBarItem.postMessage(null);
};

window.nativeApp.fetchNativeAppVersion = function() {
    window.webkit.messageHandlers.fetchNativeAppVersion.postMessage(null);
};

window.nativeApp.focusElement = function(elementToFocus) {
    window.webkit.messageHandlers.focusElement.postMessage(elementToFocus);
};

window.nativeApp.goToLoginOptions = function() {
    window.webkit.messageHandlers.goToLoginOptions.postMessage(null);
};

window.nativeApp.hideHeader = function() {
    window.webkit.messageHandlers.hideHeader.postMessage(null);
};

window.nativeApp.hideHeaderSlim = function() {
    window.webkit.messageHandlers.hideHeaderSlim.postMessage(null);
};

window.nativeApp.hideMenuBar = function() {
    window.webkit.messageHandlers.hideMenuBar.postMessage(null);
};

window.nativeApp.hideWhiteScreen = function() {
    window.webkit.messageHandlers.hideWhiteScreen.postMessage(null);
};

window.nativeApp.onLogin = function() {
    window.webkit.messageHandlers.onLogin.postMessage(null);
};

window.nativeApp.onLogout = function() {
    window.webkit.messageHandlers.onLogout.postMessage(null);
};

window.nativeApp.onSessionExpiring = function(sessionDuration) {
    window.webkit.messageHandlers.onSessionExpiring.postMessage(sessionDuration);
};

window.nativeApp.pageLoadComplete = function () {
    window.webkit.messageHandlers.pageLoadComplete.postMessage(null);
};

window.nativeApp.requestPnsToken = function() {
    window.webkit.messageHandlers.requestPnsToken.postMessage(null);
};

window.nativeApp.resetPageFocus = function() {
    window.webkit.messageHandlers.resetPageFocus.postMessage(null);
};

window.nativeApp.setMenuBarItem = function(index) {
    window.webkit.messageHandlers.setMenuBarItem.postMessage(index);
};

window.nativeApp.showHeader = function() {
    window.webkit.messageHandlers.showHeader.postMessage(null);
};

window.nativeApp.showHeaderSlim = function() {
    window.webkit.messageHandlers.showHeaderSlim.postMessage(null);
};

window.nativeApp.updateHeaderText = function(header) {
    window.webkit.messageHandlers.updateHeaderText.postMessage(header);
};
