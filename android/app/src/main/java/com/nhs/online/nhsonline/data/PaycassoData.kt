package com.nhs.online.nhsonline.data

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class PaycassoData (
    val credentials: PaycassoCredentials,
    val externalReferences: PaycassoExternalReferences,
    val transactionDetails: PaycassoTransactionDetails
)
