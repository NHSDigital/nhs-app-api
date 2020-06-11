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
    @Deprecated("since 1.35.0 (NHSO-9622), here for backwards compatibility")
    @JavascriptInterface
    fun goToHomepage() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering goToHomepage")
        runAction {
            nhsWeb.loadWelcomePage()
            uiInteractor.clearMenuBarItem()
        }
    }
    
    @JavascriptInterface
    fun goToPage(page: String) {
      Log.d(Application.TAG, "${this::class.java.simpleName}: Entering goToPage")
      runAction {
          nhsWeb.goToPage(page)
      }
    }

    private fun runAction(action: () -> Unit){
        if(nhsWeb.javaScriptInteractionMode == JavaScriptInteractionMode.SilverThirdParty){
            activity.runOnUiThread(action)
        }
    }
}