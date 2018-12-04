package com.nhs.online.nhsonline.services

import android.util.Log
import com.android.volley.NoConnectionError
import com.android.volley.Request
import com.android.volley.Response
import com.android.volley.TimeoutError
import com.android.volley.toolbox.StringRequest
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import org.json.JSONObject

class ConfigurationResponse {
    var isValidConfiguration: Boolean = false
    var isThrottlingEnabled: Boolean = false
}

class ConfigurationService(private val context: MainActivity) {

    fun getConfiguration(callback: IVolleyCallback) {

        var configurationUrl = String.format(
                context.resources.getString(R.string.baseApiURL)
                        + context.resources.getString(R.string.configurationApiPath), BuildConfig.VERSION_NAME)

        // Request a string response from the provided URL.
        val stringReq = StringRequest(Request.Method.GET, configurationUrl,
                Response.Listener<String> { response ->

                    try {
                        val isValidConfiguration = parseBoolean(response, R.string.isSupportedVersion)
                        var isThrottlingEnabled = parseBoolean(response, R.string.isThrottlingEnabled)

                        var configurationResponse = ConfigurationResponse()
                        configurationResponse.isThrottlingEnabled = isThrottlingEnabled
                        configurationResponse.isValidConfiguration = isValidConfiguration

                        Log.d(Application.TAG, "${this::class.java.simpleName}: Configuration success: isValidConfiguration $isValidConfiguration. Throttling enabled: $isThrottlingEnabled.")
                        callback.onSuccess(configurationResponse)
                    } catch (e: ClassCastException) {
                        Log.d(Application.TAG, "${this::class.java.simpleName}: Configuration success: failed to parse response")
                        callback.onError(serverErrorMessage)
                    }
                },
                Response.ErrorListener { error ->
                    Log.d(Application.TAG, "${this::class.java.simpleName}: Configuration error: $error")
                    if (error is TimeoutError || error is NoConnectionError) {
                        callback.onError(connectionErrorMessage)
                    } else {
                        callback.onError(serverErrorMessage)
                    }
                })
        context.getRequestQueue().add(stringReq)
    }

    private val connectionErrorMessage =
            ErrorMessage(context.resources.getString(R.string.connection_error_title),
                    context.resources.getString(R.string.connection_error_message),
                    context.resources.getString(
                            R.string.Accessible_connection_error_message))

    private val serverErrorMessage =
            ErrorMessage(context.resources.getString(R.string.server_error_title),
                    context.resources.getString(R.string.server_error_message),
                    context.resources.getString(
                            R.string.accessible_server_error_message))

    private fun parseBoolean(response: String, propertyId: Int) : Boolean {
        val jsonObj = JSONObject(response)

        var propertyName = context.resources.getString(propertyId)

        if(jsonObj.has(propertyName)) {
            return jsonObj.getBoolean(context.resources.getString(propertyId))
        }

        return false
    }
}