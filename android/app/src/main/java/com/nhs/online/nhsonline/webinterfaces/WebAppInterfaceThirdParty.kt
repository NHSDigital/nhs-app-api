package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb

class WebAppInterfaceThirdParty(
        private val activity: Activity,
        private val nhsWeb: NhsWeb)
{
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