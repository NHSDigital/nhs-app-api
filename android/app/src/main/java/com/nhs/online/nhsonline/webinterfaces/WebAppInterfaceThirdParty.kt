package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.data.AddToCalendarData
import com.nhs.online.nhsonline.interfaces.IAddToCalendarHandler
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb
import org.json.JSONObject

class WebAppInterfaceThirdParty(
        private val activity: Activity,
        private val nhsWeb: NhsWeb,
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

        val eventObject = JSONObject(calendarData)

        val subject = when {
            eventObject.isNull("subject") -> null
            else -> eventObject.getString("subject")
        }

        val body = when {
            eventObject.isNull("body") -> null
            else -> eventObject.getString("body")
        }

        val location = when {
            eventObject.isNull("location") -> null
            else -> eventObject.getString("location")
        }

        val startTime = when {
            eventObject.isNull("startTimeEpochInSeconds") -> null
            else -> eventObject.getLong("startTimeEpochInSeconds")
        }

        val endTime = when {
            eventObject.isNull("endTimeEpochInSeconds") -> null
            else -> eventObject.getLong("endTimeEpochInSeconds")
        }

        runAction {
            addToCalendarHelper.addToCalendar(
                    AddToCalendarData(subject, body, location, startTime, endTime))
        }
  }

    private fun runAction(action: () -> Unit){
        if(nhsWeb.javaScriptInteractionMode == JavaScriptInteractionMode.SilverThirdParty){
            activity.runOnUiThread(action)
        }
    }
}