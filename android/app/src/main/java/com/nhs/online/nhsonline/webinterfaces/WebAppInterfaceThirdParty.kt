package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.interfaces.IAddToCalendarHandler
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb

class WebAppInterfaceThirdParty(
        private val activity: Activity,
        private val nhsWeb: NhsWeb,
        private val uiInteractor: IInteractor,
        private val addToCalendarHelper: IAddToCalendarHandler)
{
    @JavascriptInterface
    fun goToPage(page: String) {
      Log.d(Application.TAG, "${this::class.java.simpleName}: Entering goToPage")
      runAction {
          nhsWeb.goToPage(page)
      }
    }

    @JavascriptInterface
    fun addEventToCalendar(calendarData: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering addEventToCalendar")
        var addToCalendarData= addToCalendarHelper.parseCalendarData(calendarData, JavaScriptInteractionMode.SilverThirdParty)
        runAction { addToCalendarHelper.addToCalendar(addToCalendarData) }
    }

    @JavascriptInterface
    fun downloadFromBytes(base64Data: String, fileName: String, mimeType: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering downloadFromBytes")
        runAction { uiInteractor.downloadFromBytes(base64Data, fileName, mimeType, JavaScriptInteractionMode.SilverThirdParty) }
    }

    private fun runAction(action: () -> Unit){
        if(nhsWeb.javaScriptInteractionMode == JavaScriptInteractionMode.SilverThirdParty){
            activity.runOnUiThread(action)
        }
    }
}
