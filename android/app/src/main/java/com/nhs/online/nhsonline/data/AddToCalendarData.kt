package com.nhs.online.nhsonline.data

data class AddToCalendarData(val subject: String?,
                             val body: String?,
                             val location: String?,
                             val startTimeEpochInSeconds: Long?,
                             val endTimeEpochInSeconds: Long?
)