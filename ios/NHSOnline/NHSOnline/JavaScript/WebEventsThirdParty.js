window.nhsappNative = {};

window.nhsappNative.goToHomepage = function() {
    window.webkit.messageHandlers.goToHomepage.postMessage(null);
};
