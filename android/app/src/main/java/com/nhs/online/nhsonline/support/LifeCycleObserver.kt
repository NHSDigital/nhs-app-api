package com.nhs.online.nhsonline.support

import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*
import com.nhs.online.nhsonline.services.KnownServices


class LifeCycleObserver(
    private var context: MainActivity,
    private var appWebInterface: AppWebInterface,
    private var knownServices: KnownServices
) {

    fun onMoveToForeground() {
        val currentUrl: String? = context.webview.url
        currentUrl?.let {
            if (currentUrl.contains("auth-return")) return

            val knownServiceInfo = knownServices.findMatchingServiceInfo(it)

            if (knownServiceInfo != null && knownServiceInfo.shouldValidateSession) {
                appWebInterface.validateSession()
            } else {
                context.hideBlankScreen()
            }
        }
    }

    fun onMoveToBackground() {
        context.showBlankScreen()
    }
}