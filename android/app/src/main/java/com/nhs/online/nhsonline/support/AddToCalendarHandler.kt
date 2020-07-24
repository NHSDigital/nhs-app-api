package com.nhs.online.nhsonline.support

import android.app.Activity
import android.app.AlertDialog
import android.content.Intent
import android.provider.CalendarContract
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.AddToCalendarData
import com.nhs.online.nhsonline.interfaces.IAddToCalendarHandler
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.logging.ILoggingService
import org.json.JSONObject

class AddToCalendarHandler(
        val context: Activity,
        private val loggingService: ILoggingService): IAddToCalendarHandler {

    override fun addToCalendar(addToCalendarData: AddToCalendarData) {

        if (!isCalendarDataValid(
                        addToCalendarData.subject,
                        addToCalendarData.startTimeEpochInSeconds,
                        addToCalendarData.endTimeEpochInSeconds)) {
            showAddToCalendarErrorDialog()
            logAddToCalendarError(addToCalendarData.source)
            return
        }

        try {
            val intent = Intent(Intent.ACTION_INSERT)
                .setData(CalendarContract.Events.CONTENT_URI)
                .putExtra(CalendarContract.Events.TITLE, addToCalendarData.subject)
                .putExtra(CalendarContract.Events.DESCRIPTION, addToCalendarData.body)
                .putExtra(CalendarContract.Events.EVENT_LOCATION, addToCalendarData.location)
                .putExtra(CalendarContract.Events.AVAILABILITY, CalendarContract.Events.AVAILABILITY_BUSY)
                .putExtra(CalendarContract.EXTRA_EVENT_BEGIN_TIME, addToCalendarData.startTimeEpochInSeconds!! * 1000)
                .putExtra(CalendarContract.EXTRA_EVENT_END_TIME, addToCalendarData.endTimeEpochInSeconds!! * 1000)
            
            context.startActivity(intent)
            logAddToCalendarSuccess(addToCalendarData.source)

        } catch (e: Exception) {
            showAddToCalendarErrorDialog()
            logAddToCalendarError(addToCalendarData.source)
        }
    }

    override fun showAddToCalendarErrorDialog() {
        AlertDialog.Builder(context)
                .setTitle(R.string.add_calendar_event_dialog_title)
                .setMessage(R.string.add_calendar_event_dialog_message)
                .setCancelable(false)
                .setNegativeButton(R.string.add_calendar_event_dialog_negative_button_text) {
                    dialog, _ -> dialog.cancel()
                }
                .setPositiveButton(R.string.add_calendar_event_dialog_positive_button_text) {
                    dialog, _ -> dialog.cancel()

                    val intent = Intent(Intent.ACTION_INSERT)
                            .setData(CalendarContract.Events.CONTENT_URI)
                    context.startActivity(intent)
                }
                .setIcon(android.R.drawable.ic_dialog_alert)
                .show()
    }

    override fun parseCalendarData(calendarData: String, source: JavaScriptInteractionMode): AddToCalendarData {
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

        return AddToCalendarData(subject, body, location, startTime, endTime, source)
    }

    private fun isCalendarDataValid(subject: String?, startTimeEpochInSeconds: Long?, endTimeEpochInSeconds: Long?): Boolean {
        if (subject.isNullOrBlank() || startTimeEpochInSeconds == null || endTimeEpochInSeconds == null ||
                (startTimeEpochInSeconds > endTimeEpochInSeconds)) {
            return false
        }
        return true
    }

    private fun logAddToCalendarSuccess(source: JavaScriptInteractionMode) {
        loggingService.logInfo("Add to calendar success from " + addToCalendarSourceMessage(source));
    }

    private fun logAddToCalendarError(source: JavaScriptInteractionMode) {
        loggingService.logError("Add to calendar failure from " + addToCalendarSourceMessage(source));
    }

    private fun addToCalendarSourceMessage(source: JavaScriptInteractionMode) : String {
        when (source) {
            JavaScriptInteractionMode.NhsApp -> return "the NhsApp"
            JavaScriptInteractionMode.SilverThirdParty -> return "a third party"
            else -> return "an unknown source"
        }
    }

}