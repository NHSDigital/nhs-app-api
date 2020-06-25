package com.nhs.online.nhsonline.services.logging

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class LoggingRequest(
        var level: String,
        var message: String,
        var timeStamp: String
)