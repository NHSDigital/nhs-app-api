package com.nhs.online.nhsonline.data

import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode

data class AddToCalendarData(val subject: String?,
                             val body: String?,
                             val location: String?,
                             val startTimeEpochInSeconds: Long?,
                             val endTimeEpochInSeconds: Long?,
                             val source : JavaScriptInteractionMode
)