package com.nhs.online.nhsonline.support

import android.util.Log
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig


class LifeCycleObserver(
    private var context: MainActivity,
    private var appWebInterface: AppWebInterface,
    private var knownServices: KnownServices
) {

    fun onMoveToForeground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground")
        val currentUrl: String? = context.webview.url
        currentUrl?.let {
            if (currentUrl.contains("auth-return")) return

            val knownServiceInfo = knownServices.findMatchingServiceInfo(it)

            if (knownServiceInfo != null && knownServiceInfo.shouldValidateSession) {
                Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground > isKnownService > ${knownServiceInfo.baseUrl} and shouldValidateSession")
                appWebInterface.validateSession()
            } else {
                context.hideBlankScreen()
            }
        }
    }

    fun onMoveToBackground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToBackground")
        context.showBlankScreen()
    }
}