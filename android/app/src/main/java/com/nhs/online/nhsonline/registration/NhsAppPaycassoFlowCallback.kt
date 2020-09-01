package com.nhs.online.nhsonline.registration

import android.util.Log
import com.paycasso.sdk.api.flow.PaycassoFlowCallback
import com.paycasso.sdk.api.flow.model.*

private val TAG = NhsAppPaycassoFlowCallback::class.simpleName

class NhsAppPaycassoFlowCallback(private val onSuccess: (PaycassoCallbackResponse) -> Unit,
                                 private val onFailure: (PaycassoCallbackResponse) -> Unit) :
    PaycassoFlowCallback {
    override fun onSuccess(response: AbstractFlowResponse) {
        var type = "";
        var isFaceMatched = false
        var paycassoError: PaycassoError? = null
        when (response) {
            is InstaSureFlowResponse -> {
                type = "InstaSureFlowResponse"
                isFaceMatched = true
            }
            is VeriSureFlowResponse ->  type = "VeriSureFlowResponse"
            is DocuSureFlowResponse ->  type = "DocuSureFlowResponse"
            else ->
                paycassoError = PaycassoError(errorMessage = "DocumentResponse type not recognised")
        }
        val paycassoCallbackResponse = PaycassoCallbackResponse(
            response.transactionId,
            type,
            isFaceMatched,
            paycassoError
        )
        onSuccess(paycassoCallbackResponse)
    }

    override fun onFailure(flowFailureResponse: FlowFailureResponse) {
        onFailure(PaycassoCallbackResponse(
            paycassoError = PaycassoError(flowFailureResponse.failureCode, flowFailureResponse.failureMsg)))
        Log.e(TAG, "Failure Occurred: ${flowFailureResponse.failureCode} - ${flowFailureResponse.failureMsg}")
    }
}
