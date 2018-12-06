/*
 * Copyright 2015 eBay Software Foundation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Modified heavily (including conversion to Kotlin) by NHS App
 */

package com.nhs.online.nhsonline.fido.uaf.client.operation

import android.annotation.SuppressLint
import android.util.Log
import com.google.gson.GsonBuilder
import com.nhs.online.nhsonline.fido.Fido
import com.nhs.online.nhsonline.fido.uaf.client.AuthAssertionBuilder
import com.nhs.online.nhsonline.fido.uaf.crypto.Base64url
import com.nhs.online.nhsonline.fido.uaf.msg.*
import com.nhs.online.nhsonline.fido.uaf.operationcall.AuthenticationCall
import com.nhs.online.nhsonline.fido.uaf.util.FidoEndpointConfig
import com.nhs.online.nhsonline.support.BiometricAssertionException
import com.nhs.online.nhsonline.support.BiometricsInvalidSignatureException
import com.nhs.online.nhsonline.support.GenericBiometricException
import com.nhs.online.nhsonline.support.extractJSONString

class Authentication(
    endpointConfig: FidoEndpointConfig,
    private val auth: AuthenticationCall = AuthenticationCall(endpointConfig)
) {
    private val TAG = Authentication::class.java.simpleName
    private val gson = GsonBuilder().disableHtmlEscaping().create()

    private val defaultScheme = "UAFV1TLV"

    @Throws(GenericBiometricException::class, BiometricsInvalidSignatureException::class)
    fun auth(
        uafMessage: String,
        assertionBuilder: AuthAssertionBuilder
    ): String {
        Log.d(TAG, "[UAF] Auth")
        try {
            Log.d(TAG, "  [UAF] Auth - priv key retrieved")
            val regResponse = processRequest(getAuthRequest(uafMessage), assertionBuilder)
            Log.d(TAG, "  [UAF] Auth - Authentication Response Formed  ")
            Log.d(TAG, regResponse.assertions[0]!!.assertion)
            Log.d(TAG, "  [UAF] Auth - done  ")
            val registrationResponses = arrayOf(regResponse)
            return getUafProtocolMessage(gson.toJson(registrationResponses))
        } catch (e: BiometricsInvalidSignatureException) {
            throw BiometricsInvalidSignatureException("Biometric authentication revoked.", e)
        } catch (e: GenericBiometricException) {
            throw GenericBiometricException("Failed to process auth request.", e)
        }

    }

    fun processRequest(
        request: AuthenticationRequest,
        assertionBuilder: AuthAssertionBuilder
    ): AuthenticationResponse {
        val requestHeader = request.header ?: OperationHeader()
        val response = AuthenticationResponse(requestHeader)
        val fcParams = FinalChallengeParams(requestHeader.appID, request.challenge, "")
        response.fcParams = Base64url.encodeToString(gson.toJson(fcParams).toByteArray())
        setAssertions(response, assertionBuilder)
        return response
    }


    private fun setAssertions(response: AuthenticationResponse, builder: AuthAssertionBuilder) {
        try {
            val assertion = builder.getAssertions(response)
            val authSignAssertion =
                AuthenticatorSignAssertion(assertionScheme = defaultScheme, assertion = assertion)
            response.assertions = listOf(authSignAssertion)

        } catch (e: BiometricsInvalidSignatureException) {
            throw BiometricsInvalidSignatureException("Biometrics signature invalid", e)
        } catch (e: Exception) {
            throw BiometricAssertionException("Failed to sign authentication assertions", e)
        }

    }

    @SuppressLint("PackageManagerGetSignatures") // This vulnerability can only be exploited in Android version 4.4 and below. This is below the minimum supported version of the app
    fun requestUafAuthenticationMessage(facetId: String): String {

        val uafMessage = auth.getUafMessageRequest(facetId, false)
        return uafMessage.extractJSONString(Fido.UAF_AUTH_RESPONSE_FIELD)
    }

    private fun getAuthRequest(uafMessage: String): AuthenticationRequest {
        Log.d(TAG, "  [UAF]Registration - getAuthRequest  : $uafMessage")
        return gson.fromJson(uafMessage, Array<AuthenticationRequest>::class.java)[0]
    }

    fun getUafProtocolMessage(uafMessage: String): String {
        var message = "{\"uafProtocolMessage\":\"" + uafMessage.replace("\"", "\\\"")
        message = "$message\"}"
        return message
    }
}
