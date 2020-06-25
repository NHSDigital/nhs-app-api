package com.nhs.online.nhsonline.services.logging

import com.android.volley.NetworkResponse
import com.android.volley.Response
import com.android.volley.VolleyError
import com.android.volley.toolbox.JsonObjectRequest
import org.json.JSONObject

class NoResponseRequest(
        method: Int,
        url: String,
        jsonRequest: JSONObject,
        listener: Response.Listener<JSONObject>,
        errorListener: Response.ErrorListener
) : JsonObjectRequest(method, url, jsonRequest, listener, errorListener) {

    override fun parseNetworkResponse(response: NetworkResponse?): Response<JSONObject> {
        response?.statusCode?.let { statusCode ->
            if (statusCode < 400) {
                return Response.success(null, null)
            }
        }
        return Response.error(VolleyError("Response received with status code ${response?.statusCode}"))
    }
}