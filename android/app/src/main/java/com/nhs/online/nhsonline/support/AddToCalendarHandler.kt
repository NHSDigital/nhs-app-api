package com.nhs.online.nhsonline.support

import android.app.Activity
import android.app.AlertDialog
import android.content.Intent
import android.provider.CalendarContract
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.AddToCalendarData
import com.nhs.online.nhsonline.interfaces.IAddToCalendarHandler

class AddToCalendarHandler(val context: Activity): IAddToCalendarHandler {

    override fun addToCalendar(addToCalendarData: AddToCalendarData) {

        if (!isCalendarDataValid(
                        addToCalendarData.subject,
                        addToCalendarData.startTimeEpochInSeconds,
                        addToCalendarData.endTimeEpochInSeconds)) {
            showAddToCalendarErrorDialog()
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
        } catch (e: Exception) {
            showAddToCalendarErrorDialog()
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

    private fun isCalendarDataValid(subject: String?, startTimeEpochInSeconds: Long?, endTimeEpochInSeconds: Long?): Boolean {
        if (subject.isNullOrBlank() || startTimeEpochInSeconds == null || endTimeEpochInSeconds == null ||
                (startTimeEpochInSeconds > endTimeEpochInSeconds)) {
            return false
        }
        return true
    }

}