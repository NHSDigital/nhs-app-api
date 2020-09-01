package com.nhs.online.nhsonline.data

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class PaycassoCredentials (
    val hostUrl: String,
    val token: String
)
