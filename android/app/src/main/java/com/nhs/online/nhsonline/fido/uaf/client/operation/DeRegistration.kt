package com.nhs.online.nhsonline.fido.uaf.client.operation

import android.util.Log
import com.nhs.online.nhsonline.fido.Fido
import com.nhs.online.nhsonline.fido.uaf.msg.*
import com.nhs.online.nhsonline.fido.uaf.operationcall.DeRegistrationCall
import com.nhs.online.nhsonline.fido.uaf.util.FidoEndpointConfig
import com.nhs.online.nhsonline.support.GenericBiometricException

class DeRegistration(endpointConfig: FidoEndpointConfig) {
    private val deRegistrationCall: DeRegistrationCall = DeRegistrationCall(endpointConfig)
    private val TAG = DeRegistration::class.java.simpleName

    fun sendDeRegistrationOperation(appId: String, keyId: String) {
        val version = Version(1, 0)
        val operation = Operation.Dereg.toString()
        val requestHeader = OperationHeader(version, operation, appId)
        val deRegAuthenticator = DeregisterAuthenticator(Fido.AAID, keyId)

        val deRegistrationRequest = DeregistrationRequest(requestHeader, listOf(deRegAuthenticator))
        try {
            val deRegistrationResponse = deRegistrationCall.post(deRegistrationRequest)
            Log.d(TAG, "De-registration Response: $deRegistrationResponse")
        } catch (e: Exception) {
            throw GenericBiometricException("Failed to send de-registration request.", e)
        }
    }
}