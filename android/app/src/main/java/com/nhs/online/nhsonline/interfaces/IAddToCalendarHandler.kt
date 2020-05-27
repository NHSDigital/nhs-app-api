package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.AddToCalendarData

interface IAddToCalendarHandler {

    fun showAddToCalendarErrorDialog()

    fun addToCalendar(addToCalendarData: AddToCalendarData)

}