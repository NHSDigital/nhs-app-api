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

package com.nhs.online.nhsonline.fido.uaf.operationcall


import com.google.gson.Gson
import com.nhs.online.nhsonline.fido.uaf.curl.Curl
import com.nhs.online.nhsonline.fido.uaf.msg.DeregistrationRequest
import com.nhs.online.nhsonline.fido.uaf.util.FidoEndpointConfig

class DeRegistrationCall(private val endpointConfig: FidoEndpointConfig) {
    private val gson = Gson()

    fun post(regResponse: DeregistrationRequest?): String {
        val header = "Content-Type:Application/json Accept:Application/json"
        val json = getDeRegistrationUafMessage(regResponse)

        return Curl.post(endpointConfig.getDeregPost(), header, json)
    }

    private fun getDeRegistrationUafMessage(regResponse: DeregistrationRequest?): String {
        val forSending = arrayOfNulls<DeregistrationRequest>(1)
        forSending[0] = regResponse

        return gson.toJson(forSending, Array<DeregistrationRequest>::class.java)
    }
}
