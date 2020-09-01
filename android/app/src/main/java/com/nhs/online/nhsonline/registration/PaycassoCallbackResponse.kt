package com.nhs.online.nhsonline.registration

import com.google.gson.annotations.SerializedName

data class PaycassoCallbackResponse @JvmOverloads constructor(
    @SerializedName("transactionId") val transactionId: String? = null,
    @SerializedName("transactionType") val transactionType: String? = null,
    @SerializedName("isFaceMatched") val isFaceMatched: Boolean? = false,
    @SerializedName("error") val paycassoError: PaycassoError?
)

data class PaycassoError @JvmOverloads constructor(
    @SerializedName("errorCode") val errorCode: Int? = null,
    @SerializedName("errorMessage") val errorMessage: String)
