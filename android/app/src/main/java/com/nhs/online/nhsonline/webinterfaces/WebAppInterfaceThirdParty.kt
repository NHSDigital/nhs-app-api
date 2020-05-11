package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb

class WebAppInterfaceThirdParty(
        private val activity: Activity,
        private val nhsWeb: NhsWeb,
        private val uiInteractor: IInteractor)
  {
    @JavascriptInterface
    fun goToHomepage() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering goToHomepage")
        runAction {
            nhsWeb.loadWelcomePage()
            uiInteractor.clearMenuBarItem()
        }

    }

    private fun runAction(action: () -> Unit){
        if(nhsWeb.javaScriptInteractionMode == JavaScriptInteractionMode.SilverThirdParty){
            activity.runOnUiThread(action)
        }
    }
}