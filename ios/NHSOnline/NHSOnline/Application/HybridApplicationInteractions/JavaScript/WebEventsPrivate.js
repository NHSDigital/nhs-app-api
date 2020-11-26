window.nativeApp = {};

window.nativeApp.getNotificationsStatus = function() {
    window.webkit.messageHandlers.getNotificationsStatus.postMessage(null);
};

window.nativeApp.attemptBiometricLogin = function() {
    window.webkit.messageHandlers.attemptBiometricLogin.postMessage(null);
};

window.nativeApp.clearMenuBarItem = function(header) {
    window.webkit.messageHandlers.clearMenuBarItem.postMessage(null);
};

window.nativeApp.fetchBiometricSpec = function() {
    window.webkit.messageHandlers.fetchBiometricSpec.postMessage(null);
};

window.nativeApp.fetchNativeAppVersion = function() {
    window.webkit.messageHandlers.fetchNativeAppVersion.postMessage(null);
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

window.nativeApp.onSessionExpiring = function() {
    window.webkit.messageHandlers.onSessionExpiring.postMessage(null);
};

window.nativeApp.displayPageLeaveWarning = function() {
    window.webkit.messageHandlers.displayPageLeaveWarning.postMessage(null);
};

window.nativeApp.openAppSettings = function() {
    window.webkit.messageHandlers.openAppSettings.postMessage(null)
}

window.nativeApp.pageLoadComplete = function () {
    window.webkit.messageHandlers.pageLoadComplete.postMessage(null);
};

window.nativeApp.requestPnsToken = function(trigger) {
    window.webkit.messageHandlers.requestPnsToken.postMessage(trigger);
};

window.nativeApp.setMenuBarItem = function(index) {
    window.webkit.messageHandlers.setMenuBarItem.postMessage(index);
};

window.nativeApp.showInternetConnectionError = function() {
    window.webkit.messageHandlers.showInternetConnectionError.postMessage(null);
}

window.nativeApp.configureWebContext = function(helpUrl, retryPath) {
    window.webkit.messageHandlers.setHelpUrl.postMessage(helpUrl)
    window.webkit.messageHandlers.setRetryPath.postMessage(retryPath)
}

window.nativeApp.updateHeaderText = function(header) {
    window.webkit.messageHandlers.updateHeaderText.postMessage(header);
};

window.nativeApp.startDownload = function(base64Data, fileName, mimeType) {
    window.webkit.messageHandlers.startDownload.postMessage(base64Data + "|split|" + fileName + "|split|" + mimeType);
}

window.nativeApp.updateBiometricRegistrationWithToken = function(accessToken) {
    window.webkit.messageHandlers.updateBiometricRegistrationWithToken.postMessage(accessToken);
};

window.nativeApp.dismissPageLeaveWarningDialogue = function() {
    window.webkit.messageHandlers.dismissPageLeaveWarningDialogue.postMessage(null);
};

window.nativeApp.dismissAllDialogues = function() {
    window.webkit.messageHandlers.dismissAllDialogues.postMessage(null);
};

window.nativeApp.addEventToCalendar = function(calendarData) {
    window.webkit.messageHandlers.addEventToCalendar.postMessage(calendarData);
};
