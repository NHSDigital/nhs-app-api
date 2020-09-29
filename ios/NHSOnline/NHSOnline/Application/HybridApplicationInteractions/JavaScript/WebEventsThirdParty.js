window.nhsappNative = {};

window.nhsappNative.goToPage = function(page) {
    window.webkit.messageHandlers.goToPage.postMessage(page);
};

window.nhsappNative.addEventToCalendar = function(calendarData) {
    window.webkit.messageHandlers.addEventToCalendar.postMessage(calendarData);
};
