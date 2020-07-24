package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.AddToCalendarData
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode

interface IAddToCalendarHandler {

    fun showAddToCalendarErrorDialog()

    fun addToCalendar(addToCalendarData: AddToCalendarData)

    fun parseCalendarData(calendarData: String, source: JavaScriptInteractionMode) : AddToCalendarData
}