window.nhsappNative = {};

window.nhsappNative.goToPage = function(page) {
    window.webkit.messageHandlers.goToPage.postMessage(page);
};
