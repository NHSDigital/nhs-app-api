package com.nhs.online.nhsonline.data

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class PaycassoExternalReferences (
    val consumerReference: String,
    val transactionReference: String,
    val appUserId: String,
    val deviceId: String,
    val hasNfcJourney: Boolean,
    val transactionType: String
)
