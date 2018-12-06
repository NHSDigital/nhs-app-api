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

import android.util.Log
import com.nhs.online.nhsonline.fido.uaf.curl.Curl
import com.nhs.online.nhsonline.fido.uaf.util.FidoEndpointConfig

class AuthenticationCall(private val endpointConfig: FidoEndpointConfig) : FidoServerCall() {
    fun getUafMessageRequest(facetId: String, isTransaction: Boolean): String {
        val url = endpointConfig.getAuthRequestGet()
        val serverResponse = Curl.get(url)
        Log.d("getUAFRequest", serverResponse)
        return processUafResponseMessage(serverResponse, facetId, isTransaction)
    }
}
