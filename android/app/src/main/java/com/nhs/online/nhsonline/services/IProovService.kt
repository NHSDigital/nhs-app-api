package com.nhs.online.nhsonline.services;

import android.webkit.WebView
import com.iproov.sdk.IProov;

class IProovService () {

    fun installIProov (webView: WebView) {
        IProov.nativeBridge.install(webView);
    }

    fun uninstallIProov (webView: WebView) {
        IProov.nativeBridge.uninstall(webView);
    }
}
