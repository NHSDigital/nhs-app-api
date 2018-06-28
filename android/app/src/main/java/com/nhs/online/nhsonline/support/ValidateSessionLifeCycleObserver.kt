package com.nhs.online.nhsonline.support

import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.services.KnownServices
import kotlinx.android.synthetic.main.activity_main.*

class ValidateSessionLifeCycleObserver(
    private var context: MainActivity,
    private var appWebInterface: AppWebInterface,
    private var knownServices: KnownServices
) {

    fun onMoveToForeground() {
        val currentUrl: String? = context.webview.url
        currentUrl?.let {
            var knownService = knownServices.findMatchingKnownService(it)

            if (knownService != null && knownService.shouldValidateSession) {
                appWebInterface.validateSession()
            }
        }
    }
}