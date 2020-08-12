window.nativeNhsLogin = {};

window.nativeNhsLogin.startPaycasso = function(paycassoRequest) {
    window.webkit.messageHandlers.showPaycasso.postMessage(paycassoRequest);
};
