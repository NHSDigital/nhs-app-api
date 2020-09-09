package com.nhs.online.nhsonline.webinterfaces

import android.util.Log
import com.nhaarman.mockitokotlin2.argumentCaptor
import com.nhaarman.mockitokotlin2.mock
import com.nhaarman.mockitokotlin2.verify
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.*
import com.nhs.online.nhsonline.registration.PaycassoCallbackResponse
import com.nhs.online.nhsonline.registration.PaycassoError
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class WebAppInterfaceLoginTest {
    private lateinit var contextMock: MainActivity
    private lateinit var nhsWebMock: NhsWeb
    private lateinit var webAppInterfaceNhsLogin: WebAppInterfaceNhsLogin

    @Before
    fun setUp(){
        contextMock = mock()
        nhsWebMock = mock {
            on { javaScriptInteractionMode }.thenReturn(JavaScriptInteractionMode.NhsLogin)
            on { onFailure }.thenReturn { _ ->
                PaycassoCallbackResponse("", "", false, PaycassoError(0, "")) }
        }
        webAppInterfaceNhsLogin = WebAppInterfaceNhsLogin(contextMock, nhsWebMock)

    }

    @Test
    fun onLogin() {
        val payload = """{
          "credentials": {
            "hostUrl":"hostUrl",
            "token":"token"
            },
            "externalReferences": {
                "consumerReference":"consumerReference",
                "transactionReference":"transactionReference",
                "appUserId":"externalAppUserId",
                "deviceId":"externalDeviceId",
                "hasNfcJourney": true,
                "transactionType":"DocuSure"
            },
            "transactionDetails": {
                "documentType":"PhotoId"
             }
        }"""
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        val credentials = PaycassoCredentials("hostUrl", "token")
        val externalReferences = PaycassoExternalReferences(
            "consumerReference",
            "transactionReference",
            "externalAppUserId",
            "externalDeviceId",
            true,
            "DocuSure"
        )
        val transactionDetails = PaycassoTransactionDetails(
            PaycassoDocumentType.PhotoId
        )
        val paycassoData = PaycassoData(credentials, externalReferences, transactionDetails)


        webAppInterfaceNhsLogin.startPaycasso(payload)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).startPaycasso(paycassoData)
    }
}
