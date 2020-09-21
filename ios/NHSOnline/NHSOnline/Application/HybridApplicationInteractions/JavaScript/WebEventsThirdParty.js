window.nhsappNative = {};

window.nhsappNative.goToPage = function(page) {
    window.webkit.messageHandlers.goToPage.postMessage(page);
};

window.nhsappNative.addEventToCalendar = function(calendarData) {
    window.webkit.messageHandlers.addEventToCalendar.postMessage(calendarData);
};

window.nhsappNative.downloadFromBytes = function(base64Data, fileName, mimeType) {
    window.webkit.messageHandlers.downloadFromBytes.postMessage(base64Data + "|split|" + fileName + "|split|" + mimeType);
}
