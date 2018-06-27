package com.nhs.online.nhsonline

import android.app.Application
import android.content.pm.ApplicationInfo
import android.webkit.WebView

@Suppress("unused") // This is referenced by the AndroidManifest.xml
class Application : Application() {
    override fun onCreate() {
        super.onCreate()

        enableWebDebuggingIfApplicationDebuggable()
    }

    private fun enableWebDebuggingIfApplicationDebuggable() {
        if ((applicationInfo.flags and ApplicationInfo.FLAG_DEBUGGABLE) == ApplicationInfo.FLAG_DEBUGGABLE) {
            WebView.setWebContentsDebuggingEnabled(true)
        }
    }
}