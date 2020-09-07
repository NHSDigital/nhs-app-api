package com.nhs.online.nhsonline.data

import android.util.Log
import com.nhs.online.nhsonline.registration.PaycassoCallbackResponse
import com.nhs.online.nhsonline.registration.PaycassoError
import com.paycasso.sdk.api.flow.enums.BarcodeLocation
import com.paycasso.sdk.api.flow.enums.DocumentShape
import com.paycasso.sdk.api.flow.enums.FaceLocation
import com.paycasso.sdk.api.flow.enums.MrzLocation
import com.squareup.moshi.FromJson
import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class PaycassoTransactionDetails (
    val documentType: PaycassoDocumentType
)

enum class PaycassoDocumentType(
    val mrzLocation: MrzLocation,
    val barcodeLocation: BarcodeLocation,
    val faceLocation: FaceLocation,
    val documentShape: DocumentShape,
    val isBothSides: Boolean,
    val eChipPresence: Boolean,
    val docCheck: Boolean,
    val documentName: String
) {
    DriversLicence(MrzLocation.NO,
        BarcodeLocation.NO,
        FaceLocation.FRONT,
        DocumentShape.ID,
        true,
        false,
        true,
        "ID"),
    Passport(MrzLocation.NO,
        BarcodeLocation.NO,
        FaceLocation.FRONT,
        DocumentShape.PASSPORT,
        false,
        false,
        true,
        "Passport"),
    PhotoId(MrzLocation.NO,
        BarcodeLocation.NO,
        FaceLocation.FRONT,
        DocumentShape.ID,
        true,
        false,
        true,
        "PhotoId")
}


class PaycassoDocumentTypeAdapter(val onFailure: (PaycassoCallbackResponse) -> Unit) {
    @FromJson
    fun fromJson(value: String): PaycassoDocumentType? {
        return try {
            PaycassoDocumentType.valueOf(value)
        } catch(e: IllegalArgumentException) {
            Log.e("TAG", "An unknown PaycassoDocumentType has been encountered: $value")
            val paycassoCallbackResponse = PaycassoCallbackResponse(
                paycassoError = PaycassoError(errorMessage = "An unknown PaycassoDocumentType has been encountered: $value"))
            onFailure(paycassoCallbackResponse)
            null
        }
    }
}
