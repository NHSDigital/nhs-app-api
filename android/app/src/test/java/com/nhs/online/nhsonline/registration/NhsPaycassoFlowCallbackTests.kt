package com.nhs.online.nhsonline.registration

import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.paycasso.sdk.api.flow.model.DocuSureFlowResponse
import com.paycasso.sdk.api.flow.model.FlowFailureResponse
import com.paycasso.sdk.api.flow.model.InstaSureFlowResponse
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class NhsPaycassoFlowCallbackTests {

    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var nhsAppPaycassoFlowCallback: NhsAppPaycassoFlowCallback
    private lateinit var resultCallback: (PaycassoCallbackResponse) -> Unit
    private lateinit var callbackResult: PaycassoCallbackResponse

    @Before
    fun setup(){
        resultCallback = { message -> callbackResult = message}
        appWebInterfaceMock = mock()
        nhsAppPaycassoFlowCallback = NhsAppPaycassoFlowCallback(
            resultCallback,
            resultCallback)
    }

    @Test
    fun onSuccessCallback_DocuSureResponse_SuccessfullyCallsJavascriptInterface(){
        val response = DocuSureFlowResponse(true)
        response.transactionId = "transactionIdDocuSure"
        nhsAppPaycassoFlowCallback.onSuccess(response)
        Assert.assertEquals("transactionIdDocuSure", callbackResult.transactionId)
        Assert.assertEquals("DocuSureFlowResponse", callbackResult.transactionType)
        Assert.assertFalse(callbackResult.isFaceMatched!!)
        Assert.assertNull(callbackResult.paycassoError)

    }

    @Test
    fun onSuccessCallback_InstaSureResponse_SuccessfullyCallsJavascriptInterface(){
        val response = InstaSureFlowResponse(true)
        response.transactionId = "transactionIdInstaSure"
        nhsAppPaycassoFlowCallback.onSuccess(response)
        Assert.assertEquals("transactionIdInstaSure", callbackResult.transactionId)
        Assert.assertEquals("InstaSureFlowResponse", callbackResult.transactionType)
        Assert.assertTrue(callbackResult.isFaceMatched!!)
        Assert.assertNull(callbackResult.paycassoError)
    }

    @Test
    fun onFailure_SuccessfullyCallsJavascript_WithCorrectFailureCode(){
        val failureResponse = FlowFailureResponse(905, "No connectivity")
        nhsAppPaycassoFlowCallback.onFailure(failureResponse)

        Assert.assertEquals(callbackResult.paycassoError!!.errorCode, 905)
        Assert.assertEquals(callbackResult.paycassoError!!.errorMessage, "No connectivity")
    }
}
