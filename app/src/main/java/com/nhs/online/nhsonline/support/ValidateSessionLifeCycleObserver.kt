package com.nhs.online.nhsonline.support

import com.nhs.online.nhsonline.AppWebInterface
import com.nhs.online.nhsonline.MainActivity
import com.nhs.online.nhsonline.services.KnownServices
import kotlinx.android.synthetic.main.activity_main.*

class ValidateSessionLifeCycleObserver(private var context: MainActivity,
                               private var appWebInterface: AppWebInterface,
                               private var knownServices: KnownServices) {

    fun onMoveToForeground() {
        var knownService = knownServices.findMatchingKnownService(context.webview.url)

        if (knownService != null && knownService.shouldValidateSession) {
            appWebInterface.validateSession()
        }
    }
}