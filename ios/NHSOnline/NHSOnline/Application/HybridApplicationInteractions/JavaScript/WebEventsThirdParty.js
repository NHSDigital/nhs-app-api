window.nhsappNative = {};

/**
 * @todo remove as part of NHSO-9622
 * */
window.nhsappNative.goToHomepage = function() {
    window.webkit.messageHandlers.goToHomepage.postMessage(null);
};

window.nhsappNative.goToPage = function() {
    window.webkit.messageHandlers.goToPage.postMessage(null);
};
